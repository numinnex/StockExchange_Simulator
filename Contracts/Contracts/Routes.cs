namespace Contracts;

public static class Routes
{
    public const string Root = "api";
    public const string Version = "v1"; 
    
    public const string Base = $"{Root}/{Version}";

    public static class Stocks
    {
        public const string GetBySymbol = Base + "/stocks/{symbol}";
        public const string GetRealtimePrice = Base + "/stocks/price/{symbol}";
    }
}