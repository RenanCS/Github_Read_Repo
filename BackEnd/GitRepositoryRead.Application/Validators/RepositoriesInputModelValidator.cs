using FluentValidation;
using GitRepositoryRead.InputModel;

namespace GitRepositoryRead.Validators
{
    public class RepositoriesInputModelValidator : AbstractValidator<RepositoriesInputModel>
    {
        public RepositoriesInputModelValidator()
        {
            RuleFor(p => p.UserName)
           .NotEmpty()
           .WithMessage("Deve ser informado o nome do usuário");

        }
    }
}
