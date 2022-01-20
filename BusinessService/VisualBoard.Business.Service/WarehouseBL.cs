using Dapper;
using FluentFTP;
//using Microsoft.Ajax.Utilities;
//using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.XSSF.UserModel;
using Rokin.Common.RabbitMQ;
using Rokin.Common.RabbitMQ.Interface;
//using NWebsec.Helpers;
//using OfficeOpenXml;
//using OfficeOpenXml.Style;
using Rokin.Common.Tools;
using Rokin.Dapper;
using Rokin.EFCore.VIPBI.VIP_BI;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
//using System.Web.Helpers;
using VisualBoard.Business.Interface;
using VisualBoard.Models.Constant;
using VisualBoard.Models.Request;
using VisualBoard.Models.Response;

namespace VisualBoard.Business.Service
{
    public class WarehouseBL : IWarehouseBL
    {
        private readonly _WMS_VisualboardContext WMS_Visualboard;
        private readonly _VIP_BIContext VIP_BIContext;
        private readonly IDbConnection connection;
        private readonly IOrganizationBL Organization;
        //private readonly IRabbitMQHelper publish;

        //public static System.Timers.Timer CheckTimer;
        //public bool is_RouterOrderExecl = false,is_RstpOrderExcel=false;
        public string DCname = "";
        public int ia = 0;
        SelectRouteObject indexQuery, rstpQuery;

        public WarehouseBL(_VIP_BIContext _VIP_BI, IDbConnection _connection, IOrganizationBL organization)
        {
            VIP_BIContext = _VIP_BI;
            connection = _connection;
            this.Organization = organization;
            //this.publish = publish;
        }
        //public WarehouseBL(_VIP_BIContext _VIP_BI, IDbConnection _connection, IOrganizationBL organization, IRabbitMQHelper publish)
        //{
        //    VIP_BIContext = _VIP_BI;
        //    connection = _connection;
        //    this.Organization = organization;
        //    this.publish = publish;
        //}

        #region 1:总公司看板 


