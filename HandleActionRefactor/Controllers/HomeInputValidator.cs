using FluentValidation;

namespace HandleActionRefactor.Controllers
{
    public class HomeInputValidator : AbstractValidator<HomeInputModel>
    {
        public HomeInputValidator()
        {
            RuleFor(x => x.Age)
                .NotEmpty()
                .GreaterThan(10);

            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}