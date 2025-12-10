using SkyCore.Engines;
using SkyCore.Abstractions;
using Xunit;

namespace SkyCoreAtlas.Tests;

public class RiskEngineTests
{
    [Fact]
    public void CalculatePositionSize_WithValidInputs_ReturnsCorrectSize()
    {
        // Arrange
        var engine = new RiskEngine();
        var parameters = new EngineParameters();
        parameters.Set("MaxRiskPerTrade", 0.02); // 2%
        engine.Initialize(parameters);

        double accountBalance = 10000;
        double stopLossDistance = 50;

        // Act
        var positionSize = engine.CalculatePositionSize(accountBalance, stopLossDistance);

        // Assert
        // Risk amount = 10000 * 0.02 = 200
        // Position size = 200 / 50 = 4
        Assert.Equal(4.0, positionSize);
    }

    [Fact]
    public void CanOpenPosition_WhenDrawdownExceeded_ReturnsFalse()
    {
        // Arrange
        var engine = new RiskEngine();
        var parameters = new EngineParameters();
        parameters.Set("MaxDailyDrawdown", 0.05); // 5%
        engine.Initialize(parameters);

        var metrics = new TradingMetrics
        {
            CurrentDrawdown = 0.06, // 6% - excede el límite
            MaxDrawdown = 0.05
        };

        // Act
        var canOpen = engine.CanOpenPosition(metrics, currentPositions: 0);

        // Assert
        Assert.False(canOpen);
    }

    [Fact]
    public void CanOpenPosition_WhenMaxPositionsReached_ReturnsFalse()
    {
        // Arrange
        var engine = new RiskEngine();
        var parameters = new EngineParameters();
        parameters.Set("MaxPositions", 3.0);
        engine.Initialize(parameters);

        var metrics = new TradingMetrics
        {
            CurrentDrawdown = 0.01 // Drawdown OK
        };

        // Act
        var canOpen = engine.CanOpenPosition(metrics, currentPositions: 3);

        // Assert
        Assert.False(canOpen);
    }

    [Fact]
    public void CanOpenPosition_WhenAllConditionsMet_ReturnsTrue()
    {
        // Arrange
        var engine = new RiskEngine();
        var parameters = new EngineParameters();
        parameters.Set("MaxDailyDrawdown", 0.05);
        parameters.Set("MaxPositions", 3.0);
        engine.Initialize(parameters);

        var metrics = new TradingMetrics
        {
            CurrentDrawdown = 0.02, // OK
            MaxDrawdown = 0.05
        };

        // Act
        var canOpen = engine.CanOpenPosition(metrics, currentPositions: 1);

        // Assert
        Assert.True(canOpen);
    }

    [Fact]
    public void CalculateStopLoss_ForLongPosition_ReturnsCorrectValue()
    {
        // Arrange
        var engine = new RiskEngine();
        var parameters = new EngineParameters();
        parameters.Set("ATRMultiplier", 2.0);
        engine.Initialize(parameters);

        double entryPrice = 1.2000;
        double atr = 0.0010;
        bool isLong = true;

        // Act
        var stopLoss = engine.CalculateStopLoss(entryPrice, isLong, atr);

        // Assert
        // Stop distance = 0.0010 * 2.0 = 0.0020
        // Stop loss = 1.2000 - 0.0020 = 1.1980
        Assert.Equal(1.1980, stopLoss, precision: 4);
    }

    [Fact]
    public void CalculateStopLoss_ForShortPosition_ReturnsCorrectValue()
    {
        // Arrange
        var engine = new RiskEngine();
        var parameters = new EngineParameters();
        parameters.Set("ATRMultiplier", 2.0);
        engine.Initialize(parameters);

        double entryPrice = 1.2000;
        double atr = 0.0010;
        bool isLong = false;

        // Act
        var stopLoss = engine.CalculateStopLoss(entryPrice, isLong, atr);

        // Assert
        // Stop distance = 0.0010 * 2.0 = 0.0020
        // Stop loss = 1.2000 + 0.0020 = 1.2020
        Assert.Equal(1.2020, stopLoss, precision: 4);
    }

    [Fact]
    public void CalculateTakeProfit_WithRiskRewardRatio2_ReturnsCorrectValue()
    {
        // Arrange
        var engine = new RiskEngine();
        var parameters = new EngineParameters();
        parameters.Set("RiskRewardRatio", 2.0);
        engine.Initialize(parameters);

        double entryPrice = 1.2000;
        double stopLoss = 1.1980; // 20 pips de riesgo
        bool isLong = true;

        // Act
        var takeProfit = engine.CalculateTakeProfit(entryPrice, stopLoss, isLong);

        // Assert
        // Stop distance = |1.2000 - 1.1980| = 0.0020
        // Profit distance = 0.0020 * 2.0 = 0.0040
        // Take profit = 1.2000 + 0.0040 = 1.2040
        Assert.Equal(1.2040, takeProfit, precision: 4);
    }

    [Fact]
    public void Validate_WhenNotInitialized_ReturnsFalse()
    {
        // Arrange
        var engine = new RiskEngine();

        // Act
        var isValid = engine.Validate();

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void Validate_WhenInitialized_ReturnsTrue()
    {
        // Arrange
        var engine = new RiskEngine();
        var parameters = new EngineParameters();
        engine.Initialize(parameters);

        // Act
        var isValid = engine.Validate();

        // Assert
        Assert.True(isValid);
    }
}
