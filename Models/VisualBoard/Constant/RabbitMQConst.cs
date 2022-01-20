using System;
using System.Collections.Generic;
using System.Text;

namespace VisualBoard.Models.Constant
{
    public class RabbitMQConst
    {
        public const string ZhuzhuyunOrderRoutekey = "rabbit:routeKey:ZhuzhuyunOrderQuery";
        public const string ZhuzhuyunOrderExchange = "rabbit:exchange:ZhuzhuyunOrderQuery";
        public const string ZhuzhuyunExpressRoutekey = "rabbit:routeKey:ZhuzhuyunExpressRouteQuery";
        public const string ZhuzhuyunExpressExchange = "rabbit:exchange:ZhuzhuyunExpressRouteQuery";

        public const string MoLiExpressRoutekey = "rabbit:routingkey:ExpressRouting";
        public const string MoLiExpressExpressExchange = "rabbit:exchange:ExpressRouting";
    }
}
