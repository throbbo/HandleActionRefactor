using System;
using System.Web.Mvc;

namespace HandleActionRefactor.Controllers
{
	public class HandleAction<TRet, T>
	{
		public Func<TRet, bool> On;
		public Func<T,ControllerContext, ActionResult> Run;
	}
}