using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.DAL.Interfaces;
using PinnaFace.Service;
using PinnaFace.Web.Filters;
using PinnaFace.Web.Models;
using WebMatrix.WebData;

namespace PinnaFace.Web.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string recoverystat)
        {
            if (!string.IsNullOrEmpty(recoverystat) && recoverystat == "1")
                ViewData["RecoveryStatHidden"] = "visible";
            else
                ViewData["RecoveryStatHidden"] = "hidden";

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            ViewData["RecoveryStatHidden"] = "hidden";

            if (!string.IsNullOrEmpty(model.Email)) //.Contains("@")))
            {
                //var em = model.Email;
                model.UserName = Membership.GetUserNameByEmail(model.Email);
            }

            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, model.RememberMe))
            {
                
                Singleton.User = new UserService(true)
                    .GetAll(new UserSearchCriteria<UserDTO>())
                    .FirstOrDefault(u => u.UserName == model.UserName);

                if (Singleton.User != null && Singleton.User.Agency != null)
                    Singleton.Agency = Singleton.User.Agency;
                
                if (returnUrl != null && returnUrl.Contains("LogOff"))
                    return Redirect("~/Home/Index");

                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            return Redirect("~/Home/Index");
        }

        public ActionResult UserProfile()
        {
            UserDTO userProfile = new UserService(true).GetByName(@User.Identity.Name);
            return View(userProfile);
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserProfile(UserDTO userProfile)
        {
            if (ModelState.IsValid)
            {
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult RegisterConfimation(string un, string ct)
        {
            string urlAction;
            string unDecrypted = un; // EncryptionUtility.Decrypt(un);

            if (WebSecurity.ConfirmAccount(unDecrypted, ct))
            {
                urlAction = "Login";
                //get userid of received username
                int userid = new UserService(true).GetByName(unDecrypted).UserId;

                //Account Confirmation should be propagated to the local server database through sync
                using (IDbContext dbCon = DbContextUtil.GetDbContextInstance())
                {
                    MembershipDTO membershipDTO = dbCon.Set<MembershipDTO>().FirstOrDefault(m => m.UserId == userid);
                    if (membershipDTO != null)
                    {
                        membershipDTO.DateLastModified = DateTime.Now;
                        dbCon.Set<MembershipDTO>().Add(membershipDTO);
                        dbCon.Entry(membershipDTO).State = EntityState.Modified;
                        dbCon.SaveChanges();
                    }
                }
            }
            else
                urlAction = "ConfirmationFailure";

            return RedirectToAction(urlAction, new UserCompanyDetail
            {
                CompanyName = unDecrypted,
                AddNewVisaVisibility = ct,
                AddNewEmployeeVisibility = un
            });
        }

        [AllowAnonymous]
        public ActionResult ConfirmationFailure(UserCompanyDetail ag)
        {
            return View(ag);
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(LoginModel model)
        {
            if (!string.IsNullOrEmpty(model.Email)) //.Contains("@")))
            {
                //var em = model.Email;
                model.UserName = Membership.GetUserNameByEmail(model.Email);
            }
            string userName = model.UserName;
            //check user existance
            //var user = new UserService(false).GetByName(userName);
            MembershipUser user = Membership.GetUser(userName);
            if (user == null)
            {
                TempData["Message"] = "User Not exist.";
            }
            else
            {
                //generate password token
                string token = WebSecurity.GeneratePasswordResetToken(userName);

                //create url with above token
                string resetLink = Url.Action("ResetPassword", "Account", new {un = userName, rt = token}, "http");
                    //"<a href='" +
                                   //Url.Action("ResetPassword", "Account", new {un = userName, rt = token}, "http") +
                                   //"'>Reset Password</a>";

                
                //get user emailid
                string email = model.Email; // new UserService(true).GetByName(userName).Email; // user.Email;

                //send mail
                const string subject = "Password Reset Url";
                
                string serverpath = Server.MapPath("../Views/Account/");
                var sr = new System.IO.StreamReader(Path.Combine(serverpath, "ResetPassword.html"));//Server.MapPath("../Views/Account/ResetPassword.html"));
                string body = sr.ReadToEnd();
                body = body.Replace("resetLink", resetLink);

                string sentStatus = EmailUtil.SendEMail(email, subject, body);
                if (string.IsNullOrEmpty(sentStatus))
                    TempData["Message"] = "Mail Sent.";
                else TempData["Message"] = "Error occured while sending email." + sentStatus;

                //only for testing
                TempData["Message"] = resetLink;
            }
            return RedirectToLocal("/Account/Login?recoverystat=1");
            //return View();
        }

        public ActionResult ChangePassword(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed." : "";

            ViewBag.ReturnUrl = Url.Action("ChangePassword");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(LocalPasswordModel model)
        {
            ViewBag.ReturnUrl = Url.Action("ChangePassword");

            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword,
                        model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePassword", new {Message = ManageMessageId.ChangePasswordSuccess});
                }
                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult SetPassword(ManageMessageId? message, string token)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.SetPasswordSuccess ? "Your password has been set." : "";

            ViewBag.ReturnUrl = Url.Action("SetPassword");
            var model = new LocalPasswordModel
            {
                ResetToken = token,
                OldPassword = "randomuser"
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult SetPassword(LocalPasswordModel model)
        {
            ViewBag.ReturnUrl = Url.Action("SetPassword");

            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                bool setPasswordSucceeded;
                try
                {
                    setPasswordSucceeded = WebSecurity.ResetPassword(model.ResetToken, model.NewPassword);
                }
                catch (Exception)
                {
                    setPasswordSucceeded = false;
                }

                if (setPasswordSucceeded)
                {
                    return RedirectToAction("Login", new {Message = ManageMessageId.SetPasswordSuccess});
                }
                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string un, string rt)
        {
            //get userid of received username
            int userid = new UserService(true).GetByName(un).UserId;

            //check userid and token matches
            bool any;
            using (IDbContext dbCon = DbContextUtil.GetDbContextInstance())
            {
                any = dbCon.Set<MembershipDTO>().Any(m => m.UserId == userid
                                                          && (m.PasswordVerificationToken == rt));
                //&& (m.PasswordVerificationTokenExpirationDate < DateTime.Now)
            }

            return any ? RedirectToAction("SetPassword", new {token = rt}) : RedirectToAction("ForgotPassword");
        }

        #region Helpers

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return
                        "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return
                        "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return
                        "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return
                        "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion
    }
}