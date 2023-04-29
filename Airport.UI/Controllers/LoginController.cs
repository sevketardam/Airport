using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using Airport.DBEntitiesDAL.Interfaces;
using System.Security.Cryptography;
using System.Linq;

namespace Airport.UI.Controllers
{
    public class LoginController : Controller
    {
        IUserDatasDAL _user;
        public LoginController(IUserDatasDAL user)
        {
            _user = user;
        }

        [HttpGet("login")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Login(LoginVM loginValues,bool rememberMe)
        {
            if (loginValues != null)
            {
                try
                {
                    var convertPassword = GetMD5(loginValues.UserPassword);
                    var userData = _user.SelectByFunc(a => a.Eposta == loginValues.UserEposta && a.Password == convertPassword).FirstOrDefault();
                    if (userData != null)
                    {
                        var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.Sid,userData.Id.ToString()),
                            new Claim(ClaimTypes.Role,userData.Type.ToString()),
                        };

                        var userIdentity = new ClaimsIdentity(claims, "login");

                        ClaimsPrincipal pri = new ClaimsPrincipal(userIdentity);

                        if (rememberMe)
                        {
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, pri, new AuthenticationProperties
                            {
                                IsPersistent = true,
                                AllowRefresh = true,
                                RedirectUri = "/"
                            });
                        }
                        else
                        {
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, pri, new AuthenticationProperties
                            {
                                ExpiresUtc = DateTime.UtcNow.AddHours(2),
                                IsPersistent = true,
                                AllowRefresh = true,
                                RedirectUri = "/"
                            });
                        }


                        return new JsonResult(new { status = 200 });
                    }
                }
                catch (Exception ex)
                {
                    return new JsonResult(new { status = 400 });
                }


            }

            return new JsonResult(new { status = 404 });
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index","Home");
        }

        public static string GetMD5(string value)
        {
            MD5 md5 = MD5.Create();
            byte[] md5Bytes = System.Text.Encoding.Default.GetBytes(value);
            byte[] cryString = md5.ComputeHash(md5Bytes);
            string md5Str = string.Empty;
            for (int i = 0; i < cryString.Length; i++)
            {
                md5Str += cryString[i].ToString("X");
            }
            return md5Str;
        }
    }
}
