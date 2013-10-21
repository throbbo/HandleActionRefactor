using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Resources;
using System.Web.Script.Serialization;
using HandleActionRefactor.Handlers;
using SchoStack.Web;

namespace HandleActionRefactor.Controllers
{
    public class BaseController : Controller
    {
    	public IInvoker Invoker { get; set; }

    	public HandleActionResult<T> Handle<T>(T inputModel)
		{
			return new HandleActionResult<T>(inputModel, Invoker);
		}
    }

	public class HandleActionResult<T> : ActionResult
	{
		private readonly T _inputModel;
		private readonly IInvoker _invoker;

		public HandleActionResult(T inputModel, IInvoker invoker)
		{
			_inputModel = inputModel;
			_invoker = invoker;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			
		}

		public HandleActionResult<T,TRet> Returning<TRet>()
		{
			var result = new HandleActionResult<T, TRet>(_invoker, _inputModel);

			return result;
		}
	}
	public class MyObj<TRet, T>
	{
		public Func<TRet, bool> On;
		public Func<T, ActionResult> Run;
	}
	public class HandleActionResult<T, TRet> : ActionResult
	{
		private readonly IInvoker _invoker;
		private readonly T _inputModel;
		private Func<T, ActionResult> _success;
		private Func<T, ActionResult> _error;
		private TRet _result;

		List<MyObj<TRet,T>> Ons = new List<MyObj<TRet,T>>();

		public HandleActionResult(IInvoker invoker, T inputModel)
		{
			_invoker = invoker;
			_inputModel = inputModel;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			_result = _invoker.Execute<TRet>(_inputModel);	// Run Handler Get Result

			foreach (var on in Ons)
			{
				if (on.On != null)
					if ( on.On(_result))
					{
						on.Run(_inputModel).ExecuteResult(context);
						return;
					}
			}

			if (_success != null){
				_success(_inputModel).ExecuteResult(context);
				return;
			}

			if (_error != null)
			{
				_error(_inputModel).ExecuteResult(context);
				return;
			}
		}

		public HandleActionResult<T, TRet> OnSuccess(Func<T, ActionResult> redirectTo)
		{
			_success = redirectTo;
			return this;
		}

		public HandleActionResult<T, TRet> OnError(Func<T, ActionResult> index)
		{
			_error = index;
			return this;
		}

		public HandleActionResult<T, TRet> On(Func<TRet, bool> func, Func<T, ActionResult> redirectTo)
		{
			var on = new MyObj<TRet, T>() { On = func, Run = redirectTo };

			Ons.Add(on);
			
			return this;
		}
	}

}