public interface IFeeProvider
{
    Task<Fee?> GetFee(int feeId);
}