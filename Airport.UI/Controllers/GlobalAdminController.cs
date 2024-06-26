using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Airport.UI.Controllers;

[Authorize(Roles = "0,4")]
public class GlobalAdminController(ICouponsDAL couponsDal, ILoginAuthDAL loginAuthDal, IUserDatasDAL userDataDal, IUserDocsDAL userDocsDal, IGlobalSettingsDAL globalSettingsDal) : Controller
{

    [HttpGet("panel/coupons")]
    public IActionResult CreateCoupon()
    {

        var userId = Convert.ToInt32(Request.HttpContext.User.Claims.Where(a => a.Type == ClaimTypes.Sid).Select(a => a.Value).SingleOrDefault());
        var myCoupons = couponsDal.SelectByFunc(a => a.UserId == userId);
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
                couponsDal.Insert(new Coupons
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
            var coupon = couponsDal.SelectByID(id);
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
            var coupon = couponsDal.SelectByID(updatedCoupon.Id);
            if (coupon != null)
            {
                coupon.Active = updatedCoupon.Active;
                coupon.CouponLimit = updatedCoupon.CouponLimit;
                coupon.Comment = updatedCoupon.Comment;
                coupon.CouponCode = updatedCoupon.CouponCode;
                coupon.CouponFinishDate = updatedCoupon.CouponFinishDate;
                coupon.CouponStartDate = updatedCoupon.CouponStartDate;
                coupon.Discount = updatedCoupon.Discount;
                couponsDal.Update(coupon);
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
    public IActionResult Agencies(UserDatas data, string Eposta, string Password, byte agencyType)
    {
        if (data != null)
        {
            var datas = loginAuthDal.SelectByFunc(a => a.Email == Eposta).FirstOrDefault();
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
                    Img = "/img/airglobal-icon-black.png"
                };

                var addedAgencies = userDataDal.Insert(newAgencies);

                loginAuthDal.Insert(new LoginAuth
                {
                    Email = Eposta,
                    Password = GetMD5(Password),
                    Type = agencyType,
                    UserId = addedAgencies.Id,
                    DriverId = 0
                });

                return Json(new { result = 1 });
            }

            return Json(new { result = 2 });
        }

        return View();
    }

    [Authorize(Roles = "0")]
    [HttpGet("docs-list")]
    public IActionResult Docs()
    {
        var docs = new List<UserDocs>();
        userDocsDal.Select().ForEach(doc =>
        {
            var auth = loginAuthDal.SelectByID(doc.UserId);
            doc.User = userDataDal.SelectByID(auth.UserId);
            docs.Add(doc);
        });


        return View(docs);
    }

    [Authorize(Roles = "0")]
    public IActionResult GetDocsAttr(int docsId)
    {
        try
        {
            var docs = userDocsDal.SelectByID(docsId);

            return Json(new { result = 1, data = docs });
        }
        catch (Exception)
        {
            return Json(new { });
        }

    }

    [Authorize(Roles = "0")]
    public IActionResult UpdateDocs(UserDocs docs)
    {
        try
        {
            var selectedDocs = userDocsDal.SelectByID(docs.Id);
            if (selectedDocs != null)
            {
                selectedDocs.Docs1AdminStatus = docs.Docs1AdminStatus;
                selectedDocs.Docs2AdminStatus = docs.Docs2AdminStatus;
                selectedDocs.Docs3AdminStatus = docs.Docs3AdminStatus;

                userDocsDal.Update(selectedDocs);

                return Json(new { result = 1 });
            }

            return Json(new { result = 2 });

        }
        catch (Exception)
        {
            return Json(new { });
        }

    }

    [Authorize(Roles = "0,4")]
    [HttpGet("global-settings")]
    public IActionResult Settings()
    {
        var settings = globalSettingsDal.SelectByID(1);

        return View(settings);
    }

    [Authorize(Roles = "0,4")]
    [HttpPost("global-settings")]
    public IActionResult Settings(GlobalSettings settings)
    {
        try
        {
            var globalSettings = globalSettingsDal.SelectByID(1);
            if (globalSettings != null)
            {
                globalSettings.SalesPer = settings.SalesPer;
                globalSettings.GlobalPer = settings.GlobalPer;
                globalSettings.PartnerPer = settings.PartnerPer;
                globalSettings.LastChange = DateTime.Now;

                globalSettingsDal.Update(globalSettings);
                return Json(new { result = 1 });
            }
            return Json(new { });
        }
        catch (Exception)
        {
            throw;
        }
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
