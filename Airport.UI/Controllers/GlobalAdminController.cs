using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Airport.UI.Controllers
{
    [Authorize(Roles = "0,4")]
    public class GlobalAdminController : Controller
    {
        ICouponsDAL _coupons;
        ILoginAuthDAL _loginAuth;
        IUserDatasDAL _userData;
        public GlobalAdminController(ICouponsDAL coupons,ILoginAuthDAL loginAuth,IUserDatasDAL userDta)
        {
            _coupons = coupons;
            _loginAuth = loginAuth;
            _userData = userDta;
        }

        [HttpGet("panel/coupons")]
        public IActionResult CreateCoupon()
        {

            var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
            var myCoupons = _coupons.SelectByFunc(a => a.UserId == userId);
            return View(myCoupons);
        }

        [HttpPost]
        public JsonResult CreateCouponJSON(Coupons coupon)
        {
            try
            {
                if (coupon != null)
                {
                    var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
                    _coupons.Insert(new Coupons
                    {
                        Active = coupon.Active,
                        Comment = coupon.Comment,
                        CouponCode = coupon.CouponCode,
                        CouponFinishDate = coupon.CouponFinishDate,
                        CouponStartDate = coupon.CouponStartDate,
                        CouponLimit = coupon.CouponLimit,
                        CreatedDate = DateTime.Now,
                        IsPerma = coupon.IsPerma,
                        Discount = coupon.Discount,
                        UserId = userId,
                        IsOffer = coupon.IsOffer,
                        UsingCount = 0
                    });
                    return new JsonResult(new { result = 1 });
                }
                return new JsonResult(new { result = 2 });
            }
            catch (Exception)
            {
                return new JsonResult(new { });
            }
        }

        [HttpPost]
        public JsonResult GetCouponJSON(int id)
        {
            try
            {
                var coupon = _coupons.SelectByID(id);
                if (coupon != null)
                {
                    return new JsonResult(new { result = 1, data = coupon });
                }
                return new JsonResult(new { result = 2 });
            }
            catch (Exception)
            {
                return new JsonResult(new { });
            }
        }

        [HttpPost]
        public JsonResult UpdateCouponJSON(Coupons updatedCoupon)
        {
            try
            {
                var coupon = _coupons.SelectByID(updatedCoupon.Id);
                if (coupon != null)
                {
                    coupon.Active = updatedCoupon.Active;
                    coupon.CouponLimit = updatedCoupon.CouponLimit;
                    coupon.Comment = updatedCoupon.Comment;
                    coupon.CouponCode = updatedCoupon.CouponCode;
                    coupon.CouponFinishDate = updatedCoupon.CouponFinishDate;
                    coupon.CouponStartDate = updatedCoupon.CouponStartDate;
                    coupon.Discount = updatedCoupon.Discount;
                    _coupons.Update(coupon);
                    return new JsonResult(new { result = 1 });
                }
                return new JsonResult(new { result = 2 });
            }
            catch (Exception)
            {
                return new JsonResult(new { });
            }
        }

        [Authorize(Roles = "0")]
        [HttpGet("agencies")]
        public IActionResult Agencies()
        {
            return View();
        }

        [Authorize(Roles = "0")]
        [HttpPost("agencies")]
        public IActionResult Agencies(UserDatas data, string Eposta, string Password,int agencyType)
        {
            if (data != null)
            {
                var datas = _loginAuth.SelectByFunc(a => a.Email == Eposta).FirstOrDefault();
                if (datas == null)
                {
                    var newAgencies = new UserDatas
                    {
                        Linkedin = data.Linkedin,
                        Name = data.Name,
                        AboutUs = data.AboutUs,
                        Address = data.Address,
                        CompanyEmail = data.CompanyEmail,
                        CompanyName = data.CompanyName,
                        CompanyPhoneNumber = data.CompanyPhoneNumber,
                        CompanyWebsite = data.CompanyWebsite,
                        Facebook = data.Facebook,
                        Profession = data.Profession,
                        TransferRequest = data.TransferRequest,
                        TransferRequestLocation = data.TransferRequestLocation,
                        PhoneNumber = data.PhoneNumber,
                    };

                    var addedAgencies = _userData.Insert(newAgencies);

                    _loginAuth.Insert(new LoginAuth
                    {
                        Email = Eposta,
                        Password = GetMD5(Password),
                        Type = 2,
                        UserId = addedAgencies.Id,
                        DriverId = 0
                    });

                    return Json(new { result = 1 });
                }

                return Json(new { result = 2 });
            }

            return View();
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
