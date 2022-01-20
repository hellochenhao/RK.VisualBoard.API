using Dapper;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Rokin.Common.RabbitMQ.Interface;
using Rokin.Common.Tools;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualBoard.Business.Interface;
using VisualBoard.Models;
using VisualBoard.Models.Constant;
using VisualBoard.Models.Request;
using VisualBoard.Models.Response;
using static VisualBoard.Models.MoQiuLiObject;

namespace VisualBoard.Business.Service
{
    public class ExpressrBL : IExpressrBL
    {
        private readonly _WMS_VisualboardContext WMS_Visualboard;
        private readonly IDbConnection connection;
        //private readonly IRabbitMQHelper publish;

        public ExpressrBL(_WMS_VisualboardContext _WMS_Visualboard, IDbConnection _connection, HttpHelperAsync httpHelper)
        {
            WMS_Visualboard = _WMS_Visualboard;
            connection = _connection;
            //this.publish = publish;
        }
        //public ExpressrBL(_WMS_VisualboardContext _WMS_Visualboard, IDbConnection _connection, HttpHelperAsync httpHelper, IRabbitMQHelper publish)
        //{
        //    WMS_Visualboard = _WMS_Visualboard;
        //    connection = _connection;
        //    this.publish = publish;
        //}

        public async Task<SubscribeCallBack2Service> MoQiuLliSubscribeCallBack(MoQiuLiObject.SubscribeCallBackResult callbackResult)
        {
            SubscribeCallBack2Service respObj = new SubscribeCallBack2Service();
            //int result = 0;
            //string sql = string.Empty;

            try
            {
                string jsonText = Newtonsoft.Json.JsonConvert.SerializeObject(callbackResult);
                //publish.PublishToQuery(jsonText, null, RabbitMQConst.MoLiExpressRoutekey, RabbitMQConst.MoLiExpressExpressExchange);
                /*   ReuestObj.RequestCount += 1;
                   if (ReuestObj.RequestCount % 100 == 0)
                       Console.WriteLine(ReuestObj.RequestCount + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));*/
                respObj.code = "000";
                respObj.message = "接收并写入墨丘利订阅推送数据成功！";

                return respObj;



                #region
                //#region 写入ex_moqiuli_callback_log
                //try
                //{
                //    sql = $"insert into ex_moqiuli_callback_log(reqText,isReq) values ('{jsonText}', 0)";
                //    result = connection.Execute(sql);
                //}
                //catch (Exception ex)
                //{
                //    throw ex;
                //}
                //#endregion

                //var sqlBuilder = new StringBuilder();
                //if (callbackResult == null)
                //{
                //    respObj.code = 1;
                //    respObj.message = "墨丘利回调推送数据=null";
                //    return respObj;
                //}

                ////第一个物流节点
                //var firstNode = callbackResult.data.traceOpNodes.OrderBy(t => t.opTime).First();
                ////最新物流节点
                //var latestNode = callbackResult.data.traceOpNodes.OrderByDescending(t => t.opTime).First();

                //#region 写入墨丘利快递路由明细表
                //if (callbackResult.code == "000")
                //{
                //    string xiangxiwuliu = string.Join("<br>", callbackResult.data.traceOpNodes.Select(p => $@"<li>{p.opTime}</li> | {p.opType} | {p.opMemo}"));

                //    sqlBuilder.Append("insert into ex_moqiuli_trace_v2(kdgs,kddh, jsonText, code, message,traceId, opType, opTime,xiangxiwuliu) values");
                //    sqlBuilder.Append($"('{callbackResult.data.expressCompanyCode}'," +
                //                        $"'{callbackResult.data.wayBillNo}'," +
                //                        $"'{jsonText}', " +
                //                        $"'{callbackResult.code}'," +
                //                        $"'{callbackResult.message}', " +
                //                        $"'{callbackResult.traceId}', " +
                //                        $"'{latestNode.opType}', " +
                //                        $"'{latestNode.opTime:yyyy-MM-dd HH:mm:ss}', " +
                //                        $"'{xiangxiwuliu}')");
                //    sqlBuilder.Append(" ON DUPLICATE KEY UPDATE " +
                //                         "kdgs = VALUES(kdgs)" +
                //                        ",kddh = VALUES(kddh)" +
                //                        ",jsonText = VALUES(jsonText)" +
                //                        ",code = VALUES(code)" +
                //                        ",message = VALUES(message)" +
                //                        ",traceId = VALUES(traceId)" +
                //                        ",opType = VALUES(opType)" +
                //                        ",opTime = VALUES(opTime)" +
                //                        ",xiangxiwuliu = VALUES(xiangxiwuliu)" +
                //                        ",UpdateTime = VALUES(UpdateTime);");
                //}
                //else
                //{
                //    sqlBuilder.Append("insert into ex_moqiuli_trace_v2(jsonText, code, message,traceId) values");
                //    sqlBuilder.Append($"('{jsonText}', " +
                //                        $"'{callbackResult.code}'," +
                //                        $"'{callbackResult.message}', " +
                //                        $"'{callbackResult.traceId}')");
                //    sqlBuilder.Append(" ON DUPLICATE KEY UPDATE " +
                //                         "kdgs = VALUES(kdgs)" +
                //                        ",kddh = VALUES(kddh)" +
                //                        ",jsonText = VALUES(jsonText)" +
                //                        ",code = VALUES(code)" +
                //                        ",message = VALUES(message)" +
                //                        ",traceId = VALUES(traceId)" +
                //                        ",opType = VALUES(opType)" +
                //                        ",opTime = VALUES(opTime)" +
                //                        ",UpdateTime = VALUES(UpdateTime);");
                //}
                //result = connection.Execute(sqlBuilder.ToString());
                //#endregion

                //respObj.code = 0;
                //respObj.message = "接收并写入墨丘利订阅推送数据成功！";
                //return respObj;
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
                respObj.code = "111";
                respObj.message = ex.Message;

                string Insertsql = $@"insert into bi_api_log (title, errText) values (,'墨丘利批量订阅回调接口--错误','{Newtonsoft.Json.JsonConvert.SerializeObject(ex)}')";

                connection.Execute(Insertsql, commandTimeout: 6000);
            }

            return respObj;
        }

