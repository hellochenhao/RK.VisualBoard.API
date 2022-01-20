using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Request
{
    public class SelectOrderInfoObject
    {
        public string WavePick { get; set; }
        public string OrderType { get; set; }
        public string OrderState { get; set; }
        public int Order { get; set; }
        public int PCS { get; set; }
        public int SKU { get; set; }
        public DateTime? STARTTIME { get; set; }
        public DateTime? ENDTIME { get; set; }
        public int? Leadtime { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
    }
}
