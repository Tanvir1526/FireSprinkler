namespace FireSprinklerDesign.Domain.ValueObjects;

public readonly record struct LineSegment3D
{
    public Point3D Start { get; }

    public Point3D End { get; }

    public LineSegment3D(Point3D start, Point3D end)
    {
        Start = start;
        End = end;
    }

    public Vector3D Direction => End - Start;

    public double Length => Start.DistanceTo(End);

    /// <summary>
    /// Calculates the closest point on this line segment to the given point.
    /// </summary>
    public Point3D ClosestPointTo(Point3D point)
    {
        var ab = Direction;
        var ap = point - Start;

        var abSquared = ab.MagnitudeSquared;
        if (abSquared < 1e-10)
            return Start;

        var t = ap.Dot(ab) / abSquared;
        t = Math.Clamp(t, 0.0, 1.0);

        return Start.Lerp(End, t);
    }

    /// <summary>
    /// Calculates the distance from this line segment to the given point.
    /// </summary>
    public double DistanceTo(Point3D point)
    {
        var closestPoint = ClosestPointTo(point);
        return point.DistanceTo(closestPoint);
    }

    public override string ToString() => $"{Start} -> {End}";
}