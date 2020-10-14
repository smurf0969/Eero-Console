using System;
using System.Collections.Generic;
using System.Text;

namespace Eero_Models
{
    public class Device_Connectivity
    {
        public string Rx_Bitrate { get; set; }
        public string Signal { get; set; }
        public string Signal_Avg { get; set; }
        public float Score { get; set; }
        public int Score_Bars { get; set; }
        public Device_Interface Interface { get; set; }
    }
}
