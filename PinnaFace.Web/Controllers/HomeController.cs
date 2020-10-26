using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.Service;
using PinnaFace.Web.Models;
using WebMatrix.WebData;

namespace PinnaFace.Web.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {

        public ActionResult Index(bool? logout)
        {
            var employeeVisa = new EmployeeVisaViewModel
            {
                IsVisaAssigned = false,
                Comment = ""
            };

            //@ViewData["AgentFilterVisibility"] = Singleton.User != null && !Singleton.User.IsUserAgent ? "hidden" : "visible";
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            try
            {
                if (logout != null && (bool)logout)
                {
                    WebSecurity.Logout();
                    Redirect("~/Home/Index");
                }
            }
            catch 
            {
            }
            return View(employeeVisa);
        }
       
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";
           

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //[HttpPost]
        //public ActionResult SubmitContact()
        //{
        //    var response = Request["g-recaptacha-response"];
        //    string secretKey = "6LcVpLYUAAAAAO6HYPqFIEA4ddaumLBkBvUPAarr";
        //    var client = new WebClient();
        //    ViewData["Message"] = "Contact Submitted";

        //    return View("Index");
        //}
        
        ////[ValidateAntiForgeryToken]
        //[HttpPost]
        //public ActionResult SubmitContact()//[Bind(Include = "ID,Name,Designation,Location")] Employee employee)
        //{
        //    CaptchaResponse response = ValidateCaptcha(Request["g-recaptcha-response"]);
        //    if (response.Success && ModelState.IsValid)
        //    {
        //        //db.Employees.Add(employee);
        //        //db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        return Content("Error From Google ReCaptcha : " + response.ErrorMessage[0].ToString());
        //    }
        //} 
        //[HttpPost]
        //[CaptchaValidator]
        //public ActionResult SubmitContact(RegisterModel registerModel, bool captchaValid)
        //{
        //    if (ModelState.IsValid)
        //    {
        //    }
        //    return View("Index");
        //}
        //public static CaptchaResponse ValidateCaptcha(string response)
        //{
        //    const string secret = "6LcVpLYUAAAAAO6HYPqFIEA4ddaumLBkBvUPAarr"; // System.Web.Configuration.WebConfigurationManager.AppSettings["recaptchaPrivateKey"];
        //    var client = new WebClient();
        //    var jsonResult = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
        //    return JsonConvert.DeserializeObject<CaptchaResponse>(jsonResult.ToString());
        //} 

        [Authorize]
        public ActionResult AgencyDetail(string agencyId)
        {
            if (!string.IsNullOrEmpty(agencyId))
            {
                var localAgencyId = EncryptionUtility.Hash64Decode(agencyId);
                var cri = new SearchCriteria<AgencyDTO>();
                cri.FiList.Add(v => v.Id == localAgencyId);
                var localAgencyDTO = new LocalAgencyService(true)
                    .GetAll(cri).ToList().FirstOrDefault();

                return View(localAgencyDTO);
            }

            return View();
        }

        [Authorize]
        public ActionResult AgentDetail(string agentId)
        {
            if (!string.IsNullOrEmpty(agentId))
            {
                var foreignAgentId = EncryptionUtility.Hash64Decode(agentId);
                var cri = new SearchCriteria<AgentDTO>();
                cri.FiList.Add(v => v.Id == foreignAgentId);
                var foreignAgentDTO = new ForeignAgentService(true,true)
                    .GetAll(cri).ToList().FirstOrDefault();

                return View(foreignAgentDTO);
            }
            return View();
        }

        public ActionResult CompanyHeader()
        {
            ViewBag.Message = "Your app description page.";

            var company = new UserCompanyDetail();
            var dir = PathUtil.GetDestinationPhotoPath();
            if (Singleton.User != null)
            {
                if (Singleton.User.Agent != null)
                {
                    var agName = Singleton.User.Agent.AgentName;
                    company.CompanyName = agName;
                    company.AddNewVisaVisibility = "visible";

                    var agentHeader = Singleton.User.Agent.Header;
                    if (agentHeader != null)
                    {
                        company.HeaderUrl = agentHeader.RowGuid + ".jpg";
                        try
                        {
                            var fiName = Path.Combine(dir, company.HeaderUrl);
                            if (System.IO.File.Exists(fiName))
                                company.CompanyName = "";
                        }
                        catch
                        {
                        }
                    }
                }
                else if (Singleton.User.Agency != null)
                {
                    var agName = Singleton.User.Agency.AgencyName;
                    company.CompanyName = agName;
                    company.AddNewVisaVisibility = "hidden";

                    var agencyHeader = Singleton.User.Agency.Header;
                    if (agencyHeader != null)
                    {
                        company.HeaderUrl = agencyHeader.RowGuid + ".jpg";
                        try
                        {
                            var fiName = Path.Combine(dir, company.HeaderUrl);
                            if (System.IO.File.Exists(fiName))
                                company.CompanyName = "";
                        }
                        catch
                        {
                        }
                    }
                }
            }


            return PartialView(company);
        }

        public ActionResult _LoginPartial()
        {
            ViewBag.Message = "Your app description page.";

            var company = new UserCompanyDetail
            {
                AddNewVisaVisibility = "hidden",
                AddNewEmployeeVisibility = "hidden"
            };
        
            if (Singleton.User != null)
            {
                if (Singleton.User.Agent != null)
                {
                    company.AddNewVisaVisibility = "visible";
                    company.AddNewEmployeeVisibility = "hidden";
                }
                else if (Singleton.User.Agency != null)
                {
                    company.AddNewVisaVisibility = "hidden";
                    company.AddNewEmployeeVisibility = "visible";
                }
            }

            return PartialView(company);
        }


    }
}
