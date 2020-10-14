using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace Eero_API
{
    public static class eero
    {

        private const string API_URL = "https://api-user.e2ro.com";//"http://www.google.com";
        private const string API_VER = "2.2";
        private const string FILE_NAME = "cookie.json";

        public static string API => API_URL;
        public static string Version => API_VER;

        private static readonly string _filename;
        public static string Filename => _filename;

        private static CookieContainer cookies;
        private static HttpClient client;
        private static HttpClientHandler handler;
        private static Uri uri;

        public static string Identifier { get; set; }
        public static bool Debug { get; set; }

        public static string Account { get; private set; }

        public static readonly Dictionary<string, string> Urls;

        static eero()
        {
            _filename = Path.Combine(AppContext.BaseDirectory, FILE_NAME);

            bool found = File.Exists(_filename);
            Console.WriteLine("Cookie file found: {0}", found);

            uri = new Uri(API_URL);

            Urls = new Dictionary<string, string>()
            {
                {"login",$"/{API_VER}/login" },
                {"login refresh",$"/{API_VER}/login/refresh" },
                {"login verify",$"/{API_VER}/login/verify" },
                {"account",$"/{API_VER}/account" },
                {"networks" ,$"/{API_VER}/networks"},
                {"network devices","/"+API_VER + "/networks/{0}/devices" },
                {"network eeros","/"+API_VER + "/networks/{0}/eeros" },
                {"reboot eero",$"/{API_VER}/eeros/{0}/reboot" }
            };

            cookies = new CookieContainer();

            if (found)
            {
                try
                {
                    var cct = JsonConvert.DeserializeObject<CookieCollection>(File.ReadAllText(_filename));
                    cookies.Add(uri,cct);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            handler.UseCookies = true;
            handler.UseDefaultCredentials = false;

            client = new HttpClient(handler);
            client.BaseAddress = uri;
        }

        public static  bool Login()
        {
            return Login(Identifier);
        }

        public static bool Login(string identifier)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(identifier), "login");
            HttpResponseMessage response=client.PostAsync(Urls["login"], content).Result;
            if (Debug)
            {
                File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "login.txt"), response.Content.ReadAsStringAsync().Result);
            }
            return (response.IsSuccessStatusCode);
        }

        public static bool LoginVerify(string code)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(code), "code");
            HttpResponseMessage response = client.PostAsync(Urls["login verify"], content).Result;
 
       
                Account =response.IsSuccessStatusCode? response.Content.ReadAsStringAsync().Result:string.Empty;
            
           if (Debug)
            {
                File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "login_verify.txt"), response.Content.ReadAsStringAsync().Result);
            }
            return (response.IsSuccessStatusCode);

        }

        public static bool LoginRefresh()
        {
            HttpResponseMessage response = client.PostAsync(Urls["login refresh"], null).Result;
            if (Debug)
            {
                File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "login_refresh.txt"), response.Content.ReadAsStringAsync().Result);
            }
            return (response.IsSuccessStatusCode);
        }

        public static bool IsLoggedIn()
        {
            var cc = cookies.GetCookies(uri);
            foreach(Cookie c in cc)
            {
                if (c.Name == "s") return true;
            }
            try
            {
                if(File.Exists(Filename))File.Delete(Filename);
            }
            catch (Exception) { }
            return false;
        }

        public static bool GetAccount()
        {
            HttpResponseMessage response = client.GetAsync(Urls["account"]).Result;
            if (Debug)
            {
                File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "account.txt"), response.Content.ReadAsStringAsync().Result);
            }
            Account = response.IsSuccessStatusCode ? response.Content.ReadAsStringAsync().Result : string.Empty;
            return (response.IsSuccessStatusCode);
        }

        public static string GetNetworks()
        {
            HttpResponseMessage response = client.GetAsync(Urls["networks"]).Result;
            if (Debug)
            {
                File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "networks.txt"), response.Content.ReadAsStringAsync().Result);
            }
            return response.Content.ReadAsStringAsync().Result;
        }

        public static string GetNetworkDevices(string networkID)
        {
            HttpResponseMessage response = client.GetAsync(string.Format(Urls["network devices"],networkID)).Result;
            if (Debug)
            {
                File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "network_devices.txt"), response.Content.ReadAsStringAsync().Result);
            }
            return response.Content.ReadAsStringAsync().Result;
        }

        public static void Save()
        {
            string j = JsonConvert.SerializeObject(cookies.GetCookies(uri), Formatting.Indented);
            try
            {
                File.WriteAllText(Filename, j);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
