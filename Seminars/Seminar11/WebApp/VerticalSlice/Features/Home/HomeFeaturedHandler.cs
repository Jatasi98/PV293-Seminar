using MediatR;
using VerticalSlice.Services;

namespace VerticalSlice.Features.Home;

public class HomeFeaturedHandler(ISearchService searchService) : IRequestHandler<HomeFeaturedQuery, HomeIndexModel>
{
    private readonly ISearchService searchService = searchService;

    public async Task<HomeIndexModel> Handle(HomeFeaturedQuery request, CancellationToken cancellationToken)
    {
        var categories = await searchService.GetFeaturedCategories();

        var products = await searchService.GetFeaturedProducts();

        return new HomeIndexModel
        {
            PopularCategories = [.. categories ?? []],
            FeaturedProducts = [.. products ?? []],
        };
    }
}
