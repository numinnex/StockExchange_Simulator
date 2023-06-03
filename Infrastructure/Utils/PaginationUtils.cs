using Application.Common.Interfaces;
using Contracts.V1.Responses;
namespace Infrastructure.Utils;

public sealed class PaginationUtils
{
    public static PagedResponse<T> CreatePaginatedResponse<T>(IUriService uriService,
        int pageSize, int pageNumber,
        List<T> response)
    {
        var nextPage = pageNumber >= 1
            ? uriService
                .GetAllActiveMarketOrders(pageNumber + 1, pageSize).ToString()
            : null;
        var previousPage = pageNumber - 1 >= 1
            ? uriService
                .GetAllActiveMarketOrders(pageNumber - 1, pageSize).ToString()
            : null;

        var paginationResponse = new PagedResponse<T>()
        {
            Data = response,
            PageNumber = pageNumber >= 1 ? pageNumber : null,
            PageSize = pageSize >= 1 ? pageSize : null,
            NextPage = response.Any() ? nextPage : "",
            PreviousPage = previousPage ?? ""
        };
        return paginationResponse;
    }
}