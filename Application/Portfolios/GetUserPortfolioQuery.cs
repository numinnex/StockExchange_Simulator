using Application.Common.Interfaces.Repository;
using Application.Common.Models;
using Contracts.V1.Responses;
using MediatR;

namespace Application.Portfolios;

public record GetUserPortfolioQuery(string UserId) : IRequest<Result<PortfolioResponse>>;
public sealed class GetUserPortfolioQueryHandler: IRequestHandler<GetUserPortfolioQuery, Result<PortfolioResponse>>
{
    private readonly IPortfolioRepository _portfolioRepository;

    public GetUserPortfolioQueryHandler(IPortfolioRepository portfolioRepository)
    {
        _portfolioRepository = portfolioRepository;
    }
    public async Task<Result<PortfolioResponse>> Handle(GetUserPortfolioQuery request, CancellationToken cancellationToken)
    {
        var result = await _portfolioRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (result is not null)
        {
            return Result<PortfolioResponse>.Success(new PortfolioResponse
            {
                ValueSnapshots = result.ValueSnapshots.Select(x => new ValueSnapshotResponse
                {
                    Timestamp = x.Timestamp,
                    Value = x.Value
                }).ToList(),
                TotalValue = result.TotalValue
            });
        }
        return Result<PortfolioResponse>.Failure(new []
        {
            new Error
            {
                Message = "Portfolio not found",
                Code = "Not Found"
            }
        });
}
}