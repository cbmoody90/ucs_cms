using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebMatrix.Data;
using System.Web.Mvc;
using United_Caring_Services.Models;
using System.Web.Helpers;
using System.IO;
using System.Text;
using System.Web.Mvc.Filters;
using System.Web.Routing;
using System.Text.RegularExpressions;

namespace United_Caring_Services.Controllers
{
    public class HomeController : Controller
    {
        protected override bool DisableAsyncSupport => base.DisableAsyncSupport;

        public ActionResult Index(string vision_div, string need_details)
        {
            ViewBag.apiKey = "AIzaSyAUKg-hyZKBhJSHwdykoRw6TiwBupXLFzk";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {

            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Events()
        {
            return View();
        }

        public ActionResult Chart()
        {
            return PartialView();
        }
        public JsonResult GetFlags()
        {
            flags sflags = new flags();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["UCSConnection"].ConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"exec get_flag_status";
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        sflags.Whiteflag = reader.GetBoolean(0);
                        sflags.Redflag = reader.GetBoolean(1);
                    }

                    return Json(new { response = sflags });
                }
            }
            catch (SqlException e)
            {
                Console.Write(e);
                sflags.Whiteflag = false;
                sflags.Redflag = false;
                return Json(new { response = sflags });
            }
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public string getContent(string page, string id)
        {
            string cont = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["UCSConnection"].ConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"exec get_content @page = @page, @id = @id";
                    cmd.Parameters.Add(new SqlParameter("@page", SqlDbType.NVarChar) { Value = page });
                    cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.NVarChar) { Value = id });
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        cont = reader.GetString(0);
                    }

                    var myStringBuilder = new StringBuilder(cont);

                    if (cont != null)
                    {
                        bool needs = cont.Contains("@needs");
                        bool sneeds = cont.Contains("@sneeds");
                        bool volOps = cont.Contains("@vols");
                        bool svolOps = cont.Contains("@svols");
                        bool fb = cont.Contains("@facebook");
                        if (needs)
                        {
                            string needContent = getContent("Utility", "Needs");

                            string image = "@needs";
                            int count = Regex.Matches(cont, image).Count;
                            for (int i = 0; i < count; i++)
                            {
                                int replacements = myStringBuilder.ToString().IndexOf("@needs");
                                myStringBuilder.Remove(replacements, 6);
                                myStringBuilder.Insert(replacements, needContent);
                            }
                        }
                        if (sneeds)
                        {
                            string needContent = getContent("Utility", "Short-Needs");

                            string image = "@sneeds";
                            int count = Regex.Matches(cont, image).Count;
                            for (int i = 0; i < count; i++)
                            {
                                int replacements = myStringBuilder.ToString().IndexOf("@sneeds");
                                myStringBuilder.Remove(replacements, 7);
                                myStringBuilder.Insert(replacements, needContent);
                            }
                        }
                        if (volOps)
                        {
                            string volContent = getContent("Utility", "Volunteer");

                            string image = "@vols";
                            int count = Regex.Matches(cont, image).Count;
                            for (int i = 0; i < count; i++)
                            {
                                int replacements = myStringBuilder.ToString().IndexOf("@vols");
                                myStringBuilder.Remove(replacements, 5);
                                myStringBuilder.Insert(replacements, volContent);
                            }
                        }
                        if (svolOps)
                        {
                            string volContent = getContent("Utility", "Short-Volunteer");

                            string image = "@svols";
                            int count = Regex.Matches(cont, image).Count;
                            for (int i = 0; i < count; i++)
                            {
                                int replacements = myStringBuilder.ToString().IndexOf("@svols");
                                myStringBuilder.Remove(replacements, 6);
                                myStringBuilder.Insert(replacements, volContent);
                            }
                        }
                        if (fb)
                        {
                            string fbContent = getContent("Utility", "Facebook");

                            string image = "@facebook";
                            int count = Regex.Matches(cont, image).Count;
                            for (int i = 0; i < count; i++)
                            {
                                int replacements = myStringBuilder.ToString().IndexOf("@facebook");
                                myStringBuilder.Remove(replacements, 9);
                                myStringBuilder.Insert(replacements, fbContent);
                            }
                        }
                    }


                    connection.Close();
                    return myStringBuilder.ToString();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
            }

            return cont;
        }

        [HttpGet]
        public List<SelectListItem> getPages()
        {
            List<SelectListItem> pages = new List<SelectListItem>();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["UCSConnection"].ConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"exec getPages";
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        pages.Add(new SelectListItem() { Text = reader.GetString(0), Value = reader.GetString(0) });
                    }

                    connection.Close();
                    return pages;
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                return pages; 
            }
        }

        //Used like an api as well to return list of available divs in a certain page to edit. This 
        [HttpPost]
        public ActionResult getBoxes(string page)
        {
            List<SelectListItem> pages = new List<SelectListItem>();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["UCSConnection"].ConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"exec getBoxes @page = @pages";
                    cmd.Parameters.Add(new SqlParameter("@pages", SqlDbType.NVarChar) { Value = page });
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        pages.Add(new SelectListItem() { Text = reader.GetString(0), Value = reader.GetString(0) });
                    }

                    connection.Close();
                    return Json(pages);
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                return Json(pages);
            }
        }

        //Used like an api as well to return the page content in a json format for page in question.
        [HttpPost]
        public ActionResult getBoxContent(string page, string id)
        {
            string content = "";
            try { 
            content = getContent(page, id);
            return Json(content);
             
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
                return Json(content);
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Edit()
        {
            List<SelectListItem> pages = new List<SelectListItem>();
            pages = getPages();

            return View(pages);
        }

        [Authorize]
        [HttpGet]
        public ActionResult imageUpload()
        {
            TempData["success"] = "";
            TempData["message"] = "";
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult imageUpload([Bind(Include = "collection,fileName")] upload newUpload)
        {
            //tempData is used to display error and success messages on the front end.
            TempData["message"] = "none";
            TempData["success"] = "none";
            string filename = newUpload.fileName;
            HttpPostedFileBase image = null;
            string path ="~/Assets/";
            string fullPath = path + filename;
            try
            {
                image = newUpload.collection;
            }
            catch (NotImplementedException e)
            {
                Console.WriteLine(e);
                TempData["message"] = "Unexpected Error";
                TempData["success"] = "failure";
                return View();
            }
            if (ModelState.IsValid)
            {
                if (newUpload.collection != null)
                {
                    try
                    {

                        image.SaveAs(Server.MapPath(path + filename));
                        TempData["message"] = "Upload Successful!";
                        TempData["success"] = "success";
                        return View();


                    }
                    catch (NotImplementedException e)
                    {
                        Console.WriteLine(e);
                        TempData["success"] = "failure";
                        return View();

                    }
                }
                else
                {
                    TempData["message"] = "No File Found";
                    TempData["success"] = "failure";
                    return View();

                }
            }
            else
            {
                TempData["message"] = "Unexpected Error";
                TempData["success"] = "failure";
                return View();

            }
        }


        public ActionResult Directors()
        {
            return View();
        }

        public ActionResult Staff()
        {
            return View();
        }

        public ActionResult Medical()
        {
            return View();
        }
        public ActionResult Housing()
        {
            return View();
        }

        public ActionResult Flag()
        {
            return View();
        }
        public ActionResult Admin()
        {
            return View();
        }

        //used more as an api without the route. Use Ajax on the front end to get the response and post changes.
        [HttpGet]
        public ActionResult ChangeFlagStatus()
        {
            flags sflags = new flags();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["UCSConnection"].ConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"exec get_flag_status";
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        sflags.Whiteflag = reader.GetBoolean(0);
                        sflags.Redflag = reader.GetBoolean(1);
                    }
                    connection.Close();
                    return View(sflags);
                }
            }catch(SqlException e)
            {
                Console.WriteLine(e);
                sflags.Redflag = false;
                sflags.Whiteflag = false;
                return View(sflags);
            }
        }

        [HttpPost]
        public ActionResult ChangeFlagStatus(flags flag)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["UCSConnection"].ConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"exec update_flags @whiteflag = @whiteflag, @redflag = @redflag";
                    cmd.Parameters.Add(new SqlParameter("@whiteflag", SqlDbType.Bit) { Value = flag.Whiteflag });
                    cmd.Parameters.Add(new SqlParameter("@redflag", SqlDbType.Bit) { Value = flag.Redflag });
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
            }
                return View();
            }
        public ActionResult give()
        {
            return View();
        }

        public ActionResult Volunteer()
        {
            return View();
        }

        public ActionResult TellFriend()
        {
            return View();
        }

        public ActionResult plannedGiving()
        {
            return View();
        }

        public ActionResult day()
        {
            return View();
        }


        public ActionResult ruth()
        {
            return View();
        }
        
        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected override void Execute(RequestContext requestContext)
        {
            base.Execute(requestContext);
        }

        protected override ContentResult Content(string content, string contentType, Encoding contentEncoding)
        {
            return base.Content(content, contentType, contentEncoding);
        }

        protected override IActionInvoker CreateActionInvoker()
        {
            return base.CreateActionInvoker();
        }

        protected override ITempDataProvider CreateTempDataProvider()
        {
            return base.CreateTempDataProvider();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void ExecuteCore()
        {
            base.ExecuteCore();
        }

        protected override FileContentResult File(byte[] fileContents, string contentType, string fileDownloadName)
        {
            return base.File(fileContents, contentType, fileDownloadName);
        }

        protected override FileStreamResult File(Stream fileStream, string contentType, string fileDownloadName)
        {
            return base.File(fileStream, contentType, fileDownloadName);
        }

        protected override FilePathResult File(string fileName, string contentType, string fileDownloadName)
        {
            return base.File(fileName, contentType, fileDownloadName);
        }

        protected override void HandleUnknownAction(string actionName)
        {
            base.HandleUnknownAction(actionName);
        }

        protected override HttpNotFoundResult HttpNotFound(string statusDescription)
        {
            return base.HttpNotFound(statusDescription);
        }

        protected override JavaScriptResult JavaScript(string script)
        {
            return base.JavaScript(script);
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding)
        {
            return base.Json(data, contentType, contentEncoding);
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return base.Json(data, contentType, contentEncoding, behavior);
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            base.OnAuthentication(filterContext);
        }

        protected override void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            base.OnAuthenticationChallenge(filterContext);
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }

        protected override PartialViewResult PartialView(string viewName, object model)
        {
            return base.PartialView(viewName, model);
        }

        protected override RedirectResult Redirect(string url)
        {
            return base.Redirect(url);
        }

        protected override RedirectResult RedirectPermanent(string url)
        {
            return base.RedirectPermanent(url);
        }

        protected override RedirectToRouteResult RedirectToAction(string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return base.RedirectToAction(actionName, controllerName, routeValues);
        }

        protected override RedirectToRouteResult RedirectToActionPermanent(string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            return base.RedirectToActionPermanent(actionName, controllerName, routeValues);
        }

        protected override RedirectToRouteResult RedirectToRoute(string routeName, RouteValueDictionary routeValues)
        {
            return base.RedirectToRoute(routeName, routeValues);
        }

        protected override RedirectToRouteResult RedirectToRoutePermanent(string routeName, RouteValueDictionary routeValues)
        {
            return base.RedirectToRoutePermanent(routeName, routeValues);
        }

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            return base.View(viewName, masterName, model);
        }

        protected override ViewResult View(IView view, object model)
        {
            return base.View(view, model);
        }

        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            return base.BeginExecute(requestContext, callback, state);
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            return base.BeginExecuteCore(callback, state);
        }

        protected override void EndExecute(IAsyncResult asyncResult)
        {
            base.EndExecute(asyncResult);
        }

        protected override void EndExecuteCore(IAsyncResult asyncResult)
        {
            base.EndExecuteCore(asyncResult);
        }
    }
}
