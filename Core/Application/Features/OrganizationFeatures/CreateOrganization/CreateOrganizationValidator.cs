using FluentValidation;

namespace Realchat.Application.Features.OrganizationFeatures.CreateOrganization;

public sealed class CreateOrganizationValidator : AbstractValidator<CreateOrganizationRequest>
{
    public CreateOrganizationValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(50);
    }
}