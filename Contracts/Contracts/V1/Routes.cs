namespace Contracts.V1;

public static class Routes
{
    public const string Root = "api";
    public const string Version = "v1"; 
    
    public const string Base = $"{Root}/{Version}";

    public static class Stocks
    {
        public const string GetBySymbol = Base + "/stocks/{symbol}";
        public const string GetRealtimePrice = Base + "/stocks/price/{symbol}";
        public const string BuyMarket = Base + "/stocks/buymarket";
        public const string SellMarket = Base + "/stocks/sellmarket";
    }
    public static class Identity
    {
        public const string Login = Base + "/identity/login";
        public const string Register = Base + "/identity/register";
        public const string Refresh = Base + "/identity/refresh";
    }
}