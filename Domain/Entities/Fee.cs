public class Fee
{
    public int Id { get; init; }
    public required decimal MakerFee { get; init; }
    public required decimal TakerFee { get; init; }
}