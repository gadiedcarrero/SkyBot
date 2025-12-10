namespace SkyCore.Abstractions;

/// <summary>
/// Parámetros de configuración para motores
/// Representa los "slots" configurables de cada módulo
/// </summary>
public class EngineParameters
{
    private readonly Dictionary<string, object> _parameters = new();

    public void Set<T>(string key, T value)
    {
        _parameters[key] = value!;
    }

    public T Get<T>(string key, T defaultValue = default!)
    {
        if (_parameters.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return defaultValue;
    }

    public bool TryGet<T>(string key, out T value)
    {
        if (_parameters.TryGetValue(key, out var objValue) && objValue is T typedValue)
        {
            value = typedValue;
            return true;
        }
        value = default!;
        return false;
    }
}
