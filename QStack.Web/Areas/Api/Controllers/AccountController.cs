using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using QStack.Web.Areas.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QStack.Framework.Basic;
using QStack.Framework.Basic.Enum;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.ViewModel.Auth;
using QStack.Framework.Util;
using QStack.Framework.Core.Model;
using QStack.Framework.Core.CommonSearch;

namespace QStack.Web.Areas.Api.Controllers
{

    [Route("{area:exists}/[controller]/[action]/{id?}")]
    
    public class AccountController : ApiBaseController
    {
        private readonly IUserService _userService;
        readonly IFunctionService _functionService;
        private readonly JWTTokenOptions tokenOptions;
        public AccountController(IUserService userService,IFunctionService functionService, JWTTokenOptions jwtTokenOptions)
        {
            _userService = userService;
            _functionService = functionService;
            this.tokenOptions = jwtTokenOptions;
        }
      
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="isCookie">是否将token写入cookie承载</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm]string username, [FromForm]string password, bool isCookie = false)
        {

            var result = new ResponseResult<TokenResult>();
            var user = await _userService.Get<UserDto>(dto => dto.Name == username);

            if (user == null)
            {
                result.Message = nameof(BusinessCode.User_NotFound);
                result.Code = BusinessCode.User_NotFound;
                return NotFound(result);
            }
            if (user.PassWord != password.Trim())
            {
                result.Message = nameof(BusinessCode.Password_Invalid);
                result.Code = BusinessCode.Password_Invalid;

            }
            if (user.State == UserState.UnActive)
            {
                result.Message = nameof(BusinessCode.User_UnActive);
                result.Code = BusinessCode.User_UnActive;
                return BadRequest(result);
            }
            if (user.State == UserState.Freezed)
            {
                result.Message = nameof(BusinessCode.User_Freeze);
                result.Code = BusinessCode.User_Freeze;
                return BadRequest(result);
            }


            var claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            claimsIdentity.AddClaims(new Claim[]
               {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim("Id",user.Id.ToString())
               });
            var accessToken = GenerateAccessToken(claimsIdentity.Claims);
            ClaimsPrincipal userPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                RedirectUri = "/Home/Index",
            };
            if (isCookie)
            {
                authenticationProperties.StoreTokens(
                    new[]
                    {
                    new AuthenticationToken()
                    {
                        Name =JwtBearerDefaults.AuthenticationScheme,
                        Value = accessToken
                    }
                    });
            }
            else
                result.Data = new TokenResult { Token = accessToken };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, authenticationProperties);
            
            return Ok(result);
        }

        public async Task<IActionResult> Logout()
        {
            if (User?.Identity.IsAuthenticated == true)
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var result = new ResponseResult();
            
            return Ok(result);
        }

        private string GenerateAccessToken(IEnumerable<Claim> userClaims)
        {

            var expiration = this.tokenOptions.Expiration;
            var jwt = new JwtSecurityToken(issuer: this.tokenOptions.Issuer,
                                           audience: this.tokenOptions.Audience,
                                           claims: userClaims,
                                           notBefore: DateTime.UtcNow,
                                           expires: DateTime.UtcNow.Add(expiration),
                                           signingCredentials: this.tokenOptions.Credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            return accessToken;
        }
      
        /// <summary>
        /// <![CDATA[{"code":200,"data":{"user":{"id":0,"username":"admin","password":"any","name":"Super Admin","avatar":"https://wpimg.wallstcn.com/f778738c-e4f8-4870-b634-56703b4acafe.gif","introduction":"I am a super administrator","email":"admin@test.com","phone":"1234567890","roles":["admin"]}}}]]>
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> UserInfo()
        {
            //throw new Exception("'sss");
            //var user1 = User;
            //var cookes = HttpContext.Request.Cookies;
            var result = new ResponseResult<UserDto>();
            var user = await _userService.GetUserFunctions(CurrentUser.Id);
            if (user == null)
            {
                result.Message = nameof(BusinessCode.User_NotFound);
                result.Code = BusinessCode.User_NotFound;
                return NotFound(result);
            }
            //var functions = await _functionService.GetByRoles(user.Roles.Select(r => r.Id).ToArray());

            //result.Data = new UserV {
            //    Email = user.Email,
            //    Id = user.Id,
            //    Name = user.Name,
            //    UserName = user.Name,
            //    Phone = user.Mobile,
            //    Roles = user.Roles.Select(r => r.Name).ToList(),
            //    Functions = functions.Where(f => f.FunctionType == FunctionType.Menu).ToList()
            //};
            //user.Functions = GetForTree<FunctionDto>(user.Functions.ToList(), null);
            result.Data = user;

            return Ok(result);
        }

        
        public async Task<ResponseResult<PageModel<UserDto>>> GetUsers(DataTableOption query)
        {
            query.IncludePaths = new string[]{ "UserRoles.Role", "Group" };
     
            return await base.SearchQuery<UserDto, IUserService>(query);
        }

        public async Task<ResponseResult<int>> Post(UserDto dto)
        {
            //UpdateModel(dto);
            UserDto item = null;
            if (dto.Id > 0)
            {
                item = await _userService.Update(dto);
            }
            else
            {
                item = await _userService.AddAsync<UserDto>(dto);
            }

            var result = new ResponseResult<int>(item.Id);

            return result;
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = new ResponseResult();
            await _userService.UpdateState(id,UserState.Freezed);
            return Ok(result);
        }

        public async Task<ResponseResult> PostUserRoles(UserRoleV userRoleV)
        {
            var result = new ResponseResult();
            if (userRoleV != null && userRoleV.RoleIds != null )
            {
                var userRoles = new List<UserRoleDto>();
                foreach (var rid in userRoleV.RoleIds)
                {
                    userRoles.Add(new UserRoleDto { RoleId = rid, UserId = userRoleV.UserId });
                }

                await _userService.UpdateUserRoles(userRoleV.UserId, userRoles);
            }

            return result;
        }
        
        public async Task<ResponseResult> ChangePassword(PasswordV passwordV)
        {
            var result = new ResponseResult();
        
            if(passwordV.OldPassword.IsNullOrWhiteSpace()||
                passwordV.NewPassword.IsNullOrWhiteSpace()||
                passwordV.NewPassword1.IsNullOrWhiteSpace()||
                !passwordV.NewPassword.Equals(passwordV.NewPassword1))
            {
                result.Code = BusinessCode.Password_Params_Invalid;
                result.Message = nameof(BusinessCode.Password_Params_Invalid);
                return result;
            }
            var user = await _userService.Get<UserDto>(i => i.Id == passwordV.UserId);

            if (user == null)
            {
                result.Message = nameof(BusinessCode.User_NotFound);
                result.Code = BusinessCode.User_NotFound;
                return result;
            }
            if (!passwordV.OldPassword.Equals(user.PassWord))
            {
                result.Message = nameof(BusinessCode.Password_Invalid);
                result.Code = BusinessCode.Password_Invalid;
                return result;
            }

            await _userService.Update<UserDto>(u=>u.Id==passwordV.UserId,u=>new UserDto { PassWord = passwordV.NewPassword });

            return result;
        }

        /// <summary>
        /// reset不需要原密码。分开两个方法是为了权限更方便控制
        /// </summary>
        /// <param name="passwordV"></param>
        /// <returns></returns>
        public async Task<ResponseResult> ReSetPassword(PasswordV passwordV)
        {
            var result = new ResponseResult();

            if (passwordV.NewPassword.IsNullOrWhiteSpace() ||
                passwordV.NewPassword1.IsNullOrWhiteSpace() ||
                !passwordV.NewPassword.Equals(passwordV.NewPassword1))
            {
                result.Code = BusinessCode.Password_Params_Invalid;
                result.Message = nameof(BusinessCode.Password_Params_Invalid);
                return result;
            }
            var user = await _userService.Get<UserDto>(i => i.Id == passwordV.UserId);

            if (user == null)
            {
                result.Message = nameof(BusinessCode.User_NotFound);
                result.Code = BusinessCode.User_NotFound;
                return result;
            }
           
            await _userService.Update<UserDto>(u => u.Id == passwordV.UserId, u => new UserDto { PassWord = passwordV.NewPassword });

            return result;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Test()
        {
            await _userService.TestMutipleTrans(new UserDto {Name="test" });
            var user =await _userService.Get<UserDto>(u => u.Name == "test");
            await _functionService.AddOrUpdate<FunctionDto, int>(new FunctionDto { Name="testfunc", Code="5555" });
            return Ok("ok");
        }
    }
}