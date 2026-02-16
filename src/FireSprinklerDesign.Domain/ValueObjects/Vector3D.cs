namespace FireSprinklerDesign.Domain.ValueObjects;

public readonly record struct Vector3D
{
    public double X { get; }

    public double Y { get; }

    public double Z { get; }

    public Vector3D(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public double Magnitude => Math.Sqrt(X * X + Y * Y + Z * Z);

    public double MagnitudeSquared => X * X + Y * Y + Z * Z;

    public Vector3D Normalized
    {
        get
        {
            var mag = Magnitude;
            return mag > 1e-10 ? new Vector3D(X / mag, Y / mag, Z / mag) : Zero;
        }
    }

    public static Vector3D Zero => new(0, 0, 0);

    public static Vector3D operator +(Vector3D a, Vector3D b) =>
        new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vector3D operator -(Vector3D a, Vector3D b) =>
        new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vector3D operator *(Vector3D v, double scalar) =>
        new(v.X * scalar, v.Y * scalar, v.Z * scalar);

    public static Vector3D operator *(double scalar, Vector3D v) =>
        v * scalar;

    public static Vector3D operator /(Vector3D v, double scalar) =>
        new(v.X / scalar, v.Y / scalar, v.Z / scalar);

    public static Vector3D operator -(Vector3D v) =>
        new(-v.X, -v.Y, -v.Z);

    public double Dot(Vector3D other) =>
        X * other.X + Y * other.Y + Z * other.Z;

    public Vector3D Cross(Vector3D other) =>
        new(
            Y * other.Z - Z * other.Y,
            Z * other.X - X * other.Z,
            X * other.Y - Y * other.X
        );

    public override string ToString() => $"<{X:F2}, {Y:F2}, {Z:F2}>";
}