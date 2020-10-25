using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Eero_API;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Eero_Console
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }
        private static long LastPoll = DateTime.UtcNow.Ticks;
        private static TimeSpan PollInterval = new TimeSpan(0, 5, 0);
        private static CancellationTokenSource TokenSource = new CancellationTokenSource();

        static void Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;

            Console.WriteLine("Eero API: {0}",eero.API);
            Console.WriteLine("API Version: {0}\r\n", eero.Version);


            var devEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

            var isDevelopment = string.IsNullOrEmpty(devEnvironmentVariable) ||
                                devEnvironmentVariable.ToLower() == "development";
            //Determines the working environment as IHostingEnvironment is unavailable in a console app

            var builder = new ConfigurationBuilder();
            // tell the builder to look for the appsettings.json file
            builder
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

            //only add secrets in development
            if (isDevelopment)
            {
                builder.AddUserSecrets<Program>();
            }

            Configuration = builder.Build();

            var api=Configuration.GetSection("API");
            eero.Identifier=api.GetSection("identifier").Value;

            var fn = Configuration.GetSection("Who:FriendlyNames").AsEnumerable();
            foreach(var kvp in fn)
            {
                if(!string.IsNullOrWhiteSpace(kvp.Value)) MyPresence.Who.FriendlyNames.Add(kvp.Value);
            }

            MyPresence.HomeAway.FullFilename = Configuration.GetSection("Who:FullFilename").Value;

            string poll = Configuration.GetSection("API:Poll")?.Value;
            if (!string.IsNullOrEmpty(poll))
            {
                TimeSpan span = TimeSpan.Zero;
                if (TimeSpan.TryParse(poll, out span))
                {
                    PollInterval = span;
                }
            }
            LastPoll = DateTime.UtcNow.Ticks-PollInterval.Ticks;

            eero.Debug = true;

            Console.WriteLine("Logged In: {0}", eero.IsLoggedIn());

            if (!eero.IsLoggedIn())
            {
                bool success = eero.Login();
                Console.WriteLine("Login Succeeded: {0}",success);
                if (success)
                {
                    int count = 0;
                    while (count < 3)
                    {
                        Console.Write("\r\nEnter Code: ");
                        string code = Console.ReadLine();
                        if (code.Length == 0) return;
                        if (!eero.LoginVerify(code))
                        {
                            count ++;
                        }
                        else
                        {
                            count = 3;
                            Console.WriteLine("Login Verified.");
                            eero.Save();
                        }
                    }
                }
                else
                {
                    eero.Save();
                    return;
                }
            }
            else
            {
                if (!eero.LoginRefresh())
                {
                    Console.WriteLine("Failed to refresh Login");
                    eero.Save();
                    return;
                }
                else
                {
                    eero.Save();
                    if (!eero.GetAccount())
                    {
                        Console.WriteLine("Failed to get Account");
                        return;
                    }
                }
            }
            //If we get here, we are logged in and should have account details
            if (string.IsNullOrEmpty(eero.Account))
            {
                Console.WriteLine("Account Details Missing!");
                return;
            }

            Eero_Models.EeroAccount eeroAccount = Eero_Models.EeroAccount.FromString(eero.Account);

            if (eeroAccount.Networks.Count > 0)
            {
                Task task = new Task(() => {
                    var ct = TokenSource.Token;
                    while (!ct.IsCancellationRequested)
                    {
                        try
                        {
                            if (DateTime.UtcNow.Ticks >= (LastPoll + PollInterval.Ticks))
                            {
                                LastPoll = DateTime.UtcNow.Ticks;
                                string devices = eero.GetNetworkDevices(eeroAccount.Networks[0].Id);
                                eeroAccount.Networks[0].SetDevicesFromString(devices);
                                long AMonthAgo = DateTime.UtcNow.AddMonths(-1).Ticks;// not sure when or if the device list gets purged
                                List<Eero_Models.Device> Phones = eeroAccount.Networks[0].Devices?.Where(x => x.Device_Type == "phone" && x.Wireless && x.Last_Active.Ticks >= AMonthAgo && x.Is_Guest==false).ToList();
                                List<Eero_Models.Device> DevicesHome = Phones.Where(x => x.Connected).ToList();
                                List<Eero_Models.Device> DevicessAway = Phones.Where(x => !x.Connected).ToList();
                                MyPresence.HomeAway HomeAndAway = new MyPresence.HomeAway(DevicesHome, DevicessAway);
                                HomeAndAway.Save();
                            }
                            else
                            {
                                Task.Delay(500);
                            }
                            if (Console.KeyAvailable)
                            {
                                if(Console.ReadKey().Key== ConsoleKey.Escape)
                                {
                                    TokenSource.Cancel();
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "ERROR.txt"), $"\r\n{ex.ToString()}");
                            break;
                        }
                    }

                }, TokenSource.Token);
                task.Start();
                Console.WriteLine("Running... Press Escape to exit");
                Task.WaitAll(task);
            }

            eero.Save();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            TokenSource.Cancel();
        }
    }
}
