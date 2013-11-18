using System;
using System.Web.Mvc;

namespace HandleActionRefactor.Controllers
{
	public class MyObj<TRet, T>
	{
        public Func<TRet, ControllerContext, bool> On;
        public Func<T, ControllerContext, ActionResult> Run;
	}
}