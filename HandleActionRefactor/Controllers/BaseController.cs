using System.Web.Mvc;
using SchoStack.Web;

namespace HandleActionRefactor.Controllers
{
    public class BaseController : Controller
    {
        public IInvoker Invoker { get; set; }
    }
}