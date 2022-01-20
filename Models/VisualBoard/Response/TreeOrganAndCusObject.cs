using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Response
{
    public class TreeOrganAndCusObject
    {
        public int value { get; set; }
        public string label { get; set; }
        public string icon { get; set; }
        public List<TreeOrganAndCusObject> children { get; set; }
    }
}
