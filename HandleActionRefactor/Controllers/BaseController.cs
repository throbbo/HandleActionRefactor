using System;
using System.Collections.Generic;
using System.Web.Mvc;
using SchoStack.Web;

namespace HandleActionRefactor.Controllers
{
    public class BaseController : Controller
    {
        public IInvoker Invoker { get; set; }

		public ActionResultFactoryBuilder<T> Handle<T>(T inputModel)
		{
			return new ActionResultFactoryBuilder<T>(Invoker, inputModel);
		}
    }

	public class ActionResultFactoryBuilder<T>
	{
		private readonly IInvoker _invoker;
		private readonly T _inputModel;
		private readonly ActionResultFactory<T> _actionResultFactory;
        private Func<ControllerContext, ActionResult> _runOnError;
        private Func<ControllerContext, ActionResult> _runOnSuccess;

	    public ActionResultFactoryBuilder(IInvoker invoker, T inputModel)
		{
			_invoker = invoker;
			_inputModel = inputModel;
			_actionResultFactory = new ActionResultFactory<T>(this);
		}

		public static implicit operator ActionResultFactory<T>(ActionResultFactoryBuilder<T> builder)
		{
			return builder._actionResultFactory;
		}

		public ActionResultFactoryBuilder<T> OnError(Func<ActionResult> runOnError )
		{
            _runOnError = x => runOnError();
			return this;
		}
        public ActionResultFactoryBuilder<T> OnError(Func<ControllerContext,ActionResult> runOnError)
        {
            _runOnError = runOnError;
            return this;
        }
        public ActionResultFactoryBuilder<T> OnSuccess(Func<ActionResult> runOnSuccess)
		{
            _runOnSuccess = x => runOnSuccess();
            return this;
		}
        public ActionResultFactoryBuilder<T> OnSuccess(Func<ControllerContext, ActionResult> runOnSuccess)
        {
            _runOnSuccess = runOnSuccess;
            return this;
        }
        public ActionResultFactoryBuilder<T, TRet> Returning<TRet>()
		{
            return new ActionResultFactoryBuilder<T, TRet>(_invoker, _inputModel, _runOnError, _runOnSuccess);
		}

		public class ActionResultFactory<T> : ActionResult
		{
			private readonly ActionResultFactoryBuilder<T> _builder;

			public ActionResultFactory(ActionResultFactoryBuilder<T> actionResultFactoryBuilder)
			{
				_builder = actionResultFactoryBuilder;
			}

			public override void ExecuteResult(ControllerContext context)
			{
				if(!context.Controller.ViewData.ModelState.IsValid && _builder._runOnError!=null)
				{
					_builder._runOnError(context).ExecuteResult(context);
					return;
				}

				_builder._invoker.Execute<T>(_builder._inputModel);

				if(_builder._runOnSuccess!=null)
				{
					_builder._runOnSuccess(context);
					return;
				}
			}
		}

	    public void OnErrorAjax1(Func<JsonResult> func, Func<ActionResult> func1)
	    {
	        throw new NotImplementedException();
	    }
	}


	public class ActionResultFactoryBuilder<T, TRet>
	{
        private readonly IInvoker _invoker;
        private readonly T _inputModel;
        List<MyObj<TRet, T>> Ons = new List<MyObj<TRet, T>>();
        private Func<T, ActionResult> _redirectIfOn = null;
        private Func<TRet, ControllerContext, ActionResult> _onSuccess;
        private Func<T, ControllerContext, ActionResult> _runOnError;
        private Func<T, ControllerContext, ActionResult> _runOnErrorAjax;
        private readonly ActionResultFactory<T, TRet> _actionResultFactory;

