using FireSprinklerDesign.Domain.ValueObjects;

namespace FireSprinklerDesign.Domain.Entities;

public sealed class Room
{
    public IReadOnlyList<Point3D> CeilingCorners { get; }

    public string Name { get; }

    public Room(IEnumerable<Point3D> ceilingCorners, string name = "Room")
    {
        var corners = ceilingCorners.ToList();
        if (corners.Count != 4)
            throw new ArgumentException("Room ceiling must have exactly 4 corners.", nameof(ceilingCorners));

        CeilingCorners = corners.AsReadOnly();
        Name = name;
    }

    /// <summary>
    /// Gets the edges of the ceiling as line segments.
    /// </summary>
    public IEnumerable<LineSegment3D> GetEdges()
    {
        for (int i = 0; i < CeilingCorners.Count; i++)
        {
            var start = CeilingCorners[i];
            var end = CeilingCorners[(i + 1) % CeilingCorners.Count];
            yield return new LineSegment3D(start, end);
        }
    }

    /// <summary>
    /// Calculates the average Z coordinate of the ceiling.
    /// </summary>
    public double GetAverageCeilingHeight() =>
        CeilingCorners.Average(p => p.Z);

    /// <summary>
    /// Gets the centroid of the ceiling.
    /// </summary>
    public Point3D GetCentroid() =>
        new(
            CeilingCorners.Average(p => p.X),
            CeilingCorners.Average(p => p.Y),
            CeilingCorners.Average(p => p.Z)
        );
}