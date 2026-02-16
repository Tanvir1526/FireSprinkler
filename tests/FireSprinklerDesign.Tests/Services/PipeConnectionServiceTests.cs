using FireSprinklerDesign.Domain.Entities;
using FireSprinklerDesign.Domain.Services;
using FireSprinklerDesign.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FireSprinklerDesign.Tests.Services;

public class PipeConnectionServiceTests
{
    private readonly PipeConnectionService _sut = new();

    [Fact]
    public void ConnectSprinklersToPipes_WithValidInputs_ConnectsAllSprinklers()
    {
        // Arrange
        var sprinklers = new List<Sprinkler>
        {
            new(1, new Point3D(5, 5, 0)),
            new(2, new Point3D(15, 5, 0))
        };
        var pipes = new List<Pipe>
        {
            new(1, new Point3D(0, 0, 0), new Point3D(20, 0, 0))
        };

        // Act
        _sut.ConnectSprinklersToPipes(sprinklers, pipes);

        // Assert
        sprinklers.Should().AllSatisfy(s => s.IsConnected.Should().BeTrue());
    }

    [Fact]
    public void ConnectSprinklersToPipes_ConnectsToNearestPipe()
    {
        // Arrange
        var sprinkler = new Sprinkler(1, new Point3D(5, 2, 0));
        var sprinklers = new List<Sprinkler> { sprinkler };
        var pipes = new List<Pipe>
        {
            new(1, new Point3D(0, 0, 0), new Point3D(10, 0, 0)),  // Distance: 2
            new(2, new Point3D(0, 10, 0), new Point3D(10, 10, 0)) // Distance: 8
        };

        // Act
        _sut.ConnectSprinklersToPipes(sprinklers, pipes);

        // Assert
        sprinkler.ConnectedPipe!.Id.Should().Be(1);
    }

    [Fact]
    public void ConnectSprinklersToPipes_SetsCorrectConnectionPoint()
    {
        // Arrange
        var sprinkler = new Sprinkler(1, new Point3D(5, 3, 0));
        var sprinklers = new List<Sprinkler> { sprinkler };
        var pipes = new List<Pipe>
        {
            new(1, new Point3D(0, 0, 0), new Point3D(10, 0, 0))
        };

        // Act
        _sut.ConnectSprinklersToPipes(sprinklers, pipes);

        // Assert
        sprinkler.ConnectionPoint?.X.Should().BeApproximately(5, 1e-10);
        sprinkler.ConnectionPoint?.Y.Should().BeApproximately(0, 1e-10);
    }

    [Fact]
    public void ConnectSprinklersToPipes_SetsCorrectConnectionDistance()
    {
        // Arrange
        var sprinkler = new Sprinkler(1, new Point3D(5, 4, 0));
        var sprinklers = new List<Sprinkler> { sprinkler };
        var pipes = new List<Pipe>
        {
            new(1, new Point3D(0, 0, 0), new Point3D(10, 0, 0))
        };

        // Act
        _sut.ConnectSprinklersToPipes(sprinklers, pipes);

        // Assert
        sprinkler.ConnectionDistance.Should().BeApproximately(4, 1e-10);
    }

    [Fact]
    public void ConnectSprinklersToPipes_NullSprinklers_ThrowsArgumentNullException()
    {
        // Arrange
        var pipes = new List<Pipe> { new(1, new Point3D(0, 0, 0), new Point3D(10, 0, 0)) };

        // Act
        var act = () => _sut.ConnectSprinklersToPipes(null!, pipes);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("sprinklers");
    }

    [Fact]
    public void ConnectSprinklersToPipes_NullPipes_ThrowsArgumentNullException()
    {
        // Arrange
        var sprinklers = new List<Sprinkler> { new(1, new Point3D(5, 5, 0)) };

        // Act
        var act = () => _sut.ConnectSprinklersToPipes(sprinklers, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("pipes");
    }

    [Fact]
    public void ConnectSprinklersToPipes_EmptyPipes_ThrowsArgumentException()
    {
        // Arrange
        var sprinklers = new List<Sprinkler> { new(1, new Point3D(5, 5, 0)) };
        var pipes = new List<Pipe>();

        // Act
        var act = () => _sut.ConnectSprinklersToPipes(sprinklers, pipes);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("pipes")
            .WithMessage("*At least one pipe must be provided*");
    }

    [Fact]
    public void ConnectSprinklersToPipes_EmptySprinklers_CompletesWithoutError()
    {
        // Arrange
        var sprinklers = new List<Sprinkler>();
        var pipes = new List<Pipe> { new(1, new Point3D(0, 0, 0), new Point3D(10, 0, 0)) };

        // Act
        var act = () => _sut.ConnectSprinklersToPipes(sprinklers, pipes);

        // Assert
        act.Should().NotThrow();
    }
}