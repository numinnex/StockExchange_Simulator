public interface IFeeProvider
{
    Task<Fee?> GetFeeAsync(int feeId);
}