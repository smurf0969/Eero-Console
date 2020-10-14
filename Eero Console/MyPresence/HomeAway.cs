using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Eero_Console.MyPresence
{
    public class HomeAway
    {
        public HomeAway() { }
        public HomeAway(List<Eero_Models.Device> home, List<Eero_Models.Device> away)
        {
            foreach(Eero_Models.Device device in home)
            {
                WhosHome.Add(new Who(device));
            }
            foreach (Eero_Models.Device device in away)
            {
                WhosAway.Add(new Who(device));
            }
        }
        public List<Who> WhosHome { get; set; } = new List<Who>();
        public List<Who> WhosAway { get; set; } = new List<Who>();

        [Newtonsoft.Json.JsonIgnore]
        public static string FullFilename { get; set; }

        public bool Save()
        {
            if (string.IsNullOrWhiteSpace(FullFilename)) return false;
            try
            {
                FileInfo fi = new FileInfo(FullFilename);
                if (!fi.Exists)
                {
                    if (!Directory.Exists(fi.DirectoryName))
                    {
                        Directory.CreateDirectory(fi.DirectoryName);
                    }
                }
                string json=JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(fi.FullName, json);
                return true;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
    }
}
