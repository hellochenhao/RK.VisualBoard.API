using Dapper;
using Rokin.Common.Redis;
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
    public class LoginBL : ILoginBL
    {
        private readonly _WMS_VisualboardContext WMS_Visualboard;
        private readonly IUserBL userBL;
        private readonly IDbConnection connection;

        public LoginBL(_WMS_VisualboardContext _WMS_Visualboard,IUserBL userBL, IDbConnection _connection)
        {
            WMS_Visualboard = _WMS_Visualboard;
            this.userBL = userBL;
            connection = _connection;
        }

        /// <summary>
        /// 小程序用户登录
        /// </summary>
        /// <param name="_User"></param>
        /// <returns></returns>
        public ResponseObject LoginsUser(bsc_User _User)
        {
            ResponseObject result = new ResponseObject();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            try
            {
                string SQL = $@"   SELECT  a.*,b.OrganName  from bsc_User  a
left join bsc_Organization b on a.OrganID=b.ID 
where  a.UserID='{_User.UserID}' and a.`PassWord`='{ _User.PassWord}'   and  a.state =0 ";
                var obj = connection.Query(SQL).Select
                (
                    p => new ResponseUserobj
                    {
                        ID = p.ID,
                        UserID = p.UserID,
                        UserName = p.UserName,
                        PassWord = p.PASSWORD,
                        Sex = p.Sex,
                        OrganID = p.OrganID,
                        UserType = p.UserType,
                        TelPhone = p.TelPhone,
                        RoleID = p.RoleID,
                        CreateTime = p.CreateTime,
                        State = p.State,
                        Customer = p.Customer,
                        OrganName = p.OrganName
                    }).FirstOrDefault();

                //var obj = WMS_Visualboard.bsc_User.Where(p => p.UserID == _User.UserID && p.PassWord == _User.PassWord && p.State == false).FirstOrDefault();
                if (obj == null)
                {
                    result.code = 2;
                    result.message = "帐号或密码错误";
                    return result;
                }
                if (obj.RoleID == null)
                {
                    result.code = 2;
                    result.message = "该用户未分配权限";
                    return result;
                }


                obj.PassWord = null;
                keyValues.Add("userinfo", obj);
                var CusArr = Array.ConvertAll(obj.Customer.Split(','), int.Parse);
                var CusList = WMS_Visualboard.bsc_Customer.Where(p => p.State == false && CusArr.Contains(p.ID)).ToList();
                if (CusList == null || CusList.Count == 0)
                {
                    result.code = 2;
                    result.message = "该用户未分配客户";
                }
                var MenuRoles = userBL.GetAppMenuRole((int)obj.RoleID).result;

                string accessToken = userBL.GetToken();
                RedisHelper.HSet("UserToken", obj.UserID, accessToken);
                bool HasCharts = false;
                bool HasQZWJ = false;
                bool HasSXBG = false;
                //报表看板
                var objroles = ((List<TreeAppRoleObject>)MenuRoles).Where(p => p.id == 9).FirstOrDefault();
                if (objroles!=null)
                {
                    ((List<TreeAppRoleObject>)MenuRoles).Remove(objroles);
                    HasCharts = true;
                }
                var objwanjie = ((List<TreeAppRoleObject>)MenuRoles).Where(p => p.id == 10).FirstOrDefault();
                if (objwanjie != null)
                {
                    if (objwanjie.children.Any(p => p.id == 1001))
                        HasQZWJ = true;
                    if (objwanjie.children.Any(p => p.id == 1002))
                        HasSXBG = true;
                    ((List<TreeAppRoleObject>)MenuRoles).Remove(objwanjie);
                }
                keyValues.Add("customer", CusList);
                keyValues.Add("hasEchats", HasCharts);
                keyValues.Add("HasQZWJ", HasQZWJ);
                keyValues.Add("HasSXBG", HasSXBG);
                keyValues.Add("menuRoles", MenuRoles);
                keyValues.Add("accessToken", accessToken);
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
        /// 临时用户添加信息
        /// </summary>
        /// <param name="_User"></param>
        /// <returns></returns>
        public ResponseObject AddUserInfo(bsc_User _User)
        {
            ResponseObject result = new ResponseObject();

            try
            {
                _User.State = false;
                _User.CreateTime = DateTime.Now;
                WMS_Visualboard.UpdateNotNull(_User);
                WMS_Visualboard.SaveChanges();

                result.result = _User;
            }
            catch (Exception ex)
            {

                result.code = 1;
                result.message = ex.Message;
            }

            return result;
        }
    }
}
