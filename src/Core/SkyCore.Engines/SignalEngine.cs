using SkyCore.Abstractions;

namespace SkyCore.Engines;

/// <summary>
/// Motor de señales - Genera señales de entrada/salida
/// Equivale al "Motor" de la nave
/// </summary>
public class SignalEngine : IEngine
{
    public string Name => "SignalEngine";
    public string Version => "1.0.0";
    public bool IsEnabled { get; set; } = true;

    private EngineParameters? _parameters;

    public void Initialize(EngineParameters parameters)
    {
        _parameters = parameters;
    }

    public bool Validate()
    {
        return _parameters != null && IsEnabled;
    }

    /// <summary>
    /// Analiza si hay señal de entrada
    /// </summary>
    public virtual SignalResult AnalyzeEntry()
    {
        // Implementación base - override en clases derivadas
        return new SignalResult
        {
            HasSignal = false,
            SignalType = SignalType.None,
            Confidence = 0
        };
    }

    /// <summary>
    /// Analiza si hay señal de salida
    /// </summary>
    public virtual SignalResult AnalyzeExit()
    {
        // Implementación base - override en clases derivadas
        return new SignalResult
        {
            HasSignal = false,
            SignalType = SignalType.None,
            Confidence = 0
        };
    }
}

public class SignalResult
{
    public bool HasSignal { get; set; }
    public SignalType SignalType { get; set; }
    public double Confidence { get; set; }
    public string? Reason { get; set; }
}

public enum SignalType
{
    None,
    Buy,
    Sell,
    Close
}
