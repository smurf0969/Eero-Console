using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eero_Models
{
    public class EeroAccount
    {
        public string Name { get; set; }
        public Phone Phone { get; set; }
        public EmailAddress Email { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "networks1")]
        public List<Network> Networks { get; set; } = new List<Network>();
        
        public static EeroAccount FromString(string account)
        {
            var o = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(account);
            var d = o.First;
            var a = o.Last.First;
            //EeroAccount eeroAccount = a.ToObject<EeroAccount>();
            EeroAccount eeroAccount = a.ToObject<EeroAccount>();
            var n = a.SelectToken("networks")?.SelectToken("data");
            if(n!=null) eeroAccount.Networks = n.ToObject<List<Network>>();

            return eeroAccount;
        }
    }

}
