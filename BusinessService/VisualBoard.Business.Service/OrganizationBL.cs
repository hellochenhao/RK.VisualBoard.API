using Dapper;
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
    public class OrganizationBL : IOrganizationBL
    {
        private readonly _WMS_VisualboardContext WMS_Visualboard;
        private readonly IDbConnection connection;
        public OrganizationBL(_WMS_VisualboardContext _WMS_Visualboard, IDbConnection _connection)
        {
            WMS_Visualboard = _WMS_Visualboard;
            connection = _connection;
        }


        public ResponseObject InsterOrganization(bsc_Organization _Organization)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                if (WMS_Visualboard.bsc_Organization.Find(_Organization.ID) != null)
                {
                    result.code = 1;
                    result.message = "组织编码重复";
                }
                else
                {
                    _Organization.CreateTime = DateTime.Now;
                    _Organization.State = false;
                    WMS_Visualboard.Add(_Organization);
                    WMS_Visualboard.SaveChanges();
                    result.result = "保存成功";
                }
            }
            catch (Exception ex)
            {
                result.code = -1;
                result.message = ex.Message;
            }

            return result;
        }

        public ResponseObject SelectListOrganization(OrganSearchObject _Organization)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                string StrSql = "select * from bsc_Organization where State=0";
                if (_Organization.ID != null)
                    StrSql += $" and ID={_Organization.ID}";
                if(_Organization.OrganName!=null&& _Organization.OrganName.Trim()!="")
                    StrSql += $" and OrganName like '%{_Organization.OrganName}%'";
                result.result = connection.Query<bsc_Organization>(StrSql);
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }

        public ResponseObject SelectOneOrganization(bsc_Organization _Organization)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                result.result = connection.Query<bsc_Organization>(LambdaHelper.CreateWhere(_Organization)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }

        public ResponseObject UpdateOrganization(bsc_Organization _Organization)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                if (_Organization.State == true)
                    WMS_Visualboard.Remove(_Organization);
                else
                    WMS_Visualboard.UpdateNotNull(_Organization);
                WMS_Visualboard.SaveChanges();
                result.result = "保存成功";
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }


        public ResponseObject GetTreeOrganization()
        {
            ResponseObject result = new ResponseObject();
            try
            {
                var orglist = WMS_Visualboard.bsc_Organization.Where(p => p.State == false).ToList();
                var oper = GetChild(0);
                result.result = oper;
                List<TreeOrganObject> GetChild(int pid)
                {
                    List<TreeOrganObject> treeOrgans = new List<TreeOrganObject>();
                    var Porg = orglist.Where(p => p.ParentID == pid);
                    foreach (var p in Porg)
                    {
                        TreeOrganObject obj = new TreeOrganObject
                        {
                            value = p.ID,
                            label = p.OrganName,
                            children = GetChild(p.ID)
                        };
                        treeOrgans.Add(obj);
                    }
                    return treeOrgans;
                }
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }

        public string GetSubOrganization(int pid)
        {
            List<int> listcus = new List<int>();
            try
            {
                var orglist = WMS_Visualboard.bsc_Organization.Where(p => p.State == false).ToList();
                GetChild(pid);
                void GetChild(int pid)
                {
                    List<TreeOrganObject> treeOrgans = new List<TreeOrganObject>();
                    var Porg = orglist.Where(p => p.ParentID == pid);
                    listcus.AddRange(Porg.Select(p => p.ID));
                    foreach (var p in Porg)
                    {
                        GetChild(p.ID);

                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            listcus.Add(pid);
            return string.Join(",", listcus);
        }
        public ResponseObject GetTreeOrganAndCustomer()
        {
            ResponseObject result = new ResponseObject();
            try
            {
                var Customerlist = WMS_Visualboard.bsc_Customer.Where(p => p.State == false).ToList();
                var orglist = WMS_Visualboard.bsc_Organization.Where(p => p.State == false).ToList();
                var oper = GetChild(0);
                result.result = oper;
                List<TreeOrganAndCusObject> GetChild(int pid)
                {
                    List<TreeOrganAndCusObject> treeOrgans = new List<TreeOrganAndCusObject>();
                    var Porg = orglist.Where(p => p.ParentID == pid);
                    foreach (var p in Porg)
                    {
                        TreeOrganAndCusObject obj = new TreeOrganAndCusObject
                        {
                            value = p.ID,
                            label = p.OrganName,
                            children = GetChild(p.ID)
                        };
                        treeOrgans.Add(obj);
                    
                    }
                    var cusobj = Customerlist.Where(x => x.OrganID == pid).Select(x => new TreeOrganAndCusObject { label = x.CustomerName, icon = "el-icon-s-order" });
                    treeOrgans.AddRange(cusobj);
                    return treeOrgans;
                }
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }

        public ResponseObject GetCustomerForOrgan(int OrganID)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                var orglist = WMS_Visualboard.bsc_Organization.Where(p => p.State == false).ToList();
                List<int> OrganArr = new List<int>();
                OrganArr.Add(OrganID);
                GetChild(OrganID);
                result.result = WMS_Visualboard.bsc_Customer.Where(p => p.State == false && OrganArr.Contains((int)p.OrganID)).ToList();


                void GetChild(int pid)
                {
                    List<TreeOrganObject> treeOrgans = new List<TreeOrganObject>();
                    var Porg = orglist.Where(p => p.ParentID == pid);
                    foreach (var p in Porg)
                    {
                        GetChild(p.ID);
                        OrganArr.Add(p.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
         
        }

        public ResponseObject BatDelOrganization(int[] IDs)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                string Sql = $"update bsc_Organization set State=1 where ID in ({string.Join(',', IDs)})";
                if (connection.Execute(Sql) > 0)
                    result.result = "保存成功";
                else
                {
                    result.message = "保存失败";
                    result.code = 1;
                }
            }
            catch (Exception ex)
            {
                result.code = -1;
                result.message = ex.Message;
            }
            
            return result;
        }
    }
}
