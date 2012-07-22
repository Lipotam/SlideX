using System;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using SlideX.Models;

namespace SlideX.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    if (new PresentationDataAccessModel().IsUserPassEmailConfirm(model.UserName))
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    return View("Error", new ErrorPageModels { Title = Localization.ViewPhrases.EmailNotConfirmed, Message = Localization.ViewPhrases.EmailNotConfirmedMessage, ShowGotoBack = true });
                }
                ModelState.AddModelError("",Localization.ValidationStrings.UserOrPassIncorrect);
            }
            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    SendMail(model.Email, model.UserName,
                             new PresentationDataAccessModel().GetUserIdByUserName(model.UserName));
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }
            return View(model);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }
                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                ModelState.AddModelError("", Localization.ViewPhrases.ChangePasswordIncorrent);
            }
            return View(model);
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }
        
        public ActionResult Verification(Guid id)
        {
            new PresentationDataAccessModel().SetUserEmailConfirmed(id);
            return View();
        }

        private void SendMail(string emailTosend, string userName, Guid userId)
        {
            System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Request.ApplicationPath);
            var settings = (System.Net.Configuration.MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
            if (settings.Smtp.Network.UserName != string.Empty && settings.Smtp.Network.Password != string.Empty && settings.Smtp.Network.Host != string.Empty)
            {
                var message = new MailMessage(new MailAddress(settings.Smtp.Network.UserName),
                                              new MailAddress(emailTosend))
                                  {
                                      BodyEncoding = System.Text.Encoding.GetEncoding("KOI8-R"),
                                      SubjectEncoding = System.Text.Encoding.GetEncoding("KOI8-R"),
                                      Subject =
                                          (new StreamReader(Server.MapPath("/Content/EmailSubjectText.txt"))).ReadToEnd(),
                                      Body = new StreamReader(Server.MapPath("/Content/EmailBodyText.txt")).ReadToEnd(),
                                      IsBodyHtml = true
                                  };
                message.Body = message.Body.Replace("<%VerificationUrl%>",
                                                        Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "Account/Verification/" + userId.ToString());
                message.Body = message.Body.Replace("<%UserName%>", userName);
                using (var client = new SmtpClient())
                {
                    client.Credentials = new System.Net.NetworkCredential(settings.Smtp.Network.UserName, settings.Smtp.Network.Password);
                    client.Host = settings.Smtp.Network.Host;
                    client.Port = settings.Smtp.Network.Port;
                    client.EnableSsl = settings.Smtp.Network.EnableSsl;
                    client.Send(message);
                }
            }
        }

        public ActionResult ChangeCulture(string lang, string returnUrl)
        {
            Session["Culture"] = new CultureInfo(lang);
            return Redirect(returnUrl);
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return Localization.ViewPhrases.DuplicateUserName;
                case MembershipCreateStatus.DuplicateEmail:
                    return Localization.ViewPhrases.DuplicateEmail;

                case MembershipCreateStatus.InvalidPassword:
                    return Localization.ViewPhrases.InvalidPassword;

                case MembershipCreateStatus.InvalidEmail:
                    return Localization.ViewPhrases.InvalidEmail;

                case MembershipCreateStatus.InvalidAnswer:
                    return Localization.ViewPhrases.InvalidAnswer;

                case MembershipCreateStatus.InvalidQuestion:
                    return Localization.ViewPhrases.InvalidQuestion;

                case MembershipCreateStatus.InvalidUserName:
                    return Localization.ViewPhrases.InvalidUserName;

                case MembershipCreateStatus.ProviderError:
                    return Localization.ViewPhrases.ProviderError;

                case MembershipCreateStatus.UserRejected:
                    return Localization.ViewPhrases.UserRejected;

                default:
                    return Localization.ViewPhrases.UnknownMistake;
            }
        }
        #endregion
    }
}