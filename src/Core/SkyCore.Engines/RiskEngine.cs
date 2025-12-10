using SkyCore.Abstractions;

namespace SkyCore.Engines;

/// <summary>
/// Motor de gestión de riesgo - Protege el capital
/// Equivale al "Escudo" de la nave
/// </summary>
public class RiskEngine : IEngine
{
    public string Name => "RiskEngine";
    public string Version => "1.0.0";
    public bool IsEnabled { get; set; } = true;

    private EngineParameters? _parameters;
    private double _maxRiskPerTrade = 0.02; // 2% por defecto
    private double _maxDailyDrawdown = 0.05; // 5% por defecto
    private double _maxPositions = 3;

    public void Initialize(EngineParameters parameters)
    {
        _parameters = parameters;
        _maxRiskPerTrade = parameters.Get("MaxRiskPerTrade", 0.02);
        _maxDailyDrawdown = parameters.Get("MaxDailyDrawdown", 0.05);
        _maxPositions = parameters.Get("MaxPositions", 3.0);
    }

    public bool Validate()
    {
        return _parameters != null && IsEnabled;
    }

    /// <summary>
    /// Calcula el tamaño de posición basado en riesgo
    /// </summary>
    public double CalculatePositionSize(double accountBalance, double stopLossDistance)
    {
        if (stopLossDistance <= 0)
            return 0;

        var riskAmount = accountBalance * _maxRiskPerTrade;
        return riskAmount / stopLossDistance;
    }

    /// <summary>
    /// Verifica si se puede abrir una nueva posición
    /// </summary>
    public bool CanOpenPosition(TradingMetrics metrics, int currentPositions)
    {
        if (!IsEnabled)
            return true;

        // Verificar drawdown
        if (metrics.CurrentDrawdown >= _maxDailyDrawdown)
            return false;

        // Verificar límite de posiciones
        if (currentPositions >= _maxPositions)
            return false;

        return true;
    }

    /// <summary>
    /// Calcula el stop loss recomendado
    /// </summary>
    public double CalculateStopLoss(double entryPrice, bool isLong, double atrValue)
    {
        var multiplier = _parameters?.Get("ATRMultiplier", 2.0) ?? 2.0;
        var stopDistance = atrValue * multiplier;

        return isLong
            ? entryPrice - stopDistance
            : entryPrice + stopDistance;
    }

    /// <summary>
    /// Calcula el take profit recomendado
    /// </summary>
    public double CalculateTakeProfit(double entryPrice, double stopLoss, bool isLong)
    {
        var riskRewardRatio = _parameters?.Get("RiskRewardRatio", 2.0) ?? 2.0;
        var stopDistance = Math.Abs(entryPrice - stopLoss);
        var profitDistance = stopDistance * riskRewardRatio;

        return isLong
            ? entryPrice + profitDistance
            : entryPrice - profitDistance;
    }
}
