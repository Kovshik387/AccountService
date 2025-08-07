namespace AccountService.Features;
/// <summary>
/// Обобщенный тип результата выполнения
/// </summary>
/// <typeparam name="TData"></typeparam>
public record MbResult<TData> : MbResult where TData : class
{
    /// <summary>
    /// Результат выполнения
    /// </summary>
    public TData? Result { get; init; }
    public static MbResult<TData> SuccessResult(TData result)
    {
        return new MbResult<TData> 
            { 
                Success = true,
                Result = result 
            };
    }
}
/// <summary>
/// Тип результат выполнения
/// </summary>
public record MbResult
{
    /// <summary>
    /// Успех выполнения
    /// </summary>
    public bool Success { get; init; }
    /// <summary>
    /// Ошибка
    /// </summary>
    public string? ErrorMessage { get; init; }
    /// <summary>
    /// Подробная информация об ошибке
    /// </summary>
    public Dictionary<string,string>? MbError { get; init; }
    
    public static MbResult Error(string error, Dictionary<string,string>? errorDetails = null)
    {
        return new MbResult 
        { 
            Success = false,
            ErrorMessage = error,
            MbError = errorDetails
        };
    }
}