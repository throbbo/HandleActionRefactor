using System;
using System.Web.Mvc;

namespace HandleActionRefactor.Controllers
{
	public class MyObj<TRet, T>
	{
		public Func<TRet, bool> On;
		public Func<T, ActionResult> Run;
	}
}