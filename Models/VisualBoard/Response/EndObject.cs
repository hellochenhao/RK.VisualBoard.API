using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Response
{
    public class EndObject: acc_WavePick
    {
        public int? AfterState { get; set; }
        public string AfterStateC { get; set; }//完结后的状态中文 

        public string OrderStateNowC { get; set; }//订单最新状态中文 
        public string OrderTypeC { get; set; }//订单类型中文 
    }
}
