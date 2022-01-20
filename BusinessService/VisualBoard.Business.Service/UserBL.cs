using Dapper;
using IdentityModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Rokin.Common.Redis;
using Rokin.Common.Tools;
using Rokin.Dapper;
using Rokin.EFCore.WMS_VisualBoard.WMS_Visualboard;
using Rokin.Shared.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using VisualBoard.Business.Interface;
using VisualBoard.Models.Request;
using VisualBoard.Models.Response;


namespace VisualBoard.Business.Service
{
    public class UserBL : IUserBL
    {
        private readonly _WMS_VisualboardContext WMS_Visualboard;
        private JwtSettings _jwtSettings;
        private readonly IDbConnection connection;
        public UserBL(_WMS_VisualboardContext _WMS_Visualboard, IOptions<JwtSettings> _jwtSettingsAccesser, IDbConnection _connection)
        {
            WMS_Visualboard = _WMS_Visualboard;
            _jwtSettings = _jwtSettingsAccesser.Value;
            connection = _connection;
        }

        public ResponseObject login(bsc_User _User)
        {
            ResponseObject result = new ResponseObject();
            Dictionary<string, object> keyValues = new Dictionary<string, object>();
            try
            {
                var Userobj = WMS_Visualboard.bsc_User.Where(p => p.UserID == _User.UserID && p.PassWord == _User.PassWord && p.State == false).FirstOrDefault();
                if (Userobj != null)
                {
                    var roleObj = WMS_Visualboard.bsc_Role.Find(Userobj.RoleID);
                    if (roleObj.PCRole == null || roleObj.PCRole == "")
                    {
                        result.code = 3;
                        result.message = "该账号未分配权限";
                        return result;
                    }
                    var PCRole =Array.ConvertAll(roleObj.PCRole.Split(','),int.Parse);
                    var Menulist = WMS_Visualboard.bsc_menus.Where(p=>p.state==false&& PCRole.Contains(p.id)).ToList();
                    var FatherMenu = Menulist.Where(p => p.issubs == true).ToList();
                    List<ListMenuObject> menu = new List<ListMenuObject>();
                    FatherMenu.ForEach(p => {
                        ListMenuObject obj = JsonConvert.DeserializeObject<ListMenuObject>(JsonConvert.SerializeObject(p));
                        obj.Subs = Menulist.Where(x => x.issubs == false && x.father_id == p.id).ToList();
                        menu.Add(obj);
                    });
                    //var menu = Menulist.GroupBy(p => p.father_id).Select(p => p.ToList());
                    var CusArr = Array.ConvertAll(Userobj.Customer.Split(','), int.Parse);
                    var CusList = WMS_Visualboard.bsc_Customer.Where(p => p.State == false && CusArr.Contains(p.ID)).ToList();
                    Userobj.PassWord = null;
                    string accessToken = GetToken();
                    RedisHelper.HSet("UserToken", Userobj.UserID, accessToken);

                    keyValues.Add("UserInfo", Userobj);
                    keyValues.Add("menulist", menu);
                    keyValues.Add("accessToken", accessToken);
                    keyValues.Add("customer", CusList);

                    result.result = keyValues;
                }
                else
                {
                    result.code = 2;
                    result.message = "帐号或密码错误";
                }
            }
            catch (Exception ex)
            {
                result.code = -1;
                result.message = ex.Message;
            }
            return result;
        }


        public ResponseObject InsterUser(bsc_User _User)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                _User.CreateTime = DateTime.Now;
                _User.State = false;
                WMS_Visualboard.Add(_User);
                WMS_Visualboard.SaveChanges();
                result.result = "保存成功";
            }
            catch(Exception ex)  {
                result.code = 1;
                result.message = ex.Message;
            }
          
