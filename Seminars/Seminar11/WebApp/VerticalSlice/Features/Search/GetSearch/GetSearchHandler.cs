using MediatR;
using VerticalSlice.Services;

namespace VerticalSlice.Features.Search.GetSearch;

public class GetSearchHandler(ISearchService searchService) : IRequestHandler<GetSearchQuery, GetSearchModel>
{
    private readonly ISearchService searchService = searchService;

    public async Task<GetSearchModel> Handle(GetSearchQuery request, CancellationToken cancellationToken)
    {
        var result = await searchService.SearchAsync(request.Query);

        return new GetSearchModel
        {
            Query = request.Query ?? string.Empty,
            Products = result.Products != null ? [.. result.Products.Items] : [],
            Categories = [.. result.Categories],
        };
    }
}
