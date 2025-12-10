# SkyCoreAtlas - Integración con cTrader

## 📦 Instalación

### Opción 1: Desarrollo Local (Recomendado)

1. **Compilar el proyecto**:
   ```bash
   dotnet build ../../SkyBot.sln -c Release
   ```

2. **Localizar las DLLs compiladas** en:
   ```
   src/Robots/SkyCoreAtlas/SkyCoreAtlas.Core/bin/Release/net8.0/
   src/Core/SkyCore.Engines/bin/Release/net8.0/
   src/Core/SkyCore.Abstractions/bin/Release/net8.0/
   src/Core/SkyCore.Common/bin/Release/net8.0/
   ```

3. **Copiar las DLLs** a la carpeta de referencias de cTrader:
   ```
   %USERPROFILE%\Documents\cAlgo\Sources\Robots\SkyCoreAtlas\
   ```

4. **Crear el archivo del cBot en cTrader**:
   - Abrir cTrader
   - Ir a Automate → cBots → Nuevo
   - Copiar el código del ejemplo (ver `SkyCoreAtlasBot.cs`)
   - Agregar referencias a las DLLs copiadas

### Opción 2: Código Único (Sin DLLs)

Si prefieres un solo archivo sin dependencias externas:

1. Combinar todo el código en un solo archivo `.cs`
2. Copiar directamente a cTrader
3. No requiere DLLs externas

**Nota**: Esta opción sacrifica la modularidad pero es más simple para backtesting rápido.

## 🎮 Parámetros Configurables

### Gestión de Riesgo
- **Initial Capital**: Capital inicial en cuenta
- **Max Risk Per Trade**: % máximo de riesgo por operación (default: 2%)
- **Max Daily Drawdown**: % máximo de drawdown diario (default: 5%)
- **Max Positions**: Número máximo de posiciones simultáneas (default: 3)

### Configuración de Stop Loss/Take Profit
- **ATR Multiplier**: Multiplicador del ATR para stop loss (default: 2.0)
- **Risk Reward Ratio**: Ratio riesgo/beneficio (default: 2.0)

### Sistema de Recuperación
- **Max Consecutive Losses**: Pérdidas consecutivas antes de activar recuperación (default: 3)
- **Recovery Mode**: Modo de recuperación (ReduceSize/Stop/Conservative)

### Detector de Rangos
- **Range Tolerance**: Tolerancia para detectar rangos (default: 2%)
- **Min Bars In Range**: Barras mínimas para confirmar rango (default: 10)
- **Avoid Ranging Markets**: Evitar operar en mercados laterales (default: true)

## 🔧 Personalización

### Crear tu propio SignalEngine

El `SignalEngine` base no tiene lógica de señales. Debes crear tu propia implementación:

```csharp
using SkyCore.Engines;
using SkyCore.Abstractions;

public class AtlasSignalEngine : SignalEngine
{
    public override SignalResult AnalyzeEntry()
    {
        // Tu lógica aquí
        // Ejemplo: Cruce de medias móviles, RSI, etc.

        return new SignalResult
        {
            HasSignal = true,
            SignalType = SignalType.Buy,
            Confidence = 0.85,
            Reason = "Cruce alcista de medias móviles"
        };
    }

    public override SignalResult AnalyzeExit()
    {
        // Lógica de salida
        return new SignalResult
        {
            HasSignal = false,
            SignalType = SignalType.None,
            Confidence = 0
        };
    }
}
```

Luego reemplaza el `SignalEngine` en `AtlasBotCore`:

```csharp
_signalEngine = new AtlasSignalEngine(); // En lugar de new SignalEngine()
```

## 🧪 Backtesting

1. En cTrader, selecciona el bot `SkyCoreAtlasBot`
2. Configura los parámetros
3. Selecciona el rango de fechas
4. Ejecuta el backtest
5. Analiza las estadísticas generadas

## 📊 Métricas Monitoreadas

El bot automáticamente rastrea:
- ✅ Drawdown actual vs máximo permitido
- ✅ Rachas ganadoras/perdedoras
- ✅ Win rate
- ✅ Profit factor
- ✅ Capital utilizado
- ✅ Total de operaciones

## 🚀 Próximos Pasos

1. **Implementa tu SignalEngine**: Define tu estrategia de entrada/salida
2. **Backtestea**: Prueba con datos históricos
3. **Optimiza**: Ajusta parámetros
4. **Paper Trading**: Prueba en cuenta demo
5. **Live**: Despliega en cuenta real (con precaución)

## ⚠️ Advertencias

- Este es un framework base - **debes implementar tu lógica de señales**
- Siempre backtestea antes de usar en real
- Empieza con cuenta demo
- El trading tiene riesgo de pérdida de capital
- No hay garantía de rentabilidad

## 🔗 Estructura del Proyecto

```
SkyCoreAtlas/
├── SkyCoreAtlas.Core/     → Lógica de negocio (independiente de cTrader)
├── SkyCoreAtlas.cBot/     → Integración con cTrader (este proyecto)
└── SkyCoreAtlas.Tests/    → Tests unitarios
```

Esta separación permite:
- ✅ Testear lógica sin cTrader
- ✅ Migrar a otras plataformas fácilmente
- ✅ Desarrollar offline
- ✅ Reutilizar código entre bots
