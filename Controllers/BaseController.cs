using System.Web.Mvc;

namespace TechStore.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["admin"] == null)
            {
                filterContext.Result = RedirectToAction("Login", "Admins");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}