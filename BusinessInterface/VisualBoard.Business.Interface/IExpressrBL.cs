using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using VisualBoard.Models;
using VisualBoard.Models.Request;
using static VisualBoard.Models.MoQiuLiObject;
using VisualBoard.Models.Response;
using System.Web;

namespace VisualBoard.Business.Interface
{
    public interface IExpressrBL
    {
        /// <summary>
        /// 墨丘利批量轨迹订阅后，接受墨丘利推送消息的回调接口
        /// </summary>
        /// <param name="callbackResult"></param>
        /// <returns></returns>
        public Task<SubscribeCallBack2Service> MoQiuLliSubscribeCallBack(MoQiuLiObject.SubscribeCallBackResult callbackResult);

        public Task<ResponseObject> MoQiuLliSubscribeFormatXiangXiWuLiu();

        public Task<ResponseObject> ReWriteMoQiuLiTrace();

        /// <summary>
        /// 每日运作报表
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObjectV2 RptRunningDaily(SelectRouteObject indexQuery);
        /// <summary>
        /// 导出每日运作报表
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public string RptRunningDailyExport(SelectRouteObject indexQuery);

        /// <summary>
        /// 2C超时订单预警报表
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObjectV2 RptOrder2CTimeOut(SelectRouteObject indexQuery);
        /// <summary>
        /// 2C超时订单预警报表-各个Tab状态下的订单数量
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObject RptOrder2CTimeOutTabsCount(SelectRouteObject indexQuery);
        /// <summary>
        /// 导出2C超时订单预警报表
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public string RptOrder2CTimeOutExport(SelectRouteObject indexQuery);

        /// <summary>
        /// 2C预警时效维护
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObjectV2 RptOrder2CTimeOutConfig(SelectRouteObject indexQuery);
        /// <summary>
        /// 导出2C预警时效维护
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public List<TwoCTimeOutConfigForExport> RptOrder2CTimeOutConfigExport(SelectRouteObject indexQuery);

        /// <summary>
        /// SaveTimeOutConfig
        /// </summary>
        /// <param name="filePath"></param>
        public int SaveTimeOutConfig(List<TwoCTimeOutConfig> list);
    }
}
