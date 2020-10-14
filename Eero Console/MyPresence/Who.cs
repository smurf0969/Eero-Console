using System;
using System.Collections.Generic;
using System.Text;

namespace Eero_Console.MyPresence
{
    public class Who
    {
        public static List<string> FriendlyNames { get; set; } =new List<string>();
        public Who() { }
        public Who(Eero_Models.Device device)
        {
            Name= !string.IsNullOrWhiteSpace(device.Nickname) ? device.Nickname : device.Hostname;
            string tmp = Name.ToLower();
            foreach(string n in FriendlyNames)
            {
                if (tmp.StartsWith(n.ToLower()))
                {
                    Name = n;
                    break;
                }
            }
            LastSeen = device.Last_Active;
        }

        public string Name { get; set; }
        public DateTime LastSeen { get; set; }
    }
}
