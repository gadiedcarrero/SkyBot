namespace SkyCore.Abstractions;

/// <summary>
/// Interfaz base para todos los bots (naves)
/// Representa la "CPU central" que coordina todos los módulos
/// </summary>
public interface IBotCore
{
    /// <summary>
    /// Nombre del bot (ej: Atlas, Hydra, Raptor, Sentinel)
    /// </summary>
    string BotName { get; }

    /// <summary>
    /// Versión del bot
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Registra un motor/módulo en el bot
    /// </summary>
    void RegisterEngine(IEngine engine);

    /// <summary>
    /// Obtiene un motor registrado por su tipo
    /// </summary>
    T? GetEngine<T>() where T : class, IEngine;

    /// <summary>
    /// Inicia el bot
    /// </summary>
    void Start();

    /// <summary>
    /// Detiene el bot
    /// </summary>
    void Stop();
}
