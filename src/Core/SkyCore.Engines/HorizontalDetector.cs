using SkyCore.Abstractions;

namespace SkyCore.Engines;

/// <summary>
/// Detector de rangos horizontales - Identifica mercados laterales
/// Equivale a los "Sensores" de la nave
/// </summary>
public class HorizontalDetector : IEngine
{
    public string Name => "HorizontalDetector";
    public string Version => "1.0.0";
    public bool IsEnabled { get; set; } = true;

    private EngineParameters? _parameters;
    private double _rangeTolerance = 0.02; // 2% de tolerancia
    private int _minBarsInRange = 10;

    public void Initialize(EngineParameters parameters)
    {
        _parameters = parameters;
        _rangeTolerance = parameters.Get("RangeTolerance", 0.02);
        _minBarsInRange = parameters.Get("MinBarsInRange", 10);
    }

    public bool Validate()
    {
        return _parameters != null && IsEnabled;
    }

    /// <summary>
    /// Detecta si el mercado está en rango horizontal
    /// </summary>
    public virtual bool IsMarketRanging(double[] highs, double[] lows, double[] closes)
    {
        if (!IsEnabled || highs.Length < _minBarsInRange)
            return false;

        // Calcular el rango del mercado
        var maxHigh = highs.Max();
        var minLow = lows.Min();
        var range = maxHigh - minLow;
        var avgPrice = (maxHigh + minLow) / 2;

        // Si el rango es menor que la tolerancia, está lateral
        var rangePercent = range / avgPrice;
        return rangePercent <= _rangeTolerance;
    }

    /// <summary>
    /// Detecta niveles de soporte y resistencia
    /// </summary>
    public virtual SupportResistanceLevels DetectLevels(double[] highs, double[] lows, double[] closes)
    {
        if (highs.Length < _minBarsInRange)
        {
            return new SupportResistanceLevels
            {
                Support = 0,
                Resistance = 0,
                IsValid = false
            };
        }

        // Implementación simple - puede ser más sofisticada
        var recentBars = Math.Min(_minBarsInRange, highs.Length);
        var recentHighs = highs.TakeLast(recentBars).ToArray();
        var recentLows = lows.TakeLast(recentBars).ToArray();

        return new SupportResistanceLevels
        {
            Support = recentLows.Min(),
            Resistance = recentHighs.Max(),
            IsValid = true
        };
    }

    /// <summary>
    /// Calcula la fuerza del rango
    /// </summary>
    public double CalculateRangeStrength(double[] highs, double[] lows, double[] closes)
    {
        if (highs.Length < 2)
            return 0;

        // Cuantas más veces toca soporte/resistencia, más fuerte el rango
        var levels = DetectLevels(highs, lows, closes);
        if (!levels.IsValid)
            return 0;

        var touches = 0;
        var tolerance = (levels.Resistance - levels.Support) * 0.1; // 10% de tolerancia

        for (int i = 0; i < highs.Length; i++)
        {
            if (Math.Abs(highs[i] - levels.Resistance) <= tolerance)
                touches++;
            if (Math.Abs(lows[i] - levels.Support) <= tolerance)
                touches++;
        }

        // Normalizar entre 0 y 1
        return Math.Min(1.0, touches / (double)(_minBarsInRange * 2));
    }
}

public class SupportResistanceLevels
{
    public double Support { get; set; }
    public double Resistance { get; set; }
    public bool IsValid { get; set; }
    public double RangeSize => Resistance - Support;
}
