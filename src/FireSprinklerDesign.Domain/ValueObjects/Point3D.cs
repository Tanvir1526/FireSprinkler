namespace FireSprinklerDesign.Domain.ValueObjects;

public readonly record struct Point3D
{
    public double X { get; }

    public double Y { get; }

    public double Z { get; }

    public Point3D(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Point3D operator +(Point3D p, Vector3D v) =>
        new(p.X + v.X, p.Y + v.Y, p.Z + v.Z);

    public static Point3D operator -(Point3D p, Vector3D v) =>
        new(p.X - v.X, p.Y - v.Y, p.Z - v.Z);

    public static Vector3D operator -(Point3D a, Point3D b) =>
        new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public double DistanceTo(Point3D other)
    {
        var dx = X - other.X;
        var dy = Y - other.Y;
        var dz = Z - other.Z;
        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    public Point3D Lerp(Point3D other, double t) =>
        new(
            X + t * (other.X - X),
            Y + t * (other.Y - Y),
            Z + t * (other.Z - Z)
        );

    public override string ToString() => $"({X:F2}, {Y:F2}, {Z:F2})";
}