using SchoStack.Web;

namespace HandleActionRefactor.Controllers
{
    public class HomeInputHandler : IHandler<HomeInputModel, HomeResponseModel>
    {
        public HomeResponseModel Handle(HomeInputModel input)
        {
            if (input.Age == 42)
                return new HomeResponseModel() { GotoAbout = true };
            return new HomeResponseModel();
        }

    }

	public class HomeInputHandlerNoRet : ICommandHandler<HomeInputModel>
	{
		public void Handle(HomeInputModel command)
		{
			return;
		}
	}
}