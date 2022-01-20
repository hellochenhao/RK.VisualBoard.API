using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Response
{
    public class PieOrderObject
    {

        
        public int? DespatchOrderNum { get; set; }  //已发运单量
        public int? WDespatchOrderNum { get; set; } //未发运单量
        public DateTime? DataTime { get; set; } //数据更新时间
        public Data[] OrderData { get; set; }
    }

    public class Data
    {
        public string name { get; set; } //名称
        public int? data { get; set; } //数据

    }

}
