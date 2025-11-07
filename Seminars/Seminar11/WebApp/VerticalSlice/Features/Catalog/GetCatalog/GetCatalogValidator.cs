using FluentValidation;

namespace VerticalSlice.Features.Catalog.GetCatalog;

public class GetCatalogValidator : AbstractValidator<GetCatalogQuery>
{
    public GetCatalogValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page can not be negative or zero");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size can not be greater than 100");
    }
}
