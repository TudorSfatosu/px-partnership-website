using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestSharp;
using RestSharp.Authenticators;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using MailChimp.Net;
using MailChimp.Net.Models;
using MailChimp.Net.Core;
using System.Net;
using Umbraco.Web.PublishedContentModels;

namespace PXP.Controllers
{
    public class ContactFormController : SurfaceController
    {
        // GET: ContactForm
        public ActionResult ContactForm()
        {

            var helper = new UmbracoHelper(UmbracoContext);
            var settingsNode = new FormSettings(helper.TypedContent(1129));

            string gDPREmailMessage = settingsNode.GDpremailMessage;
            return PartialView("~/Views/Partials/ContactForm.cshtml", new ContactFormViewModel
            {
                gDPREmailMessage = gDPREmailMessage,
            }
            );
        }
        // GET: BrochureRequest
        public ActionResult BrochureRequest()
        {

            var helper = new UmbracoHelper(UmbracoContext);
            var settingsNode = new FormSettings(helper.TypedContent(1129));

            string gDPREmailMessage = settingsNode.GDpremailMessage;
            return PartialView("~/Views/Partials/BrochureRequest.cshtml", new BrochureRequestFormViewModel
            {
                Brochure = BrochureSelect(),
                gDPREmailMessage = gDPREmailMessage,
            }
            );
        }







        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> ContactFormSubmit(ContactFormViewModel model)
        {
            RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();
            if (string.IsNullOrEmpty(recaptchaHelper.Response))
            {
                TempData["success"] = false;
                ModelState.AddModelError("reCAPTCHA", "Please complete the reCAPTCHA");
                return CurrentUmbracoPage();
            }
            else
            {
                RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();
                if (recaptchaResult != RecaptchaVerificationResult.Success)
                {
                    TempData["success"] = false;
                    ModelState.AddModelError("reCAPTCHA", "The reCAPTCHA is incorrect");
                    return CurrentUmbracoPage();
                }
            }

            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var helper = new UmbracoHelper(UmbracoContext);
            var settingsNode = new FormSettings(helper.TypedContent(1129));

            // email
            string EmailTo = settingsNode.EmailTo;
            string EmailFrom = settingsNode.EmailFrom;
            string MailgunApiKey = settingsNode.MailgunApikey;
            string MailgunDomain = settingsNode.MailgunDomain;
            string MailChimpApiKey = settingsNode.MailChimpApikey;
            string MailChimpListId = settingsNode.MailChimpListId;



            string maillist = (model.Maillist == true ? "Yes" : "No");

            string subject = "New Contact Form Enquiry - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            string messageHead = "<head>" +
                                "<style>" +
                                "table { font-family:Arial, Helvetica, sans-serif; font-size:14px; } td, th { padding:6px; text-align:left; border-bottom:1px solid #dddddd; } th { font-weight: 700; }" +
                                "</style>" +
                                "</head>";
            string messageBody = "<body>" +
                                    "<table>" +
                                    "<tr><th>Name:</th><td>" + model.Name + "</td></tr>" +
                                    "<tr><th>Company:</th><td>" + model.Company + "</td></tr>" +
                                    "<tr><th>Position:</th><td>" + model.Position + "</td></tr>" +
                                    "<tr><th>Email:</th><td>" + model.Email + "</td></tr>" +
                                    "<tr><th>Telephone:</th><td>" + model.Tel + "</td></tr>" +
                                    "<tr><th>Message:</th><td>" + model.Message + "</td></tr>" +
                                    "<tr><th>Added to mailing list:</th><td>" + maillist + "</td></tr>" +
                                    "</table>" +
                                    "</body>";
            string message = "<html>" + messageHead + messageBody + "</html>";
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                    new HttpBasicAuthenticator("api", MailgunApiKey);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", MailgunDomain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Contact Form <" + EmailFrom + ">");
            request.AddParameter("to", EmailTo);
            request.AddParameter("bcc", "james@boomerangcreative.agency");
            request.AddParameter("subject", subject);
            request.AddParameter("html", message);
            request.AddParameter("h:Reply-To", model.Email);
            request.Method = RestSharp.Method.POST;
            var response = client.Execute(request);

            TempData["success"] = true;


