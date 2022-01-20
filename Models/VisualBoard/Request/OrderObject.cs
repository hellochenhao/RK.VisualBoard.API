using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Request
{
    public class OrderObject
    {
        public int ID { get; set; }
        public string WavePick { get; set; }
        public int? Order { get; set; }
        public int? PCS { get; set; }
        public int? SKU { get; set; }
        public DateTime? OperateTime { get; set; }
        public bool? State { get; set; }
        public int? OrderType { get; set; }
        public int? OrderStateNow { get; set; }

        public int? PeopleNumber { get; set; }
        public string? TableNumber { get; set; }
        public string? BoxNumber { get; set; }
        public string? Remark { get; set; }

        public int? CustomerID { get; set; }

        public string UserID { get; set; }
    
        
    }
}