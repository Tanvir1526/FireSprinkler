using FireSprinklerDesign.Domain.ValueObjects;
using FireSprinklerDesign.Infrastructure.Services;
using FluentAssertions;
using Xunit;

namespace FireSprinklerDesign.Tests.Services;

public class GeometryServiceTests
{
    private readonly GeometryService _sut = new();

    [Fact]
    public void IsPointInsidePolygon_PointInside_ReturnsTrue()
    {
        // Arrange
        var polygon = new List<Point3D>
        {
            new(0, 0, 0),
            new(10, 0, 0),
            new(10, 10, 0),
            new(0, 10, 0)
        };
        var point = new Point3D(5, 5, 0);

        // Act
        var result = _sut.IsPointInsidePolygon(point, polygon);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsPointInsidePolygon_PointOutside_ReturnsFalse()
    {
        // Arrange
        var polygon = new List<Point3D>
        {
            new(0, 0, 0),
            new(10, 0, 0),
            new(10, 10, 0),
            new(0, 10, 0)
        };
        var point = new Point3D(15, 15, 0);

        // Act
        var result = _sut.IsPointInsidePolygon(point, polygon);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CalculatePolygonArea_Square_CalculatesCorrectly()
    {
        // Arrange
        var polygon = new List<Point3D>
        {
            new(0, 0, 0),
            new(10, 0, 0),
            new(10, 10, 0),
            new(0, 10, 0)
        };

        // Act
        var area = _sut.CalculatePolygonArea(polygon);

        // Assert
        area.Should().BeApproximately(100, 1e-10);
    }

    [Fact]
    public void GetBoundingBox_ReturnsCorrectBounds()
    {
        // Arrange
        var polygon = new List<Point3D>
        {
            new(5, 3, 1),
            new(15, 8, 2),
            new(10, 12, 3),
            new(2, 7, 4)
        };

        // Act
        var (min, max) = _sut.GetBoundingBox(polygon);

        // Assert
        min.X.Should().BeApproximately(2, 1e-10);
        min.Y.Should().BeApproximately(3, 1e-10);
        max.X.Should().BeApproximately(15, 1e-10);
        max.Y.Should().BeApproximately(12, 1e-10);
    }

    [Fact]
    public void InsetPolygon_Square_CreatesValidInset()
    {
        // Arrange
        var polygon = new List<Point3D>
        {
            new(0, 0, 0),
            new(100, 0, 0),
            new(100, 100, 0),
            new(0, 100, 0)
        };

        // Act
        var inset = _sut.InsetPolygon(polygon, 10);

        // Assert
        inset.Should().HaveCount(4);
        _sut.CalculatePolygonArea(inset).Should().BeGreaterThan(_sut.CalculatePolygonArea(polygon));
    }
}