using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Request
{
    public class CustmoerSearchObject: bsc_Customer
    {
        public string CustomerCodeLike { get; set; }
        public string CustomerNameLike { get; set; }
        public new bool State { get; set; } = false;
    }
}
