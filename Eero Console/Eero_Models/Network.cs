using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eero_Models
{
    public class Network
    {
        public string Id { get; private set; }
        private string _url;
        public string Url { 
            get { return _url; }
            set {
                _url = value;
                if (!string.IsNullOrEmpty(value))
                {
                    Id = value.Substring(value.LastIndexOf("/")+1);
                }
            }
        }
        public string Name { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Access_Expires_On { get; set; }
        public string Amazon_Directed_Id { get; set; }
        public List<Device> Devices { get; set; } = new List<Device>();
        public void SetDevicesFromString(string json)
        {
            var o = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);
            Devices = o.SelectToken("data").ToObject<List<Device>>();
        }
    }
}
