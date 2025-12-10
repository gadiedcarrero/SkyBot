using SkyCore.Abstractions;

namespace SkyCore.Engines;

/// <summary>
/// Motor de recuperación - Maneja rachas perdedoras
/// Equivale al "Reactor" de la nave
/// </summary>
public class RecoveryEngine : IEngine
{
    public string Name => "RecoveryEngine";
    public string Version => "1.0.0";
    public bool IsEnabled { get; set; } = true;

    private EngineParameters? _parameters;
    private int _maxConsecutiveLosses = 3;
    private RecoveryMode _mode = RecoveryMode.ReduceSize;

    public void Initialize(EngineParameters parameters)
    {
        _parameters = parameters;
        _maxConsecutiveLosses = parameters.Get("MaxConsecutiveLosses", 3);
        _mode = parameters.Get("RecoveryMode", RecoveryMode.ReduceSize);
    }

    public bool Validate()
    {
        return _parameters != null && IsEnabled;
    }

    /// <summary>
    /// Determina si el bot debe entrar en modo recuperación
    /// </summary>
    public bool ShouldActivateRecovery(TradingMetrics metrics)
    {
        if (!IsEnabled)
            return false;

        // Activar si hay racha perdedora
        if (metrics.StreakType == -1 && Math.Abs(metrics.CurrentStreak) >= _maxConsecutiveLosses)
            return true;

        // Activar si el drawdown es alto
        if (metrics.CurrentDrawdown >= metrics.MaxDrawdown * 0.7) // 70% del máximo
            return true;

        return false;
    }

    /// <summary>
    /// Calcula el ajuste de tamaño de posición durante recuperación
    /// </summary>
    public double GetPositionSizeMultiplier(TradingMetrics metrics)
    {
        if (!ShouldActivateRecovery(metrics))
            return 1.0;

        return _mode switch
        {
            RecoveryMode.ReduceSize => 0.5, // Reducir al 50%
            RecoveryMode.Stop => 0.0, // Detener trading
            RecoveryMode.Conservative => 0.25, // Muy conservador
            _ => 1.0
        };
    }

    /// <summary>
    /// Obtiene el estado de recuperación
    /// </summary>
    public RecoveryStatus GetStatus(TradingMetrics metrics)
    {
        return new RecoveryStatus
        {
            IsActive = ShouldActivateRecovery(metrics),
            Mode = _mode,
            Reason = GetRecoveryReason(metrics),
            PositionSizeMultiplier = GetPositionSizeMultiplier(metrics)
        };
    }

    private string GetRecoveryReason(TradingMetrics metrics)
    {
        if (metrics.StreakType == -1 && Math.Abs(metrics.CurrentStreak) >= _maxConsecutiveLosses)
            return $"Racha perdedora de {Math.Abs(metrics.CurrentStreak)} operaciones";

        if (metrics.CurrentDrawdown >= metrics.MaxDrawdown * 0.7)
            return $"Drawdown alto: {metrics.CurrentDrawdown:P2}";

        return "Normal";
    }
}

public enum RecoveryMode
{
    ReduceSize,    // Reducir tamaño de posición
    Stop,          // Detener trading
    Conservative   // Muy conservador
}

public class RecoveryStatus
{
    public bool IsActive { get; set; }
    public RecoveryMode Mode { get; set; }
    public string Reason { get; set; } = string.Empty;
    public double PositionSizeMultiplier { get; set; }
}
