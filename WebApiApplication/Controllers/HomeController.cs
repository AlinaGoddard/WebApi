using System;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using WebApiApplication.Models;

namespace WebApiApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ExtractEmailContent(EmailContentModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    if (Request.Url != null)
                    {
                        client.BaseAddress = new Uri(Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, string.Empty));
                        var content = new StringContent(model.EmailContent, Encoding.UTF8, "application/json");
                        var response = client.PostAsync("/api/values/GetXmlLikePortions", content).Result;
                        if (response.IsSuccessStatusCode)
                            model.XmlLikeContent = response.Content.ReadAsStringAsync().Result; 
                        else
                        {
                            var errorContent = response.Content.ReadAsAsync<HttpError>();
                            ModelState.AddModelError(response.ReasonPhrase, $"{response.ReasonPhrase} {(string)errorContent.Result["Message"]}");
                        }
                    }
                }
            }

            return View("Index", model);
        }
    }
}
