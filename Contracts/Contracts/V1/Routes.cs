namespace Contracts.V1;

public static class Routes
{
    public const string Root = "api";
    public const string Version = "v1";

    public const string Base = $"{Root}/{Version}";

    public static class Stock
    {
        public const string GetBySymbol = Base + "/stock/{symbol}";
        public const string SymbolLookup = Base + "/stock/lookup";
        public const string GetRealtimePrice = Base + "/stock/price/{symbol}";
    }

    public static class Portfolio
    {
        public const string GetUserPortfolio = Base + "/portfolio/{userId}";
        public const string GetUserSecurities = Base + "/portfolio/securities/{userId}";
    }
    public static class Identity
    {
        public const string Login = Base + "/identity/login";
        public const string Register = Base + "/identity/register";
        public const string Refresh = Base + "/identity/refresh";
        public const string Identify = Base + "/identity/identify";
    }
    public static class Order
    {
        public const string PlaceMarketOrderWithQuantity = Base + "/order/market/quantity";
        public const string PlaceStopOrderWithQuantity = Base + "/order/stop/quantity";
        public const string PlaceMarketOrderWithAmount = Base + "/order/market/amount";
        public const string GetActiveTrades = Base + "/order/active/";
    }
}