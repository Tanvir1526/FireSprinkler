using FireSprinklerDesign.Domain.ValueObjects;

namespace FireSprinklerDesign.Domain.Entities;

public sealed class Pipe
{
    public int Id { get; }

    public LineSegment3D Segment { get; }

    public Pipe(int id, Point3D start, Point3D end)
    {
        Id = id;
        Segment = new LineSegment3D(start, end);
    }

    public Pipe(int id, LineSegment3D segment)
    {
        Id = id;
        Segment = segment;
    }

    /// <summary>
    /// Gets the closest point on this pipe to the given point.
    /// </summary>
    public Point3D GetConnectionPoint(Point3D sprinklerPosition) =>
        Segment.ClosestPointTo(sprinklerPosition);

    /// <summary>
    /// Gets the distance from this pipe to the given point.
    /// </summary>
    public double GetDistanceTo(Point3D point) =>
        Segment.DistanceTo(point);

    public override string ToString() => $"Pipe {Id}: {Segment}";
}