            return result;
        }

        public ResponseObject UpdateUser(bsc_User _User)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                WMS_Visualboard.UpdateNotNull(_User);
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

        public ResponseObject SelectListUser(Userobj _User)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                result.result =connection.Query<bsc_User>(LambdaHelper.CreateWhere(_User, TableName: "bsc_User"));
                result.message = LambdaHelper.CreateWhere(_User, TableName: "bsc_User");
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }

        public ResponseObject SelectOneUser(Userobj _User)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                result.result = connection.Query<bsc_User>(LambdaHelper.CreateWhere(_User, TableName: "bsc_User")).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }

        public ResponseObject GetPCMenuList()
        {
            ResponseObject result = new ResponseObject();
            try
            {
                var Menulist = connection.Query<bsc_menus>(LambdaHelper.CreateWhere(new bsc_menus { state = false }));
                var rootlist = Menulist.Where(p => p.issubs==true).Select(p => new TreeAppMenuObject { id = p.id, label = p.menu_title }).ToList();
                rootlist.ForEach(p => p.children = Menulist.Where(x => x.father_id == p.id).Select(x => new TreeAppMenuChildObject { id = x.id, label = x.menu_title }));

                result.result = rootlist;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }

        public ResponseObject GetAppMenuList()
        {
            ResponseObject result = new ResponseObject();
            try
            {
                var Menulist = connection.Query<bsc_AppMenus>(LambdaHelper.CreateWhere(new bsc_AppMenus { state = false }));
                var rootlist = Menulist.Where(p => p.father_id == 0).Select(p => new TreeAppMenuObject { id = p.id, label = p.menu_title }).ToList();
                rootlist.ForEach(p => p.children = Menulist.Where(x => x.father_id == p.id).Select(x => new TreeAppMenuChildObject { id = x.id, label = x.menu_title }));

                result.result = rootlist;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }

        public ResponseObject GetAppMenuRole(int id)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                var WeChatRole = Array.ConvertAll(WMS_Visualboard.bsc_Role.Find(id)?.WeChatRole.Split(','), int.Parse);

                var Menulist =WMS_Visualboard.bsc_AppMenus.Where(p => p.state == false && WeChatRole.Contains(p.id)).ToList();

                var rootlist = Menulist.Where(p => p.father_id == 0).Select(p => new TreeAppRoleObject
                {
                    id = p.id,
                    father_id = p.father_id,
                    issubs = p.issubs,
                    menu_icon = p.menu_icon,
                    menu_path = p.menu_path,
                    menu_title = p.menu_title,
                    OrderType = p.OrderType,
                }).ToList();
                rootlist.ForEach(p => p.children = Menulist.Where(x => x.father_id == p.id).Select(x => new TreeAppRoleObject
                {
                    id = x.id,
                    father_id = x.father_id,
                    issubs = x.issubs,
                    menu_icon = x.menu_icon,
                    menu_path = x.menu_path,
                    menu_title = x.menu_title,
                    OrderType = x.OrderType,
                }).ToList());

                result.result = rootlist;
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }

        public ResponseObject InsertRole(bsc_Role _Role)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                _Role.CreateTime = DateTime.Now;
                WMS_Visualboard.Add(_Role);
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

        public ResponseObject UpdateRole(bsc_Role _Role)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                WMS_Visualboard.UpdateNotNull(_Role);
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

        public ResponseObject SearchListRole(bsc_Role _Role)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                result.result = connection.Query<bsc_Role>(LambdaHelper.CreateWhere(_Role));
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }

        public ResponseObject SearchOneRole(bsc_Role _Role)
        {
            ResponseObject result = new ResponseObject();
            try
            {
                result.result = connection.Query<bsc_Role>(LambdaHelper.CreateWhere(_Role)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.message = ex.Message;
            }
            return result;
        }

        public string GetToken()
        {
            //测试自己创建的对象
            var user = new 
            {
                id = 1,
                phone = "138000000",
                password = "e10adc3949ba59abbe56e057f20f883e"
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            var authTime = DateTime.Now;//授权时间
            var expiresAt = new DateTime(authTime.AddDays(1).Year, authTime.AddDays(1).Month, authTime.AddDays(1).Day);//过期时间
            var NotBefore = authTime.AddSeconds(1);//生效时间
            var tokenDescripor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(JwtClaimTypes.Audience,_jwtSettings.Audience),
                    new Claim(JwtClaimTypes.Issuer,_jwtSettings.Issuer),
                    new Claim(JwtClaimTypes.Name, user.phone.ToString()),
                    new Claim(JwtClaimTypes.Id, user.id.ToString()),
                    new Claim(JwtClaimTypes.PhoneNumber, user.phone.ToString())
                }),
             
                NotBefore= NotBefore,
                Expires = expiresAt,
                //对称秘钥SymmetricSecurityKey
                //签名证书(秘钥，加密算法)SecurityAlgorithms
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescripor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}
