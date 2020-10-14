using System;
using System.Collections.Generic;
using System.Text;

namespace Eero_Models
{
    public class Device
    {
        public string Url { get; set; }
        public string Mac { get; set; }
        public string eui64 { get; set; }
        public string Manufacturer { get; set; }
        public string IP { get; set; }
        public List<string> IPs { get; set; } = new List<string>();
        public string Nickname { get; set; }
        public string Hostname { get; set; }
        public bool Connected { get; set; }
        public bool Wireless { get; set; }
        public string Connection_Type { get; set; }
        public Device_Source Source { get; set; }
        public DateTime Last_Active { get; set; }
        public DateTime First_Active { get; set; }
        public Device_Connectivity Connectivity { get; set; }
        public Device_Usage Usage { get; set; }
        public Profile Profile { get; set; }
        public string Device_Type { get; set; }
        public bool? Blacklisted { get; set; }
        public HomeKit HomeKit { get; set; }
        public bool Is_Guest { get; set; }
        public string Channel { get; set; }
        public string Auth { get; set; }
        public bool Is_Private { get; set; }
    }
}
