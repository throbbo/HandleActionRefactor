using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SchoStack.Web;

namespace HandleActionRefactor.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var vm = new HomeViewModel();   
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index1(HomeInputModel inputModel)
        {
            if (!ModelState.IsValid)
                return Index();

            var result = Invoker.Execute<HomeResponseModel>(inputModel);
            if (result.GotoAbout)
                return RedirectToAction("About");

            return RedirectToAction("Index");
        }

		[HttpPost]
		public ActionResult Index2(HomeInputModel inputModel)
		{
			return Handle(inputModel)
			.Returning<HomeResponseModel>()
			.On(x => x.GotoAbout, _ => RedirectToAction("About"))
			.OnSuccess(_ => RedirectToAction("Index"))
			.OnError(() => Index());
		}

		[HttpPost]
		public ActionResult Index3(HomeInputModel inputModel)
		{
			return Handle(inputModel)
			.OnError(() => Index())
			.OnSuccess(() => RedirectToAction("Index"));
		}
		[HttpPost]
		public ActionResult Index4(HomeInputModel inputModel)
		{
			return Handle(inputModel)
				.OnError(Index)
				.OnSuccess(() => RedirectToAction("Index"))
				.Returning<HomeResponseModel>()
				.On(x => x.GotoAbout, _ => RedirectToAction("About"));
		}
        
		[HttpPost]
		public ActionResult Index(HomeInputModel inputModel)
		{
		    return Handle(inputModel)
		        .Returning<HomeResponseModel>()
		        .On(x => x.GotoAbout, _ => RedirectToAction("About"))
		        .OnError(() => Index())
		        .OnSuccessWithMessage(_ => RedirectToAction("Index", "Home","Robs Message"));
		}
				
    	public ActionResult About()
        {
            return View();
        }
    }

}