        public async Task<ResponseObject> MoQiuLliSubscribeFormatXiangXiWuLiu()
        {
            ResponseObject respObj = new ResponseObject();
            List<XiangXiWuLiu> list = new List<XiangXiWuLiu>();

            try
            {

                string sql = "select id,kdgs,kddh, jsonText from ex_moqiuli_trace_v2 where jsonText is not null and xiangxiwuliu is null limit 50000;";
                var query = connection.Query(sql, commandTimeout: 6000);
                foreach (var q in query)
                {
                    string jsonText = q.jsonText; //快递公司名称

                    if (string.IsNullOrWhiteSpace(jsonText))
                        continue;

                    try
                    {
                        MoQiuLiObject.SubscribeCallBackResult callbackResult = Newtonsoft.Json.JsonConvert.DeserializeObject<MoQiuLiObject.SubscribeCallBackResult>(jsonText);
                        string xiangxiwuliu = string.Join("<br>", callbackResult.data.traceOpNodes.Select(p => $@"<li>{p.opTime}</li> | {p.opType} | {p.opMemo}"));

                        XiangXiWuLiu item = new XiangXiWuLiu();
                        item.id = q.id;
                        item.xiangxiwuliu = xiangxiwuliu;

                        list.Add(item);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (list.Count() > 0)
                {
                    string Insertsql = "insert into ex_moqiuli_trace_v2(id,kdgs,kddh, xiangxiwuliu) values";
                    Insertsql += string.Join(",", list.Select(p => $@"({p.id},'{p.kdgs}','{p.kddh}','{p.xiangxiwuliu}')"));
                    Insertsql += " ON DUPLICATE KEY UPDATE xiangxiwuliu = VALUES(xiangxiwuliu)";

                    int num = connection.Execute(Insertsql, commandTimeout: 6000);
                    if (num >= 0)
                    {
                        list.Clear();
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return respObj;
        }

        public class XiangXiWuLiu
        {
            public long id { get; set; }
            public string kdgs { get; set; }
            public string kddh { get; set; }
            public string xiangxiwuliu { get; set; }
        }

        public async Task<ResponseObject> ReWriteMoQiuLiTrace()
        {
            string jsonText = string.Empty;
            ResponseObject respObj = new ResponseObject();
            SubscribeCallBackResult callbackResult = null;
            long fid = 1;
            string sql = $"select id,reqText,writeTime from ex_moqiuli_callback_log where id>={fid} and isReq=0 and startTime>='2021-11-07 00:00:00' order by id limit 5000;";

            try
            {
                var query = connection.Query<dynamic>(sql, commandTimeout: 6000);
                while (query.Count() > 0)
                {
                    fid = query.Max(t => t.id);

                    var sqlBuilder = new StringBuilder();
                    sqlBuilder.Append("insert into ex_moqiuli_trace_v3(kdgs,kddh, jsonText, message,traceId, opType, opTime,xiangxiwuliu, createtime, updatetime, firstNode, firstNodeTime) values");

                    foreach (var row in query)
                    {
                        try
                        {
                            DateTime? writeTime = row.writeTime;
                            callbackResult = null;
                            jsonText = row.reqText;
                            jsonText = jsonText.Replace("\r\n", "").Replace("\t", "").Replace(" ", "");

                            callbackResult = Newtonsoft.Json.JsonConvert.DeserializeObject<SubscribeCallBackResult>(jsonText);

                            //第一个物流节点
                            var firstNode = callbackResult.data.traceOpNodes.OrderBy(t => t.opTime).First();
                            //最新物流节点
                            var latestNode = callbackResult.data.traceOpNodes.OrderByDescending(t => t.opTime).First();
                            string xiangxiwuliu = string.Join("<br>", callbackResult.data.traceOpNodes.Select(p => $@"<li>{p.opTime}</li> | {p.opType} | {p.opMemo}"));

                            sqlBuilder.Append($"('{callbackResult.data.expressCompanyCode}'," +
                                                $"'{callbackResult.data.wayBillNo}'," +
                                                $"'{jsonText}', " +
                                                //$"'{callbackResult.code}'," +
                                                $"'{callbackResult.message}', " +
                                                $"'{callbackResult.traceId}', " +
                                                $"'{latestNode.opType}', " +
                                                $"'{latestNode.opTime:yyyy-MM-dd HH:mm:ss}', " +
                                                $"'{xiangxiwuliu}', " +
                                                $"'{writeTime:yyyy-MM-dd HH:mm:ss}', " +
                                                $"'{writeTime:yyyy-MM-dd HH:mm:ss}', " +
                                                $"'{firstNode.opType}', " +
                                                $"'{firstNode.opTime:yyyy-MM-dd HH:mm:ss}'),");
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    string __sql = sqlBuilder.Remove(sqlBuilder.Length - 1, 1).ToString();
                    __sql += " ON DUPLICATE KEY UPDATE " +
                                         "kdgs = VALUES(kdgs)" +
                                        ",kddh = VALUES(kddh)" +
                                        ",jsonText = VALUES(jsonText)" +
                                        ",code = VALUES(code)" +
                                        ",message = VALUES(message)" +
                                        ",traceId = VALUES(traceId)" +
                                        ",opType = VALUES(opType)" +
                                        ",opTime = VALUES(opTime)" +
                                        ",xiangxiwuliu = VALUES(xiangxiwuliu)" +
                                        ",firstNode = VALUES(firstNode)" +
                                        ",firstNodeTime = VALUES(firstNodeTime)" +
                                        ",UpdateTime = VALUES(UpdateTime);";

                    int val = connection.Execute(__sql, commandTimeout: 6000);

                    sql = $"select id,reqText from ex_moqiuli_callback_log where id>={fid} and isReq=0 and startTime>='2021-11-07 00:00:00' order by id limit 1000;";
                    query = connection.Query<dynamic>(sql, commandTimeout: 6000);
                }

            }
            catch (Exception ex)
            {

            }

            respObj.code = 0;
            respObj.message = "成功！";
            return respObj;
        }

        #region 报表相关
        /// <summary>
        /// 每日运作报表
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObjectV2 RptRunningDaily(SelectRouteObject indexQuery)
        {
            ResponseObjectV2 result = new ResponseObjectV2();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();

            try
            {
                if (indexQuery.PageIndex <= 0)
                    indexQuery.PageIndex = 1;
                if (indexQuery.PageSize <= 0)
                    indexQuery.PageSize = 20;

                string Sql = $@"
select count(1) 
from bi_shipment s  
where 1=1 
        and s.id>398612172 
 ";
                #region 查询条件
                //转单时间
                if (indexQuery.OrderCreateTimeStart != null)
                {
                    Sql += $" and s.OrderCreateTime >= '{indexQuery.OrderCreateTimeStart:yyyy-MM-dd HH:mm:ss}' ";
                }
                if (indexQuery.OrderCreateTimeEnd != null)
                {
                    Sql += $" and s.OrderCreateTime < '{indexQuery.OrderCreateTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
                }
                //付款时间
                if (indexQuery.PayTimeStart != null)
                {
                    Sql += $" and s.PayTime >= '{indexQuery.PayTimeStart:yyyy-MM-dd HH:mm:ss}' ";
                }
                if (indexQuery.PayTimeEnd != null)
                {
                    Sql += $" and s.PayTime < '{indexQuery.PayTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
                }
                //更新时间
                if (indexQuery.UpdatedTimeStart != null)
                {
                    Sql += $" and s.LastUpdateTime >= '{indexQuery.UpdatedTimeStart:yyyy-MM-dd HH:mm:ss}' ";
                }
                if (indexQuery.UpdatedTimeEnd != null)
                {
                    Sql += $" and s.LastUpdateTime < '{indexQuery.UpdatedTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
                }
                //订单号
                if (!string.IsNullOrWhiteSpace(indexQuery.OutboundCode))
                {
                    Sql += $" and s.OutboundCode like '{indexQuery.OutboundCode}%' ";
                }
                //快递单号
                if (!string.IsNullOrWhiteSpace(indexQuery.kddh))
                {
                    Sql += $" and s.kddh like '{indexQuery.kddh}%' ";
                }
                //快递单号（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.kddhStr))
                {
                    string kddhStr = string.Join("','", indexQuery.kddhStr.Split(','));
                    Sql += $" and s.kddh in ('{kddhStr}') ";
                }
                //快递公司（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.kdgsStr))
                {
                    string kdgsStr = string.Join("','", indexQuery.kdgsStr.Split(','));
                    Sql += $" and s.kdgs in ('{kdgsStr}') ";
                }
                //仓库（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.WarehouseCodeStr))
                {
                    string WarehouseCodeStr = string.Join("','", indexQuery.WarehouseCodeStr.Split(','));
                    Sql += $" and s.WarehouseCode in ('{WarehouseCodeStr}') ";
                }
                //货主（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.CompanyCodeStr))
                {
                    string CompanyCodeStr = string.Join("','", indexQuery.CompanyCodeStr.Split(','));
                    Sql += $" and s.CompanyCode in ('{CompanyCodeStr}') ";
                }
                //来源平台（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.SourcePlatformStr))
                {
                    string SourcePlatformStr = string.Join("','", indexQuery.SourcePlatformStr.Split(','));
                    Sql += $" and s.SourcePlatform in ('{SourcePlatformStr}') ";
                }
                //店铺名称（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.StoreNameStr))
                {
                    string StoreNameStr = string.Join("','", indexQuery.StoreNameStr.Split(','));
                    Sql += $" and s.StoreName in ('{StoreNameStr}') ";
                }
                //首状态
                if (!string.IsNullOrWhiteSpace(indexQuery.leadingSts))
                {
                    string leadingSts = string.Join("','", indexQuery.leadingSts.Split(','));
                    Sql += $" and s.LeadingSts in ('{leadingSts}') ";
                }
                //尾状态
                if (!string.IsNullOrWhiteSpace(indexQuery.trailingSts))
                {
                    string trailingSts = string.Join("','", indexQuery.trailingSts.Split(','));
                    Sql += $" and s.TrailingSts in ('{trailingSts}') ";
                }
                //分公司
                if (!string.IsNullOrWhiteSpace(indexQuery.OrgName))
                {
                    string OrgName = string.Join("','", indexQuery.OrgName.Split(','));
                    Sql += $" and s.OrgName in ('{OrgName}') ";
                }
                if (!string.IsNullOrWhiteSpace(indexQuery.ProcessType))
                {
                    string ProcessType = string.Join("','", indexQuery.ProcessType.Split(','));
                    Sql += $" and s.ProcessType in ('{ProcessType}') ";
                }
                //来源系统
                if (!string.IsNullOrWhiteSpace(indexQuery.SourceSys))
                {
                    string SourceSys = string.Join("','", indexQuery.SourceSys.Split(','));
                    Sql += $" and s.SourceSys in ('{SourceSys}') ";
                }
                //大类分类
                if (!string.IsNullOrWhiteSpace(indexQuery.BigClass))
                {
                    string BigClass = string.Join("','", indexQuery.BigClass.Split(','));
                    Sql += $@" and s.BigClass in ('{BigClass}') ";
                }

                //省
                if (!string.IsNullOrWhiteSpace(indexQuery.Privince))
                {
                    Sql += $" and s.Privince like '{indexQuery.Privince}%' ";
                }
                //市
                if (!string.IsNullOrWhiteSpace(indexQuery.City))
                {
                    Sql += $" and s.City like '{indexQuery.City}%' ";
                }
                //区
                if (!string.IsNullOrWhiteSpace(indexQuery.District))
                {
                    Sql += $" and s.District like '{indexQuery.District}%' ";
                }
                //ERP单号
                if (!string.IsNullOrWhiteSpace(indexQuery.erpOrderCode))
                {
                    string erpOrderCode = string.Join("','", indexQuery.erpOrderCode.Split(','));
                    Sql += $@" and s.erpOrderCode in ('{erpOrderCode}') ";
                }
                //订单类型
                if (!string.IsNullOrWhiteSpace(indexQuery.shipmentType))
                {
                    string shipmentType = string.Join("','", indexQuery.shipmentType.Split(','));
                    Sql += $@" and s.shipmentType in ('{shipmentType}') ";
                }

                #endregion

                result.TotalRows = (long)connection.ExecuteScalar(Sql);
                if (indexQuery.PageSize > 0)
                    result.TotalPages = (int)Math.Ceiling(result.TotalRows * 1.00 / indexQuery.PageSize);
                else
                    result.TotalPages = 1;

                Sql = Sql.Replace("select count(1)", @"select s.*,s.fachushijian as 'lanshoushijian' ");
                #region 排序条件
                if (!string.IsNullOrWhiteSpace(indexQuery.OrderBy))
                {
                    Sql += $"order by {indexQuery.OrderBy}";
                }
                else
                {
                    Sql += $"order by s.CreateTime desc ";
                }
                #endregion

                if (indexQuery.PageIndex > 0 && indexQuery.PageSize > 0)
                {
                    Sql += $"limit {(indexQuery.PageIndex - 1) * indexQuery.PageSize},{indexQuery.PageSize}";
                }

                dynamic tb = connection.Query<dynamic>(Sql, commandTimeout: 600).ToArray();

                /// 大促看板
                keyValues.Add("Rows", tb);

                result.result = keyValues;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 每日运作报表导出
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <param name="DCname"></param>
        public string RptRunningDailyExport(SelectRouteObject query)
        {
            //根据ReportName，通知Excel处理中心跳转到哪个报表导出方法里。可以不用每增加一个报表导出就要新开一个MQ队列
            query.ReportName = "每日运作报表";
            //publish.PublishToQuery(query, null, RabbitMQConst.ZhuzhuyunOrderRoutekey, RabbitMQConst.ZhuzhuyunOrderExchange);

            return "请求成功";
        }

        /// <summary>
        /// 2C超时订单预警报表
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObjectV2 RptOrder2CTimeOut(SelectRouteObject indexQuery)
        {
            ResponseObjectV2 result = new ResponseObjectV2();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();

            try
            {
                if (indexQuery.PageIndex <= 0)
                    indexQuery.PageIndex = 1;
                if (indexQuery.PageSize <= 0)
                    indexQuery.PageSize = 20;

                string Sql = @"
select count(1) 
from bi_shipment s 
{0}
where 1=1 
        and s.id>398612172 
 ";
                #region 查询条件
                //转单时间
                if (indexQuery.OrderCreateTimeStart != null)
                {
                    Sql += $" and s.OrderCreateTime >= '{indexQuery.OrderCreateTimeStart:yyyy-MM-dd HH:mm:ss}' ";
                }
                if (indexQuery.OrderCreateTimeEnd != null)
                {
                    Sql += $" and s.OrderCreateTime < '{indexQuery.OrderCreateTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
                }
                //付款时间
                if (indexQuery.PayTimeStart != null)
                {
                    Sql += $" and s.PayTime >= '{indexQuery.PayTimeStart:yyyy-MM-dd HH:mm:ss}' ";
                }
                if (indexQuery.PayTimeEnd != null)
                {
                    Sql += $" and s.PayTime < '{indexQuery.PayTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
                }
                //更新时间
                if (indexQuery.UpdatedTimeStart != null)
                {
                    Sql += $" and s.LastUpdateTime >= '{indexQuery.UpdatedTimeStart:yyyy-MM-dd HH:mm:ss}' ";
                }
                if (indexQuery.UpdatedTimeEnd != null)
                {
                    Sql += $" and s.LastUpdateTime < '{indexQuery.UpdatedTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
                }
                //订单号
                if (!string.IsNullOrWhiteSpace(indexQuery.OutboundCode))
                {
                    Sql += $" and s.OutboundCode like '{indexQuery.OutboundCode}%' ";
                }
                //快递单号
                if (!string.IsNullOrWhiteSpace(indexQuery.kddh))
                {
                    Sql += $" and s.kddh like '{indexQuery.kddh}%' ";
                }
                //快递单号（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.kddhStr))
                {
                    string kddhStr = string.Join("','", indexQuery.kddhStr.Split(','));
                    Sql += $" and s.kddh in ('{kddhStr}') ";
                }
                //快递公司（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.kdgsStr))
                {
                    string kdgsStr = string.Join("','", indexQuery.kdgsStr.Split(','));
                    Sql += $" and s.kdgs in ('{kdgsStr}') ";
                }
                //仓库（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.WarehouseCodeStr))
                {
                    string WarehouseCodeStr = string.Join("','", indexQuery.WarehouseCodeStr.Split(','));
                    Sql += $" and s.WarehouseCode in ('{WarehouseCodeStr}') ";
                }
                //货主（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.CompanyCodeStr))
                {
                    string CompanyCodeStr = string.Join("','", indexQuery.CompanyCodeStr.Split(','));
                    Sql += $" and s.CompanyCode in ('{CompanyCodeStr}') ";
                }
                //来源平台（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.SourcePlatformStr))
                {
                    string SourcePlatformStr = string.Join("','", indexQuery.SourcePlatformStr.Split(','));
                    Sql += $" and s.SourcePlatform in ('{SourcePlatformStr}') ";
                }
                //店铺名称（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.StoreNameStr))
                {
                    string StoreNameStr = string.Join("','", indexQuery.StoreNameStr.Split(','));
                    Sql += $" and s.StoreName in ('{StoreNameStr}') ";
                }
                //首状态
                if (!string.IsNullOrWhiteSpace(indexQuery.leadingSts))
                {
                    string leadingSts = string.Join("','", indexQuery.leadingSts.Split(','));
                    Sql += $" and s.LeadingSts in ('{leadingSts}') ";
                }
                //尾状态
                if (!string.IsNullOrWhiteSpace(indexQuery.trailingSts))
                {
                    string trailingSts = string.Join("','", indexQuery.trailingSts.Split(','));
                    Sql += $" and s.TrailingSts in ('{trailingSts}') ";
                }
                //分公司
                if (!string.IsNullOrWhiteSpace(indexQuery.OrgName))
                {
                    string OrgName = string.Join("','", indexQuery.OrgName.Split(','));
                    Sql += $" and s.OrgName in ('{OrgName}') ";
                }
                if (!string.IsNullOrWhiteSpace(indexQuery.ProcessType))
                {
                    string ProcessType = string.Join("','", indexQuery.ProcessType.Split(','));
                    Sql += $" and s.ProcessType in ('{ProcessType}') ";
                }
                //来源系统
                if (!string.IsNullOrWhiteSpace(indexQuery.SourceSys))
                {
                    string SourceSys = string.Join("','", indexQuery.SourceSys.Split(','));
                    Sql += $" and s.SourceSys in ('{SourceSys}') ";
                }
                //大类分类
                if (!string.IsNullOrWhiteSpace(indexQuery.BigClass))
                {
                    string BigClass = string.Join("','", indexQuery.BigClass.Split(','));
                    Sql += $@" and s.BigClass in ('{BigClass}') ";
                }

                //省
                if (!string.IsNullOrWhiteSpace(indexQuery.Privince))
                {
                    Sql += $" and s.Privince like '{indexQuery.Privince}%' ";
                }
                //市
                if (!string.IsNullOrWhiteSpace(indexQuery.City))
                {
                    Sql += $" and s.City like '{indexQuery.City}%' ";
                }
                //区
                if (!string.IsNullOrWhiteSpace(indexQuery.District))
                {
                    Sql += $" and s.District like '{indexQuery.District}%' ";
                }
                //ERP单号
                if (!string.IsNullOrWhiteSpace(indexQuery.erpOrderCode))
                {
                    string erpOrderCode = string.Join("','", indexQuery.erpOrderCode.Split(','));
                    Sql += $@" and s.erpOrderCode in ('{erpOrderCode}') ";
                }
                //订单类型
                if (!string.IsNullOrWhiteSpace(indexQuery.shipmentType))
                {
                    string shipmentType = string.Join("','", indexQuery.shipmentType.Split(','));
                    Sql += $@" and s.shipmentType in ('{shipmentType}') ";
                }

                #endregion

                string _Sql = null;
                //dynamic _tb = null;
                decimal _timeSpan = 0;
                switch (indexQuery.TabGroupTitle)
                {
                    case "付款-揽收预警":
                        _Sql = $"select timespan from ex_2c_timeout_config where title = '{indexQuery.TabGroupTitle}'";
                        _timeSpan = (decimal)connection.ExecuteScalar(Sql, commandTimeout: 600);
                        Sql = string.Format(Sql, "");
                        Sql += $" and s.FK2LS>{_timeSpan} and s.FK2LS<24";
                        break;
                    case "付款-揽收超时":
                        _Sql = $"select timespan from ex_2c_timeout_config where title = '{indexQuery.TabGroupTitle}'";
                        _timeSpan = (decimal)connection.ExecuteScalar(Sql, commandTimeout: 600);
                        Sql = string.Format(Sql, "");
                        Sql += $" and s.FK2LS>{_timeSpan}";
                        break;
                    case "发运-揽收预警":
                        _Sql = $"select timespan from ex_2c_timeout_config where title = '{indexQuery.TabGroupTitle}'";
                        _timeSpan = (decimal)connection.ExecuteScalar(Sql, commandTimeout: 600);
                        Sql = string.Format(Sql, "");
                        Sql += $" and s.FH2LS>{_timeSpan} and s.FH2LS<24";
                        break;
                    case "发运-揽收超时":
                        _Sql = $"select timespan from ex_2c_timeout_config where title = '{indexQuery.TabGroupTitle}'";
                        _timeSpan = (decimal)connection.ExecuteScalar(Sql, commandTimeout: 600);
                        Sql = string.Format(Sql, "");
                        Sql += $" and s.FH2LS>{_timeSpan}";
                        break;
                    case "揽收-中转超时":
                        _Sql = $"select timespan from ex_2c_timeout_config where title = '{indexQuery.TabGroupTitle}'";
                        _timeSpan = (decimal)connection.ExecuteScalar(Sql, commandTimeout: 600);
                        Sql = string.Format(Sql, "");
                        Sql += $" and s.LS2ZY>{_timeSpan}";
                        break;
                    case "揽收-签收超时":
                        Sql = string.Format(Sql, "left join ex_2c_timeout_config t on t.CompanyName=s.CompanyName and t.Privince=s.Privince");
                        Sql += $" and s.LS2QS>t.timeSpan ";
                        break;
                    default:
                        Sql = string.Format(Sql, "");
                        break;
                }

                result.TotalRows = (long)connection.ExecuteScalar(Sql);

                if (indexQuery.PageSize > 0)
                    result.TotalPages = (int)Math.Ceiling(result.TotalRows * 1.00 / indexQuery.PageSize);
                else
                    result.TotalPages = 1;

                Sql = Sql.Replace("select count(1)", @"select s.*,s.fachushijian as 'lanshoushijian' ");
                #region 排序条件
                if (!string.IsNullOrWhiteSpace(indexQuery.OrderBy))
                {
                    Sql += $"order by {indexQuery.OrderBy}";
                }
                else
                {
                    Sql += $"order by s.CreateTime desc ";
                }
                #endregion

                if (indexQuery.PageIndex > 0 && indexQuery.PageSize > 0)
                {
                    Sql += $"limit {(indexQuery.PageIndex - 1) * indexQuery.PageSize},{indexQuery.PageSize}";
                }

                dynamic tb = connection.Query<dynamic>(Sql, commandTimeout: 600).ToArray();

                /// 大促看板
                keyValues.Add("Rows", tb);

                result.result = keyValues;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 2C超时订单预警报表-各个Tab状态下的订单数量
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObject RptOrder2CTimeOutTabsCount(SelectRouteObject indexQuery)
        {
            ResponseObject result = new ResponseObject();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            try
            {
                string Sql = @"select 
(select timespan
from ex_2c_timeout_config
where title = '付款-揽收预警') as 'class1'
,(select timespan
from ex_2c_timeout_config
where title = '付款-揽收超时') as 'class2'
,(select timespan
from ex_2c_timeout_config
where title = '发运-揽收预警') as 'class3'
,(select timespan
from ex_2c_timeout_config
where title = '发运-揽收超时') as 'class4'
,(select timespan
from ex_2c_timeout_config
where title = '揽收-中转超时') as 'class5'
from dual
";
                dynamic tb = connection.Query<dynamic>(Sql, commandTimeout: 600).ToArray();
                decimal class1_TimeSpan = tb[0].class1;
                decimal class2_TimeSpan = tb[0].class2;
                decimal class3_TimeSpan = tb[0].class3;
                decimal class4_TimeSpan = tb[0].class4;
                decimal class5_TimeSpan = tb[0].class5;

                Sql = $@"
select count(1) as 'all' 
        ,ifnull(sum(case when s.FK2LS>{class1_TimeSpan} and s.FK2LS<24 then 1 else 0 end),0) as '付款揽收预警'
        ,ifnull(sum(case when s.FK2LS>{class2_TimeSpan} then 1 else 0 end),0) as '付款揽收超时'
        ,ifnull(sum(case when s.FH2LS>{class3_TimeSpan} and s.FH2LS<24 then 1 else 0 end),0) as '发运揽收预警'
        ,ifnull(sum(case when s.FH2LS>{class4_TimeSpan} then 1 else 0 end),0) as '发运揽收超时'
        ,ifnull(sum(case when s.LS2ZY>{class5_TimeSpan} then 1 else 0 end),0) as '揽收中转超时'
        ,ifnull(sum(case when s.LS2QS>t.timeSpan then 1 else 0 end),0) as '揽收签收超时' 
from bi_shipment s  
left join ex_2c_timeout_config t on t.CompanyName=s.CompanyName and t.Privince=s.Privince 
where 1=1 
        and s.id>398612172 
 ";
                #region 查询条件
                //转单时间
                if (indexQuery.OrderCreateTimeStart != null)
                {
                    Sql += $" and s.OrderCreateTime >= '{indexQuery.OrderCreateTimeStart:yyyy-MM-dd HH:mm:ss}' ";
                }
                if (indexQuery.OrderCreateTimeEnd != null)
                {
                    Sql += $" and s.OrderCreateTime < '{indexQuery.OrderCreateTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
                }
                //付款时间
                if (indexQuery.PayTimeStart != null)
                {
                    Sql += $" and s.PayTime >= '{indexQuery.PayTimeStart:yyyy-MM-dd HH:mm:ss}' ";
                }
                if (indexQuery.PayTimeEnd != null)
                {
                    Sql += $" and s.PayTime < '{indexQuery.PayTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
                }
                //更新时间
                if (indexQuery.UpdatedTimeStart != null)
                {
                    Sql += $" and s.LastUpdateTime >= '{indexQuery.UpdatedTimeStart:yyyy-MM-dd HH:mm:ss}' ";
                }
                if (indexQuery.UpdatedTimeEnd != null)
                {
                    Sql += $" and s.LastUpdateTime < '{indexQuery.UpdatedTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
                }
                //订单号
                if (!string.IsNullOrWhiteSpace(indexQuery.OutboundCode))
                {
                    Sql += $" and s.OutboundCode like '{indexQuery.OutboundCode}%' ";
                }
                //快递单号
                if (!string.IsNullOrWhiteSpace(indexQuery.kddh))
                {
                    Sql += $" and s.kddh like '{indexQuery.kddh}%' ";
                }
                //快递单号（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.kddhStr))
                {
                    string kddhStr = string.Join("','", indexQuery.kddhStr.Split(','));
                    Sql += $" and s.kddh in ('{kddhStr}') ";
                }
                //快递公司（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.kdgsStr))
                {
                    string kdgsStr = string.Join("','", indexQuery.kdgsStr.Split(','));
                    Sql += $" and s.kdgs in ('{kdgsStr}') ";
                }
                //仓库（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.WarehouseCodeStr))
                {
                    string WarehouseCodeStr = string.Join("','", indexQuery.WarehouseCodeStr.Split(','));
                    Sql += $" and s.WarehouseCode in ('{WarehouseCodeStr}') ";
                }
                //货主（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.CompanyCodeStr))
                {
                    string CompanyCodeStr = string.Join("','", indexQuery.CompanyCodeStr.Split(','));
                    Sql += $" and s.CompanyCode in ('{CompanyCodeStr}') ";
                }
                //来源平台（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.SourcePlatformStr))
                {
                    string SourcePlatformStr = string.Join("','", indexQuery.SourcePlatformStr.Split(','));
                    Sql += $" and s.SourcePlatform in ('{SourcePlatformStr}') ";
                }
                //店铺名称（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.StoreNameStr))
                {
                    string StoreNameStr = string.Join("','", indexQuery.StoreNameStr.Split(','));
                    Sql += $" and s.StoreName in ('{StoreNameStr}') ";
                }
                //首状态
                if (!string.IsNullOrWhiteSpace(indexQuery.leadingSts))
                {
                    string leadingSts = string.Join("','", indexQuery.leadingSts.Split(','));
                    Sql += $" and s.LeadingSts in ('{leadingSts}') ";
                }
                //尾状态
                if (!string.IsNullOrWhiteSpace(indexQuery.trailingSts))
                {
                    string trailingSts = string.Join("','", indexQuery.trailingSts.Split(','));
                    Sql += $" and s.TrailingSts in ('{trailingSts}') ";
                }
                //分公司
                if (!string.IsNullOrWhiteSpace(indexQuery.OrgName))
                {
                    string OrgName = string.Join("','", indexQuery.OrgName.Split(','));
                    Sql += $" and s.OrgName in ('{OrgName}') ";
                }
                if (!string.IsNullOrWhiteSpace(indexQuery.ProcessType))
                {
                    string ProcessType = string.Join("','", indexQuery.ProcessType.Split(','));
                    Sql += $" and s.ProcessType in ('{ProcessType}') ";
                }
                //来源系统
                if (!string.IsNullOrWhiteSpace(indexQuery.SourceSys))
                {
                    string SourceSys = string.Join("','", indexQuery.SourceSys.Split(','));
                    Sql += $" and s.SourceSys in ('{SourceSys}') ";
                }
                //大类分类
                if (!string.IsNullOrWhiteSpace(indexQuery.BigClass))
                {
                    string BigClass = string.Join("','", indexQuery.BigClass.Split(','));
                    Sql += $@" and s.BigClass in ('{BigClass}') ";
                }

                //省
                if (!string.IsNullOrWhiteSpace(indexQuery.Privince))
                {
                    Sql += $" and s.Privince like '{indexQuery.Privince}%' ";
                }
                //市
                if (!string.IsNullOrWhiteSpace(indexQuery.City))
                {
                    Sql += $" and s.City like '{indexQuery.City}%' ";
                }
                //区
                if (!string.IsNullOrWhiteSpace(indexQuery.District))
                {
                    Sql += $" and s.District like '{indexQuery.District}%' ";
                }
                //ERP单号
                if (!string.IsNullOrWhiteSpace(indexQuery.erpOrderCode))
                {
                    string erpOrderCode = string.Join("','", indexQuery.erpOrderCode.Split(','));
                    Sql += $@" and s.erpOrderCode in ('{erpOrderCode}') ";
                }
                //订单类型
                if (!string.IsNullOrWhiteSpace(indexQuery.shipmentType))
                {
                    string shipmentType = string.Join("','", indexQuery.shipmentType.Split(','));
                    Sql += $@" and s.shipmentType in ('{shipmentType}') ";
                }

                #endregion

                tb = connection.Query<dynamic>(Sql, commandTimeout: 600).ToArray();
                result.result = tb;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 2C超时订单预警报表导出
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <param name="DCname"></param>
        public string RptOrder2CTimeOutExport(SelectRouteObject query)
        {
            //根据ReportName，通知Excel处理中心跳转到哪个报表导出方法里。可以不用每增加一个报表导出就要新开一个MQ队列
            query.ReportName = "2C超时订单预警报表";
            //publish.PublishToQuery(query, null, RabbitMQConst.ZhuzhuyunOrderRoutekey, RabbitMQConst.ZhuzhuyunOrderExchange);

            return "请求成功";
        }

        /// <summary>
        /// 2C预警时效维护
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObjectV2 RptOrder2CTimeOutConfig(SelectRouteObject indexQuery)
        {
            ResponseObjectV2 result = new ResponseObjectV2();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();

            try
            {
                if (indexQuery.PageIndex <= 0)
                    indexQuery.PageIndex = 1;
                if (indexQuery.PageSize <= 0)
                    indexQuery.PageSize = 20;

                string Sql = @"
select count(1) 
from ex_2c_timeout_config s 
where 1=1 
 ";
                #region 查询条件
                ////货主（可多选）
                if (!string.IsNullOrWhiteSpace(indexQuery.CompanyName))
                {
                    string CompanyName = string.Join("','", indexQuery.CompanyName.Split(','));
                    Sql += $" and s.CompanyName in ('{CompanyName}') ";
                }
                //区
                if (!string.IsNullOrWhiteSpace(indexQuery.Privince))
                {
                    string Privince = string.Join("','", indexQuery.Privince.Split(','));
                    Sql += $" and s.Privince like '{Privince}%' ";
                }
                if (indexQuery.TimeSpan > 0)
                {
                    Sql += $" and s.TimeSpan={indexQuery.TimeSpan} ";
                }

                #endregion

                result.TotalRows = (long)connection.ExecuteScalar(Sql);
                if (indexQuery.PageSize > 0)
                    result.TotalPages = (int)Math.Ceiling(result.TotalRows * 1.00 / indexQuery.PageSize);
                else
                    result.TotalPages = 1;

                Sql = Sql.Replace("select count(1)", @"select s.* ");
                #region 排序条件
                if (!string.IsNullOrWhiteSpace(indexQuery.OrderBy))
                {
                    Sql += $"order by {indexQuery.OrderBy}";
                }
                else
                {
                    Sql += $"order by s.CompanyName,s.Privince ";
                }
                #endregion

                if (indexQuery.PageIndex > 0 && indexQuery.PageSize > 0)
                {
                    Sql += $"limit {(indexQuery.PageIndex - 1) * indexQuery.PageSize},{indexQuery.PageSize}";
                }

                dynamic tb = connection.Query<dynamic>(Sql, commandTimeout: 600).ToArray();

                /// 大促看板
                keyValues.Add("Rows", tb);

                result.result = keyValues;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 2C预警时效维护导出
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <param name="DCname"></param>
        public List<TwoCTimeOutConfigForExport> RptOrder2CTimeOutConfigExport(SelectRouteObject indexQuery)
        {
            string Sql = @"
select *  
from ex_2c_timeout_config s 
where 1=1 and CompanyName is not null 
 ";
            #region 查询条件
            ////货主（可多选）
            if (!string.IsNullOrWhiteSpace(indexQuery.CompanyName))
            {
                string CompanyName = string.Join("','", indexQuery.CompanyName.Split(','));
                Sql += $" and s.CompanyName in ('{CompanyName}') ";
            }
            //区
            if (!string.IsNullOrWhiteSpace(indexQuery.Privince))
            {
                string Privince = string.Join("','", indexQuery.Privince.Split(','));
                Sql += $" and s.Privince like '{Privince}%' ";
            }
            if (indexQuery.TimeSpan > 0)
            {
                Sql += $" and s.TimeSpan={indexQuery.TimeSpan} ";
            }

            #endregion

            #region 排序条件
            if (!string.IsNullOrWhiteSpace(indexQuery.OrderBy))
            {
                Sql += $"order by {indexQuery.OrderBy}";
            }
            else
            {
                Sql += $"order by s.CompanyName,s.Privince ";
            }
            #endregion

            dynamic tb = connection.Query<TwoCTimeOutConfigForExport>(Sql, commandTimeout: 600).ToList();

            return tb;
        }

        /// <summary>
        /// SaveTimeOutConfig
        /// </summary>
        /// <param name="filePath"></param>
        public int SaveTimeOutConfig(List<TwoCTimeOutConfig> list)
        {
            string filePath = string.Empty;

            try
            {
                if (list != null && list.Count() > 0)
                {
                    var sqlBuilder = new StringBuilder();
                    //筛选出全局设置项
                    var query = list.Where(a => a.Title == "付款-揽收预警" 
                                                    || a.Title == "付款-揽收超时" 
                                                    || a.Title == "发运-揽收预警" 
                                                    || a.Title == "发运-揽收超时" 
                                                    || a.Title == "揽收-中转超时").ToList();
                    if (query.Any())
                    {
                        for (int i=0; i<query.Count(); i++)
                        {
                            //单独更新全局设置项
                            sqlBuilder.Append(@$"update ex_2c_timeout_config set timespan={query[i].TimeSpan}, OperateUserID={query[i].OperateUserID}, OperateUserName='{query[i].OperateUserName}' where title='{query[i].Title}';");
                            //去掉以上全局设置项
                            list.Remove(query[i]);
                        }
                    }

                    sqlBuilder.Append(@$"
INSERT INTO `WMS_VisualBoard`.`ex_2c_timeout_config`(title, `companyname`, `privince`, `timespan`, `OperateUserID`, `OperateUserName`) VALUES ");
                    sqlBuilder.Append(string.Join(",", list.Select(p => $@"
(
{(p.Title != null ? "'" + p.Title + "'" : "null")}
,{(p.CompanyName != null ? "'" + p.CompanyName + "'" : "null")}
,{(p.Privince != null ? "'" + p.Privince + "'" : "null")}
,{p.TimeSpan}
,{p.OperateUserID}
,'{p.OperateUserName}'
)")));
                    sqlBuilder.Append(" ON DUPLICATE KEY UPDATE " +
                        "timespan=VALUES(timespan)" +
                        ",OperateUserID=VALUES(OperateUserID)" +
                        ",OperateUserName=VALUES(OperateUserName)" +
                        ";");

                    int num = connection.Execute(sqlBuilder.ToString(), commandTimeout: 6000);
                    if (num >= 0)
                    {
                        return num;
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }

            return 0;
        }

        //        private void _exportRptRunningDaily(SelectRouteObject indexQuery)
        //        {
        //            string DCname = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xlsx";
        //            //SelectRouteObject indexQuery = _indexQuery as SelectRouteObject;

        //            string cretime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff");
        //            ResponseObjectV2 result = new ResponseObjectV2();
        //            Dictionary<string, object> keyValues = new Dictionary<string, object>();
        //            try
        //            {

        //                string Sql = $@"
        //select count(1) 
        //from bi_shipment s  
        //where 1=1 
        //        and s.id>398612172 
        // ";
        //                #region 查询条件
        //                //转单时间
        //                if (indexQuery.OrderCreateTimeStart != null)
        //                {
        //                    Sql += $" and s.OrderCreateTime >= '{indexQuery.OrderCreateTimeStart:yyyy-MM-dd HH:mm:ss}' ";
        //                }
        //                if (indexQuery.OrderCreateTimeEnd != null)
        //                {
        //                    Sql += $" and s.OrderCreateTime < '{indexQuery.OrderCreateTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
        //                }
        //                //付款时间
        //                if (indexQuery.PayTimeStart != null)
        //                {
        //                    Sql += $" and s.PayTime >= '{indexQuery.PayTimeStart:yyyy-MM-dd HH:mm:ss}' ";
        //                }
        //                if (indexQuery.PayTimeEnd != null)
        //                {
        //                    Sql += $" and s.PayTime < '{indexQuery.PayTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
        //                }
        //                //更新时间
        //                if (indexQuery.UpdatedTimeStart != null)
        //                {
        //                    Sql += $" and s.LastUpdateTime >= '{indexQuery.UpdatedTimeStart:yyyy-MM-dd HH:mm:ss}' ";
        //                }
        //                if (indexQuery.UpdatedTimeEnd != null)
        //                {
        //                    Sql += $" and s.LastUpdateTime < '{indexQuery.UpdatedTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
        //                }
        //                //订单号
        //                if (!string.IsNullOrWhiteSpace(indexQuery.OutboundCode))
        //                {
        //                    Sql += $" and s.OutboundCode like '{indexQuery.OutboundCode}%' ";
        //                }
        //                //快递单号
        //                if (!string.IsNullOrWhiteSpace(indexQuery.kddh))
        //                {
        //                    Sql += $" and s.kddh like '{indexQuery.kddh}%' ";
        //                }
        //                //快递单号（可多选）
        //                if (!string.IsNullOrWhiteSpace(indexQuery.kddhStr))
        //                {
        //                    string kddhStr = string.Join("','", indexQuery.kddhStr.Split(','));
        //                    Sql += $" and s.kddh in ('{kddhStr}') ";
        //                }
        //                //快递公司（可多选）
        //                if (!string.IsNullOrWhiteSpace(indexQuery.kdgsStr))
        //                {
        //                    string kdgsStr = string.Join("','", indexQuery.kdgsStr.Split(','));
        //                    Sql += $" and s.kdgs in ('{kdgsStr}') ";
        //                }
        //                //仓库（可多选）
        //                if (!string.IsNullOrWhiteSpace(indexQuery.WarehouseCodeStr))
        //                {
        //                    string WarehouseCodeStr = string.Join("','", indexQuery.WarehouseCodeStr.Split(','));
        //                    Sql += $" and s.WarehouseCode in ('{WarehouseCodeStr}') ";
        //                }
        //                //货主（可多选）
        //                if (!string.IsNullOrWhiteSpace(indexQuery.CompanyCodeStr))
        //                {
        //                    string CompanyCodeStr = string.Join("','", indexQuery.CompanyCodeStr.Split(','));
        //                    Sql += $" and s.CompanyCode in ('{CompanyCodeStr}') ";
        //                }
        //                //来源平台（可多选）
        //                if (!string.IsNullOrWhiteSpace(indexQuery.SourcePlatformStr))
        //                {
        //                    string SourcePlatformStr = string.Join("','", indexQuery.SourcePlatformStr.Split(','));
        //                    Sql += $" and s.SourcePlatform in ('{SourcePlatformStr}') ";
        //                }
        //                //店铺名称（可多选）
        //                if (!string.IsNullOrWhiteSpace(indexQuery.StoreNameStr))
        //                {
        //                    string StoreNameStr = string.Join("','", indexQuery.StoreNameStr.Split(','));
        //                    Sql += $" and s.StoreName in ('{StoreNameStr}') ";
        //                }
        //                //首状态
        //                if (!string.IsNullOrWhiteSpace(indexQuery.leadingSts))
        //                {
        //                    string leadingSts = string.Join("','", indexQuery.leadingSts.Split(','));
        //                    Sql += $" and s.LeadingSts in ('{leadingSts}') ";
        //                }
        //                //尾状态
        //                if (!string.IsNullOrWhiteSpace(indexQuery.trailingSts))
        //                {
        //                    string trailingSts = string.Join("','", indexQuery.trailingSts.Split(','));
        //                    Sql += $" and s.TrailingSts in ('{trailingSts}') ";
        //                }
        //                //分公司
        //                if (!string.IsNullOrWhiteSpace(indexQuery.OrgName))
        //                {
        //                    string OrgName = string.Join("','", indexQuery.OrgName.Split(','));
        //                    Sql += $" and s.OrgName in ('{OrgName}') ";
        //                }
        //                if (!string.IsNullOrWhiteSpace(indexQuery.ProcessType))
        //                {
        //                    string ProcessType = string.Join("','", indexQuery.ProcessType.Split(','));
        //                    Sql += $" and s.ProcessType in ('{ProcessType}') ";
        //                }
        //                //来源系统
        //                if (!string.IsNullOrWhiteSpace(indexQuery.SourceSys))
        //                {
        //                    string SourceSys = string.Join("','", indexQuery.SourceSys.Split(','));
        //                    Sql += $" and s.SourceSys in ('{SourceSys}') ";
        //                }
        //                //大类分类
        //                if (!string.IsNullOrWhiteSpace(indexQuery.BigClass))
        //                {
        //                    string BigClass = string.Join("','", indexQuery.BigClass.Split(','));
        //                    Sql += $@" and s.BigClass in ('{BigClass}') ";
        //                }

        //                //省
        //                if (!string.IsNullOrWhiteSpace(indexQuery.Privince))
        //                {
        //                    Sql += $" and s.Privince like '{indexQuery.Privince}%' ";
        //                }
        //                //市
        //                if (!string.IsNullOrWhiteSpace(indexQuery.City))
        //                {
        //                    Sql += $" and s.City like '{indexQuery.City}%' ";
        //                }
        //                //区
        //                if (!string.IsNullOrWhiteSpace(indexQuery.District))
        //                {
        //                    Sql += $" and s.District like '{indexQuery.District}%' ";
        //                }
        //                //ERP单号
        //                if (!string.IsNullOrWhiteSpace(indexQuery.erpOrderCode))
        //                {
        //                    string erpOrderCode = string.Join("','", indexQuery.erpOrderCode.Split(','));
        //                    Sql += $@" and s.erpOrderCode in ('{erpOrderCode}') ";
        //                }
        //                //订单类型
        //                if (!string.IsNullOrWhiteSpace(indexQuery.shipmentType))
        //                {
        //                    string shipmentType = string.Join("','", indexQuery.shipmentType.Split(','));
        //                    Sql += $@" and s.shipmentType in ('{shipmentType}') ";
        //                }

        //                #endregion

        //                Sql = Sql.Replace("select count(1)", @"select s.*,s.fachushijian as 'lanshoushijian' ");

        //                #region 排序条件
        //                if (!string.IsNullOrWhiteSpace(indexQuery.OrderBy))
        //                {
        //                    Sql += $"order by {indexQuery.OrderBy}";
        //                }
        //                else
        //                {
        //                    Sql += $"order by s.CreateTime desc ";
        //                }
        //                #endregion

        //                List<SelectRouteObject> tb = connection.Query<SelectRouteObject>(Sql, commandTimeout: 1000000).ToList();

        //                string Addsql = $"insert into bi_download(ReportName,Cstate,WJstate,WJsize,CreateTime,OverTime,ErrorRZ) values ('每日运作报表','创建中','','','{cretime}','','') ";
        //                connection.Execute(Addsql);

        //                #region 导出数据

        //                XSSFWorkbook book = new XSSFWorkbook();
        //                ISheet s1 = book.CreateSheet("每日运作报表");
        //                IRow r1 = s1.CreateRow(0);
        //                r1.CreateCell(0).SetCellValue("行号");
        //                r1.CreateCell(1).SetCellValue("出库订单号");
        //                r1.CreateCell(2).SetCellValue("快递单号");
        //                r1.CreateCell(3).SetCellValue("快递公司");
        //                r1.CreateCell(4).SetCellValue("来源系统");
        //                r1.CreateCell(5).SetCellValue("分公司");
        //                r1.CreateCell(6).SetCellValue("波次号");
        //                r1.CreateCell(7).SetCellValue("波次类型");
        //                r1.CreateCell(8).SetCellValue("来源平台");
        //                r1.CreateCell(9).SetCellValue("店铺");
        //                r1.CreateCell(10).SetCellValue("出库类型3");
        //                r1.CreateCell(11).SetCellValue("出库类型5");
        //                r1.CreateCell(12).SetCellValue("首状态");
        //                r1.CreateCell(13).SetCellValue("尾状态");
        //                r1.CreateCell(14).SetCellValue("处理类型");
        //                r1.CreateCell(15).SetCellValue("货主编号");
        //                r1.CreateCell(16).SetCellValue("货主名称");
        //                r1.CreateCell(17).SetCellValue("仓库名称");
        //                r1.CreateCell(18).SetCellValue("件数");
        //                r1.CreateCell(19).SetCellValue("行数");
        //                r1.CreateCell(20).SetCellValue("订单创建时间");
        //                r1.CreateCell(21).SetCellValue("订单更新时间");
        //                r1.CreateCell(22).SetCellValue("付款时间");
        //                r1.CreateCell(23).SetCellValue("预售尾款支付时间");
        //                r1.CreateCell(24).SetCellValue("波次创建时间");
        //                r1.CreateCell(25).SetCellValue("拣货完成时间");
        //                r1.CreateCell(26).SetCellValue("复合完成时间");
        //                r1.CreateCell(27).SetCellValue("称重完成时间");
        //                r1.CreateCell(28).SetCellValue("发运时间");
        //                r1.CreateCell(29).SetCellValue("快递最新状态");
        //                r1.CreateCell(30).SetCellValue("快递最新更新时间");
        //                r1.CreateCell(31).SetCellValue("快递揽收时间");
        //                r1.CreateCell(32).SetCellValue("快递签收时间");
        //                r1.CreateCell(33).SetCellValue("转运时间");
        //                r1.CreateCell(34).SetCellValue("省");
        //                r1.CreateCell(35).SetCellValue("市");
        //                r1.CreateCell(36).SetCellValue("区");
        //                r1.CreateCell(37).SetCellValue("付款-创建");
        //                r1.CreateCell(38).SetCellValue("创建-发货");
        //                r1.CreateCell(39).SetCellValue("发货-揽收");
        //                r1.CreateCell(40).SetCellValue("付款-揽收");
        //                r1.CreateCell(41).SetCellValue("揽收-转运");
        //                r1.CreateCell(42).SetCellValue("揽收-签收");
        //                r1.CreateCell(43).SetCellValue("ERP单号");
        //                r1.CreateCell(44).SetCellValue("订单类型");

        //                for (int i = 0; i < tb.Count; i++)
        //                {
        //                    IRow rt = s1.CreateRow(i + 1);
        //                    rt.CreateCell(0).SetCellValue(i + 1);                             /* "行号");*/
        //                    rt.CreateCell(1).SetCellValue(tb[i].OutboundCode);                /* "出库订单号");*/
        //                    rt.CreateCell(2).SetCellValue(tb[i].kddh);                        /* "快递单号");*/
        //                    rt.CreateCell(3).SetCellValue(tb[i].kdgs);                        /* "快递公司");*/
        //                    rt.CreateCell(4).SetCellValue(tb[i].SourceSys);                   /* "来源系统");*/
        //                    rt.CreateCell(5).SetCellValue(tb[i].OrgName);                     /* "分公司");*/
        //                    rt.CreateCell(6).SetCellValue(tb[i].WaveId);                      /* "波次号");*/
        //                    rt.CreateCell(7).SetCellValue(tb[i].WaveType);                    /* "波次类型");*/
        //                    rt.CreateCell(8).SetCellValue(tb[i].SourcePlatform);              /* "来源平台");*/
        //                    rt.CreateCell(9).SetCellValue(tb[i].StoreName);                   /* "店铺");*/
        //                    rt.CreateCell(10).SetCellValue(tb[i].ShipmentCategory3);          /* ("出库类型3");*/
        //                    rt.CreateCell(11).SetCellValue(tb[i].ShipmentCategory5);          /* ("出库类型5");*/
        //                    rt.CreateCell(12).SetCellValue(tb[i].leadingSts);                 /* ("首状态");*/
        //                    rt.CreateCell(13).SetCellValue(tb[i].trailingSts);                /* ("尾状态");*/
        //                    rt.CreateCell(14).SetCellValue(tb[i].ProcessType);                /* ("处理类型");*/
        //                    rt.CreateCell(15).SetCellValue(tb[i].CompanyCode);                /* ("货主编号");*/
        //                    rt.CreateCell(16).SetCellValue(tb[i].CompanyName);                /* ("货主名称");*/
        //                    rt.CreateCell(17).SetCellValue(tb[i].WarehouseName);              /* ("仓库名称");*/
        //                    rt.CreateCell(18).SetCellValue(tb[i].TotalQty);                   /* ("件数");*/
        //                    rt.CreateCell(19).SetCellValue(tb[i].TotalLines);                 /* ("行数");*/
        //                    rt.CreateCell(20).SetCellValue(tb[i].OrderCreateTime);            /* ("订单创建时间");*/
        //                    rt.CreateCell(21).SetCellValue(tb[i].LastUpdateTime);             /* ("订单更新时间");*/
        //                    rt.CreateCell(22).SetCellValue(tb[i].PayTime);                    /* ("付款时间");*/
        //                    rt.CreateCell(23).SetCellValue(tb[i].PaidTime);                   /* ("预售尾款支付时间");*/
        //                    rt.CreateCell(24).SetCellValue(tb[i].WaveCreateTime);             /* ("波次创建时间");*/
        //                    rt.CreateCell(25).SetCellValue(tb[i].PickingCompleteTime);        /* ("拣货完成时间");*/
        //                    rt.CreateCell(26).SetCellValue(tb[i].CheckCompleteTime);          /* ("复合完成时间");*/
        //                    rt.CreateCell(27).SetCellValue(tb[i].WeighCompleteTime);          /* ("称重完成时间");*/
        //                    rt.CreateCell(28).SetCellValue(tb[i].ActualShipTime);             /* ("发运时间");*/
        //                    rt.CreateCell(29).SetCellValue(tb[i].wuliuzhuangtai);             /* ("快递最新状态");*/
        //                    rt.CreateCell(30).SetCellValue(tb[i].zuixinshijian);              /* ("快递最新更新时间");*/
        //                    rt.CreateCell(31).SetCellValue(tb[i].lanshoushijian);             /* ("快递揽收时间");*/
        //                    rt.CreateCell(32).SetCellValue(tb[i].qianshoushijian);            /*  ("快递签收时间");*/


        //                    rt.CreateCell(33).SetCellValue(tb[i].zhuanyunshijian);            /*  ("快递签收时间");*/
        //                    rt.CreateCell(34).SetCellValue(tb[i].Privince);            /*  ("快递签收时间");*/
        //                    rt.CreateCell(35).SetCellValue(tb[i].City);            /*  ("快递签收时间");*/
        //                    rt.CreateCell(36).SetCellValue(tb[i].District);            /*  ("快递签收时间");*/
        //                    rt.CreateCell(37).SetCellValue(tb[i].FK2CJ.ToString());            /*  ("快递签收时间");*/
        //                    rt.CreateCell(38).SetCellValue(tb[i].CJ2FH.ToString());            /*  ("快递签收时间");*/
        //                    rt.CreateCell(39).SetCellValue(tb[i].FH2LS.ToString());            /*  ("快递签收时间");*/
        //                    rt.CreateCell(40).SetCellValue(tb[i].FK2LS.ToString());            /*  ("快递签收时间");*/
        //                    rt.CreateCell(41).SetCellValue(tb[i].LS2ZY.ToString());            /*  ("快递签收时间");*/
        //                    rt.CreateCell(42).SetCellValue(tb[i].LS2QS.ToString());            /*  ("快递签收时间");*/
        //                    rt.CreateCell(43).SetCellValue(tb[i].erpOrderCode);            /*  ("快递签收时间");*/
        //                    rt.CreateCell(44).SetCellValue(tb[i].shipmentType);            /*  ("快递签收时间");*/
        //                }
        //                #endregion
        //                var path = "导出/";

        //                if (!Directory.Exists(path))
        //                {
        //                    Directory.CreateDirectory(path);
        //                }

        //                using (FileStream stream = new FileStream(path + DCname, FileMode.CreateNew))
        //                {
        //                    book.Write(stream);

        //                    //stream.Seek(0, SeekOrigin.Begin);


        //                    string overtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff");
        //                    //string DCname = (stream.Name).Substring((stream.Name).Length - 22, 22);

        //                    //获取文件大小
        //                    System.IO.FileInfo fileInfo = null;
        //                    fileInfo = new System.IO.FileInfo(stream.Name);
        //                    if (fileInfo != null && fileInfo.Exists)
        //                    {
        //                        var Size = System.Math.Ceiling(fileInfo.Length / 1024.0);
        //                        string Uptsql = $"UPDATE bi_download set WJsize = '{Size}KB' where CreateTime = '{cretime}'";
        //                        connection.Execute(Uptsql);
        //                    }


        //                    string XZurl = "https://wcepod.rokin.cn/wvs/wvsexcel/" + $"{DCname}";
        //                    FtpClient client = new FtpClient("attach.rokin.cn");
        //                    client.Credentials = new NetworkCredential("wvsftp", "Qdv@1412$.3");
        //                    client.Connect();
        //                    var state = client.UploadFile(@$"导出/{DCname}", @$"wvsexcel\{DCname}");
        //                    client.Disconnect();
        //                    if (state == FtpStatus.Success)
        //                    {
        //                        string Uptsql = $"UPDATE bi_download set Cstate = '创建成功',WJstate='已创建',OverTime = '{overtime}',Url='{XZurl}' where CreateTime = '{cretime}'";
        //                        connection.Execute(Uptsql);
        //                    }
        //                    else
        //                    {
        //                        string Uptsql = $"UPDATE bi_download set Cstate = '创建失败',WJstate='未创建',OverTime = '{overtime}',ErrorRZ = '文件未创建' where CreateTime = '{cretime}'";
        //                        connection.Execute(Uptsql);
        //                    }
        //                    book.Close();
        //                    stream.Close();
        //                    #region 删除文件
        //                    //path为路径，可以右键文件选择属性看到 
        //                    //判断路径或文件夹是否存在(绝对路径)
        //                    if (Directory.Exists(path))
        //                    {
        //                        //返回所有文件夹的路径
        //                        var dirs = Directory.GetDirectories(path);
        //                        //遍历所有文件夹路径
        //                        for (int index = 0; index < dirs.Length; ++index)
        //                        {
        //                            //删除文件夹
        //                            Directory.Delete(dirs[index]);
        //                        }
        //                        //返回所有文件的路径
        //                        var files = Directory.GetFiles(path);
        //                        //遍历所有文件路径
        //                        for (int index = 0; index < files.Length; ++index)
        //                        {
        //                            //删除文件
        //                            File.Delete(files[index]);
        //                        }
        //                    }
        //                }
        //                #endregion
        //            }
        //            catch (Exception ex)
        //            {
        //                throw;
        //            }
        //        }

        //        /// <summary>
        //        /// 订单报表-各个状态下的订单数量
        //        /// </summary>
        //        /// <param name="indexQuery"></param>
        //        /// <returns></returns>
        //        public ResponseObject RptOrdeStatusCount(SelectRouteObject indexQuery)
        //        {
        //            ResponseObject result = new ResponseObject();
        //            Dictionary<string, object> keyValues = new Dictionary<string, object>();
        //            try
        //            {
        //                string Sql = $@"
        //select count(1) as 'all' 
        //        ,ifnull(sum(case when s.BigClass='待分配' then 1 else 0 end),0) as '待分配'
        //        ,ifnull(sum(case when s.BigClass='波次中' then 1 else 0 end),0) as '波次中'
        //        ,ifnull(sum(case when s.BigClass='已发运' then 1 else 0 end),0) as '已发运'
        //        ,ifnull(sum(case when s.BigClass='未揽收' then 1 else 0 end),0) as '未揽收'
        //        ,ifnull(sum(case when s.BigClass='已揽收' then 1 else 0 end),0) as '已揽收'
        //        ,ifnull(sum(case when s.BigClass='已签收' then 1 else 0 end),0) as '已签收'
        //        ,ifnull(sum(case when s.BigClass='已取消' then 1 else 0 end),0) as '已取消'
        //from bi_shipment s force index(idx_LastUpdateTime_BigClass) 
        //where 1=1 
        //        and s.OrderCreateTime>='2021-10-20' 
        // ";
        //                #region 查询条件
        //                //转单时间
        //                if (indexQuery.OrderCreateTimeStart != null)
        //                {
        //                    Sql += $" and s.OrderCreateTime >= '{indexQuery.OrderCreateTimeStart:yyyy-MM-dd HH:mm:ss}' ";
        //                }
        //                if (indexQuery.OrderCreateTimeEnd != null)
        //                {
        //                    Sql += $" and s.OrderCreateTime < '{indexQuery.OrderCreateTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
        //                }
        //                //付款时间
        //                if (indexQuery.PayTimeStart != null)
        //                {
        //                    Sql += $" and s.PayTime >= '{indexQuery.PayTimeStart:yyyy-MM-dd HH:mm:ss}' ";
        //                }
        //                if (indexQuery.PayTimeEnd != null)
        //                {
        //                    Sql += $" and s.PayTime < '{indexQuery.PayTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
        //                }
        //                //更新时间
        //                if (indexQuery.UpdatedTimeStart != null)
        //                {
        //                    Sql += $" and s.LastUpdateTime >= '{indexQuery.UpdatedTimeStart:yyyy-MM-dd HH:mm:ss}' ";
        //                }
        //                if (indexQuery.UpdatedTimeEnd != null)
        //                {
        //                    Sql += $" and s.LastUpdateTime < '{indexQuery.UpdatedTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
        //                }
        //                //订单号
        //                if (!string.IsNullOrWhiteSpace(indexQuery.OutboundCode))
        //                {
        //                    Sql += $" and s.OutboundCode like '%{indexQuery.OutboundCode}%' ";
        //                }
        //                //快递单号
        //                if (!string.IsNullOrWhiteSpace(indexQuery.kddh))
        //                {
        //                    Sql += $" and s.kddh like '%{indexQuery.kddh}%' ";
        //                }
        //                //快递单号（可多选）
        //                if (!string.IsNullOrWhiteSpace(indexQuery.kddhStr))
        //                {
        //                    string kddhStr = string.Join("','", indexQuery.kddhStr.Split(','));
        //                    Sql += $" and s.kddh in ('{kddhStr}') ";
        //                }
        //                //快递公司（可多选）
        //                if (!string.IsNullOrWhiteSpace(indexQuery.kdgsStr))
        //                {
        //                    string kdgsStr = string.Join("','", indexQuery.kdgsStr.Split(','));
        //                    Sql += $" and s.kdgs in ('{kdgsStr}') ";
        //                }
        //                //仓库（可多选）
        //                if (!string.IsNullOrWhiteSpace(indexQuery.WarehouseCodeStr))
        //                {
        //                    string WarehouseCodeStr = string.Join("','", indexQuery.WarehouseCodeStr.Split(','));
        //                    Sql += $" and s.WarehouseCode in ('{WarehouseCodeStr}') ";
        //                }
        //                //货主（可多选）
        //                if (!string.IsNullOrWhiteSpace(indexQuery.CompanyCodeStr))
        //                {
        //                    string CompanyCodeStr = string.Join("','", indexQuery.CompanyCodeStr.Split(','));
        //                    Sql += $" and s.CompanyCode in ('{CompanyCodeStr}') ";
        //                }
        //                //来源平台（可多选）
        //                if (!string.IsNullOrWhiteSpace(indexQuery.SourcePlatformStr))
        //                {
        //                    string SourcePlatformStr = string.Join("','", indexQuery.SourcePlatformStr.Split(','));
        //                    Sql += $" and s.SourcePlatform in ('{SourcePlatformStr}') ";
        //                }
        //                //店铺名称（可多选）
        //                if (!string.IsNullOrWhiteSpace(indexQuery.StoreNameStr))
        //                {
        //                    string StoreNameStr = string.Join("','", indexQuery.StoreNameStr.Split(','));
        //                    Sql += $" and s.StoreName in ('{StoreNameStr}') ";
        //                }
        //                //首状态
        //                if (!string.IsNullOrWhiteSpace(indexQuery.leadingSts))
        //                {
        //                    string leadingSts = string.Join("','", indexQuery.leadingSts.Split(','));
        //                    Sql += $" and s.LeadingSts in ('{leadingSts}') ";
        //                }
        //                //尾状态
        //                if (!string.IsNullOrWhiteSpace(indexQuery.trailingSts))
        //                {
        //                    string trailingSts = string.Join("','", indexQuery.trailingSts.Split(','));
        //                    Sql += $" and s.TrailingSts in ('{trailingSts}') ";
        //                }
        //                //分公司
        //                if (!string.IsNullOrWhiteSpace(indexQuery.OrgName))
        //                {
        //                    string OrgName = string.Join("','", indexQuery.OrgName.Split(','));
        //                    Sql += $" and s.OrgName in ('{OrgName}') ";
        //                }
        //                if (!string.IsNullOrWhiteSpace(indexQuery.ProcessType))
        //                {
        //                    string ProcessType = string.Join("','", indexQuery.ProcessType.Split(','));
        //                    Sql += $" and s.ProcessType in ('{ProcessType}') ";
        //                }
        //                //来源系统
        //                if (!string.IsNullOrWhiteSpace(indexQuery.SourceSys))
        //                {
        //                    string SourceSys = string.Join("','", indexQuery.SourceSys.Split(','));
        //                    Sql += $" and s.SourceSys in ('{SourceSys}') ";
        //                }
        //                //大类分类
        //                if (!string.IsNullOrWhiteSpace(indexQuery.BigClass))
        //                {
        //                    string BigClass = string.Join("','", indexQuery.BigClass.Split(','));
        //                    Sql += $@" and s.BigClass in ('{BigClass}') ";
        //                }
        //                #endregion

        //                dynamic tb = connection.Query<dynamic>(Sql, commandTimeout: 600).ToArray();
        //                result.result = tb;
        //            }
        //            catch (Exception ex)
        //            {
        //                result.code = 1;
        //                result.message = ex.Message;
        //            }

        //            return result;
        //        }
        #endregion
    }
}
