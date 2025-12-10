# 🚀 Quick Start - SkyBot Universe

## 1️⃣ Primeros Pasos (5 minutos)

### Verificar Instalación

```bash
# Verificar .NET instalado
dotnet --version
# Debe mostrar: 8.0.x o superior
```

### Compilar el Proyecto

**Opción A: Usando el script** (Windows)
```bash
build.bat
```

**Opción B: Comando directo**
```bash
dotnet build SkyBot.sln
```

### Ejecutar Tests

```bash
dotnet test
```

Deberías ver:
```
Passed!  - Failed: 0, Passed: 17, Skipped: 0
```

---

## 2️⃣ Entender la Estructura (10 minutos)

### Archivos Clave

| Archivo | Descripción |
|---------|-------------|
| `README.md` | Documentación principal |
| `docs/architecture.md` | Arquitectura detallada |
| `src/Core/SkyCore.Engines/` | Motores reutilizables |
| `src/Robots/SkyCoreAtlas/SkyCoreAtlas.Core/` | Lógica del bot Atlas |
| `src/Robots/SkyCoreAtlas/SkyCoreAtlas.cBot/README_CTRADER.md` | Guía de cTrader |

### Concepto Principal

```
🚢 Bot = Nave espacial
⚙️ Engine = Módulo de la nave
🔧 Parameters = Configuración del módulo
📊 Metrics = Estado del combate
```

---

## 3️⃣ Desplegar en cTrader (30 minutos)

### Paso 1: Compilar en Release

```bash
dotnet build -c Release
```

### Paso 2: Localizar DLLs

Las DLLs están en:
```
src/Robots/SkyCoreAtlas/SkyCoreAtlas.Core/bin/Release/net8.0/SkyCoreAtlas.Core.dll
src/Core/SkyCore.Engines/bin/Release/net8.0/SkyCore.Engines.dll
src/Core/SkyCore.Abstractions/bin/Release/net8.0/SkyCore.Abstractions.dll
src/Core/SkyCore.Common/bin/Release/net8.0/SkyCore.Common.dll
```

### Paso 3: Copiar a cTrader

Crear carpeta en cTrader:
```
%USERPROFILE%\Documents\cAlgo\Sources\Robots\SkyCoreAtlas\
```

Copiar las 4 DLLs a esa carpeta.

### Paso 4: Crear el cBot en cTrader

Ver código de ejemplo en:
```
src/Robots/SkyCoreAtlas/SkyCoreAtlas.cBot/SkyCoreAtlasBot.cs
```

1. Abrir cTrader
2. Automate → cBots → Nuevo
3. Pegar el código del ejemplo
4. Agregar referencias a las 4 DLLs
5. Compilar

### Paso 5: Implementar tu Estrategia

**IMPORTANTE**: El `SignalEngine` base NO tiene lógica de señales. Debes crear tu propia implementación:

```csharp
public class MyAtlasSignal : SignalEngine
{
    public override SignalResult AnalyzeEntry()
    {
        // TU LÓGICA AQUÍ
        // Ejemplo: Cruce de medias, RSI, MACD, etc.

        return new SignalResult
        {
            HasSignal = /* tu condición */,
            SignalType = SignalType.Buy, // o Sell
            Confidence = 0.85,
            Reason = "Tu razón aquí"
        };
    }
}
```

Luego en `AtlasBotCore.cs`:
```csharp
_signalEngine = new MyAtlasSignal(); // En lugar de new SignalEngine()
```

---

## 4️⃣ Modificar y Extender (Variable)

### Cambiar Parámetros de Riesgo

Editar en `SkyCoreAtlas.Core/AtlasBotCore.cs`:

```csharp
var config = new AtlasConfiguration
{
    MaxRiskPerTrade = 0.01,      // 1% en lugar de 2%
    MaxDailyDrawdown = 0.03,     // 3% en lugar de 5%
    MaxPositions = 5,            // 5 en lugar de 3
    RiskRewardRatio = 3.0        // 1:3 en lugar de 1:2
};
```

