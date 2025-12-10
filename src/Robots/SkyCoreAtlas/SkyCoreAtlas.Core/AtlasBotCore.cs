using SkyCore.Abstractions;
using SkyCore.Common;
using SkyCore.Engines;

namespace SkyCoreAtlas.Core;

/// <summary>
/// SkyCoreAtlas - La primera nave del SkyBot Universe
/// Bot de trading con estrategia completa y gestión de riesgo
/// </summary>
public class AtlasBotCore : BaseBotCore
{
    public override string BotName => "SkyCoreAtlas";
    public override string Version => "1.0.0";

    private SignalEngine? _signalEngine;
    private RiskEngine? _riskEngine;
    private RecoveryEngine? _recoveryEngine;
    private HorizontalDetector? _horizontalDetector;

    public AtlasConfiguration Configuration { get; private set; }

    public AtlasBotCore(AtlasConfiguration configuration)
    {
        Configuration = configuration;
        InitializeEngines();
    }

    private void InitializeEngines()
    {
        // Crear y configurar SignalEngine
        _signalEngine = new SignalEngine();
        var signalParams = new EngineParameters();
        _signalEngine.Initialize(signalParams);
        RegisterEngine(_signalEngine);

        // Crear y configurar RiskEngine
        _riskEngine = new RiskEngine();
        var riskParams = new EngineParameters();
        riskParams.Set("MaxRiskPerTrade", Configuration.MaxRiskPerTrade);
        riskParams.Set("MaxDailyDrawdown", Configuration.MaxDailyDrawdown);
        riskParams.Set("MaxPositions", Configuration.MaxPositions);
        riskParams.Set("ATRMultiplier", Configuration.ATRMultiplier);
        riskParams.Set("RiskRewardRatio", Configuration.RiskRewardRatio);
        _riskEngine.Initialize(riskParams);
        RegisterEngine(_riskEngine);

        // Crear y configurar RecoveryEngine
        _recoveryEngine = new RecoveryEngine();
        var recoveryParams = new EngineParameters();
        recoveryParams.Set("MaxConsecutiveLosses", Configuration.MaxConsecutiveLosses);
        recoveryParams.Set("RecoveryMode", Configuration.RecoveryMode);
        _recoveryEngine.Initialize(recoveryParams);
        RegisterEngine(_recoveryEngine);

        // Crear y configurar HorizontalDetector
        _horizontalDetector = new HorizontalDetector();
        var detectorParams = new EngineParameters();
        detectorParams.Set("RangeTolerance", Configuration.RangeTolerance);
        detectorParams.Set("MinBarsInRange", Configuration.MinBarsInRange);
        _horizontalDetector.Initialize(detectorParams);
        RegisterEngine(_horizontalDetector);
    }

    protected override void OnStart()
    {
        Console.WriteLine($"🚀 {BotName} v{Version} iniciado");
        Console.WriteLine($"   Motor: {_signalEngine?.Name}");
        Console.WriteLine($"   Escudo: {_riskEngine?.Name}");
        Console.WriteLine($"   Reactor: {_recoveryEngine?.Name}");
        Console.WriteLine($"   Sensores: {_horizontalDetector?.Name}");

        _metrics.InitialCapital = Configuration.InitialCapital;
        _metrics.CurrentCapital = Configuration.InitialCapital;
        _metrics.MaxDrawdown = Configuration.MaxDailyDrawdown;
    }

    protected override void OnStop()
    {
        Console.WriteLine($"🛑 {BotName} detenido");
        PrintStatistics();
    }

