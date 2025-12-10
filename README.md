# 🚀 SkyBot Universe

**Framework modular para bots de trading algorítmico**

Cada bot es una "nave" independiente con módulos intercambiables (motores) que comparten un lenguaje conceptual común.

---

## 🌌 Concepto del Universe

| Universo | Equivalente en Código |
|----------|----------------------|
| 🚢 Nave | Bot (Atlas, Hydra, Raptor, Sentinel) |
| ⚙️ Módulo | Clase/Engine (SignalEngine, RiskEngine, etc) |
| 🔧 Slot | Parámetros configurables |
| 💥 Daño | Drawdown |
| ⚔️ Combate | Rachas (winning/losing streaks) |
| ⛽ Combustible | Capital |

---

## 📦 Arquitectura del Proyecto

```
SkyBot/
├── src/
│   ├── Core/                           # Código compartido (motores reutilizables)
│   │   ├── SkyCore.Abstractions/      # Interfaces y contratos
│   │   ├── SkyCore.Common/            # Utilidades comunes
│   │   └── SkyCore.Engines/           # Motores del Universe
│   │       ├── SignalEngine           # 🔧 Motor - Genera señales
│   │       ├── RiskEngine             # 🛡️ Escudo - Protege capital
│   │       ├── RecoveryEngine         # ⚡ Reactor - Maneja rachas
│   │       └── HorizontalDetector     # 📡 Sensores - Detecta rangos
│   │
│   └── Robots/                         # Bots específicos (naves)
│       └── SkyCoreAtlas/              # Primera nave ✅
│           ├── SkyCoreAtlas.Core/     # Lógica de negocio
│           ├── SkyCoreAtlas.cBot/     # Integración cTrader
│           └── SkyCoreAtlas.Tests/    # Tests unitarios
│
├── docs/                              # Documentación
├── SkyBot.sln                         # Solución Visual Studio
└── README.md                          # Este archivo
```

---

## 🎯 Características Principales

### ✅ Modularidad Total
- Cada bot puede existir **sin backend**
- Código compartido reutilizable entre bots
- Fácil agregar nuevos bots (Hydra, Raptor, Sentinel)

### ✅ 100% Compatible con cTrader
- Arquitectura separada de la plataforma
- Fácil migración a otras plataformas
- Backtesting local sin dependencias

### ✅ Gestión Avanzada de Riesgo
- Control de drawdown automático
- Límites de posiciones simultáneas
- Cálculo dinámico de stop loss/take profit
- Sistema de recuperación ante rachas perdedoras

### ✅ Sistema de Recuperación Inteligente
- Detecta rachas perdedoras
- Reduce automáticamente el tamaño de posiciones
- Modos: ReduceSize, Stop, Conservative

### ✅ Detección de Mercados Laterales
- Identifica rangos horizontales
- Evita operar en mercados sin tendencia
- Detecta soportes y resistencias

---

## 🚀 Quick Start

### 1. Compilar el Proyecto

```bash
# Clonar o abrir el proyecto
cd D:\Work\SkyBot

# Compilar
dotnet build SkyBot.sln

# Ejecutar tests
dotnet test
```

### 2. Desplegar en cTrader

Ver instrucciones detalladas en:
```
src/Robots/SkyCoreAtlas/SkyCoreAtlas.cBot/README_CTRADER.md
```

**Resumen rápido**:
1. Compilar en modo Release: `dotnet build -c Release`
2. Copiar DLLs a cTrader
3. Crear cBot en cTrader con el código de ejemplo
4. Configurar parámetros y ejecutar

---

## 🔧 Motores Disponibles

### 1️⃣ SignalEngine (Motor)
- Genera señales de entrada/salida
- **Base abstracta** - implementa tu propia lógica
- Override `AnalyzeEntry()` y `AnalyzeExit()`

### 2️⃣ RiskEngine (Escudo)
- Calcula tamaño de posición basado en riesgo
- Gestiona límites de drawdown
- Calcula stop loss y take profit automáticamente
- Control de posiciones simultáneas

### 3️⃣ RecoveryEngine (Reactor)
- Detecta rachas perdedoras
- Activa modo recuperación automático
- Ajusta tamaño de posiciones dinámicamente
- Protege el capital en malas rachas

### 4️⃣ HorizontalDetector (Sensores)
- Detecta mercados en rango
- Identifica soportes y resistencias
- Calcula fuerza del rango
- Evita falsas señales en laterales

---

## 📊 Métricas Automáticas

El sistema rastrea automáticamente:

| Métrica | Descripción |
|---------|-------------|
| 💰 Capital | Balance actual vs inicial |
| 📉 Drawdown | Pérdida desde el pico |
| 🎯 Win Rate | % de operaciones ganadoras |
| 📈 Profit Factor | Ratio ganancia/pérdida |
| ⚔️ Rachas | Operaciones consecutivas ganadoras/perdedoras |
| 🔢 Total Trades | Contador de operaciones |

