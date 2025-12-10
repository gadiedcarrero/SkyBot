namespace SkyCore.Abstractions;

/// <summary>
/// Métricas de trading (mapeo del "Universe")
/// Daño = Drawdown
/// Combate = Rachas
/// Combustible = Capital
/// </summary>
public class TradingMetrics
{
    /// <summary>
    /// Drawdown actual (Daño)
    /// </summary>
    public double CurrentDrawdown { get; set; }

    /// <summary>
    /// Drawdown máximo permitido
    /// </summary>
    public double MaxDrawdown { get; set; }

    /// <summary>
    /// Capital actual (Combustible)
    /// </summary>
    public double CurrentCapital { get; set; }

    /// <summary>
    /// Capital inicial
    /// </summary>
    public double InitialCapital { get; set; }

    /// <summary>
    /// Racha actual de operaciones (Combate)
    /// </summary>
    public int CurrentStreak { get; set; }

    /// <summary>
    /// Tipo de racha: 1 = ganadora, -1 = perdedora, 0 = ninguna
    /// </summary>
    public int StreakType { get; set; }

    /// <summary>
    /// Total de operaciones ganadoras
    /// </summary>
    public int WinningTrades { get; set; }

    /// <summary>
    /// Total de operaciones perdedoras
    /// </summary>
    public int LosingTrades { get; set; }

    /// <summary>
    /// Profit total
    /// </summary>
    public double TotalProfit { get; set; }

    /// <summary>
    /// Pérdida total
    /// </summary>
    public double TotalLoss { get; set; }

    /// <summary>
    /// Calcula el win rate
    /// </summary>
    public double WinRate =>
        (WinningTrades + LosingTrades) > 0
            ? (double)WinningTrades / (WinningTrades + LosingTrades)
            : 0;

    /// <summary>
    /// Calcula el profit factor
    /// </summary>
    public double ProfitFactor =>
        TotalLoss > 0
            ? TotalProfit / Math.Abs(TotalLoss)
            : 0;

    /// <summary>
    /// Calcula el % de capital utilizado
    /// </summary>
    public double CapitalUsagePercent =>
        InitialCapital > 0
            ? ((CurrentCapital - InitialCapital) / InitialCapital) * 100
            : 0;
}