    /// <summary>
    /// Procesa una nueva barra/tick
    /// Este es el "loop principal" del bot
    /// </summary>
    public void ProcessBar(MarketData marketData)
    {
        if (_signalEngine == null || _riskEngine == null || _recoveryEngine == null || _horizontalDetector == null)
            return;

        // 1. Actualizar métricas
        UpdateMetrics(marketData.AccountBalance, CalculateCurrentDrawdown(marketData), marketData.OpenPositions);

        // 2. Verificar estado de recuperación
        var recoveryStatus = _recoveryEngine.GetStatus(_metrics);
        if (recoveryStatus.IsActive)
        {
            Console.WriteLine($"⚠️ Modo recuperación activo: {recoveryStatus.Reason}");
        }

        // 3. Detectar si el mercado está en rango
        var isRanging = _horizontalDetector.IsMarketRanging(
            marketData.RecentHighs,
            marketData.RecentLows,
            marketData.RecentCloses);

        if (isRanging && Configuration.AvoidRangingMarkets)
        {
            Console.WriteLine("📊 Mercado en rango - Evitando entrada");
            return;
        }

        // 4. Verificar si podemos abrir posición
        if (!_riskEngine.CanOpenPosition(_metrics, marketData.OpenPositions))
        {
            return;
        }

        // 5. Buscar señal de entrada
        var entrySignal = _signalEngine.AnalyzeEntry();
        if (entrySignal.HasSignal)
        {
            // 6. Calcular tamaño de posición con ajuste de recuperación
            var baseSize = _riskEngine.CalculatePositionSize(
                marketData.AccountBalance,
                marketData.ATR * Configuration.ATRMultiplier);

            var adjustedSize = baseSize * recoveryStatus.PositionSizeMultiplier;

            // 7. Calcular stop loss y take profit
            var stopLoss = _riskEngine.CalculateStopLoss(
                marketData.CurrentPrice,
                entrySignal.SignalType == SignalType.Buy,
                marketData.ATR);

            var takeProfit = _riskEngine.CalculateTakeProfit(
                marketData.CurrentPrice,
                stopLoss,
                entrySignal.SignalType == SignalType.Buy);

            // 8. Ejecutar orden (esto lo implementa el cBot específico)
            OnTradeSignal(new TradeSignal
            {
                Type = entrySignal.SignalType,
                Size = adjustedSize,
                StopLoss = stopLoss,
                TakeProfit = takeProfit,
                Confidence = entrySignal.Confidence,
                Reason = entrySignal.Reason ?? "Señal generada"
            });
        }
    }

    /// <summary>
    /// Callback cuando hay una señal de trading
    /// Override en el cBot para ejecutar la orden
    /// </summary>
    protected virtual void OnTradeSignal(TradeSignal signal)
    {
        // Implementar en clase derivada (cBot)
    }

    private double CalculateCurrentDrawdown(MarketData marketData)
    {
        if (_metrics.InitialCapital == 0)
            return 0;

        var currentEquity = marketData.AccountBalance;
        var peak = Math.Max(_metrics.InitialCapital, _metrics.CurrentCapital);

        return peak > 0 ? (peak - currentEquity) / peak : 0;
    }

    private void PrintStatistics()
    {
        Console.WriteLine("\n📊 Estadísticas finales:");
        Console.WriteLine($"   Capital inicial: ${_metrics.InitialCapital:F2}");
        Console.WriteLine($"   Capital final: ${_metrics.CurrentCapital:F2}");
        Console.WriteLine($"   Profit/Loss: ${(_metrics.CurrentCapital - _metrics.InitialCapital):F2}");
        Console.WriteLine($"   Win Rate: {_metrics.WinRate:P2}");
        Console.WriteLine($"   Profit Factor: {_metrics.ProfitFactor:F2}");
        Console.WriteLine($"   Total trades: {_metrics.WinningTrades + _metrics.LosingTrades}");
        Console.WriteLine($"   Ganadoras: {_metrics.WinningTrades}");
        Console.WriteLine($"   Perdedoras: {_metrics.LosingTrades}");
    }
}

/// <summary>
/// Configuración del bot Atlas
/// </summary>
public class AtlasConfiguration
{
    public double InitialCapital { get; set; } = 10000;
    public double MaxRiskPerTrade { get; set; } = 0.02; // 2%
    public double MaxDailyDrawdown { get; set; } = 0.05; // 5%
    public double MaxPositions { get; set; } = 3;
    public double ATRMultiplier { get; set; } = 2.0;
    public double RiskRewardRatio { get; set; } = 2.0;
    public int MaxConsecutiveLosses { get; set; } = 3;
    public RecoveryMode RecoveryMode { get; set; } = RecoveryMode.ReduceSize;
    public double RangeTolerance { get; set; } = 0.02;
    public int MinBarsInRange { get; set; } = 10;
    public bool AvoidRangingMarkets { get; set; } = true;
}

/// <summary>
/// Datos de mercado requeridos por el bot
/// </summary>
public class MarketData
{
    public double CurrentPrice { get; set; }
    public double AccountBalance { get; set; }
    public int OpenPositions { get; set; }
    public double ATR { get; set; }
    public double[] RecentHighs { get; set; } = Array.Empty<double>();
    public double[] RecentLows { get; set; } = Array.Empty<double>();
    public double[] RecentCloses { get; set; } = Array.Empty<double>();
}

/// <summary>
/// Señal de trading generada por el bot
/// </summary>
public class TradeSignal
{
    public SignalType Type { get; set; }
    public double Size { get; set; }
    public double StopLoss { get; set; }
    public double TakeProfit { get; set; }
    public double Confidence { get; set; }
    public string Reason { get; set; } = string.Empty;
}
