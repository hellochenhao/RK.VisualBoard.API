using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Response
{
    public class TreeOrganObject
    {
        public int value { get; set; }
        public string label { get; set; }
        public List<TreeOrganObject> children { get; set; }
    }
}
