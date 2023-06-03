namespace Application.Common.Interfaces;

public interface IUriService
{
    public Uri GetAllActiveMarketOrders(int pageNumber, int pageSize);
}