### Agregar un Nuevo Motor

```csharp
// 1. Crear archivo en src/Core/SkyCore.Engines/
public class MyNewEngine : IEngine
{
    public string Name => "MyNewEngine";
    public string Version => "1.0.0";
    public bool IsEnabled { get; set; } = true;

    public void Initialize(EngineParameters parameters) { }
    public bool Validate() => true;

    // Tus métodos aquí
}

// 2. Registrar en AtlasBotCore
RegisterEngine(new MyNewEngine());
```

---

## 5️⃣ Backtesting y Optimización

### En cTrader

1. Seleccionar bot `SkyCoreAtlasBot`
2. Configurar parámetros
3. Elegir par de divisas y timeframe
4. Seleccionar rango de fechas
5. Ejecutar backtest
6. Analizar:
   - Win Rate > 50%
   - Profit Factor > 1.5
   - Drawdown < 20%

### Métricas a Vigilar

| Métrica | Bueno | Aceptable | Malo |
|---------|-------|-----------|------|
| Win Rate | >60% | 50-60% | <50% |
| Profit Factor | >2.0 | 1.5-2.0 | <1.5 |
| Drawdown | <10% | 10-20% | >20% |
| Sharpe Ratio | >1.5 | 1.0-1.5 | <1.0 |

---

## 6️⃣ Solución de Problemas

### Error: "DLL not found"
```bash
# Recompilar
dotnet clean
dotnet build -c Release

# Verificar que las DLLs existan
dir src/Robots/SkyCoreAtlas/SkyCoreAtlas.Core/bin/Release/net8.0/
```

### Error: "SignalEngine no genera señales"
```
✅ NORMAL - Debes implementar tu propia lógica en AnalyzeEntry()
Ver sección "Implementar tu Estrategia" arriba
```

### Tests fallan
```bash
# Ver detalles del error
dotnet test --logger "console;verbosity=detailed"

# Ejecutar solo tests específicos
dotnet test --filter "FullyQualifiedName~RiskEngine"
```

---

## 7️⃣ Próximos Pasos

1. ✅ **Ahora**: Implementa tu `SignalEngine` personalizado
2. ⏭️ **Después**: Backtestea con datos históricos
3. 🎯 **Luego**: Optimiza parámetros (ATR multiplier, risk/reward, etc.)
4. 🧪 **Antes de live**: Paper trading en cuenta demo
5. 🚀 **Finalmente**: Deploy con capital real (mínimo al principio)

---

## 📚 Recursos Adicionales

- [README.md](README.md) - Documentación completa
- [architecture.md](docs/architecture.md) - Arquitectura técnica
- [README_CTRADER.md](src/Robots/SkyCoreAtlas/SkyCoreAtlas.cBot/README_CTRADER.md) - Guía detallada de cTrader

---

## ⚡ Comandos Útiles

```bash
# Compilar
dotnet build

# Compilar en Release
dotnet build -c Release

# Tests
dotnet test

# Limpiar + rebuild
dotnet clean && dotnet build

# Ver estructura de la solución
dotnet sln list

# Agregar nuevo proyecto
dotnet new classlib -n MiProyecto
dotnet sln add MiProyecto/MiProyecto.csproj
```

---

## ⚠️ Checklist Antes de Live Trading

- [ ] Backtest completado con >100 operaciones
- [ ] Win rate >50%
- [ ] Profit factor >1.5
- [ ] Drawdown máximo <20%
- [ ] Paper trading exitoso por 1+ semana
- [ ] Entiendes cada parámetro de configuración
- [ ] Tienes un plan de gestión de riesgo
- [ ] Empiezas con capital mínimo
- [ ] Monitoring configurado
- [ ] Plan de salida definido

---

**¡Listo para construir tu primera nave del SkyBot Universe! 🚀**
