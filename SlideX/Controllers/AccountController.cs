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
    /// <summary>
    /// Let user get the project account
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// Let user to log on .
        /// </summary>
        public ActionResult LogOn()
        {
            return View();
        }

        /// <summary>
        /// Check log on parameters
        /// </summary>
        /// <param name="model">contain login and password</param>  
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

        /// <summary>
        ///Let user get out of his account
        /// </summary>
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Lets user to get new acount
        /// </summary>
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Check registry parameters
        /// </summary>
        /// <param name="model">contains login,password and email</param> 
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

        /// <summary>
        /// Lets user change his account password 
        /// </summary>
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Check change password parametrs
        /// </summary>
        /// <param name="model">contains old and new password</param> 
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

        /// <summary>
        /// Show change password succees message
        /// </summary>
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        /// <summary>
        /// provide email check
        /// </summary>
        /// <param name="id">Ge user.</param> 
        public ActionResult Verification(Guid id)
        {
            new PresentationDataAccessModel().SetUserEmailConfirmed(id);
            return View();
        }

        /// <summary>
        /// Sends the mail.
        /// </summary>
        /// <param name="emailTosend">The email tosend.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="userId">The user id.</param>
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

        /// <summary>
        /// Change localization
        /// </summary>
        /// <param name="lang">The lang.</param>
        /// <param name="returnUrl">The return URL.</param>
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