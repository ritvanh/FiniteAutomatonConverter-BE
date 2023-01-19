using FiniteAutomatonConverter.DTOs;
using FluentValidation;

namespace FiniteAutomatonConverter.Validators
{
    public class AutomatonDtoValidator : AbstractValidator<AutomatonDto>
    {
        public AutomatonDtoValidator()
        {
            RuleFor(x => x.InitialState).NotNull().WithMessage("Gjendja fillestare mungon");
            RuleFor(x => x.States.Count).GreaterThan(0).WithMessage("Jepni te pakten nje gjendje");
            RuleFor(x => x.AllTransitions.Count).GreaterThan(0).WithMessage("Jepni ndonje kalim");
        }
    }
}
