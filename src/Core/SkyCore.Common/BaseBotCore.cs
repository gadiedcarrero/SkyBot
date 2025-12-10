using SkyCore.Abstractions;

namespace SkyCore.Common;

/// <summary>
/// Implementación base para todos los bots
/// Proporciona funcionalidad común de gestión de motores
/// </summary>
public abstract class BaseBotCore : IBotCore
{
    protected readonly Dictionary<Type, IEngine> _engines = new();
    protected readonly TradingMetrics _metrics = new();

    public abstract string BotName { get; }
    public abstract string Version { get; }

    public TradingMetrics Metrics => _metrics;

    public void RegisterEngine(IEngine engine)
    {
        var engineType = engine.GetType();

        // Buscar todas las interfaces IEngine que implementa
        var engineInterfaces = engineType
            .GetInterfaces()
            .Where(i => i != typeof(IEngine) && typeof(IEngine).IsAssignableFrom(i));

        // Registrar por tipo concreto
        _engines[engineType] = engine;

        // Registrar también por interfaces específicas
        foreach (var iface in engineInterfaces)
        {
            _engines[iface] = engine;
        }

        OnEngineRegistered(engine);
    }

    public T? GetEngine<T>() where T : class, IEngine
    {
        var requestedType = typeof(T);

        if (_engines.TryGetValue(requestedType, out var engine))
        {
            return engine as T;
        }

        // Buscar por tipo derivado
        var derivedEngine = _engines.Values
            .FirstOrDefault(e => e is T);

        return derivedEngine as T;
    }

    public virtual void Start()
    {
        ValidateEngines();
        OnStart();
    }

    public virtual void Stop()
    {
        OnStop();
    }

    protected virtual void ValidateEngines()
    {
        foreach (var engine in _engines.Values)
        {
            if (!engine.Validate())
            {
                throw new InvalidOperationException(
                    $"Motor '{engine.Name}' no está configurado correctamente");
            }
        }
    }

    protected virtual void OnEngineRegistered(IEngine engine) { }
    protected virtual void OnStart() { }
    protected virtual void OnStop() { }

    /// <summary>
    /// Actualiza las métricas del bot
    /// </summary>
    protected void UpdateMetrics(double currentCapital, double currentDrawdown, int currentPositions)
    {
        _metrics.CurrentCapital = currentCapital;
        _metrics.CurrentDrawdown = currentDrawdown;
    }

    /// <summary>
    /// Registra una operación cerrada
    /// </summary>
    protected void RecordTrade(bool isWin, double profitOrLoss)
    {
        if (isWin)
        {
            _metrics.WinningTrades++;
            _metrics.TotalProfit += profitOrLoss;

            if (_metrics.StreakType == 1)
                _metrics.CurrentStreak++;
            else
            {
                _metrics.StreakType = 1;
                _metrics.CurrentStreak = 1;
            }
        }
        else
        {
            _metrics.LosingTrades++;
            _metrics.TotalLoss += Math.Abs(profitOrLoss);

            if (_metrics.StreakType == -1)
                _metrics.CurrentStreak++;
            else
            {
                _metrics.StreakType = -1;
                _metrics.CurrentStreak = 1;
            }
        }
    }
}
