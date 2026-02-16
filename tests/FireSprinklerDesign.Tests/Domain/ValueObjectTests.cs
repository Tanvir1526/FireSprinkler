using FireSprinklerDesign.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FireSprinklerDesign.Tests.Domain;

public class ValueObjectTests
{
    [Fact]
    public void Point3D_DistanceTo_CalculatesCorrectDistance()
    {
        // Arrange
        var point1 = new Point3D(0, 0, 0);
        var point2 = new Point3D(3, 4, 0);

        // Act
        var distance = point1.DistanceTo(point2);

        // Assert
        distance.Should().BeApproximately(5.0, 1e-10);
    }

    [Fact]
    public void Vector3D_Magnitude_CalculatesCorrectly()
    {
        // Arrange
        var vector = new Vector3D(3, 4, 0);

        // Act & Assert
        vector.Magnitude.Should().BeApproximately(5.0, 1e-10);
    }

    [Fact]
    public void Vector3D_DotProduct_CalculatesCorrectly()
    {
        // Arrange
        var v1 = new Vector3D(1, 2, 3);
        var v2 = new Vector3D(4, 5, 6);

        // Act
        var dot = v1.Dot(v2);

        // Assert
        dot.Should().BeApproximately(32.0, 1e-10); // 1*4 + 2*5 + 3*6 = 32
    }

    [Fact]
    public void Vector3D_CrossProduct_CalculatesCorrectly()
    {
        // Arrange
        var v1 = new Vector3D(1, 0, 0);
        var v2 = new Vector3D(0, 1, 0);

        // Act
        var cross = v1.Cross(v2);

        // Assert
        cross.X.Should().BeApproximately(0, 1e-10);
        cross.Y.Should().BeApproximately(0, 1e-10);
        cross.Z.Should().BeApproximately(1, 1e-10);
    }

    [Fact]
    public void LineSegment3D_ClosestPoint_MidSegment()
    {
        // Arrange
        var segment = new LineSegment3D(
            new Point3D(0, 0, 0),
            new Point3D(10, 0, 0));
        var point = new Point3D(5, 5, 0);

        // Act
        var closest = segment.ClosestPointTo(point);

        // Assert
        closest.X.Should().BeApproximately(5, 1e-10);
        closest.Y.Should().BeApproximately(0, 1e-10);
    }

    [Fact]
    public void LineSegment3D_ClosestPoint_BeyondStart()
    {
        // Arrange
        var segment = new LineSegment3D(
            new Point3D(0, 0, 0),
            new Point3D(10, 0, 0));
        var point = new Point3D(-5, 5, 0);

        // Act
        var closest = segment.ClosestPointTo(point);

        // Assert
        closest.X.Should().BeApproximately(0, 1e-10);
        closest.Y.Should().BeApproximately(0, 1e-10);
    }

    [Fact]
    public void LineSegment3D_ClosestPoint_BeyondEnd()
    {
        // Arrange
        var segment = new LineSegment3D(
            new Point3D(0, 0, 0),
            new Point3D(10, 0, 0));
        var point = new Point3D(15, 5, 0);

        // Act
        var closest = segment.ClosestPointTo(point);

        // Assert
        closest.X.Should().BeApproximately(10, 1e-10);
        closest.Y.Should().BeApproximately(0, 1e-10);
    }
}