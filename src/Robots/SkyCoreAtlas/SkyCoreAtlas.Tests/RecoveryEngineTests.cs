using SkyCore.Engines;
using SkyCore.Abstractions;
using Xunit;

namespace SkyCoreAtlas.Tests;

public class RecoveryEngineTests
{
    [Fact]
    public void ShouldActivateRecovery_WhenConsecutiveLossesExceeded_ReturnsTrue()
    {
        // Arrange
        var engine = new RecoveryEngine();
        var parameters = new EngineParameters();
        parameters.Set("MaxConsecutiveLosses", 3);
        engine.Initialize(parameters);

        var metrics = new TradingMetrics
        {
            StreakType = -1, // Racha perdedora
            CurrentStreak = 3 // 3 pérdidas consecutivas
        };

        // Act
        var shouldActivate = engine.ShouldActivateRecovery(metrics);

        // Assert
        Assert.True(shouldActivate);
    }

    [Fact]
    public void ShouldActivateRecovery_WhenHighDrawdown_ReturnsTrue()
    {
        // Arrange
        var engine = new RecoveryEngine();
        var parameters = new EngineParameters();
        parameters.Set("MaxConsecutiveLosses", 3);
        engine.Initialize(parameters);

        var metrics = new TradingMetrics
        {
            CurrentDrawdown = 0.04, // 4%
            MaxDrawdown = 0.05,     // Máximo 5%
            StreakType = 0
        };

        // Act
        var shouldActivate = engine.ShouldActivateRecovery(metrics);

        // Assert
        // 4% >= 5% * 0.7 (3.5%) → true
        Assert.True(shouldActivate);
    }

    [Fact]
    public void ShouldActivateRecovery_WhenNormalConditions_ReturnsFalse()
    {
        // Arrange
        var engine = new RecoveryEngine();
        var parameters = new EngineParameters();
        parameters.Set("MaxConsecutiveLosses", 3);
        engine.Initialize(parameters);

        var metrics = new TradingMetrics
        {
            StreakType = 1,       // Racha ganadora
            CurrentStreak = 2,
            CurrentDrawdown = 0.01, // 1% - bajo
            MaxDrawdown = 0.05
        };

        // Act
        var shouldActivate = engine.ShouldActivateRecovery(metrics);

        // Assert
        Assert.False(shouldActivate);
    }

    [Fact]
    public void GetPositionSizeMultiplier_InReduceSizeMode_ReturnsHalf()
    {
        // Arrange
        var engine = new RecoveryEngine();
        var parameters = new EngineParameters();
        parameters.Set("MaxConsecutiveLosses", 2);
        parameters.Set("RecoveryMode", RecoveryMode.ReduceSize);
        engine.Initialize(parameters);

        var metrics = new TradingMetrics
        {
            StreakType = -1,
            CurrentStreak = 3 // Activar recuperación
        };

        // Act
        var multiplier = engine.GetPositionSizeMultiplier(metrics);

        // Assert
        Assert.Equal(0.5, multiplier); // 50% del tamaño original
    }

    [Fact]
    public void GetPositionSizeMultiplier_InStopMode_ReturnsZero()
    {
        // Arrange
        var engine = new RecoveryEngine();
        var parameters = new EngineParameters();
        parameters.Set("MaxConsecutiveLosses", 2);
        parameters.Set("RecoveryMode", RecoveryMode.Stop);
        engine.Initialize(parameters);

        var metrics = new TradingMetrics
        {
            StreakType = -1,
            CurrentStreak = 3 // Activar recuperación
        };

        // Act
        var multiplier = engine.GetPositionSizeMultiplier(metrics);

        // Assert
        Assert.Equal(0.0, multiplier); // Detener trading
    }

    [Fact]
    public void GetPositionSizeMultiplier_InConservativeMode_ReturnsQuarter()
    {
        // Arrange
        var engine = new RecoveryEngine();
        var parameters = new EngineParameters();
        parameters.Set("MaxConsecutiveLosses", 2);
        parameters.Set("RecoveryMode", RecoveryMode.Conservative);
        engine.Initialize(parameters);

        var metrics = new TradingMetrics
        {
            StreakType = -1,
            CurrentStreak = 3 // Activar recuperación
        };

        // Act
        var multiplier = engine.GetPositionSizeMultiplier(metrics);

        // Assert
        Assert.Equal(0.25, multiplier); // 25% del tamaño original
    }

    [Fact]
    public void GetPositionSizeMultiplier_WhenNotInRecovery_ReturnsOne()
    {
        // Arrange
        var engine = new RecoveryEngine();
        var parameters = new EngineParameters();
        parameters.Set("MaxConsecutiveLosses", 5);
        engine.Initialize(parameters);

        var metrics = new TradingMetrics
        {
            StreakType = 1,      // Racha ganadora
            CurrentStreak = 2,
            CurrentDrawdown = 0.01,
            MaxDrawdown = 0.05   // 5% - el drawdown actual está muy por debajo
        };

        // Act
        var multiplier = engine.GetPositionSizeMultiplier(metrics);

        // Assert
        Assert.Equal(1.0, multiplier); // Tamaño normal
    }

    [Fact]
    public void GetStatus_ReturnsCorrectInformation()
    {
        // Arrange
        var engine = new RecoveryEngine();
        var parameters = new EngineParameters();
        parameters.Set("MaxConsecutiveLosses", 3);
        parameters.Set("RecoveryMode", RecoveryMode.ReduceSize);
        engine.Initialize(parameters);

        var metrics = new TradingMetrics
        {
            StreakType = -1,
            CurrentStreak = 4 // Activar recuperación
        };

        // Act
        var status = engine.GetStatus(metrics);

        // Assert
        Assert.True(status.IsActive);
        Assert.Equal(RecoveryMode.ReduceSize, status.Mode);
        Assert.Contains("Racha perdedora", status.Reason);
        Assert.Equal(0.5, status.PositionSizeMultiplier);
    }
}