        /// <summary>
        /// 1：总公司看板首页
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObject WarehouseIndex(IndexQueryObject indexQuery)
        {
            ResponseObject result = new ResponseObject();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            try
            {
                ///发运总量统计
                keyValues.Add("AllCount", AllCount(indexQuery));

                /// 发运分析
                keyValues.Add("AsAnalysisOfShipmentda", AsAnalysisOfShipmentda(indexQuery));

                /// 项目完成情况
                keyValues.Add("ProjectCompletion", ProjectCompletion(indexQuery));

                /// 客户完成情况
                keyValues.Add("CustomerCompletion", CustomerCompletion(indexQuery));

                /// 件单比
                keyValues.Add("NumberCount", NumberCount(indexQuery));

                /// 总单/总人
                keyValues.Add("NumberCountPeople", NumberCountPeople(indexQuery));

                /// 总件/总人
                keyValues.Add("PieceCountPeople", PieceCountPeople(indexQuery));

                /////首页气泡地图
                keyValues.Add("BubbleMap", BubbleMap(indexQuery));


                result.result = keyValues;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;


        }
        public TableCountAllObject AllCount(IndexQueryObject indexQuery)
        {
            TableCountAllObject tb = new TableCountAllObject();
            tb.target_cn = "发运总量统计";
            string Sql = $@"SELECT
	count( DISTINCT warehousename ) AS 'Number',
	count(DISTINCT  warehousename,ShipperName) as 'WaitDistribution',	
	sum( OrderCount ) AS 'OrderCount',
	sum( Shipping ) AS 'Shipping' 
FROM
	bi_Wms 
WHERE       createtime >= '{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}'  
  AND createtime < '{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' 	
	AND DataFrom IN ( 0, 1 ) 
   ";
            tb.table = connection.Query<bi_Wms>(Sql).
              Select(p => new TableCountAllDataObject
              {
                  warehousenameNumber = p.Number,
                  ShipperNameNumber = p.WaitDistribution,
                  OrderCount = p.OrderCount,
                  Shipping = p.Shipping,
                  Percent = Convert.ToDecimal(p.OrderCount) == 0 ? 0 : Convert.ToDecimal(p.Shipping) / Convert.ToDecimal(p.OrderCount)
              }).ToArray();

            return tb;

        }

        /// <summary>
        /// 发运分析
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public TableObject AsAnalysisOfShipmentda(IndexQueryObject indexQuery)
        {
            TableObject tb = new TableObject();
            tb.target_cn = "发运分析";
            string Sql = $@"SELECT  case warehousename when '太仓' then '昆山' else  warehousename end as ' warehousename', sum( OrderCount ) AS 'OrderCount', sum(Shipping ) AS 'Shipping' 
                                FROM bi_Wms  WHERE
                                createtime >= '{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}'  
                                AND createtime < '{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' 
                                 AND  DataFrom  IN(0,1)
group by case warehousename when '太仓' then '昆山' else  warehousename end order by  sum(Shipping )  desc,  sum( OrderCount ) desc 
";
            tb.table = connection.Query<bi_Wms>(Sql).
              Select(p => new TableDataObject
              {
                  WarehouseName = p.WarehouseName,
                  OrderCount = p.OrderCount,
                  Shipping = p.Shipping,
                  Percent = Convert.ToDecimal(p.OrderCount) == 0 ? 0 : Convert.ToDecimal(p.Shipping) / Convert.ToDecimal(p.OrderCount)
              }).ToArray();

            return tb;

        }

        /// <summary>
        /// 项目完成情况
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public TableObject ProjectCompletion(IndexQueryObject indexQuery)
        {
            TableObject tb = new TableObject();
            tb.target_cn = "项目完成情况";
            string Sql = $@" SELECT concat( warehousename, ShipperName ) AS WarehouseName,
	sum( OrderCount ) AS 'OrderCount', sum( Shipping ) AS 'Shipping' 
FROM bi_Wms  WHERE
createtime >= '{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}'  AND createtime < '{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' 
AND  DataFrom  IN(0,1)
GROUP BY concat( warehousename, ShipperName )   order  by sum( OrderCount )  desc
";
            tb.table = connection.Query<bi_Wms>(Sql).
              Select(p => new TableDataObject
              {
                  WarehouseName = p.WarehouseName,
                  OrderCount = p.OrderCount,
                  Shipping = p.Shipping,
                  Percent = Convert.ToDecimal(p.OrderCount) == 0 ? 0 : Convert.ToDecimal(p.Shipping) / Convert.ToDecimal(p.OrderCount)
              }).ToArray();

            return tb;

        }


        /// <summary>
        /// 客户完成情况
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public TableObject CustomerCompletion(IndexQueryObject indexQuery)
        {
            TableObject tb = new TableObject();
            tb.target_cn = "全国客户完成情况";
            string Sql = $@" SELECT  ShipperName   AS WarehouseName,
	sum( OrderCount ) AS 'OrderCount', sum( Shipping ) AS 'Shipping' 
FROM bi_Wms  WHERE
createtime >= '{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}'  AND createtime < '{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' 
AND  DataFrom  IN(0,1)
GROUP BY ShipperName  order  by sum( OrderCount )  desc
";
            tb.table = connection.Query<bi_Wms>(Sql).
              Select(p => new TableDataObject
              {
                  WarehouseName = p.WarehouseName,
                  OrderCount = p.OrderCount,
                  Shipping = p.Shipping,
                  Percent = Convert.ToDecimal(p.OrderCount) == 0 ? 0 : Convert.ToDecimal(p.Shipping) / Convert.ToDecimal(p.OrderCount)
              }).ToArray();

            return tb;

        }

        /// <summary>
        /// 件单比
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public MixedLineAndBarObject NumberCount(IndexQueryObject indexQuery)
        {
            MixedLineAndBarObject mixed = new MixedLineAndBarObject();
            mixed.target_cn = "件单比";

            string Sql = $@"   SELECT warehousename, sum( number ) AS 'number', sum( OrderCount ) AS 'OrderCount' 
FROM bi_Wms  
WHERE createtime >= '{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}'  
AND createtime < '{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' 
AND  DataFrom  IN(0,1)
GROUP BY warehousename order by  sum( OrderCount )  desc  ";
            var query = connection.Query<bi_Wms>(Sql).
               Select(p => new
               {
                   day = p.WarehouseName,
                   Percent = Convert.ToDecimal(p.OrderCount) == 0 ? 0 : Convert.ToDecimal(p.Number) / Convert.ToDecimal(p.OrderCount)
               }).ToArray();

            mixed.day = query.Select(p => p.day).ToArray();
            mixed.Value = query.Select(p => p.Percent).ToArray();

            return mixed;

        }


        /// <summary>
        /// 总单/总人
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public MixedLineAndBarObject NumberCountPeople(IndexQueryObject indexQuery)
        {
            MixedLineAndBarObject mixed = new MixedLineAndBarObject();
            mixed.target_cn = "总单/总人";

            string Sql1 = $@"   
SELECT  c.OrganName as WavePick, count(DISTINCT a.userid) as 'Order' ,sum(case when a.OrderState=100 then  `order` end ) as 'PCS'  
from acc_WavePickDetails  a
left join bsc_User b on a.UserID=b.UserID 
left join bsc_Organization c on b.OrganID=c.ID
where a.state =0 
and a.createtime >='{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}'
and a.createtime <'{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}'GROUP BY  c.OrganName";

            var query = connection.Query<acc_WavePickDetails>(Sql1).ToArray();
            mixed.day = query.Select(p => p.WavePick).ToArray();
            mixed.Value = query.Select(p => Convert.ToDecimal(p.Order) == 0 ? 0 : Convert.ToDecimal(p.PCS) / Convert.ToDecimal(p.Order)).ToArray();

            return mixed;
            //            string Sql = $@"   
            //SELECT  substr( warehousename,1,2) as warehousename,sum( OrderCount ) AS 'OrderCount' 
            //FROM bi_Wms  
            //WHERE createtime >= '{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}'
            //AND createtime < '{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' 
            //AND  DataFrom  IN(0,1)
            //GROUP BY substr( warehousename,1,2) ";
            //            var query = connection.Query<bi_Wms>(Sql).
            //               Select(p => new
            //               {
            //                   day = p.WarehouseName,
            //                   Percent =p.OrderCount//总单量
            //               }).ToArray();

            //            string Sql1 = $@"   
            //SELECT  substr( c.OrganName,1,2) as WavePick, count(DISTINCT a.userid) as 'Order' 
            //from acc_WavePickDetails  a
            //left join bsc_User b on a.UserID=b.UserID 
            //left join bsc_Organization c on b.OrganID=c.ID
            //where a.state =0 
            //and a.createtime >='{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}'
            //and a.createtime <'{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}'GROUP BY  substr( c.OrganName,1,2) ";
            //            var query1 = connection.Query<acc_WavePickDetails>(Sql1).
            //               Select(p => new
            //               {
            //                   WavePick = p.WavePick,
            //                   Order = p.Order//总人
            //               }).ToArray();

            //          var a =  (from que in query.AsEnumerable()
            //                join que1 in query1
            //                on que.day equals que1.WavePick
            //                into g
            //                from x in g.DefaultIfEmpty()

            //                select new  
            //                {
            //                    day = que.day,
            //                    OrderCount = que.Percent,//总单量
            //                    Order = x == null ? 1 : x.Order,//总人                     
            //                }).ToArray();

            //            mixed.day = a.Select(p => p.day).ToArray();
            //            mixed.Value = a.Select(p =>Convert.ToDecimal( p.OrderCount) /Convert.ToDecimal( p.Order)).ToArray();

            //            return mixed;

        }

        /// <summary>
        /// 总单/总人
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public MixedLineAndBarObject PieceCountPeople(IndexQueryObject indexQuery)
        {
            MixedLineAndBarObject mixed = new MixedLineAndBarObject();
            mixed.target_cn = "总件/总人";

            string Sql1 = $@"   
SELECT  c.OrganName as WavePick, count(DISTINCT a.userid) as 'Order' ,sum(case when a.OrderState=100 then pcs end ) as 'PCS'  
from acc_WavePickDetails  a
left join bsc_User b on a.UserID=b.UserID 
left join bsc_Organization c on b.OrganID=c.ID
where a.state =0 
and a.createtime >='{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}'
and a.createtime <'{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}'GROUP BY  c.OrganName";

            var query = connection.Query<acc_WavePickDetails>(Sql1).ToArray();
            mixed.day = query.Select(p => p.WavePick).ToArray();
            mixed.Value = query.Select(p => Convert.ToDecimal(p.Order) == 0 ? 0 : Convert.ToDecimal(p.PCS) / Convert.ToDecimal(p.Order)).ToArray();

            return mixed;
            //            MixedLineAndBarObject mixed = new MixedLineAndBarObject();
            //            mixed.target_cn = "总件/总人";

            //            string Sql = $@"   
            //SELECT  substr( warehousename,1,2) as warehousename,sum( Number ) AS 'OrderCount' 
            //FROM bi_Wms   
            //WHERE createtime >= '{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}'  
            //AND createtime < '{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' 
            //AND  DataFrom  IN(0,1)
            //GROUP BY substr( warehousename,1,2) ";
            //            var query = connection.Query<bi_Wms>(Sql).
            //               Select(p => new
            //               {
            //                   day = p.WarehouseName,
            //                   Percent = p.OrderCount//总件数
            //               }).ToArray();

            //            string Sql1 = $@"   
            //SELECT  substr( c.OrganName,1,2) as WavePick, count(DISTINCT a.userid) as 'Order'
            //from acc_WavePickDetails  a
            //left join bsc_User b on a.UserID=b.UserID 
            //left join bsc_Organization c on b.OrganID=c.ID
            //where a.state =0 
            //and a.createtime >='{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}'
            //and a.createtime <'{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}'GROUP BY  substr( c.OrganName,1,2) ";
            //            var query1 = connection.Query<acc_WavePickDetails>(Sql1).
            //               Select(p => new
            //               {
            //                   WavePick = p.WavePick,
            //                   Order = p.Order//总人
            //               }).ToArray();

            //            var a = (from que in query.AsEnumerable()
            //                     join que1 in query1
            //                     on que.day equals que1.WavePick
            //                     into g
            //                     from x in g.DefaultIfEmpty()

            //                     select new
            //                     {
            //                         day = que.day,
            //                         OrderCount = que.Percent,//总件数
            //                         Order = x == null ? 1 : x.Order,//总人                     
            //                     }).ToArray();

            //            mixed.day = a.Select(p => p.day).ToArray();
            //            mixed.Value = a.Select(p => Convert.ToDecimal(p.OrderCount) / Convert.ToDecimal(p.Order)).ToArray();

            //            return mixed;

        }

        public BubbleMapObject BubbleMap(IndexQueryObject indexQuery)
        {
            BubbleMapObject BubbleMap = new BubbleMapObject();
            BubbleMap.target_cn = "订单情况统计";

            string Sql = $@"SELECT CASE warehousename  WHEN '北京' THEN '北京市' 
		WHEN '太仓' THEN '昆山市'  WHEN '广州' THEN '广州市'  WHEN '成都' THEN '成都市' 
			WHEN '武汉' THEN '武汉市'  when '昆山' then  '昆山市'
	END AS warehousename , sum( OrderCount ) as OrderCount
FROM bi_Wms 
WHERE createtime >= '{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}'  
AND createtime < '{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' 
AND  DataFrom  IN(0,1)
GROUP BY CASE warehousename  WHEN '北京' THEN '北京市' 
		WHEN '太仓' THEN '昆山市'  WHEN '广州' THEN '广州市'  WHEN '成都' THEN '成都市' 
			WHEN '武汉' THEN '武汉市'  when '昆山' then  '昆山市'
	END order by sum( OrderCount ) desc  ";

            var query = connection.Query<bi_Wms>(Sql).
                Select(p => new BodyBubbleMapObject { ReceiveCity = p.WarehouseName, TotalWeight = p.OrderCount }).ToList();


            var query1 = VIP_BIContext.bi_middle_region.ToList();
            BubbleMap.Body =
               (from que in query.AsEnumerable()
                join que1 in query1
                on que.ReceiveCity equals que1.name

                into g
                from x in g.DefaultIfEmpty()

                select new BodyBubbleMapObject
                {
                    ReceiveCity = que.ReceiveCity,
                    pinyin = x == null ? "" : x.pinyin,
                    Lng = x == null ? "" : x.Lng.ToString(),
                    Lat = x == null ? "" : x.Lat.ToString(),
                    TotalWeight = que.TotalWeight
                }).ToArray();
            return BubbleMap;


        }

        #endregion


        #region 2:大促看板

        /// <summary>
        /// 2：大促看板
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObject SaleOrder(IndexQueryObject indexQuery)
        {
            ResponseObject result = new ResponseObject();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            try
            {
                /// 大促看板
                keyValues.Add("bi_WmsTable", bi_WmsTable(indexQuery));

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
        /// 发运分析
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public bi_wmsTableObject bi_WmsTable(IndexQueryObject indexQuery)
        {
            bi_wmsTableObject tb = new bi_wmsTableObject();

            tb.target_cn = "大促看板";
            string Sql = $@"SELECT concat( WarehouseName, ShipperName ) AS WarehouseName,OrderType,
	SUM(OrderCount) AS 'OrderCount', SUM(WaitDistribution) AS 'WaitDistribution', 
	SUM(WavePicking) AS 'WavePicking',SUM( WaitPick) AS 'WaitPick',SUM( EndPick)as ' EndPick',
	sum( WaitPackage) as 'WaitPackage', sum(EndReview) as 'EndReview',
	sum(EndPresell) as 'EndPresell',sum( OrderWeight) as 'OrderWeight',sum( Shipping)  as 'Shipping' , 
	sum(Cancel) as 'Cancel', sum(Exception) as 'Exception' ,sum(DataFrom) as 'DataFrom',max(OperateTime) as 'OperateTime'
FROM bi_Wms 
WHERE createtime >= '{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}' 
AND createtime < '{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' 
AND  DataFrom  IN(0,1) GROUP BY  concat( WarehouseName, ShipperName )  ,OrderType union  all 

           SELECT '欧莱雅合计' AS WarehouseName,
	'' as 'OrderType', SUM(OrderCount) AS 'OrderCount', SUM(WaitDistribution) AS 'WaitDistribution', 
	SUM(WavePicking) AS 'WavePicking',SUM( WaitPick) AS 'WaitPick',SUM( EndPick)as ' EndPick',
	sum( WaitPackage) as 'WaitPackage', sum(EndReview) as 'EndReview',
	sum(EndPresell) as 'EndPresell',sum( OrderWeight) as 'OrderWeight',sum( Shipping)  as 'Shipping' , 
	sum(Cancel) as 'Cancel', sum(Exception) as 'Exception' ,sum(DataFrom) as 'DataFrom',max(OperateTime) as 'OperateTime'
FROM bi_Wms 
WHERE createtime >= '{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}' 
AND createtime < '{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}'
and ShipperName='欧莱雅'  union  all 

SELECT '全国合计' AS WarehouseName,
	'' as 'OrderType', SUM(OrderCount) AS 'OrderCount', SUM(WaitDistribution) AS 'WaitDistribution', 
	SUM(WavePicking) AS 'WavePicking',SUM( WaitPick) AS 'WaitPick',SUM( EndPick)as ' EndPick',
	sum( WaitPackage) as 'WaitPackage', sum(EndReview) as 'EndReview',
	sum(EndPresell) as 'EndPresell',sum( OrderWeight) as 'OrderWeight',sum( Shipping)  as 'Shipping' , 
	sum(Cancel) as 'Cancel', sum(Exception) as 'Exception' ,sum(DataFrom) as 'DataFrom',max(OperateTime) as 'OperateTime'
FROM bi_Wms 
WHERE createtime >= '{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}' 
AND createtime < '{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}'
and ShipperName<>'欧莱雅' ";
            Sql += $@" AND  DataFrom  IN(0,1)   ";

            tb.table = connection.Query<bi_Wms_V2>(Sql).
                Select(p => new bi_wmsTableDataObject
                {
                    WarehouseName = p.WarehouseName,
                    OrderType = p.OrderType,
                    Percent = Convert.ToDecimal(p.OrderCount) == 0 ? 0 : Convert.ToDecimal(p.Shipping) / Convert.ToDecimal(p.OrderCount),
                    OrderCount = p.OrderCount,
                    WaitDistribution = p.WaitDistribution,
                    WavePicking = p.WavePicking,
                    WaitPick = p.WaitPick,
                    EndPick = p.EndPick,
                    WaitPackage = p.WaitPackage,
                    EndReview = p.EndReview,
                    EndPresell = p.EndPresell,
                    OrderWeight = p.OrderWeight,
                    Shipping = p.Shipping,
                    Cancel = p.Cancel,
                    Exception = p.Exception,
                    DataFrom = p.DataFrom,
                    OperateTime = p.OperateTime,
                    Collect = p.Collect
                }).ToArray();

            return tb;

        }


        #endregion

        #region 3:分公司看板
        /// <summary>
        /// 分公司看板
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObject CompanyOrder(IndexQueryObject indexQuery)
        {
            ResponseObject result = new ResponseObject();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            indexQuery.OrganizationID = Organization.GetSubOrganization(Convert.ToInt32(indexQuery.OrganizationID));
            try
            {

                ///分公司看板发运总量统计
                keyValues.Add("CompanyAllCount", CompanyAllCount(indexQuery));
                /// 边拣货边分单 未完成
                keyValues.Add("OrderPickingPart", OrderPickingPart(indexQuery));
                /// 二次分拣单 未完成
                keyValues.Add("SecondOrderPicking", SecondOrderPicking(indexQuery));
                /// 活动单现货 未完成
                keyValues.Add("ActivityOrder", ActivityOrder(indexQuery));
                /// 标准单 未完成
                keyValues.Add("StandardSingle", StandardSingle(indexQuery));
                /// 客户完成情况
                keyValues.Add("CustomerCompletion", CustomerCompletion1(indexQuery));

                result.result = keyValues;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }

        public TableObject CompanyAllCount(IndexQueryObject indexQuery)
        {
            TableObject tb = new TableObject();
            tb.target_cn = "分公司发运总量统计";
            string Sql = $@" SELECT   sum( OrderCount ) AS 'OrderCount', sum( Shipping ) AS 'Shipping' 
FROM bi_Wms   
  WHERE       createtime >= '{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}'  
   AND createtime < '{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' 	
	AND DataFrom IN ( 0, 1 ) ";

            if (indexQuery.CustomerID == "")
            {
                Sql += $" and  OrganizationID in({indexQuery.OrganizationID})  ";
            }
            else
            {
                Sql += $" and  OrganizationID  in({indexQuery.OrganizationID}) and CustomerID in ({indexQuery.CustomerID})";
            }

            tb.table = connection.Query<bi_Wms>(Sql).
              Select(p => new TableDataObject
              {
                  WarehouseName = p.WarehouseName,
                  OrderCount = p.OrderCount,
                  Shipping = p.Shipping,
                  Percent = Convert.ToDecimal(p.OrderCount) == 0 ? 0 : Convert.ToDecimal(p.Shipping) / Convert.ToDecimal(p.OrderCount)
              }).ToArray();

            return tb;

        }


        /// <summary>
        /// 边拣货边分单
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public MixedLineAndBarObject OrderPickingPart(IndexQueryObject indexQuery)
        {
            MixedLineAndBarObject mixed = new MixedLineAndBarObject();
            mixed.target_cn = "边拣货边分单 未完成";

            string Sql = $@"   
SELECT  case OrderStateNow when 1 then '未拣货' when 2 then '拣货中' END AS 'WavePick',sum(`Order`) as 'Order'
from acc_WavePick a
left join bsc_Customer b on a.CustomerID=b.id 
where  a.state =0  and  OrderStateNow in (1,2)  and OrderType=4
and  OperateTime>='{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}' and OperateTime<'{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' ";
            if (indexQuery.CustomerID == "")
            {
                Sql += $" and  b.OrganID in({indexQuery.OrganizationID})  ";
            }
            else
            {
                Sql += $" and b.id in ({indexQuery.CustomerID}) and  b.OrganID in({indexQuery.OrganizationID}) ";
            }

            Sql += $@" group by case OrderStateNow when 1 then '未拣货' when 2 then '拣货中' END ";

            var query = connection.Query<acc_WavePick>(Sql).
               Select(p => new
               {
                   day = p.WavePick,
                   Percent = Convert.ToDecimal(p.Order)
               }).ToArray();

            mixed.day = query.Select(p => p.day).ToArray();
            mixed.Value = query.Select(p => p.Percent).ToArray();

            return mixed;

        }

        /// <summary>
        /// 二次分拣单
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public MixedLineAndBarObject SecondOrderPicking(IndexQueryObject indexQuery)
        {
            MixedLineAndBarObject mixed = new MixedLineAndBarObject();
            mixed.target_cn = "二次分拣单 未完成";

            string Sql = $@"   
SELECT  case OrderStateNow when 1 then '未拣货' when 2 then '拣货中' when 12 then '未二次分拣' END AS 'WavePick',sum(`Order`) as 'Order'
from acc_WavePick a 
left join bsc_Customer b on a.CustomerID=b.id 
where  a.state =0  and  OrderStateNow in (1,2,12)  and OrderType=3
and  OperateTime>='{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}' and OperateTime<'{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' ";

            if (indexQuery.CustomerID == "")
            {
                Sql += $" and  b.OrganID in({indexQuery.OrganizationID}) ";
            }
            else
            {
                Sql += $" and b.id in ({indexQuery.CustomerID}) and  b.OrganID in({indexQuery.OrganizationID}) ";
            }

            Sql += $@" group by case OrderStateNow when 1 then '未拣货' when 2 then '拣货中' when 12 then '未二次分拣' END ";
            var query = connection.Query<acc_WavePick>(Sql).
               Select(p => new
               {
                   day = p.WavePick,
                   Percent = Convert.ToDecimal(p.Order)
               }).ToArray();

            mixed.day = query.Select(p => p.day).ToArray();
            mixed.Value = query.Select(p => p.Percent).ToArray();

            return mixed;

        }

        /// <summary>
        /// 活动单现货
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public TableObject ActivityOrder(IndexQueryObject indexQuery)
        {
            TableObject mixed = new TableObject();
            mixed.target_cn = "活动单-现货 未完成";

            string Sql = $@"   
SELECT  b.ItemName as 'WavePick', sum( `Order` ) AS 'Order' 
FROM acc_WavePick  a
	left join  pub_Idreplace b on a.OrderStateNow=b.`Value` and b.TypeName='订单最新状态'
left join bsc_Customer c on a.CustomerID=c.id 
WHERE  a.state = 0  AND a.OrderType = 1  and  OrderStateNow in (1,2,3,4,5,6,7)
	and  OperateTime>='{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}' and OperateTime<'{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' ";

            if (indexQuery.CustomerID == "")
            {
                Sql += $" and  c.OrganID in({indexQuery.OrganizationID}) ";
            }
            else
            {
                Sql += $" and c.id in ({indexQuery.CustomerID}) and  c.OrganID in({indexQuery.OrganizationID})";
            }

            Sql += $@" GROUP BY b.ItemName ";
            mixed.table = connection.Query<acc_WavePick>(Sql).
               Select(p => new TableDataObject
               {
                   WarehouseName = p.WavePick,
                   OrderCount = p.Order
               }).ToArray();

            return mixed;

        }
        /// <summary>
        /// 标准单
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public MixedLineAndBarObject StandardSingle(IndexQueryObject indexQuery)
        {
            MixedLineAndBarObject mixed = new MixedLineAndBarObject();
            mixed.target_cn = "标准单 未完成";

            string Sql = $@"   
SELECT  b.ItemName as 'WavePick', sum( `Order` ) AS 'Order' 
FROM acc_WavePick  a
	left join  pub_Idreplace b on a.OrderStateNow=b.`Value` and b.TypeName='订单最新状态'
left join bsc_Customer c on a.CustomerID=c.id 
WHERE  a.state = 0  AND a.OrderType = 2 and  OrderStateNow in (1,2,9,10)
	and  OperateTime>='{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}' and OperateTime<'{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' ";
            if (indexQuery.CustomerID == "")
            {
                Sql += $" and  c.OrganID in({indexQuery.OrganizationID}) ";
            }
            else
            {
                Sql += $" and c.id in ({indexQuery.CustomerID})  and  c.OrganID in({indexQuery.OrganizationID})  ";
            }
            Sql += $@" GROUP BY b.ItemName ";
            var query = connection.Query<acc_WavePick>(Sql).
               Select(p => new
               {
                   day = p.WavePick,
                   Percent = Convert.ToDecimal(p.Order)
               }).ToArray();

            mixed.day = query.Select(p => p.day).ToArray();
            mixed.Value = query.Select(p => p.Percent).ToArray();

            return mixed;

        }



        public TableObject CustomerCompletion1(IndexQueryObject indexQuery)
        {
            TableObject tb = new TableObject();
            tb.target_cn = "客户完成情况";
            string Sql = $@" SELECT  concat( warehousename, ShipperName )   AS WarehouseName,
	sum( OrderCount ) AS 'OrderCount', sum( Shipping ) AS 'Shipping' 
FROM bi_Wms  WHERE
createtime >= '{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}'  AND createtime < '{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' 
AND  DataFrom  IN(0,1)";
            if (indexQuery.CustomerID == "")
            {
                Sql += $" and  OrganizationID in({indexQuery.OrganizationID})  ";
            }
            else
            {
                Sql += $" and  OrganizationID  in({indexQuery.OrganizationID}) and CustomerID in ({indexQuery.CustomerID})";
            }
            Sql += $@" GROUP BY concat( warehousename, ShipperName )  order  by sum( OrderCount )  desc ";
            tb.table = connection.Query<bi_Wms>(Sql).
              Select(p => new TableDataObject
              {
                  WarehouseName = p.WarehouseName,
                  OrderCount = p.OrderCount,
                  Shipping = p.Shipping,
                  Percent = Convert.ToDecimal(p.OrderCount) == 0 ? 0 : Convert.ToDecimal(p.Shipping) / Convert.ToDecimal(p.OrderCount)
              }).ToArray();

            return tb;

        }
        #endregion


        #region 4:现场看板


        /// <summary>
        /// 现场看板
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObject SceneOrder(IndexQueryObject indexQuery)
        {
            ResponseObject result = new ResponseObject();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            indexQuery.OrganizationID = Organization.GetSubOrganization(Convert.ToInt32(indexQuery.OrganizationID));
            try
            {

                /// 活动台打包情况
                keyValues.Add("ActivityPack", ActivityPack(indexQuery));

                /// 活动台抽检
                keyValues.Add("ActivityCheck", ActivityCheck(indexQuery));
                ///活动台总检
                keyValues.Add("ActivityPicking", ActivityPicking(indexQuery));
                ///活动单复核
                keyValues.Add("ActivityReview", ActivityReview(indexQuery));
                ///二次分拣单总检
                keyValues.Add("SecondOrderPickingAll", SecondOrderPickingAll(indexQuery));
                /// 二次分拣
                keyValues.Add("SecondOrderPickingCount", SecondOrderPickingCount(indexQuery));
                ///边检边分单
                keyValues.Add("OrderPickingPartCount", OrderPickingPartCount(indexQuery));
                ///标准单打包
                keyValues.Add("StandardPack", StandardPack(indexQuery));


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
        /// 活动台打包情况
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public TablePackObject ActivityPack(IndexQueryObject indexQuery)
        {
            TablePackObject tb = new TablePackObject();
            tb.target_cn = "活动台打包";

            string Sql = $@"   
SELECT  sum(`Order`)  as 'Order' from acc_WavePick a 
left join bsc_Customer c on a.CustomerID=c.id 
where   a.state =0 and OrderStateNow=5
and OperateTime>='{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}' and OperateTime<'{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' ";
            if (indexQuery.CustomerID == "")
            {
                Sql += $" and  c.OrganID  in({indexQuery.OrganizationID})";
            }
            else
            {
                Sql += $" and c.id in ({indexQuery.CustomerID}) and  c.OrganID  in({indexQuery.OrganizationID})";
            }

            string sql1 = $@"
SELECT  TableNumber,sum( a1.Order ) AS 'Order',max(PeopleNumber) as PeopleNumber  from acc_WavePickDetails  a1
	left join acc_WavePick  a on a1.WavePick=a.WavePick
	left join bsc_Customer c on a.CustomerID=c.id 
where a1.createtime>='{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}' and a1.createtime<'{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' 
and OrderState=900 and a1.state =0 and a.state =0";

            if (indexQuery.CustomerID == "")
            {
                sql1 += $" and  c.OrganID  in({indexQuery.OrganizationID})";
            }
            else
            {
                sql1 += $" and c.id in ({indexQuery.CustomerID})  and  c.OrganID  in({indexQuery.OrganizationID})";
            }
            sql1 += $@" GROUP BY TableNumber  order by  sum( a1.Order) desc  ";

            var query = connection.Query<acc_WavePick>(Sql).Select(p => new { Order = p.Order }).ToArray();//未打包数量
            var query1 = connection.Query<acc_WavePickDetails>(sql1).Select(p =>
             new
             {
                 TableNumber = p.TableNumber,
                 order = p.Order,
                 PeopleNumber = p.PeopleNumber
             }).ToArray();//已打包数量


            tb.IspackNumber = query1.Select(p => p.order).Sum();//已经打包数量
            tb.NopackNumber = query.Select(p => p.Order).FirstOrDefault();//未打包数量
            tb.PeopleNumber = query1.Select(p => p.PeopleNumber).Sum();//总人数 
            tb.PeopleAvgNumber = tb.PeopleNumber == 0 ? 0 : tb.IspackNumber / tb.PeopleNumber;//人均单数

            tb.table = query1.Select(p => new TablePackDataObject { TableNumber = p.TableNumber, Order = p.order }).ToArray();

            return tb;

        }

        /// <summary>
        /// 活动台抽检
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public TableObject ActivityCheck(IndexQueryObject indexQuery)
        {
            TableObject tb = new TableObject();
            tb.target_cn = "活动台抽检";

            string Sql = $@"   
SELECT  c1.TableNumber,count(1) as 'PCS'  from acc_WavePickDetails a1
left join acc_WavePick  a on a1.WavePick=a.WavePick
left join bsc_Customer c on a.CustomerID=c.id 
left join acc_WavePickDetails c1 on a1.WavePick=c1.wavepick and c1.orderstate =900 and c1.state =0
where 
a1.createtime>='{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}' and a1.createtime<'{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}'
and a1.OrderState=2001 and a1.state =0 and a.state =0";

            if (indexQuery.CustomerID == "")
            {
                Sql += $" and  c.OrganID in({indexQuery.OrganizationID}) ";
            }
            else
            {
                Sql += $" and c.id in ({indexQuery.CustomerID}) and  c.OrganID in({indexQuery.OrganizationID})";
            }

            Sql += $@"  GROUP BY c1.TableNumber  ";

            var query1 = connection.Query<acc_WavePickDetails>(Sql).Select(p =>
             new
             {
                 TableNumber = p.TableNumber,
                 PCS = p.PCS
             }).ToArray();

            tb.table = query1.Select(p => new TableDataObject { WarehouseName = p.TableNumber, OrderCount = p.PCS }).ToArray();

            return tb;

        }


        /// <summary>
        /// 活动单总拣
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public TableObject ActivityPicking(IndexQueryObject indexQuery)
        {
            TableObject tb = new TableObject();
            tb.target_cn = "活动单总拣";

            string Sql = $@"    
 SELECT  b.userid,sum(b.Order) as 'order' from acc_WavePick  a left join   
 acc_WavePickDetails b  on a.WavePick =b.WavePick 
left join bsc_Customer c on a.CustomerID=c.id 
 where  a.state =0  and b.state =0 and  b.createtime >='{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}' 
 and b.createtime <'{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}'  and   b.OrderState=300 and a.OrderType in (1,5)";

            if (indexQuery.CustomerID == "")
            {
                Sql += $" and  c.OrganID in({indexQuery.OrganizationID}) ";
            }
            else
            {
                Sql += $" and c.id in ({indexQuery.CustomerID})  and  c.OrganID in({indexQuery.OrganizationID})";
            }


            Sql += $@"  GROUP BY b.userid  order by sum(b.Order)  desc 
";
            tb.table = connection.Query<acc_WavePickDetails>(Sql).
               Select(p => new TableDataObject
               {
                   WarehouseName = p.UserID,
                   OrderCount = p.Order
               }).ToArray();


            return tb;

        }

        /// <summary>
        /// 活动单复核
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public TableObject ActivityReview(IndexQueryObject indexQuery)
        {
            TableObject tb = new TableObject();
            tb.target_cn = "活动单复核";

            string Sql = $@"    
 SELECT  b.userid,sum(b.Order) as 'order' from acc_WavePick  a left join   
 acc_WavePickDetails b  on a.WavePick =b.WavePick 
left join bsc_Customer c on a.CustomerID=c.id 
 where  a.state =0  and b.state =0 and  b.createtime >='{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}' 
 and b.createtime <'{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}'  and   b.OrderState=700 and a.OrderType in (1,5)";
            if (indexQuery.CustomerID == "")
            {
                Sql += $" and  c.OrganID in({indexQuery.OrganizationID}) ";
            }
            else
            {
                Sql += $" and c.id in ({indexQuery.CustomerID}) and  c.OrganID in({indexQuery.OrganizationID})";
            }
            Sql += $@"  GROUP BY b.userid  order by sum(b.Order)  desc  ";
            tb.table = connection.Query<acc_WavePickDetails>(Sql).
               Select(p => new TableDataObject
               {
                   WarehouseName = p.UserID,
                   OrderCount = p.Order
               }).ToArray();


            return tb;

        }

        /// <summary>
        /// 二次分拣单总检
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public TableObject SecondOrderPickingAll(IndexQueryObject indexQuery)
        {
            TableObject tb = new TableObject();
            tb.target_cn = "二次分拣单总检";

            string Sql = $@"    
  SELECT a1.userid,sum( a1.Order) as 'order' from  acc_WavePickDetails a1 
left join acc_WavePick  a on a1.WavePick=a.WavePick
left join bsc_Customer c on a.CustomerID=c.id
where a1.state =0 
and a1.createtime >='{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}'   and a1.createtime <'{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' 
 and OrderState=500";
            if (indexQuery.CustomerID == "")
            {
                Sql += $" and  c.OrganID in({indexQuery.OrganizationID}) ";
            }
            else
            {
                Sql += $" and c.id in ({indexQuery.CustomerID}) and  c.OrganID in({indexQuery.OrganizationID})  ";
            }
            Sql += $@"   GROUP BY a1.userid order by sum( a1.Order) desc  ";
            tb.table = connection.Query<acc_WavePickDetails>(Sql).
               Select(p => new TableDataObject
               {
                   WarehouseName = p.UserID,
                   OrderCount = p.Order
               }).ToArray();


            return tb;

        }

        /// <summary>
        /// 二次分拣
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public TableObject SecondOrderPickingCount(IndexQueryObject indexQuery)
        {
            TableObject tb = new TableObject();
            tb.target_cn = "二次分拣";

            string Sql = $@"    
 SELECT  b.userid,sum(b.Order) as 'order' from acc_WavePick  a 
left join     acc_WavePickDetails b  on a.WavePick =b.WavePick 
left join bsc_Customer c on a.CustomerID=c.id
 where  a.state =0  and b.state =0 and  b.createtime >='{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}' 
 and b.createtime <'{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}'  and   b.OrderState=300 and a.OrderType =3";
            if (indexQuery.CustomerID == "")
            {
                Sql += $" and  c.OrganID in({indexQuery.OrganizationID}) ";
            }
            else
            {
                Sql += $" and c.id in ({indexQuery.CustomerID}) and  c.OrganID in({indexQuery.OrganizationID}) ";
            }

            Sql += $@" GROUP BY b.userid order by sum(b.Order)  desc  ";
            tb.table = connection.Query<acc_WavePickDetails>(Sql).
               Select(p => new TableDataObject
               {
                   WarehouseName = p.UserID,
                   OrderCount = p.Order
               }).ToArray();


            return tb;

        }

        /// <summary>
        /// 边拣边分单
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public TableObject OrderPickingPartCount(IndexQueryObject indexQuery)
        {
            TableObject tb = new TableObject();
            tb.target_cn = "边拣边分单";

            string Sql = $@"    
 SELECT  b.userid,sum(b.Order) as 'order' from acc_WavePick  a
left join    acc_WavePickDetails b  on a.WavePick =b.WavePick 
left join bsc_Customer c on a.CustomerID=c.id
 where  a.state =0  and b.state =0 and  b.createtime >='{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}' 
 and b.createtime <'{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}'  and   b.OrderState=300 and a.OrderType =4";
            if (indexQuery.CustomerID == "")
            {
                Sql += $" and  c.OrganID in({indexQuery.OrganizationID}) ";
            }
            else
            {
                Sql += $" and c.id in ({indexQuery.CustomerID})  and  c.OrganID in({indexQuery.OrganizationID})";
            }

            Sql += $@" GROUP BY b.userid  order by sum(b.Order)  desc 
";
            tb.table = connection.Query<acc_WavePickDetails>(Sql).
               Select(p => new TableDataObject
               {
                   WarehouseName = p.UserID,
                   OrderCount = p.Order
               }).ToArray();


            return tb;

        }

        /// <summary>
        /// 标准单打包
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public TableObject StandardPack(IndexQueryObject indexQuery)
        {
            TableObject tb = new TableObject();
            tb.target_cn = "标准单打包";

            string Sql = $@"    
SELECT  OrderType as WarehouseName, sum(number) as number  from bi_Wms 

where CreateTime>='{(DateTime)indexQuery.startTime:yyyy-MM-dd  HH:mm:ss}' 
and   CreateTime<'{(DateTime)indexQuery.endTime:yyyy-MM-dd  HH:mm:ss}' and `DataFrom` =3 ";


            if (indexQuery.CustomerID == "")
            {
                Sql += $" and  OrganizationID in({indexQuery.OrganizationID}) ";
            }
            else
            {
                Sql += $" and CustomerID in ({indexQuery.CustomerID}) and  OrganizationID in({indexQuery.OrganizationID}) ";
            }
            Sql += "group by OrderType order by sum(number)  desc  ";

            tb.table = connection.Query<bi_Wms>(Sql).
               Select(p => new TableDataObject
               {
                   WarehouseName = p.WarehouseName,
                   OrderCount = p.Number
               }).ToArray();


            return tb;

        }

        #endregion


        #region 5:快递路由报表

        /// <summary>
        /// 快递路由报表
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObjectV2 ExpressRoute(SelectRouteObject indexQuery)
        {
            ResponseObjectV2 result = new ResponseObjectV2();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            int defaultPageSize = 20;

            try
            {
                if (indexQuery.PageIndex <= 0)
                    indexQuery.PageIndex = 1;
                if (indexQuery.PageSize < 0) //indexQuery.PageSize=0为取所有数据
                    indexQuery.PageSize = defaultPageSize;

                string Sql = $@"
select count(1) 
from bi_shipment s 
where 1=1 ";

                #region 查询条件
                //分公司
                if (!string.IsNullOrWhiteSpace(indexQuery.OrgName))
                {
                    Sql += $" and s.OrgName='{indexQuery.OrgName}' ";
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
                if (indexQuery.PaidTimeStart != null)
                {
                    Sql += $" and s.PaidTime >= '{indexQuery.PaidTimeStart:yyyy-MM-dd HH:mm:ss}' ";
                }
                if (indexQuery.PaidTimeEnd != null)
                {
                    Sql += $" and s.PaidTime < '{indexQuery.PaidTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
                }
                //转单时间
                if (indexQuery.OrderCreateTimeStart != null)
                {
                    Sql += $" and s.OrderCreateTime >= '{indexQuery.OrderCreateTimeStart:yyyy-MM-dd HH:mm:ss}' ";
                }
                if (indexQuery.OrderCreateTimeEnd != null)
                {
                    Sql += $" and s.OrderCreateTime < '{indexQuery.OrderCreateTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
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
                    Sql += $" and s.OutboundCode like '%{indexQuery.OutboundCode}%' ";
                }
                //快递单号
                if (!string.IsNullOrWhiteSpace(indexQuery.kddh))
                {
                    Sql += $" and s.kddh='{indexQuery.kddh}' ";
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
                #endregion

                result.TotalRows = (long)connection.ExecuteScalar(Sql);
                result.TotalPages = (int)Math.Ceiling(result.TotalRows * 1.00 / indexQuery.PageSize);

                Sql = Sql.Replace("select count(1)", @"select s.kdgs, s.kddh, s.OutboundCode
,s.StoreName
,s.SourcePlatform, s.CompanyCode, s.CompanyName, s.WarehouseName, s.WarehouseCode, s.PayTime
,s.PaidTime
,s.fachushijian as 'lanshoushijian'
#,s.wuliuzhuangtai
#,s.zuixinshijian as 'LastRouteTime'
#,s.zuixinshijian as 'UpdatedTime'
#,s.fachushijian
#,case when s.wuliuzhuangtai = '代收' or s.wuliuzhuangtai = '已签收' then s.zuixinshijian end as 'qianshoushijian'
,(select opType from ex_moqiuli_trace_v2 where kdgs=s.kdgs and kddh=s.kddh and s.TrailingSts=900) as 'wuliuzhuangtai' 
,(select opTime from ex_moqiuli_trace_v2 where kdgs=s.kdgs and kddh=s.kddh and s.TrailingSts=900) as 'LastRouteTime' 
,(select opTime from ex_moqiuli_trace_v2 where (opType='出站' or opType='签收') and kdgs=s.kdgs and kddh=s.kddh and s.TrailingSts=900) as 'qianshoushijian'
,replace((select JSON_EXTRACT(jsonText,'$.data.traceOpNodes[0].opTime') from ex_moqiuli_trace_v2 where kdgs=s.kdgs and kddh=s.kddh and s.TrailingSts=900),'""','') as 'lanshoushijian' 
");

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
                //result.message = Sql;
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
        /// 带有详细路由信息
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObjectV2 ExpressRouteDetails(SelectRouteObject indexQuery)
        {
            ResponseObjectV2 result = new ResponseObjectV2();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            int defaultPageSize = 20;

            try
            {
                if (indexQuery.PageIndex <= 0)
                    indexQuery.PageIndex = 1;
                if (indexQuery.PageSize < 0) //indexQuery.PageSize=0为取所有数据
                    indexQuery.PageSize = defaultPageSize;

                string Sql = $@"
select count(1) 
from bi_shipment s 
where 1=1";

                #region 查询条件
                //分公司
                if (!string.IsNullOrWhiteSpace(indexQuery.OrgName))
                {
                    Sql += $" and s.OrgName='{indexQuery.OrgName}' ";
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
                if (indexQuery.PaidTimeStart != null)
                {
                    Sql += $" and s.PaidTime >= '{indexQuery.PaidTimeStart:yyyy-MM-dd HH:mm:ss}' ";
                }
                if (indexQuery.PaidTimeEnd != null)
                {
                    Sql += $" and s.PaidTime < '{indexQuery.PaidTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
                }
                //转单时间
                if (indexQuery.OrderCreateTimeStart != null)
                {
                    Sql += $" and s.OrderCreateTime >= '{indexQuery.OrderCreateTimeStart:yyyy-MM-dd HH:mm:ss}' ";
                }
                if (indexQuery.OrderCreateTimeEnd != null)
                {
                    Sql += $" and s.OrderCreateTime < '{indexQuery.OrderCreateTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
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
                    Sql += $" and s.OutboundCode like '%{indexQuery.OutboundCode}%' ";
                }
                //快递单号
                if (!string.IsNullOrWhiteSpace(indexQuery.kddh))
                {
                    Sql += $" and s.kddh='{indexQuery.kddh}' ";
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
                    Sql += $@" and s.BigClass in ('{BigClass}')) ";
                }
                #endregion

                //result.TotalRows = (long)connection.ExecuteScalar(Sql);
                //result.TotalPages = (int)Math.Ceiling(result.TotalRows * 1.00 / indexQuery.PageSize);

                Sql = Sql.Replace("select count(1)", @"
select 
s.kdgs
,s.kddh
,s.OutboundCode
,s.StoreName
,s.SourcePlatform, s.CompanyCode, s.CompanyName, s.WarehouseName, s.WarehouseCode, s.PayTime
,s.PaidTime 
#,s.wuliuzhuangtai
#,s.zuixinshijian as 'LastRouteTime'
#,s.zuixinshijian as 'UpdatedTime'
#,s.fachushijian
,(select opType from ex_moqiuli_trace_v2 where kdgs=s.kdgs and kddh=s.kddh and s.TrailingSts=900) as 'wuliuzhuangtai' 
,(select opTime from ex_moqiuli_trace_v2 where kdgs=s.kdgs and kddh=s.kddh and s.TrailingSts=900) as 'LastRouteTime' 
,(select opTime from ex_moqiuli_trace_v2 where (opType='出站' or opType='签收') and kdgs=s.kdgs and kddh=s.kddh and s.TrailingSts=900) as 'qianshoushijian'
,replace((select JSON_EXTRACT(jsonText,'$.data.traceOpNodes[0].opTime') from ex_moqiuli_trace_v2 where kdgs=s.kdgs and kddh=s.kddh and s.TrailingSts=900),'""','') as 'lanshoushijian' 
,(select xiangxiwuliu from ex_moqiuli_trace_v2 where kdgs=s.kdgs and kddh=s.kddh) as 'xiangxiwuliu' ");

                #region 排序条件
                //if (!string.IsNullOrWhiteSpace(indexQuery.OrderBy))
                //{
                //    //string orderby = indexQuery.OrderBy.Split(' ')[0].ToLower();
                //    //if (orderby == "warehousecode" || orderby == "warehousename"
                //    //    || orderby == "companycode" || orderby == "companyname"
                //    //    || orderby == "sourceplatform" || orderby == "storename"
                //    //    || orderby == "paytime" || orderby == "outboundcode")
                //    //{
                //    //    Sql += $"order by s.{indexQuery.OrderBy}";
                //    //}
                //    //if (orderby == "kdgs" || orderby == "kddh" 
                //    //    || orderby == "wuliuzhuangtai" 
                //    //    || orderby == "xiangxiwuliu" 
                //    //    || orderby == "zuixinshijian" 
                //    //    || orderby == "chaxunshijian")
                //    //{
                //    //    Sql += $"order by er.{indexQuery.OrderBy}";
                //    //}
                //    Sql += $"order by {indexQuery.OrderBy}";
                //}
                //else
                //{
                //    Sql += $"order by er.ID desc ";
                //}
                #endregion

                if (indexQuery.PageIndex > 0 && indexQuery.PageSize > 0)
                {
                    Sql += $"limit {(indexQuery.PageIndex - 1) * indexQuery.PageSize},{indexQuery.PageSize}";
                }

                VisualBoard.Models.Request.RouteObject[] routeObjs = connection.Query<RouteObject>(Sql).
                   Select(p => new RouteObject
                   {
                       WarehouseName = p.WarehouseName,
                       WarehouseCode = p.WarehouseCode,
                       CompanyCode = p.CompanyCode,
                       CompanyName = p.CompanyName,
                       OutboundCode = p.OutboundCode,
                       kdgs = p.kdgs,
                       kddh = p.kddh,
                       SourcePlatform = p.SourcePlatform,
                       StoreName = p.StoreName,
                       wuliuzhuangtai = p.wuliuzhuangtai,
                       LastRouteTime = p.LastRouteTime,
                       UpdatedTime = p.UpdatedTime,
                       xiangxiwuliu = p.xiangxiwuliu,
                       PayTime = p.PayTime
                   }).ToArray();

                //result.message = Sql;
                result.result = routeObjs;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 快递路由报表-各个大类分类状态下的订单数量
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObject ExpressRouteBigClassCount(SelectRouteObject indexQuery)
        {
            ResponseObject result = new ResponseObject();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            try
            {
                string Sql = $@"
select count(1) as 'all' 
        ,ifnull(sum(case when s.BigClass='待分配' then 1 else 0 end),0) as '待分配'
        ,ifnull(sum(case when s.BigClass='波次中' then 1 else 0 end),0) as '波次中'
        ,ifnull(sum(case when s.BigClass='已发运' then 1 else 0 end),0) as '已发运'
        ,ifnull(sum(case when s.BigClass='未揽收' then 1 else 0 end),0) as '未揽收'
        ,ifnull(sum(case when s.BigClass='已揽收' then 1 else 0 end),0) as '已揽收'
        ,ifnull(sum(case when s.BigClass='已签收' then 1 else 0 end),0) as '已签收'
        ,ifnull(sum(case when s.BigClass='已取消' then 1 else 0 end),0) as '已取消' 
from bi_shipment s 
#inner join zz_ExpressCompany ec on ec.zz_kdgs=er.kdgs 
#inner join bi_shipment s on s.kdgs=ec.wms_kdgs and s.kddh=er.kddh 
where 1=1 
        and s.BigClass in ('待分配','波次中','已发运','未揽收','已揽收','已签收','已取消') 
 ";
                #region 查询条件
                //分公司
                if (!string.IsNullOrWhiteSpace(indexQuery.OrgName))
                {
                    Sql += $" and s.OrgName='{indexQuery.OrgName}' ";
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
                if (indexQuery.PaidTimeStart != null)
                {
                    Sql += $" and s.PaidTime >= '{indexQuery.PaidTimeStart:yyyy-MM-dd HH:mm:ss}' ";
                }
                if (indexQuery.PaidTimeEnd != null)
                {
                    Sql += $" and s.PaidTime < '{indexQuery.PaidTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
                }
                //转单时间
                if (indexQuery.OrderCreateTimeStart != null)
                {
                    Sql += $" and s.OrderCreateTime >= '{indexQuery.OrderCreateTimeStart:yyyy-MM-dd HH:mm:ss}' ";
                }
                if (indexQuery.OrderCreateTimeEnd != null)
                {
                    Sql += $" and s.OrderCreateTime < '{indexQuery.OrderCreateTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
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
                    Sql += $" and s.OutboundCode like '%{indexQuery.OutboundCode}%' ";
                }
                //快递单号
                if (!string.IsNullOrWhiteSpace(indexQuery.kddh))
                {
                    Sql += $" and s.kddh='{indexQuery.kddh}' ";
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
                #endregion

                dynamic tb = connection.Query<dynamic>(Sql, commandTimeout: 600).ToArray();
                result.result = tb;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }

        #endregion

        #region 报表查询下拉框数据源相关
        /// <summary>
        /// 获取欧莱雅快递公司列表
        /// </summary>
        /// <returns></returns>
        public ResponseObject GetExpressCompany()
        {
            ResponseObject result = new ResponseObject();

            try
            {
                string Sql = $@"
select wms_kdgs as 'value', kdgsname as 'text' 
from zz_ExpressCompany ";

                VisualBoard.Models.Response.DropDownObject[] routeObjs = connection.Query<DropDownObject>(Sql).
                   Select(p => new DropDownObject
                   {
                       value = p.value,
                       text = p.text
                   }).ToArray();

                result.result = routeObjs;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 获取欧莱雅来源平台列表
        /// </summary>
        /// <returns></returns>
        public ResponseObject GetSourcePlatform()
        {
            ResponseObject result = new ResponseObject();

            try
            {
                string Sql = $@"
select distinct name as 'value', name as 'text' 
from bi_LorealSourcePlatform ";

                VisualBoard.Models.Response.DropDownObject[] routeObjs = connection.Query<DropDownObject>(Sql).
                   Select(p => new DropDownObject
                   {
                       value = p.value,
                       text = p.text
                   }).ToArray();

                result.result = routeObjs;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 获取欧莱雅货主列表
        /// </summary>
        /// <returns></returns>
        public ResponseObject GetHuoZhu()
        {
            ResponseObject result = new ResponseObject();

            try
            {
                string Sql = $@"
select distinct CompanyCode as 'value', CompanyName as 'text' 
from bi_LorealHuoZhu ";

                VisualBoard.Models.Response.DropDownObject[] routeObjs = connection.Query<DropDownObject>(Sql).
                   Select(p => new DropDownObject
                   {
                       value = p.value,
                       text = p.text
                   }).ToArray();

                result.result = routeObjs;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 获取欧莱雅店铺名称列表
        /// </summary>
        /// <returns></returns>
        public ResponseObject GetStoreName()
        {
            ResponseObject result = new ResponseObject();

            try
            {
                string Sql = $@"
select distinct Code as 'value', Name as 'text' 
from bi_LorealStoreName ";

                VisualBoard.Models.Response.DropDownObject[] routeObjs = connection.Query<DropDownObject>(Sql).
                   Select(p => new DropDownObject
                   {
                       value = p.value,
                       text = p.text
                   }).ToArray();

                result.result = routeObjs;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }

        #endregion

        /// <summary>
        /// 大促看板V2
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObject SaleOrderV2(IndexQueryObject indexQuery)
        {
            ResponseObject result = new ResponseObject();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();

            try
            {
                bi_wmsTableObject tb = new bi_wmsTableObject();
                tb.target_cn = "大促看板V2";

                #region
                //                string Sql = $@"
                //select w.WarehouseName, B.OrderType,A.* 
                //from bi_WarehouseName w 
                //inner join (
                //	select '已贴面单预售发运' as OrderType
                //	union 
                //	select '未贴面单预售发运' as OrderType
                //	union 
                //	select '现货' as OrderType
                //) B on 1=1
                //left join 
                //(
                //    select A.WarehouseName
                //		    ,'现货' as 'OrderType' 
                //		    ,sum(case when A.ProcessType='NORMAL' then 1 else 0 end) as 'OrderCount'
                //		    ,sum(case when A.ProcessType='NORMAL' and A.TrailingSts<=100 then 1 else 0 end) AS 'WaitDistribution'
                //		    ,sum(case when A.ProcessType='NORMAL' and A.TrailingSts>=200 and A.TrailingSts<=201 then 1 else 0 end) AS 'WavePicking'
                //		    ,sum(case when A.ProcessType='NORMAL' and A.TrailingSts>=300 and A.TrailingSts<=301 then 1 else 0 end) AS 'WaitPick'
                //		    ,sum(case when A.ProcessType='NORMAL' and A.TrailingSts>301 and A.TrailingSts<=500 then 1 else 0 end) AS 'EndPick'
                //		    ,0 AS 'WaitPackage'
                //		    ,sum(case when A.ProcessType='NORMAL' and A.TrailingSts=600 then 1 else 0 end) AS 'EndReview'
                //		    ,sum(case when A.ProcessType='NORMAL' and A.TrailingSts=810 then 1 else 0 end) AS 'EndPresell'
                //		    ,sum(case when A.ProcessType='NORMAL' and A.TrailingSts=899 then 1 else 0 end) AS 'OrderWeight'
                //		    ,sum(case when A.ProcessType='NORMAL' and A.TrailingSts=900 then 1 else 0 end) AS 'Shipping'
                //		    ,sum(case when A.ProcessType='CANCEL' then 1 else 0 end) AS 'Cancel'
                //		    ,sum(case when A.ProcessType='ABNORMAL' then 1 else 0 end) AS 'Exception'
                //		    ,sum(case when A.TrailingSts=900 and A.wuliuzhuangtai is not null then 1 else 0 end) AS 'Collect'
                //    from bi_shipment A 
                //    where 1=1
                //		    and A.ShipmentCategory3 not in ('preOrder','prePackage') 
                //		    and A.PayTime>='{(DateTime)indexQuery.startTime:yyyy/M/d HH:mm:ss}' 
                //		    and A.PayTime<'{(DateTime)indexQuery.endTime:yyyy/M/d HH:mm:ss}' 
                //    group by A.WarehouseName

                //    union all 

                //    select A.WarehouseName
                //		    ,case when A.ShipmentCategory3='prePackage' then '已贴面单预售发运' 
                //			      when A.ShipmentCategory3='NORMAL' and A.ShipmentCategory5='step' then '未贴面单预售发运' end as 'OrderType' 
                //		    ,sum(case when A.ProcessType='NORMAL' then 1 else 0 end) as '总单量'
                //		    ,sum(case when A.ProcessType='NORMAL' and A.TrailingSts<=100 then 1 else 0 end) AS '待分配'
                //		    ,sum(case when A.ProcessType='NORMAL' and A.TrailingSts>=200 and A.TrailingSts<=201 then 1 else 0 end) AS '波次中'
                //		    ,sum(case when A.ProcessType='NORMAL' and A.TrailingSts>=300 and A.TrailingSts<=301 then 1 else 0 end) AS '待拣货'
                //		    ,sum(case when A.ProcessType='NORMAL' and A.TrailingSts>301 and A.TrailingSts<=500 then 1 else 0 end) AS '拣货完成'
                //		    ,0 AS '待包装'
                //		    ,sum(case when A.ProcessType='NORMAL' and A.TrailingSts=600 then 1 else 0 end) AS '复核完成'
                //		    ,sum(case when A.ProcessType='NORMAL' and A.TrailingSts=810 then 1 else 0 end) AS '预售完成'
                //		    ,sum(case when A.ProcessType='NORMAL' and A.TrailingSts=899 then 1 else 0 end) AS '活动单称重'
                //		    ,sum(case when A.ProcessType='NORMAL' and A.TrailingSts=900 then 1 else 0 end) AS '已发运'
                //		    ,sum(case when A.ProcessType='CANCEL' then 1 else 0 end) AS '取消单'
                //		    ,sum(case when A.ProcessType='ABNORMAL' then 1 else 0 end) AS '异常单'
                //		    ,sum(case when A.TrailingSts=900 and A.wuliuzhuangtai is not null then 1 else 0 end) AS '快递揽收'
                //    from bi_shipment A 
                //    where 1=1
                //		    and (A.ShipmentCategory3='prePackage' or (A.ShipmentCategory3='NORMAL' and A.ShipmentCategory5='step'))
                //		    and A.PaidTime>='{(DateTime)indexQuery.startTime:yyyy/M/d HH:mm:ss}' 
                //		    and A.PaidTime<'{(DateTime)indexQuery.endTime:yyyy/M/d HH:mm:ss}' 
                //    group by A.WarehouseName,A.ShipmentCategory3,A.ShipmentCategory5 
                //) A  on B.OrderType=A.OrderType and A.WarehouseName=w.WarehouseName 
                //order by w.WarehouseName, B.ordertype
                // ";
                #endregion

                string Sql = $@"
select 
    WarehouseName, ordertype
    ,ifnull(sum(OrderCount),0) as OrderCount
    ,ifnull(sum(WaitDistribution),0) as WaitDistribution
    ,ifnull(sum(WavePicking),0) as WavePicking
    ,ifnull(sum(WaitPick),0) as WaitPick
    ,ifnull(sum(EndPick),0) as EndPick
    ,ifnull(sum(WaitPackage),0) as WaitPackage
    ,ifnull(sum(EndReview),0) as EndReview
    ,ifnull(sum(EndPresell),0) as EndPresell
    ,ifnull(sum(OrderWeight),0) as OrderWeight
    ,ifnull(sum(Shipping),0) as Shipping
    ,ifnull(sum(Cancel),0) as Cancel
    ,ifnull(sum(Exception),0) as Exception
    ,ifnull(sum(Collect),0) as Collect
    ,ifnull((select max(OperateTime) from bi_LOREAL_bigSale),'') as OperateTime 
from bi_LOREAL_bigSale 
where 1=1
    and CreateTime>='{(DateTime)indexQuery.startTime:yyyy/M/d}' 
    and CreateTime<= '{(DateTime)indexQuery.endTime:yyyy/M/d}' 
group by WarehouseName, ordertype 
";

                tb.table = connection.Query<bi_Wms_V2>(Sql, commandTimeout: 600).
                Select(p => new bi_wmsTableDataObject
                {
                    WarehouseName = p.WarehouseName,
                    OrderType = p.OrderType,
                    Percent = Convert.ToDecimal(p.OrderCount) == 0 ? 0 : Convert.ToDecimal(p.Shipping) / Convert.ToDecimal(p.OrderCount),
                    OrderCount = p.OrderCount,
                    WaitDistribution = p.WaitDistribution,
                    WavePicking = p.WavePicking,
                    WaitPick = p.WaitPick,
                    EndPick = p.EndPick,
                    WaitPackage = p.WaitPackage,
                    EndReview = p.EndReview,
                    EndPresell = p.EndPresell,
                    OrderWeight = p.OrderWeight,
                    Shipping = p.Shipping,
                    Cancel = p.Cancel,
                    Exception = p.Exception,
                    DataFrom = p.DataFrom,
                    OperateTime = p.OperateTime,
                    Collect = p.Collect
                }).ToArray();

                /// 大促看板
                keyValues.Add("bi_WmsTable", tb);

                result.result = keyValues;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }

        #region 订单报表相关
        /// <summary>
        /// 订单报表
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObjectV2 RptOrder(SelectRouteObject indexQuery)
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
        and s.OrderCreateTime>='2021-10-20' 
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
                    Sql += $" and s.OutboundCode like '%{indexQuery.OutboundCode}%' ";
                }
                //快递单号
                if (!string.IsNullOrWhiteSpace(indexQuery.kddh))
                {
                    Sql += $" and s.kddh like '%{indexQuery.kddh}%' ";
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
                #endregion

                result.TotalRows = (long)connection.ExecuteScalar(Sql);
                if (indexQuery.PageSize > 0)
                    result.TotalPages = (int)Math.Ceiling(result.TotalRows * 1.00 / indexQuery.PageSize);
                else
                    result.TotalPages = 1;

                Sql = Sql.Replace("select count(1)", @"
select s.*
        #,s.fachushijian as 'lanshoushijian'
        #,case when s.wuliuzhuangtai = '代收' or s.wuliuzhuangtai = '已签收' then s.zuixinshijian end as 'qianshoushijian' 
        ,(select opType from ex_moqiuli_trace_v2 where kdgs=s.kdgs and kddh=s.kddh and s.TrailingSts=900) as 'wuliuzhuangtai' 
		,(select opTime from ex_moqiuli_trace_v2 where kdgs=s.kdgs and kddh=s.kddh and s.TrailingSts=900) as 'zuixinshijian' 
		,(select opTime from ex_moqiuli_trace_v2 where opType='签收' and kdgs=s.kdgs and kddh=s.kddh and s.TrailingSts=900) as 'qianshoushijian' 
		,replace((select JSON_EXTRACT(jsonText,'$.data.traceOpNodes[0].opTime') from ex_moqiuli_trace_v2 where kdgs=s.kdgs and kddh=s.kddh and s.TrailingSts=900),'""','') as 'lanshoushijian' 
");

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
                keyValues.Add("bi_WmsTable", tb);

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
        /// 订单报表-各个状态下的订单数量
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public ResponseObject RptOrdeStatusCount(SelectRouteObject indexQuery)
        {
            ResponseObject result = new ResponseObject();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            try
            {
                string Sql = $@"
select count(1) as 'all' 
        ,ifnull(sum(case when s.BigClass='待分配' then 1 else 0 end),0) as '待分配'
        ,ifnull(sum(case when s.BigClass='波次中' then 1 else 0 end),0) as '波次中'
        ,ifnull(sum(case when s.BigClass='已发运' then 1 else 0 end),0) as '已发运'
        ,ifnull(sum(case when s.BigClass='未揽收' then 1 else 0 end),0) as '未揽收'
        ,ifnull(sum(case when s.BigClass='已揽收' then 1 else 0 end),0) as '已揽收'
        ,ifnull(sum(case when s.BigClass='已签收' then 1 else 0 end),0) as '已签收'
        ,ifnull(sum(case when s.BigClass='已取消' then 1 else 0 end),0) as '已取消'
from bi_shipment s force index(idx_LastUpdateTime_BigClass) 
where 1=1 
        and s.OrderCreateTime>='2021-10-20' 
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
                    Sql += $" and s.OutboundCode like '%{indexQuery.OutboundCode}%' ";
                }
                //快递单号
                if (!string.IsNullOrWhiteSpace(indexQuery.kddh))
                {
                    Sql += $" and s.kddh like '%{indexQuery.kddh}%' ";
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
                #endregion

                dynamic tb = connection.Query<dynamic>(Sql, commandTimeout: 600).ToArray();
                result.result = tb;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }

        #endregion


        #region 订单报表导出相关
        /// <summary>
        /// 订单报表导出2
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public string RptOrderExecl2(SelectRouteObject indexQuery)
        {
            //publish.PublishToQuery(indexQuery, null, RabbitMQConst.ZhuzhuyunOrderRoutekey, RabbitMQConst.ZhuzhuyunOrderExchange);

            return "请求成功";
        }

        /// <summary>
        /// 订单导出
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <param name="DCname"></param>
        public void RptOrderExecl(object _indexQuery)
        {
            DCname = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xlsx";
            SelectRouteObject indexQuery = _indexQuery as SelectRouteObject;

            string cretime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff");
            ResponseObjectV2 result = new ResponseObjectV2();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            try
            {

                string Sql = $@"
select count(1) 
from bi_shipment s  
#left join zz_ExpressCompany ec on ec.wms_kdgs=s.kdgs  
#left join zz_ExpressRouteInfo er on er.kdgs=ec.zz_kdgs and er.kddh=s.kddh 
where 1=1 
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
                    Sql += $" and s.OutboundCode like '%{indexQuery.OutboundCode}%' ";
                }
                //快递单号
                if (!string.IsNullOrWhiteSpace(indexQuery.kddh))
                {
                    Sql += $" and s.kddh like '%{indexQuery.kddh}%' ";
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
                #endregion

                result.TotalRows = (long)connection.ExecuteScalar(Sql);
                if (indexQuery.PageSize > 0)
                    result.TotalPages = (int)Math.Ceiling(result.TotalRows * 1.00 / indexQuery.PageSize);
                else
                    result.TotalPages = 1;

                Sql = Sql.Replace("select count(1)", @"
select s.*
        ,s.fachushijian as 'lanshoushijian'
        ,case when s.wuliuzhuangtai = '代收' or s.wuliuzhuangtai = '已签收' then s.zuixinshijian end as 'qianshoushijian' 
");

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

                //if (indexQuery.PageIndex > 0 && indexQuery.PageSize > 0)
                //{
                //    Sql += $"limit {(indexQuery.PageIndex - 1) * indexQuery.PageSize},{indexQuery.PageSize}";
                //}

                List<SelectRouteObject> tb = connection.Query<SelectRouteObject>(Sql, commandTimeout: 1000000).ToList();

                string Addsql = $"insert into bi_download(ReportName,Cstate,WJstate,WJsize,CreateTime,OverTime,ErrorRZ) values ('订单报表','创建中','','','{cretime}','','') ";
                connection.Execute(Addsql);

                #region 导出数据

                XSSFWorkbook book = new XSSFWorkbook();
                ISheet s1 = book.CreateSheet("订单报表");
                IRow r1 = s1.CreateRow(0);
                r1.CreateCell(0).SetCellValue("行号");
                r1.CreateCell(1).SetCellValue("出库订单号");
                r1.CreateCell(2).SetCellValue("快递单号");
                r1.CreateCell(3).SetCellValue("快递公司");
                r1.CreateCell(4).SetCellValue("来源系统");
                r1.CreateCell(5).SetCellValue("分公司");
                r1.CreateCell(6).SetCellValue("波次号");
                r1.CreateCell(7).SetCellValue("波次类型");
                r1.CreateCell(8).SetCellValue("来源平台");
                r1.CreateCell(9).SetCellValue("店铺");
                r1.CreateCell(10).SetCellValue("出库类型3");
                r1.CreateCell(11).SetCellValue("出库类型5");
                r1.CreateCell(12).SetCellValue("首状态");
                r1.CreateCell(13).SetCellValue("尾状态");
                r1.CreateCell(14).SetCellValue("处理类型");
                r1.CreateCell(15).SetCellValue("货主编号");
                r1.CreateCell(16).SetCellValue("货主名称");
                r1.CreateCell(17).SetCellValue("仓库名称");
                r1.CreateCell(18).SetCellValue("件数");
                r1.CreateCell(19).SetCellValue("行数");
                r1.CreateCell(20).SetCellValue("订单创建时间");
                r1.CreateCell(21).SetCellValue("订单更新时间");
                r1.CreateCell(22).SetCellValue("付款时间");
                r1.CreateCell(23).SetCellValue("预售尾款支付时间");
                r1.CreateCell(24).SetCellValue("波次创建时间");
                r1.CreateCell(25).SetCellValue("拣货完成时间");
                r1.CreateCell(26).SetCellValue("复合完成时间");
                r1.CreateCell(27).SetCellValue("称重完成时间");
                r1.CreateCell(28).SetCellValue("发运时间");
                r1.CreateCell(29).SetCellValue("快递最新状态");
                r1.CreateCell(30).SetCellValue("快递最新更新时间");
                r1.CreateCell(31).SetCellValue("快递揽收时间");
                r1.CreateCell(32).SetCellValue("快递签收时间");

                for (int i = 0; i < tb.Count; i++)
                {
                    IRow rt = s1.CreateRow(i + 1);
                    rt.CreateCell(0).SetCellValue(i + 1);                             /* "行号");*/
                    rt.CreateCell(1).SetCellValue(tb[i].OutboundCode);                /* "出库订单号");*/
                    rt.CreateCell(2).SetCellValue(tb[i].kddh);                        /* "快递单号");*/
                    rt.CreateCell(3).SetCellValue(tb[i].kdgs);                        /* "快递公司");*/
                    rt.CreateCell(4).SetCellValue(tb[i].SourceSys);                   /* "来源系统");*/
                    rt.CreateCell(5).SetCellValue(tb[i].OrgName);                     /* "分公司");*/
                    rt.CreateCell(6).SetCellValue(tb[i].WaveId);                      /* "波次号");*/
                    rt.CreateCell(7).SetCellValue(tb[i].WaveType);                    /* "波次类型");*/
                    rt.CreateCell(8).SetCellValue(tb[i].SourcePlatform);              /* "来源平台");*/
                    rt.CreateCell(9).SetCellValue(tb[i].StoreName);                   /* "店铺");*/
                    rt.CreateCell(10).SetCellValue(tb[i].ShipmentCategory3);          /* ("出库类型3");*/
                    rt.CreateCell(11).SetCellValue(tb[i].ShipmentCategory5);          /* ("出库类型5");*/
                    rt.CreateCell(12).SetCellValue(tb[i].leadingSts);                 /* ("首状态");*/
                    rt.CreateCell(13).SetCellValue(tb[i].trailingSts);                /* ("尾状态");*/
                    rt.CreateCell(14).SetCellValue(tb[i].ProcessType);                /* ("处理类型");*/
                    rt.CreateCell(15).SetCellValue(tb[i].CompanyCode);                /* ("货主编号");*/
                    rt.CreateCell(16).SetCellValue(tb[i].CompanyName);                /* ("货主名称");*/
                    rt.CreateCell(17).SetCellValue(tb[i].WarehouseName);              /* ("仓库名称");*/
                    rt.CreateCell(18).SetCellValue(tb[i].TotalQty);                   /* ("件数");*/
                    rt.CreateCell(19).SetCellValue(tb[i].TotalLines);                 /* ("行数");*/
                    rt.CreateCell(20).SetCellValue(tb[i].OrderCreateTime);            /* ("订单创建时间");*/
                    rt.CreateCell(21).SetCellValue(tb[i].LastUpdateTime);             /* ("订单更新时间");*/
                    rt.CreateCell(22).SetCellValue(tb[i].PayTime);                    /* ("付款时间");*/
                    rt.CreateCell(23).SetCellValue(tb[i].PaidTime);                   /* ("预售尾款支付时间");*/
                    rt.CreateCell(24).SetCellValue(tb[i].WaveCreateTime);             /* ("波次创建时间");*/
                    rt.CreateCell(25).SetCellValue(tb[i].PickingCompleteTime);        /* ("拣货完成时间");*/
                    rt.CreateCell(26).SetCellValue(tb[i].CheckCompleteTime);          /* ("复合完成时间");*/
                    rt.CreateCell(27).SetCellValue(tb[i].WeighCompleteTime);          /* ("称重完成时间");*/
                    rt.CreateCell(28).SetCellValue(tb[i].ActualShipTime);             /* ("发运时间");*/
                    rt.CreateCell(29).SetCellValue(tb[i].wuliuzhuangtai);             /* ("快递最新状态");*/
                    rt.CreateCell(30).SetCellValue(tb[i].zuixinshijian);              /* ("快递最新更新时间");*/
                    rt.CreateCell(31).SetCellValue(tb[i].lanshoushijian);             /* ("快递揽收时间");*/
                    rt.CreateCell(32).SetCellValue(tb[i].qianshoushijian);            /*  ("快递签收时间");*/
                }
                #endregion
                var path = "导出/";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (FileStream stream = new FileStream(path + DCname, FileMode.CreateNew))
                {
                    book.Write(stream);

                    //stream.Seek(0, SeekOrigin.Begin);


                    string overtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff");
                    //string DCname = (stream.Name).Substring((stream.Name).Length - 22, 22);

                    //获取文件大小
                    System.IO.FileInfo fileInfo = null;
                    fileInfo = new System.IO.FileInfo(stream.Name);
                    if (fileInfo != null && fileInfo.Exists)
                    {
                        var Size = System.Math.Ceiling(fileInfo.Length / 1024.0);
                        string Uptsql = $"UPDATE bi_download set WJsize = '{Size}KB' where CreateTime = '{cretime}'";
                        connection.Execute(Uptsql);
                    }


                    string XZurl = "https://wcepod.rokin.cn/wvs/wvsexcel/" + $"{DCname}";
                    FtpClient client = new FtpClient("attach.rokin.cn");
                    client.Credentials = new NetworkCredential("wvsftp", "Qdv@1412$.3");
                    client.Connect();
                    var state = client.UploadFile(@$"导出/{DCname}", @$"wvsexcel\{DCname}");
                    client.Disconnect();
                    if (state == FtpStatus.Success)
                    {
                        string Uptsql = $"UPDATE bi_download set Cstate = '创建成功',WJstate='已创建',OverTime = '{overtime}',Url='{XZurl}' where CreateTime = '{cretime}'";
                        connection.Execute(Uptsql);
                    }
                    else
                    {
                        string Uptsql = $"UPDATE bi_download set Cstate = '创建失败',WJstate='未创建',OverTime = '{overtime}',ErrorRZ = '文件未创建' where CreateTime = '{cretime}'";
                        connection.Execute(Uptsql);
                    }
                    book.Close();
                    stream.Close();
                    #region 删除文件
                    //path为路径，可以右键文件选择属性看到 
                    //判断路径或文件夹是否存在(绝对路径)
                    if (Directory.Exists(path))
                    {
                        //返回所有文件夹的路径
                        var dirs = Directory.GetDirectories(path);
                        //遍历所有文件夹路径
                        for (int index = 0; index < dirs.Length; ++index)
                        {
                            //删除文件夹
                            Directory.Delete(dirs[index]);
                        }
                        //返回所有文件的路径
                        var files = Directory.GetFiles(path);
                        //遍历所有文件路径
                        for (int index = 0; index < files.Length; ++index)
                        {
                            //删除文件
                            File.Delete(files[index]);
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
        /// <summary>
        /// 快递路由导出2
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        public string ExpressRouteExecl2(SelectRouteObject indexQuery)
        {
            //publish.PublishToQuery(indexQuery, null, RabbitMQConst.ZhuzhuyunExpressRoutekey, RabbitMQConst.ZhuzhuyunExpressExchange);

            return "请求成功";
        }
        /// <summary>
        /// 快递路由报表导出
        /// </summary>
        /// <param name="indexQuery"></param>
        /// <returns></returns>
        #region 快递路由报表导出
        public void ExpressRouteExecl(object _indexQuery)
        {
            DCname = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xlsx";
            SelectRouteObject indexQuery = _indexQuery as SelectRouteObject;

            string cretime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff");
            ResponseObjectV2 result = new ResponseObjectV2();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            try
            {
                string Sql = $@"
select count(1) 
from bi_shipment s  
#left join zz_ExpressCompany ec on ec.wms_kdgs=s.kdgs  
#left join zz_ExpressRouteInfo er on er.kdgs=ec.zz_kdgs and er.kddh=s.kddh 
where 1=1 
 ";
                #region 查询条件
                //分公司
                if (!string.IsNullOrWhiteSpace(indexQuery.OrgName))
                {
                    Sql += $" and s.OrgName='{indexQuery.OrgName}' ";
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
                if (indexQuery.PaidTimeStart != null)
                {
                    Sql += $" and s.PaidTime >= '{indexQuery.PaidTimeStart:yyyy-MM-dd HH:mm:ss}' ";
                }
                if (indexQuery.PaidTimeEnd != null)
                {
                    Sql += $" and s.PaidTime < '{indexQuery.PaidTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
                }
                //转单时间
                if (indexQuery.OrderCreateTimeStart != null)
                {
                    Sql += $" and s.OrderCreateTime >= '{indexQuery.OrderCreateTimeStart:yyyy-MM-dd HH:mm:ss}' ";
                }
                if (indexQuery.OrderCreateTimeEnd != null)
                {
                    Sql += $" and s.OrderCreateTime < '{indexQuery.OrderCreateTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
                }
                ////更新时间
                //if (indexQuery.OrderLastUpdateTimeStart != null)
                //{
                //    Sql += $" and s.LastUpdateTime >= '{indexQuery.OrderLastUpdateTimeStart:yyyy-MM-dd HH:mm:ss}' ";
                //}
                //if (indexQuery.OrderLastUpdateTimeEnd != null)
                //{
                //    Sql += $" and s.LastUpdateTime < '{indexQuery.OrderLastUpdateTimeEnd:yyyy-MM-dd HH:mm:ss}' ";
                //}
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
                    Sql += $" and s.OutboundCode like '%{indexQuery.OutboundCode}%' ";
                }
                //快递单号
                if (!string.IsNullOrWhiteSpace(indexQuery.kddh))
                {
                    Sql += $" and s.kddh like '%{indexQuery.kddh}%' ";
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
                #endregion

                result.TotalRows = (long)connection.ExecuteScalar(Sql);
                if (indexQuery.PageSize > 0)
                    result.TotalPages = (int)Math.Ceiling(result.TotalRows * 1.00 / indexQuery.PageSize);
                else
                    result.TotalPages = 1;

                Sql = Sql.Replace("select count(1)", @"
select s.*
        ,s.fachushijian as 'lanshoushijian'
        ,case when s.wuliuzhuangtai = '代收' or s.wuliuzhuangtai = '已签收' then s.zuixinshijian end as 'qianshoushijian' 
");

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

                //if (indexQuery.PageIndex > 0 && indexQuery.PageSize > 0)
                //{
                //    Sql += $"limit {(indexQuery.PageIndex - 1) * indexQuery.PageSize},{indexQuery.PageSize}";
                //}

                List<SelectRouteObject> tb = connection.Query<SelectRouteObject>(Sql, commandTimeout: 1000000).ToList();

                XSSFWorkbook book = new XSSFWorkbook();
                string Addsql = $"insert into bi_download(ReportName,Cstate,WJstate,WJsize,CreateTime,OverTime,ErrorRZ) values ('快递路由报表','创建中','','','{cretime}','','') ";
                connection.Execute(Addsql);
                ISheet s1 = book.CreateSheet("快递路由报表");
                IRow r1 = s1.CreateRow(0);
                r1.CreateCell(0).SetCellValue("行号");
                r1.CreateCell(1).SetCellValue("出库订单号");
                r1.CreateCell(2).SetCellValue("快递单号");
                r1.CreateCell(3).SetCellValue("快递公司");
                r1.CreateCell(4).SetCellValue("快递最新状态");
                r1.CreateCell(5).SetCellValue("快递最新时间");
                r1.CreateCell(6).SetCellValue("快递揽收时间");
                r1.CreateCell(7).SetCellValue("快递签收时间");

                for (int i = 0; i < tb.Count; i++)
                {
                    IRow rt = s1.CreateRow(i + 1);
                    rt.CreateCell(0).SetCellValue(i + 1);
                    rt.CreateCell(1).SetCellValue(tb[i].OutboundCode);
                    rt.CreateCell(2).SetCellValue(tb[i].kddh);
                    rt.CreateCell(3).SetCellValue(tb[i].kdgs);
                    rt.CreateCell(4).SetCellValue(tb[i].wuliuzhuangtai);
                    rt.CreateCell(5).SetCellValue(tb[i].OrderLastUpdateTime);
                    rt.CreateCell(6).SetCellValue(tb[i].OutboundCode);
                    rt.CreateCell(7).SetCellValue(tb[i].CompanyCodeStr);
                }

                var path = "导出/";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (FileStream stream = new FileStream(path + DCname, FileMode.CreateNew))
                {
                    book.Write(stream);

                    string overtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff");


                    //获取文件大小
                    System.IO.FileInfo fileInfo = null;
                    fileInfo = new System.IO.FileInfo(stream.Name);
                    if (fileInfo != null && fileInfo.Exists)
                    {
                        var Size = System.Math.Ceiling(fileInfo.Length / 1024.0);
                        string Uptsql = $"UPDATE bi_download set WJsize = '{Size}KB' where CreateTime = '{cretime}'";
                        connection.Execute(Uptsql);
                    }


                    string XZurl = "https://wcepod.rokin.cn/wvs/wvsexcel/" + $"{DCname}";
                    FtpClient client = new FtpClient("attach.rokin.cn");
                    client.Credentials = new NetworkCredential("wvsftp", "Qdv@1412$.3");
                    client.Connect();
                    var state = client.UploadFile(@$"导出/{DCname}", @$"wvsexcel\{DCname}");
                    client.Disconnect();
                    if (state == FtpStatus.Success)
                    {
                        string Uptsql = $"UPDATE bi_download set Cstate = '创建成功',WJstate='已创建',OverTime = '{overtime}',Url='{XZurl}' where CreateTime = '{cretime}'";
                        connection.Execute(Uptsql);
                    }
                    else
                    {
                        string Uptsql = $"UPDATE bi_download set Cstate = '创建失败',WJstate='未创建',OverTime = '{overtime}',ErrorRZ = '文件未创建' where CreateTime = '{cretime}'";
                        connection.Execute(Uptsql);
                    }
                    book.Close();
                    stream.Close();
                    #region 删除文件
                    //path为路径，可以右键文件选择属性看到 
                    //判断路径或文件夹是否存在(绝对路径)
                    if (Directory.Exists(path))
                    {
                        //返回所有文件夹的路径
                        var dirs = Directory.GetDirectories(path);
                        //遍历所有文件夹路径
                        for (int index = 0; index < dirs.Length; ++index)
                        {
                            //删除文件夹
                            Directory.Delete(dirs[index]);
                        }
                        //返回所有文件的路径
                        var files = Directory.GetFiles(path);
                        //遍历所有文件路径
                        for (int index = 0; index < files.Length; ++index)
                        {
                            //删除文件

                        }
                    }
                    #endregion




                    //  return XZurl;
                    //is_RouterOrderExecl = false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
        /// <summary>
        /// 下载列表
        /// </summary>
        /// <returns></returns>
        public ResponseObject GetDownloads(FileUploadInfoSearchObject fileUploadInfoSearch)
        {
            ResponseObject responseObject = new ResponseObject();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();

            string sql = "select * from FileUploadInfo  where 1 = 1";

            if (fileUploadInfoSearch.CreateTimeStart != null && fileUploadInfoSearch.CreateTimeEnd != null)
            {
                sql += $" and CreateTimeBegin >= '{Convert.ToDateTime(fileUploadInfoSearch.CreateTimeStart).ToString("yyyy-MM-dd HH:mm:ss")}' and CreateTimeBegin <= '{Convert.ToDateTime(fileUploadInfoSearch.CreateTimeEnd).ToString("yyyy-MM-dd HH:mm:ss")}'";
            }
            if (!string.IsNullOrWhiteSpace(fileUploadInfoSearch.ModuleName))
                sql += $" and ModuleName='{fileUploadInfoSearch.ModuleName}'";
            if (!string.IsNullOrWhiteSpace(fileUploadInfoSearch.SystemSource))
                sql += $" and SystemSource='{fileUploadInfoSearch.SystemSource}'";
            if (!string.IsNullOrWhiteSpace(fileUploadInfoSearch.CreateUserID))
                sql += $" and CreateUserID='{fileUploadInfoSearch.CreateUserID}'";
            if (!string.IsNullOrWhiteSpace(fileUploadInfoSearch.CreateUserName))
                sql += $" and CreateUserName='{fileUploadInfoSearch.CreateUserName}'";
            sql += " ORDER BY CreateTimeBegin DESC";

            IEnumerable<FileUploadInfo> downloads = connection.Query<FileUploadInfo>(sql);
            keyValues.Add("data", downloads.Skip((fileUploadInfoSearch.pagenum - 1) * fileUploadInfoSearch.pagecount).Take(fileUploadInfoSearch.pagecount));
            keyValues.Add("count", downloads.Count());
            responseObject.result = keyValues;
            return responseObject;

        }
    }
}
