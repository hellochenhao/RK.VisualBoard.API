using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Request
{
     public class ManyJHObject
    {
        public int OrderStateNow { get; set; }
        public int CustomerID { get; set; }
        public string UserID { get; set; }
        public data[] data { get; set; }
    }

    public class data
    {
        public string AfterState { get; set; }
        public string AfterStateC { get; set; }
        public string OrderStateNowC { get; set; }
        public string OrderTypeC { get; set; }
        public int ID { get; set; }
        public string WavePick { get; set; }
        public int Order { get; set; }
        public int PCS { get; set; }
        public int SKU { get; set; }
        public int OrderType { get; set; }
        public int CustomerID { get; set; }
    }
}