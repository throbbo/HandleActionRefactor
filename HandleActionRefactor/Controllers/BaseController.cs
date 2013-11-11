using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Resources;
using System.Web.Script.Serialization;
using SchoStack.Web;

namespace HandleActionRefactor.Controllers
{
    public class BaseController : Controller
    {
    	public IInvoker Invoker { get; set; }

		public HandleActionResultBuilder<T> Handle<T>(T inputModel)
		{
			return new HandleActionResultBuilder<T>(inputModel, Invoker);
		}
    }

	public class HandleActionResultBuilder<T>
	{
		private readonly T _inputModel;
		private readonly IInvoker _invoker;
		private Func<ActionResult> _success;
		private Func<ActionResult> _error;
		
		public HandleActionResultBuilder(T inputModel, IInvoker invoker)
		{
			_inputModel = inputModel;
			_invoker = invoker;
		}

		public static implicit operator HandleActionResult<T>(HandleActionResultBuilder<T> builder)
		{
			return new HandleActionResult<T>(builder);
		}

		public HandleActionResultBuilder<T> OnSuccess(Func<ActionResult> redirectTo)
		{
			_success = redirectTo;
			return this;
		}

		public HandleActionResultBuilder<T> OnError(Func<ActionResult> index)
		{
			_error = index;
			return this;
		}

		public HandleActionResultBuilder<T, TRet> Returning<TRet>()
		{
			var result = new HandleActionResultBuilder<T, TRet>(_inputModel, _invoker, _success, _error);

			return result;
		}

		public class HandleActionResult<T> : ActionResult
		{
			private readonly HandleActionResultBuilder<T> _builder;
			public HandleActionResult(HandleActionResultBuilder<T> builder )
			{
				_builder = builder;
			}

			public override void ExecuteResult(ControllerContext context)
			{
				if (!context.Controller.ViewData.ModelState.IsValid && _builder._error != null)
				{
					_builder._error().ExecuteResult(context);
					return;
				}

				_builder._invoker.Execute(_builder._inputModel);	// Run Handler Get Result

				if (_builder._success != null)
					_builder._success().ExecuteResult(context);
			}

		}

	    public HandleActionResultBuilder<T> OnSuccessWithMessage(Func<RedirectToRouteResult> func)
	    {
	        return this;
	    }
	}


	public class HandleActionResultBuilder<T,TRet> 
	{
		private readonly IInvoker _invoker;
		private readonly T _inputModel;
		private Func<TRet, ActionResult> _success;
		private Func<ActionResult> _error;
		List<MyObj<TRet, T>> Ons = new List<MyObj<TRet, T>>();

		public HandleActionResultBuilder(T inputModel, IInvoker invoker, Func<ActionResult> baseSuccess, Func<ActionResult> baseError) 
		{
			_inputModel = inputModel;
			_invoker = invoker;
			_success = _ => baseSuccess();
			_error = baseError;
		}


		public static implicit operator HandleActionResult<T, TRet> (HandleActionResultBuilder<T,TRet> builder )
		{
			return new HandleActionResult<T, TRet>(builder);
		}

		public HandleActionResultBuilder<T, TRet> OnSuccess(Func<TRet, ActionResult> redirectTo)
		{
			_success = redirectTo;
			return this;
		}

		public HandleActionResultBuilder<T, TRet> OnError(Func<ActionResult> index)
		{
			_error = index;
			return this;
		}

		public HandleActionResultBuilder<T, TRet> On(Func<TRet, bool> func, Func<T, ActionResult> redirectTo)
		{
			var on = new MyObj<TRet, T>() { On = func, Run = redirectTo };

			Ons.Add(on);

			return this;
		}

        public HandleActionResultBuilder<T, TRet> OnSuccessWithMessage(Func<TRet,ActionResult> redirectTo)
        {
            _success = redirectTo;

            return this;
        }

		public class HandleActionResult<T, TRet> : ActionResult
		{
			private readonly HandleActionResultBuilder<T, TRet> _builder;
			public HandleActionResult(HandleActionResultBuilder<T,TRet> builder)
			{
				_builder = builder;
			}

			public override void ExecuteResult(ControllerContext context)
			{
				if (!context.Controller.ViewData.ModelState.IsValid && _builder._error != null)
				{
					_builder._error().ExecuteResult(context);
					return;
				}

				var _result = _builder._invoker.Execute<TRet>(_builder._inputModel);	// Run Handler Get Result

				foreach (var on in _builder.Ons)
				{
					if (on.On != null && on.On(_result))
					{
						on.Run(_builder._inputModel).ExecuteResult(context);
						return;
					}
				}

				if (_builder._success != null)
					_builder._success(_result).ExecuteResult(context);
			}

		}

	}


}