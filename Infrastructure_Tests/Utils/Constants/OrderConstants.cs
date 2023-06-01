using Domain.Enums;

namespace Infrastructure_Tests.Utils.Constants;

public static class OrderConstants
{
    public static Guid Id = Guid.NewGuid();
    public static Guid StockId = Guid.NewGuid();
    public static int FeeId = 1;
    public static TradeCondition TradeCondition = TradeCondition.None;
    public static string Symbol = "APPL";

    public static string UserIdFromIndex(int index) => $"test-user-id{index}";
}