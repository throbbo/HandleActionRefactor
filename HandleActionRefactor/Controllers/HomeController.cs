using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HandleActionRefactor.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var vm = new HomeViewModel();
            return View(vm);
        }

        //[HttpPost]
        //public ActionResult Index1(HomeInputModel inputModel)
        //{
        //    if (!ModelState.IsValid)
        //        return Index();

        //    var result = Invoker.Execute<HomeResponseModel>(inputModel);
        //    if (result.GotoAbout)
        //        return RedirectToAction("About");

        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //public ActionResult Index2(HomeInputModel inputModel)
        //{
        //    return Handle(inputModel)
        //        .Returning<HomeResponseModel>()
        //        .On(x => x.GotoAbout, _ => RedirectToAction("About"))
        //        .OnSuccess(_ => RedirectToAction("Index"))
        //        .OnError(_ => Index());
        //}

        //[HttpPost]
        //public ActionResult Index3(HomeInputModel inputModel)
        //{
        //    //if (!ModelState.IsValid)
        //    //    return Index();

        //    //Invoker.Execute(inputModel);

        //    //return RedirectToAction("ABout");

        //    return Handle(inputModel)
        //        .OnError(() => Index())
        //        .OnSuccess(() => RedirectToAction("About"))
        //        ;
        //}

        //[HttpPost]
        //public ActionResult Index4(HomeInputModel inputModel)
        //{
        //    //if (!ModelState.IsValid)
        //    //    return Index();

        //    //var response = Invoker.Execute<HomeResponseModel>(inputModel);

        //    //return RedirectToAction("ABout");

        //    return Handle(inputModel)
        //        .OnError(() => Index())
        //        .Returning<HomeResponseModel>()
        //        .OnSuccess(x => RedirectToAction("About"))
        //        ;
        //}

        //[HttpPost]
        //public ActionResult Index5(HomeInputModel inputModel)
        //{

        //    return Handle(inputModel)
        //        .OnError(() => Index())
        //        .Returning<HomeResponseModel>()
        //        //.OnSuccessWithMessage(x => View("About", x), "Robs Message")
        //        ;

        //}


        //[HttpPost]
        //public ActionResult IndexOld1(HomeInputModel inputModel)
        //{
        //    if (!ModelState.IsValid)
        //        return Index();

        //    var result = Invoker.Execute<HomeResponseModel>(inputModel);
        //    if (result.GotoAbout)
        //        return RedirectToAction("About");

        //    TempData["message"] = "Robs Success Message GotoAbout False";
        //    return View("Success", result);
        //}

        //[HttpPost]
        //public ActionResult Index(HomeInputModel inputModel)
        //{
        //    return Handle(inputModel)
        //        .Returning<HomeResponseModel>()
        //        .OnError(_ => Index())
        //        .On(x => x.GotoAbout, y => RedirectToAction("About", y))
        //        .OnSuccessWithMessage(x => View("Success",x), "Robs Success Message GotoAbout False")
        //        ;
        //}

        //[HttpPost]
        //public ActionResult Index(HomeInputModel inputModel)
        //{

        //    var tempObj = new TempObj { GotoAbout = true };

        //    return Handle(inputModel)
        //        .OnSuccessWithMessage(() => View("Success", tempObj), "Robs Success Message GotoAbout False")
        //        .Returning<HomeResponseModel>()
        //        .OnError(_ => Index())
        //        ;
        //}




        [HttpPost]
        public ActionResult Index(HomeInputModel inputModel)
        {
            return Handle(inputModel)
                .Returning<HomeResponseModel>()
                .OnError(_ => Index())
                .On(x => x.GotoAbout, y => RedirectToAction("About", y))
                .OnSuccessWithMessage(x => View("Success",x), "Robs Success Message GotoAbout False")
                ;
        }

    	public ActionResult About()
        {

			//var list = new List<RobT> {new RobT{Name="rob",Age=45}, new RobT{Name="pete",Age=10}, new RobT{Name="john",Age=22}};
			//var aaa = list.RobsForeach(x => x.Name = x.Name+"sdaf").ToList();

            return View();
        }
	}

    public class TempObj
    {
        public bool GotoAbout { get; set; }
    }
}