            if (model.Maillist == true)
            {
                // mailchimp code here
                var MailChimpManager = new MailChimpManager(MailChimpApiKey);

                // Use the Status property if updating an existing member
                var member = new MailChimp.Net.Models.Member
                {
                    EmailAddress = model.Email,
                    Status = Status.Pending,
                    EmailType = "html",
                    //TimestampSignup = DateTime.UtcNow.ToString("s")
                };

                member.MergeFields.Add("NAME", model.Name);

                try
                {
                    var result = await MailChimpManager.Members.AddOrUpdateAsync(MailChimpListId, member);
                    TempData["Message"] = new MvcHtmlString("<p>Thank you for signing up to our mailing list.</p><p>Please check your inbox (and Junk/Spam folder) and follow instructions as you will not be added until you confirm your subscription.</p>");
                }
                catch (MailChimpException mce)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadGateway, mce.Message);
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.ServiceUnavailable, ex.Message);
                }
            }

            //return RedirectToUmbracoPage(Convert.ToInt32(CurrentPage.GetProperty("thankYouRedirect").Value));
            //return RedirectToCurrentUmbracoPage();
            var url = Umbraco.Url(CurrentPage.Id);
            return Redirect(url + "#form");
        }








        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> BrochureRequestSubmit(BrochureRequestFormViewModel model)
        {
            RecaptchaVerificationHelper recaptchaHelper = this.GetRecaptchaVerificationHelper();
            if (string.IsNullOrEmpty(recaptchaHelper.Response))
            {
                TempData["success"] = false;
                ModelState.AddModelError("reCAPTCHA", "Please complete the reCAPTCHA");
                return CurrentUmbracoPage();
            }
            else
            {
                RecaptchaVerificationResult recaptchaResult = recaptchaHelper.VerifyRecaptchaResponse();
                if (recaptchaResult != RecaptchaVerificationResult.Success)
                {
                    TempData["success"] = false;
                    ModelState.AddModelError("reCAPTCHA", "The reCAPTCHA is incorrect");
                    return CurrentUmbracoPage();
                }
            }

            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> errors = ModelState.Values.SelectMany(v => v.Errors);
                return CurrentUmbracoPage();
            }

            var helper = new UmbracoHelper(UmbracoContext);
            var settingsNode = new FormSettings(helper.TypedContent(1129));

            // email
            string EmailTo = settingsNode.EmailTo;
            string EmailFrom = settingsNode.EmailFrom;
            string MailgunApiKey = settingsNode.MailgunApikey;
            string MailgunDomain = settingsNode.MailgunDomain;
            string MailChimpApiKey = settingsNode.MailChimpApikey;
            string MailChimpListId = settingsNode.MailChimpListId;



            string maillist = (model.Maillist == true ? "Yes" : "No");

            string subject = "New Brochure Request - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            string messageHead = "<head>" +
                                "<style>" +
                                "table { font-family:Arial, Helvetica, sans-serif; font-size:14px; } td, th { padding:6px; text-align:left; border-bottom:1px solid #dddddd; } th { font-weight: 700; }" +
                                "</style>" +
                                "</head>";
            string messageBody = "<body>" +
                                "<table>" +
                                "<tr><th>Send Brochure Via:</th><td>" + model.BrochureSelected + "</td></tr>";
                                
                                if (model.BrochureSelected == "Post")
                                {
                                    messageBody += "<tr><th>Post To:</th><td>" + model.Address + "</td></tr>";
                                }

                                messageBody += "<tr><th>Name:</th><td>" + model.Name + "</td></tr>" +
                                "<tr><th>Company:</th><td>" + model.Company + "</td></tr>" +
                                "<tr><th>Position:</th><td>" + model.Position + "</td></tr>" +
                                "<tr><th>Email:</th><td>" + model.Email + "</td></tr>" +
                                "<tr><th>Telephone:</th><td>" + model.Tel + "</td></tr>" +
                                "<tr><th>Added to mailing list:</th><td>" + maillist + "</td></tr>" +
                                "</table>" +
                                "</body>";
            string message = "<html>" + messageHead + messageBody + "</html>";
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                    new HttpBasicAuthenticator("api", MailgunApiKey);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", MailgunDomain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Brochure Request <" + EmailFrom + ">");
            //request.AddParameter("to", "info@lakesidesuperbowl.co.uk");
            request.AddParameter("to", EmailTo);
            request.AddParameter("bcc", "james@boomerangcreative.agency");
            request.AddParameter("subject", subject);
            request.AddParameter("html", message);
            request.AddParameter("h:Reply-To", model.Email);
            request.Method = RestSharp.Method.POST;
            var response = client.Execute(request);

            TempData["success"] = true;


            if (model.Maillist == true)
            {
                // mailchimp code here
                var MailChimpManager = new MailChimpManager(MailChimpApiKey);

                // Use the Status property if updating an existing member
                var member = new MailChimp.Net.Models.Member
                {
                    EmailAddress = model.Email,
                    Status = Status.Pending,
                    EmailType = "html",
                    //TimestampSignup = DateTime.UtcNow.ToString("s")
                };

                member.MergeFields.Add("NAME", model.Name);

                try
                {
                    var result = await MailChimpManager.Members.AddOrUpdateAsync(MailChimpListId, member);
                    TempData["Message"] = new MvcHtmlString("<p>Thank you for signing up to our mailing list.</p><p>Please check your inbox (and Junk/Spam folder) and follow instructions as you will not be added until you confirm your subscription.</p>");
                }
                catch (MailChimpException mce)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadGateway, mce.Message);
                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.ServiceUnavailable, ex.Message);
                }
            }

            //return RedirectToUmbracoPage(Convert.ToInt32(CurrentPage.GetProperty("thankYouRedirect").Value));
            //return RedirectToCurrentUmbracoPage();
            var url = Umbraco.Url(CurrentPage.Id);
            return Redirect(url + "#form");
        }







        private IEnumerable<SelectListItem> BrochureSelect()
        {
            // list of brochure items
            var BrochureList = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Email", Value = "Email" },
                    new SelectListItem { Text = "Post", Value = "Post" }
                };

            return new SelectList(BrochureList, "Value", "Text");
        }
    }
}