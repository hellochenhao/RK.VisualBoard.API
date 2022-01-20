using Dapper;
using NPOI.SS.Formula.Functions;
using Rokin.Common.Tools;
using Rokin.Dapper;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VisualBoard.Business.Interface;
using VisualBoard.Models.Request;
using VisualBoard.Models.Response;
namespace VisualBoard.Business.Service
{
    public class B2CStatementBL : IB2CStatementBL
    {
        private readonly _WMS_VisualboardContext WMS_Visualboard;
        private readonly IDbConnection connection;
        public B2CStatementBL(IEnumerable<IDbConnection> IDbConnectionEnumer, _WMS_VisualboardContext _WMS_Visualboard)
        {
            connection = IDbConnectionEnumer.FirstOrDefault(p => p.Database == "WMS_VisualBoard");
            WMS_Visualboard = _WMS_Visualboard;
        }

        /// <summary>
        /// 分公司下拉列表
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ResponseObject SelectList(string UserID)
        {
            ResponseObject result = new ResponseObject();

            try
            {
                string Sql = @$"select Customer from bsc_User where ID = {UserID}";

                var query = connection.ExecuteReader(Sql);



                DataTable dt = new DataTable();
                dt.Load(query);

                if (dt.Rows[0][0].ToString() == "")
                {
                    result.result = null;
                }
                else
                {
                    string SQL = @$"select left(CustomerName,2) from bsc_Customer where ID in ({dt.Rows[0][0]})";

                    result.result = connection.Query(SQL);
                }


            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 项目模糊搜索
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public ResponseObject SelectProject(B2CStatementObject obj)
        {
            ResponseObject result = new ResponseObject();

            try
            {
                var Sql = "select distinct project from b2c_daily where Project not in ('太仓震亮小B','广州逸仙','太仓溢荣美乐','太仓优瑞')";

                if (obj.Filiale != null)
                {
                    var filiale = string.Join("','", obj.Filiale);
                    Sql += @$" and Filiale in ('{filiale}')";
                }

                result.result = connection.Query(Sql);
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 查询报表数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ResponseObject ShowB2CStatement(B2CStatementObject obj)
        {
            ResponseObject result = new ResponseObject();

            try
            {
                var Sql = @$"SELECT
a.Filiale,
 CASE a.Project
 WHEN '欧莱雅' THEN'欧莱雅'
 WHEN '广美赞' THEN'广州美赞臣'
 WHEN '广州高露洁' THEN'广州高露洁'
 WHEN '合生元ANC保健品' THEN'合生元ANC保健品'
 WHEN '合生元BNC奶粉' THEN'合生元BNC奶粉'
 WHEN '汤臣佰腾' THEN'广州佰腾'
 WHEN '汤臣麦优百胜' THEN'广州麦优'
 WHEN '汤臣麦优跨境' THEN'广州麦优'
 WHEN '汤臣麦优跨境吉客云' THEN'广州麦优'
 WHEN '菲时卡' THEN'太仓菲时卡'
 WHEN '成都回力鞋' THEN'成都回力'
 WHEN '成都侍魂' THEN'成都侍魂'
 WHEN '广州盛昌华' THEN'广州盛昌华'
 WHEN 'WMF' THEN '昆山WMF'
 WHEN '华东合生元ANC' THEN'太仓健合'
 WHEN '华东合生元BNC' THEN'太仓健合'
 WHEN '昆山高露洁' THEN'昆山高露洁'
 WHEN '太仓麦优' THEN'太仓麦优'
 WHEN '太仓汤臣佰嘉' THEN'太仓佰腾'
 WHEN '太仓优瑞' THEN '太仓优瑞克'
 WHEN '太仓震亮' THEN'太仓震亮'
 WHEN '南京佰健' THEN'南京佰健'
 WHEN '太仓丽人丽妆' THEN'太仓丽人丽妆'
 WHEN '北京润美康' THEN'北京润美康'
 WHEN '成都海聚良品新' THEN'成都海聚'
 WHEN '广州佳然至美' THEN'佳然至美'
 WHEN '昆山家化' THEN'昆山家化pop'
 END as 'Project',
a.Date,sum(a.IncomingD) as 'IncomingD',sum(a.IncomingJ) as 'IncomingJ',sum(a.SaleJ) as 'SaleJ',sum(a.SaleSKU) as 'SaleSKU',sum(a.PoorJ) as 'PoorJ',sum(a.PoorSKU) as 'PoorSKU',sum(a.StorageNum) as 'StorageNum',sum(a.StorageUse) as 'StorageUse',
sum(a.StorageLV) as 'StorageLV',sum(a.OrderReception) as 'OrderReception',sum(a.OrderAccomplish) as 'OrderAccomplish',sum(a.OrderAccomplishJ) as 'OrderAccomplishJ',sum(a.ShipmentsLV) as 'ShipmentsLV',sum(a.DutiableD) as 'DutiableD',sum(a.DutiableLV) as 'DutiableLV',sum(a.ReallyShipmentsD) as 'ReallyShipmentsD',
sum(a.ReallySKU) as 'ReallySKU',sum(a.ReallyShipmentsJ) as 'ReallyShipmentsJ',sum(a.ShipmentsDsku) as 'ShipmentsDsku',sum(a.ShipmentsJDLV) as 'ShipmentsJDLV',sum(a.ZTO) as 'ZTO',sum(a.STO) as 'STO',sum(a.YTO) as 'YTO',sum(a.YD) as 'YD',sum(a.EMS) as 'EMS',sum(a.SF) as 'SF',max(a.HD_D) as 'HD_D',max(a.HD_J) as 'HD_J',max(a.XN_D) as 'XN_D',max(a.XN_J) as 'XN_J',max(a.MPS_D) as 'MPS_D',max(a.MPS_J) as 'MPS_J',
max(a.Single) as 'Single',max(a.Piece) as 'Piece',sum(a.CancelPiece) as 'CancelPiece',sum(a.SalesSingle) as 'SalesSingle',sum(a.SalesPiece) as 'SalesPiece',
sum(b.OwnStaff) as 'OwnStaff',sum(b.OwnStaffNum) as 'OwnStaffNum',sum(b.LabourStaff) as 'LabourStaff',sum(b.LabourStaffNum) as 'LabourStaffNum',sum(b.SupportStaff) as 'SupportStaff',sum(b.SupportStaffNum) as 'SupportStaffNum',sum(b.LaborProductivity) as 'LaborProductivity',sum(b.Aging) as 'Aging'
FROM
 b2c_daily a
 Left JOIN b2c_persondaily b ON a.Filiale = b.Filiale and a.Project = b.Project and a.Date = b.Date where a.Project not in ('太仓震亮小B','广州逸仙','太仓溢荣美乐','太仓优瑞') and a.Filiale is not null AND a.Date BETWEEN '{Convert.ToDateTime(obj.startTime).ToString("yyyy-MM-dd")}' AND '{Convert.ToDateTime(obj.endTime).ToString("yyyy-MM-dd")}' ";

                if (obj.Filiale != null)
                {
                    var filiale = string.Join("','", obj.Filiale);
                    Sql += @$" and a.Filiale in ('{filiale}')";
                }
                if (obj.Project != null)
                {
                    var project = string.Join("','", obj.Project);
                    Sql += @$" and a.Project in ('{project}')";
                }
                Sql += @" GROUP BY CASE a.Project
 WHEN '欧莱雅' THEN'欧莱雅'
 WHEN '广美赞' THEN'广州美赞臣'
 WHEN '广州高露洁' THEN'广州高露洁'
 WHEN '合生元ANC保健品' THEN'合生元ANC保健品'
 WHEN '合生元BNC奶粉' THEN'合生元BNC奶粉'
 WHEN '汤臣佰腾' THEN'广州佰腾'
 WHEN '汤臣麦优百胜' THEN'广州麦优'
 WHEN '汤臣麦优跨境' THEN'广州麦优'
 WHEN '汤臣麦优跨境吉客云' THEN'广州麦优'
 WHEN '菲时卡' THEN'太仓菲时卡'
 WHEN '成都回力鞋' THEN'成都回力'
 WHEN '成都侍魂' THEN'成都侍魂'
 WHEN '广州盛昌华' THEN'广州盛昌华'
 WHEN 'WMF' THEN '昆山WMF'
 WHEN '华东合生元ANC' THEN'太仓健合'
 WHEN '华东合生元BNC' THEN'太仓健合'
 WHEN '昆山高露洁' THEN'昆山高露洁'
 WHEN '太仓麦优' THEN'太仓麦优'
 WHEN '太仓汤臣佰嘉' THEN'太仓佰腾'
 WHEN '太仓优瑞' THEN '太仓优瑞克'
 WHEN '太仓震亮' THEN'太仓震亮'
 WHEN '南京佰健' THEN'南京佰健'
 WHEN '太仓丽人丽妆' THEN'太仓丽人丽妆'
 WHEN '北京润美康' THEN'北京润美康'
 WHEN '成都海聚良品新' THEN'成都海聚'
 WHEN '广州佳然至美' THEN'佳然至美'
 WHEN '昆山家化' THEN'昆山家化pop'
 END,a.Filiale,a.Date";
                var query = connection.Query(Sql);
                result.result = query.Skip((obj.pagenum - 1) * obj.pagecount).Take(obj.pagecount);
                result.code = query.Count();
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }


        /// <summary>
        /// 获取导出数据
        /// </summary>
        /// <returns></returns>
        public List<B2CExcelObject> GetList(B2CStatementObject obj)
        {

            var Sql = @$"SELECT
a.Filiale,
 CASE a.Project
 WHEN '欧莱雅' THEN'欧莱雅'
 WHEN '广美赞' THEN'广州美赞臣'
 WHEN '广州高露洁' THEN'广州高露洁'
 WHEN '合生元ANC保健品' THEN'合生元ANC保健品'
 WHEN '合生元BNC奶粉' THEN'合生元BNC奶粉'
 WHEN '汤臣佰腾' THEN'广州佰腾'
 WHEN '汤臣麦优百胜' THEN'广州麦优'
 WHEN '汤臣麦优跨境' THEN'广州麦优'
 WHEN '汤臣麦优跨境吉客云' THEN'广州麦优'
 WHEN '菲时卡' THEN'太仓菲时卡'
 WHEN '成都回力鞋' THEN'成都回力'
 WHEN '成都侍魂' THEN'成都侍魂'
 WHEN '广州盛昌华' THEN'广州盛昌华'
 WHEN 'WMF' THEN '昆山WMF'
 WHEN '华东合生元ANC' THEN'太仓健合'
 WHEN '华东合生元BNC' THEN'太仓健合'
 WHEN '昆山高露洁' THEN'昆山高露洁'
 WHEN '太仓麦优' THEN'太仓麦优'
 WHEN '太仓汤臣佰嘉' THEN'太仓佰腾'
 WHEN '太仓优瑞' THEN '太仓优瑞克'
 WHEN '太仓震亮' THEN'太仓震亮'
 WHEN '南京佰健' THEN'南京佰健'
 WHEN '太仓丽人丽妆' THEN'太仓丽人丽妆'
 WHEN '北京润美康' THEN'北京润美康'
 WHEN '成都海聚良品新' THEN'成都海聚'
 WHEN '广州佳然至美' THEN'佳然至美'
 WHEN '昆山家化' THEN'昆山家化pop'
 END as 'Project',
a.Date,sum(a.IncomingD) as 'IncomingD',sum(a.IncomingJ) as 'IncomingJ',sum(a.SaleJ) as 'SaleJ',sum(a.SaleSKU) as 'SaleSKU',sum(a.PoorJ) as 'PoorJ',sum(a.PoorSKU) as 'PoorSKU',sum(a.StorageNum) as 'StorageNum',sum(a.StorageUse) as 'StorageUse',
sum(a.StorageLV) as 'StorageLV',sum(a.OrderReception) as 'OrderReception',sum(a.OrderAccomplish) as 'OrderAccomplish',sum(a.OrderAccomplishJ) as 'OrderAccomplishJ',sum(a.ShipmentsLV) as 'ShipmentsLV',sum(a.DutiableD) as 'DutiableD',sum(a.DutiableLV) as 'DutiableLV',sum(a.ReallyShipmentsD) as 'ReallyShipmentsD',
sum(a.ReallySKU) as 'ReallySKU',sum(a.ReallyShipmentsJ) as 'ReallyShipmentsJ',sum(a.ShipmentsDsku) as 'ShipmentsDsku',sum(a.ShipmentsJDLV) as 'ShipmentsJDLV',sum(a.ZTO) as 'ZTO',sum(a.STO) as 'STO',sum(a.YTO) as 'YTO',sum(a.YD) as 'YD',sum(a.EMS) as 'EMS',sum(a.SF) as 'SF',max(a.HD_D) as 'HD_D',max(a.HD_J) as 'HD_J',max(a.XN_D) as 'XN_D',max(a.XN_J) as 'XN_J',max(a.MPS_D) as 'MPS_D',max(a.MPS_J) as 'MPS_J',
max(a.Single) as 'Single',max(a.Piece) as 'Piece',sum(a.CancelPiece) as 'CancelPiece',sum(a.SalesSingle) as 'SalesSingle',sum(a.SalesPiece) as 'SalesPiece',
sum(b.OwnStaff) as 'OwnStaff',sum(b.OwnStaffNum) as 'OwnStaffNum',sum(b.LabourStaff) as 'LabourStaff',sum(b.LabourStaffNum) as 'LabourStaffNum',sum(b.SupportStaff) as 'SupportStaff',sum(b.SupportStaffNum) as 'SupportStaffNum',sum(b.LaborProductivity) as 'LaborProductivity',sum(b.Aging) as 'Aging'
FROM
 b2c_daily a
 Left JOIN b2c_persondaily b ON a.Filiale = b.Filiale and a.Project = b.Project and a.Date = b.Date where a.Project not in ('太仓震亮小B','广州逸仙','太仓溢荣美乐','太仓优瑞') and a.Filiale is not null AND a.Date BETWEEN '{Convert.ToDateTime(obj.startTime).ToString("yyyy-MM-dd")}' AND '{Convert.ToDateTime(obj.endTime).ToString("yyyy-MM-dd")}' ";

            if (obj.Filiale != null)
            {
                var filiale = string.Join("','", obj.Filiale);
                Sql += @$" and a.Filiale in ('{filiale}')";
            }
            if (obj.Project != null)
            {
                var project = string.Join("','", obj.Project);
                Sql += @$" and a.Project in ('{project}')";
            }

            Sql += @" GROUP BY CASE a.Project
 WHEN '欧莱雅' THEN'欧莱雅'
 WHEN '广美赞' THEN'广州美赞臣'
 WHEN '广州高露洁' THEN'广州高露洁'
 WHEN '合生元ANC保健品' THEN'合生元ANC保健品'
 WHEN '合生元BNC奶粉' THEN'合生元BNC奶粉'
 WHEN '汤臣佰腾' THEN'广州佰腾'
 WHEN '汤臣麦优百胜' THEN'广州麦优'
 WHEN '汤臣麦优跨境' THEN'广州麦优'
 WHEN '汤臣麦优跨境吉客云' THEN'广州麦优'
 WHEN '菲时卡' THEN'太仓菲时卡'
 WHEN '成都回力鞋' THEN'成都回力'
 WHEN '成都侍魂' THEN'成都侍魂'
 WHEN '广州盛昌华' THEN'广州盛昌华'
 WHEN 'WMF' THEN '昆山WMF'
 WHEN '华东合生元ANC' THEN'太仓健合'
 WHEN '华东合生元BNC' THEN'太仓健合'
 WHEN '昆山高露洁' THEN'昆山高露洁'
 WHEN '太仓麦优' THEN'太仓麦优'
 WHEN '太仓汤臣佰嘉' THEN'太仓佰腾'
 WHEN '太仓优瑞' THEN '太仓优瑞克'
 WHEN '太仓震亮' THEN'太仓震亮'
 WHEN '南京佰健' THEN'南京佰健'
 WHEN '太仓丽人丽妆' THEN'太仓丽人丽妆'
 WHEN '北京润美康' THEN'北京润美康'
 WHEN '成都海聚良品新' THEN'成都海聚'
 WHEN '广州佳然至美' THEN'佳然至美'
 WHEN '昆山家化' THEN'昆山家化pop'
 END,a.Filiale,a.Date";

            var query = connection.Query<B2CExcelObject>(Sql).ToList();
            
            return query;

        }
    }
}