namespace Application.Common.Models;
public sealed class Ok<T1, T2> where T1 : class 
    where T2 : class
{
    private readonly T1? _value1;
    private readonly T2? _value2;
    private readonly bool _isOk;

    public Ok(bool isOk , T1? value1, T2? value2)
    {
        _value1 = value1;
        _value2 = value2;
        _isOk = isOk;
    }

    public T1? FirstValue => _value1;
    public T2? SecondValue => _value2;

    public bool IsOk => _isOk;
    
}