        public ActionResultFactoryBuilder(IInvoker invoker, T inputModel, Func<ControllerContext, ActionResult> runOnError, Func<ControllerContext, ActionResult> runOnSuccess)
		{
			_invoker = invoker;
			_inputModel = inputModel;
	        _actionResultFactory = new ActionResultFactory<T, TRet>(this);
            if (_runOnError != null) _runOnError = (result, context) => runOnError(context);
            if (_onSuccess != null) _onSuccess = (result, context) => runOnSuccess(context);
		}
		public static implicit operator  ActionResultFactory<T, TRet>(ActionResultFactoryBuilder<T, TRet> builder)
		{
			return builder._actionResultFactory;
		}

		public ActionResultFactoryBuilder<T, TRet> On(Func<TRet, bool> funcOn, Func<T, ActionResult> redirectIfOn)
		{


            var on = new MyObj<TRet, T>() { On = (a,b) => funcOn(a), Run = (a,b) => redirectIfOn(a) };

            Ons.Add(on);

			return this;
		}

        public ActionResultFactoryBuilder<T, TRet> On(Func<TRet, ControllerContext, bool> funcOn, Func<T, ControllerContext, ActionResult> redirectIfOn)
        {

            var on = new MyObj<TRet, T>() { On = funcOn, Run = redirectIfOn };

            Ons.Add(on);

            return this;
        }

        public ActionResultFactoryBuilder<T, TRet> OnSuccess(Func<TRet, ControllerContext, ActionResult> onSuccess)
        {
            _onSuccess = onSuccess; 
			return this;
		}
        public ActionResultFactoryBuilder<T, TRet> OnSuccess(Func<TRet, ActionResult> onSuccess)
        {
            _onSuccess = (x,c) => onSuccess(x);
            return this;
        }
        public Func<T, ControllerContext, ActionResult> OnError()
        {
            return _runOnError;
        }
        public ActionResultFactoryBuilder<T, TRet> OnError(Func<T, ControllerContext, ActionResult> runOnError)
        {
            _runOnError = runOnError;
            return this;
        }
        public ActionResultFactoryBuilder<T, TRet> OnError(Func<T, ActionResult> runOnError)
		{
			_runOnError = (a,b) => runOnError(a);
			return this;
		}
		public class ActionResultFactory<T, TRet> : ActionResult
		{
			private readonly ActionResultFactoryBuilder<T, TRet> _builder;

			public ActionResultFactory(ActionResultFactoryBuilder<T,TRet> actionResultFactoryBuilder)
			{
				_builder = actionResultFactoryBuilder;
			}

			public override void ExecuteResult(ControllerContext context)
			{
				if (!context.Controller.ViewData.ModelState.IsValid && _builder._runOnError != null)
				{
                    _builder._runOnError(_builder._inputModel, context).ExecuteResult(context);
					return;
				}
                
                var result = _builder._invoker.Execute<TRet>(_builder._inputModel);

                foreach (var on in _builder.Ons)
                {
                    if (on.On != null && on.On(result, context))
                    {
                        on.Run(_builder._inputModel, context).ExecuteResult(context);
                        return;
                    }
                }

			    if (_builder._onSuccess == null) return;

                _builder._onSuccess(result, context).ExecuteResult(context);
			}

		}
	}


    public static class ActionResultFactoryBuilderExtensions
    {
       
        public static ActionResultFactoryBuilder<T, TRet> OnSuccessWithMessage<T, TRet>(this ActionResultFactoryBuilder<T, TRet> builder,
                            Func<TRet, ActionResult> redirectTo, string message)
        {
            return builder
                .OnSuccess((result,context) =>
                {
                    if (!string.IsNullOrEmpty(message))
                        context.Controller.TempData.Add("message", message);

                    return redirectTo(result);
                });
        }

        public static ActionResultFactoryBuilder<T> OnErrorAjax<T>(this ActionResultFactoryBuilder<T> builder,
                            Func<ActionResult> runOnErrorAjax, Func<ActionResult> runOnError)
        {
            return builder
                .OnError((context) =>
                {
                    if (context.HttpContext.Request.IsAjaxRequest())
                    {
                        context.Controller.TempData.Add("message", "Got an Ajax error");

                        return runOnErrorAjax();
                    }

                    return runOnErrorAjax();
                });
        }
    }

}