using Rokin.Common.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Business.Service
{
    public static class MqHelper
    {
        public static PublishTools PublishTools = new PublishTools(VirtualHost:"WMS");
        public static void pushmq(object obj, Dictionary<string, object> headerList, string routeKey = "", string exchangeName = "")
        {
            PublishTools.Publish( obj, headerList,  routeKey, exchangeName);
        }
    }
}
