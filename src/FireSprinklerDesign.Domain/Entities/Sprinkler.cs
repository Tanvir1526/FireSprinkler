using FireSprinklerDesign.Domain.ValueObjects;

namespace FireSprinklerDesign.Domain.Entities;

public sealed class Sprinkler
{
    public int Id { get; }

    public Point3D Position { get; }

    public Point3D? ConnectionPoint { get; private set; }

    public Pipe? ConnectedPipe { get; private set; }

    public double? ConnectionDistance { get; private set; }

    public Sprinkler(int id, Point3D position)
    {
        Id = id;
        Position = position;
    }

    /// <summary>
    /// Connects this sprinkler to a pipe at the specified connection point.
    /// </summary>
    public void ConnectToPipe(Pipe pipe, Point3D connectionPoint)
    {
        ConnectedPipe = pipe ?? throw new ArgumentNullException(nameof(pipe));
        ConnectionPoint = connectionPoint;
        ConnectionDistance = Position.DistanceTo(connectionPoint);
    }

    public bool IsConnected => ConnectedPipe is not null;

    public override string ToString() =>
        $"Sprinkler {Id} at {Position}" +
        (IsConnected ? $" -> Pipe {ConnectedPipe!.Id} at {ConnectionPoint}" : " (not connected)");
}