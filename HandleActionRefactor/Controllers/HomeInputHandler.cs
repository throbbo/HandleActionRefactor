﻿using SchoStack.Web;

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
}