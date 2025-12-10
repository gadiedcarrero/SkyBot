namespace SkyCore.Abstractions;

/// <summary>
/// Interfaz base para todos los motores/módulos del SkyBot Universe
/// Representa un "módulo" que puede instalarse en una "nave" (bot)
/// </summary>
public interface IEngine
{
    /// <summary>
    /// Nombre del módulo/motor
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Versión del módulo
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Indica si el motor está habilitado
    /// </summary>
    bool IsEnabled { get; set; }

    /// <summary>
    /// Inicializa el motor con parámetros específicos
    /// </summary>
    void Initialize(EngineParameters parameters);

    /// <summary>
    /// Valida que el motor esté configurado correctamente
    /// </summary>
    bool Validate();
}
