using FireSprinklerDesign.Domain.Abstractions;
using FireSprinklerDesign.Domain.ValueObjects;

namespace FireSprinklerDesign.Infrastructure.Services;

public class GeometryService : IGeometryService
{
    public bool IsPointInsidePolygon(Point3D point, IReadOnlyList<Point3D> polygon)
    {
        if (polygon.Count < 3)
            return false;

        // Ray casting algorithm (project to XY plane)
        int intersections = 0;
        int n = polygon.Count;

        for (int i = 0; i < n; i++)
        {
            var p1 = polygon[i];
            var p2 = polygon[(i + 1) % n];

            // Check if ray from point crosses this edge
            if (RayIntersectsEdge(point.X, point.Y, p1.X, p1.Y, p2.X, p2.Y))
            {
                intersections++;
            }
        }

        return intersections % 2 == 1;
    }

    private static bool RayIntersectsEdge(double px, double py,
        double x1, double y1, double x2, double y2)
    {
        // Ensure y1 <= y2
        if (y1 > y2)
        {
            (x1, x2) = (x2, x1);
            (y1, y2) = (y2, y1);
        }

        // Point is outside the y-range of the edge
        if (py <= y1 || py > y2)
            return false;

        // Calculate x-coordinate of intersection
        double xIntersect = x1 + (py - y1) / (y2 - y1) * (x2 - x1);
        return px < xIntersect;
    }

    public IReadOnlyList<Point3D> InsetPolygon(IReadOnlyList<Point3D> polygon, double insetDistance)
    {
        if (polygon.Count < 3)
            return polygon;

        var insetPoints = new List<Point3D>();
        int n = polygon.Count;

        // Calculate average Z for the ceiling plane
        double avgZ = polygon.Average(p => p.Z);

        for (int i = 0; i < n; i++)
        {
            var prev = polygon[(i - 1 + n) % n];
            var curr = polygon[i];
            var next = polygon[(i + 1) % n];

            // Calculate edge vectors (in XY plane)
            var edge1 = new Vector3D(curr.X - prev.X, curr.Y - prev.Y, 0);
            var edge2 = new Vector3D(next.X - curr.X, next.Y - curr.Y, 0);

            // Calculate inward normals (perpendicular in XY plane)
            var normal1 = GetInwardNormal(edge1);
            var normal2 = GetInwardNormal(edge2);

            // Calculate bisector direction
            var bisector = (normal1 + normal2).Normalized;

            // Calculate the angle between the two edges
            double dot = normal1.Dot(normal2);
            dot = Math.Clamp(dot, -1.0, 1.0);
            double angle = Math.Acos(dot);

            // Adjust inset distance for corners
            double adjustedDistance = insetDistance / Math.Cos(angle / 2);

            // Clamp to prevent extreme values at sharp corners
            adjustedDistance = Math.Min(adjustedDistance, insetDistance * 3);

            // Calculate inset point
            var insetPoint = new Point3D(
                curr.X + bisector.X * adjustedDistance,
                curr.Y + bisector.Y * adjustedDistance,
                avgZ
            );

            insetPoints.Add(insetPoint);
        }

        // Validate the inset polygon (check if it's valid)
        if (CalculatePolygonArea(insetPoints) <= 0)
            return Array.Empty<Point3D>();

        return insetPoints.AsReadOnly();
    }

    private static Vector3D GetInwardNormal(Vector3D edge)
    {
        // For clockwise polygon, rotate 90° clockwise for inward normal
        // For counter-clockwise, rotate 90° counter-clockwise
        var normalized = edge.Normalized;

        // Assuming counter-clockwise winding (standard convention)
        // Rotate -90° (clockwise) to get inward normal
        return new Vector3D(normalized.Y, -normalized.X, 0);
    }

    public (Point3D min, Point3D max) GetBoundingBox(IReadOnlyList<Point3D> polygon)
    {
        if (polygon.Count == 0)
            return (new Point3D(0, 0, 0), new Point3D(0, 0, 0));

        double minX = polygon.Min(p => p.X);
        double minY = polygon.Min(p => p.Y);
        double minZ = polygon.Min(p => p.Z);
        double maxX = polygon.Max(p => p.X);
        double maxY = polygon.Max(p => p.Y);
        double maxZ = polygon.Max(p => p.Z);

        return (new Point3D(minX, minY, minZ), new Point3D(maxX, maxY, maxZ));
    }

    public double CalculatePolygonArea(IReadOnlyList<Point3D> polygon)
    {
        if (polygon.Count < 3)
            return 0;

        // Shoelace formula (for 2D projection)
        double area = 0;
        int n = polygon.Count;

        for (int i = 0; i < n; i++)
        {
            var curr = polygon[i];
            var next = polygon[(i + 1) % n];
            area += curr.X * next.Y - next.X * curr.Y;
        }

        return Math.Abs(area) / 2;
    }
}