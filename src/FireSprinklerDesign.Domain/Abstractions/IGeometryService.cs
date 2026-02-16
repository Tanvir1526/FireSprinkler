using FireSprinklerDesign.Domain.ValueObjects;

namespace FireSprinklerDesign.Domain.Abstractions;

public interface IGeometryService
{
    /// <summary>
    /// Determines if a point lies inside a polygon (projected to 2D).
    /// </summary>
    bool IsPointInsidePolygon(Point3D point, IReadOnlyList<Point3D> polygon);

    /// <summary>
    /// Creates an inset (shrunk) version of a polygon by the specified distance.
    /// </summary>
    IReadOnlyList<Point3D> InsetPolygon(IReadOnlyList<Point3D> polygon, double insetDistance);

    /// <summary>
    /// Calculates the bounding box of a polygon.
    /// </summary>
    (Point3D min, Point3D max) GetBoundingBox(IReadOnlyList<Point3D> polygon);

    /// <summary>
    /// Calculates the area of a polygon (projected to 2D).
    /// </summary>
    double CalculatePolygonArea(IReadOnlyList<Point3D> polygon);
}