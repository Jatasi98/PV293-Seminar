using MediatR;

namespace VerticalSlice.Features.Search.GetSearch;

public class GetSearchQuery : IRequest<GetSearchModel>
{
    public string? Query { get; set; }
}
