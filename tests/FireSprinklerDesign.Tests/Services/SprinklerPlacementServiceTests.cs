using FireSprinklerDesign.Domain.Abstractions;
using FireSprinklerDesign.Domain.Entities;
using FireSprinklerDesign.Domain.Services;
using FireSprinklerDesign.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace FireSprinklerDesign.Tests.Services;

public class SprinklerPlacementServiceTests
{
    private readonly Mock<IGeometryService> _geometryServiceMock = new();
    private readonly SprinklerPlacementService _sut;

    public SprinklerPlacementServiceTests()
    {
        SetupDefaultMocks();
        _sut = new SprinklerPlacementService(_geometryServiceMock.Object);
    }

    private void SetupDefaultMocks()
    {
        var defaultInset = new List<Point3D>
        {
            new(5, 5, 10), new(95, 5, 10), new(95, 95, 10), new(5, 95, 10)
        };

        _geometryServiceMock
            .Setup(g => g.InsetPolygon(It.IsAny<IReadOnlyList<Point3D>>(), It.IsAny<double>()))
            .Returns(defaultInset);

        _geometryServiceMock
            .Setup(g => g.GetBoundingBox(It.IsAny<IReadOnlyList<Point3D>>()))
            .Returns((new Point3D(5, 5, 0), new Point3D(95, 95, 0)));

        _geometryServiceMock
            .Setup(g => g.IsPointInsidePolygon(It.IsAny<Point3D>(), It.IsAny<IReadOnlyList<Point3D>>()))
            .Returns(true);
    }

    [Fact]
    public void Constructor_NullGeometryService_ThrowsArgumentNullException()
    {
        // Act
        var act = () => new SprinklerPlacementService(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("geometryService");
    }

    [Fact]
    public void PlanSprinklers_NullRoom_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _sut.PlanSprinklers(null!, 1.0, 10.0);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("room");
    }

    [Fact]
    public void PlanSprinklers_ZeroWallOffset_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var room = CreateTestRoom();

        // Act
        var act = () => _sut.PlanSprinklers(room, 0, 10.0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("wallOffset");
    }

    [Fact]
    public void PlanSprinklers_NegativeWallOffset_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var room = CreateTestRoom();

        // Act
        var act = () => _sut.PlanSprinklers(room, -5, 10.0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("wallOffset");
    }

    [Fact]
    public void PlanSprinklers_ZeroSpacing_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var room = CreateTestRoom();

        // Act
        var act = () => _sut.PlanSprinklers(room, 1.0, 0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("spacing");
    }

    [Fact]
    public void PlanSprinklers_NegativeSpacing_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var room = CreateTestRoom();

        // Act
        var act = () => _sut.PlanSprinklers(room, 1.0, -10);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("spacing");
    }

    [Fact]
    public void PlanSprinklers_ValidInputs_CallsInsetPolygonWithCorrectParameters()
    {
        // Arrange
        var room = CreateTestRoom();
        const double wallOffset = 5.0;

        // Act
        _sut.PlanSprinklers(room, wallOffset, 10.0);

        // Assert
        _geometryServiceMock.Verify(
            g => g.InsetPolygon(room.CeilingCorners, wallOffset),
            Times.Once);
    }

    [Fact]
    public void PlanSprinklers_ValidInputs_ReturnsSprinklers()
    {
        // Arrange
        var room = CreateTestRoom();

        // Act
        var sprinklers = _sut.PlanSprinklers(room, 5.0, 30.0);

        // Assert
        sprinklers.Should().NotBeEmpty();
    }

    [Fact]
    public void PlanSprinklers_SprinklersHaveUniqueIds()
    {
        // Arrange
        var room = CreateTestRoom();

        // Act
        var sprinklers = _sut.PlanSprinklers(room, 5.0, 30.0);

        // Assert
        sprinklers.Select(s => s.Id).Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void PlanSprinklers_SprinklersAtCeilingHeight()
    {
        // Arrange
        var room = CreateTestRoom(ceilingHeight: 12.0);

        // Act
        var sprinklers = _sut.PlanSprinklers(room, 5.0, 30.0);

        // Assert
        sprinklers.Should().AllSatisfy(s => s.Position.Z.Should().BeApproximately(12.0, 1e-10));
    }

    [Fact]
    public void PlanSprinklers_InsetTooSmall_ReturnsEmptyList()
    {
        // Arrange
        var room = CreateTestRoom();
        _geometryServiceMock
            .Setup(g => g.InsetPolygon(It.IsAny<IReadOnlyList<Point3D>>(), It.IsAny<double>()))
            .Returns(new List<Point3D>()); // Empty inset

        // Act
        var sprinklers = _sut.PlanSprinklers(room, 5.0, 10.0);

        // Assert
        sprinklers.Should().BeEmpty();
    }

    [Fact]
    public void PlanSprinklers_PointOutsidePolygon_ExcludesPoint()
    {
        // Arrange
        var room = CreateTestRoom();
        _geometryServiceMock
            .Setup(g => g.IsPointInsidePolygon(It.IsAny<Point3D>(), It.IsAny<IReadOnlyList<Point3D>>()))
            .Returns(false);

        // Act
        var sprinklers = _sut.PlanSprinklers(room, 5.0, 30.0);

        // Assert
        sprinklers.Should().BeEmpty();
    }

    [Fact]
    public void PlanSprinklers_ChecksEachCandidatePoint()
    {
        // Arrange
        var room = CreateTestRoom();

        // Act
        _sut.PlanSprinklers(room, 5.0, 30.0);

        // Assert
        _geometryServiceMock.Verify(
            g => g.IsPointInsidePolygon(It.IsAny<Point3D>(), It.IsAny<IReadOnlyList<Point3D>>()),
            Times.AtLeastOnce);
    }

    private static Room CreateTestRoom(double ceilingHeight = 10.0)
    {
        var corners = new List<Point3D>
        {
            new(0, 0, ceilingHeight),
            new(100, 0, ceilingHeight),
            new(100, 100, ceilingHeight),
            new(0, 100, ceilingHeight)
        };
        return new Room(corners, "Test Room");
    }
}