---

## 🛠️ Configuración

### Parámetros de SkyCoreAtlas

```csharp
var config = new AtlasConfiguration
{
    InitialCapital = 10000,           // Capital inicial
    MaxRiskPerTrade = 0.02,           // 2% riesgo por operación
    MaxDailyDrawdown = 0.05,          // 5% drawdown máximo
    MaxPositions = 3,                 // Máx posiciones simultáneas
    ATRMultiplier = 2.0,              // Multiplicador para stop loss
    RiskRewardRatio = 2.0,            // Ratio riesgo/beneficio
    MaxConsecutiveLosses = 3,         // Rachas antes de recuperación
    RecoveryMode = RecoveryMode.ReduceSize,  // Modo de recuperación
    RangeTolerance = 0.02,            // Tolerancia para rangos
    MinBarsInRange = 10,              // Barras mínimas en rango
    AvoidRangingMarkets = true        // Evitar mercados laterales
};
```

---

## 🎮 Uso Básico

### Crear un Bot Personalizado

```csharp
using SkyCoreAtlas.Core;

var config = new AtlasConfiguration
{
    InitialCapital = 10000,
    MaxRiskPerTrade = 0.02,
    MaxDailyDrawdown = 0.05
};

var bot = new AtlasBotCore(config);
bot.Start();

// En cada barra/tick
var marketData = new MarketData
{
    CurrentPrice = 1.2345,
    AccountBalance = 10000,
    OpenPositions = 0,
    ATR = 0.0010,
    RecentHighs = new[] { 1.24, 1.245, 1.25 },
    RecentLows = new[] { 1.23, 1.235, 1.24 },
    RecentCloses = new[] { 1.235, 1.24, 1.245 }
};

bot.ProcessBar(marketData);
```

### Crear un SignalEngine Personalizado

```csharp
using SkyCore.Engines;

public class MyCustomSignal : SignalEngine
{
    public override SignalResult AnalyzeEntry()
    {
        // Tu lógica de análisis
        bool shouldBuy = /* tu condición */;

        return new SignalResult
        {
            HasSignal = shouldBuy,
            SignalType = SignalType.Buy,
            Confidence = 0.85,
            Reason = "Cruce de medias móviles"
        };
    }
}
```

---

## 🔮 Roadmap Futuro

### 🚢 Próximas Naves

- [ ] **SkyCoreHydra** - Bot multi-estrategia
- [ ] **SkyCoreRaptor** - Bot de alta frecuencia
- [ ] **SkyCoreSentinel** - Bot defensivo/conservador

### 🌐 SkyBot Universe (Backend + Frontend)

En el futuro, un sistema web permitirá:
- 🎨 Configurar bots visualmente (arrastrar módulos)
- 📊 Dashboard con métricas en tiempo real
- 🔧 Activar/desactivar motores dinámicamente
- 💾 Guardar configuraciones como "blueprints"
- 🤖 Generar código de bots automáticamente

**Analogía**: "Construir una nave" seleccionando:
- Motor → SignalEngine
- Escudo → RiskEngine
- Reactor → RecoveryEngine
- Sensores → HorizontalDetector

---

## 🧪 Testing

```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar tests específicos
dotnet test --filter "FullyQualifiedName~AtlasBotCore"

# Con coverage
dotnet test /p:CollectCoverage=true
```

---

## 📖 Documentación Adicional

- [Arquitectura Detallada](docs/architecture.md) *(próximamente)*
- [Guía de cTrader](src/Robots/SkyCoreAtlas/SkyCoreAtlas.cBot/README_CTRADER.md)
- [API Reference](docs/api-reference.md) *(próximamente)*
- [Ejemplos](docs/examples.md) *(próximamente)*

---

## ⚠️ Advertencias

- **Este es un framework base** - debes implementar tu propia lógica de señales
- Siempre **backtestea** antes de usar en real
- Empieza con **cuenta demo**
- El trading tiene **riesgo de pérdida** de capital
- **No hay garantía** de rentabilidad

---

## 🤝 Contribuir

Este es un proyecto personal, pero las sugerencias son bienvenidas:

1. Fork del proyecto
2. Crear feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit cambios (`git commit -m 'Add AmazingFeature'`)
4. Push al branch (`git push origin feature/AmazingFeature`)
5. Abrir Pull Request

---

## 📝 Licencia

Este proyecto es de código abierto. Úsalo bajo tu propio riesgo.

---

## 🎯 Siguiente Paso

1. **Compila el proyecto**: `dotnet build`
2. **Implementa tu SignalEngine**: Define tu estrategia
3. **Backtestea en cTrader**: Prueba con datos históricos
4. **Optimiza**: Ajusta parámetros
5. **Paper Trading**: Demo primero
6. **Live**: Con precaución

---

**¡Bienvenido al SkyBot Universe! 🚀**

*"Cada bot es una nave. Cada módulo, un componente. Juntos, forman un ecosistema de trading algorítmico."*
