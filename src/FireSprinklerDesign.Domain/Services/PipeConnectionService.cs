using FireSprinklerDesign.Domain.Abstractions;
using FireSprinklerDesign.Domain.Entities;

namespace FireSprinklerDesign.Domain.Services;

public class PipeConnectionService : IPipeConnectionService
{
    public void ConnectSprinklersToPipes(IReadOnlyList<Sprinkler> sprinklers, IReadOnlyList<Pipe> pipes)
    {
        ArgumentNullException.ThrowIfNull(sprinklers);
        ArgumentNullException.ThrowIfNull(pipes);

        if (pipes.Count == 0)
            throw new ArgumentException("At least one pipe must be provided.", nameof(pipes));

        foreach (var sprinkler in sprinklers)
        {
            ConnectToNearestPipe(sprinkler, pipes);
        }
    }

    private static void ConnectToNearestPipe(Sprinkler sprinkler, IReadOnlyList<Pipe> pipes)
    {
        Pipe? nearestPipe = null;
        double minDistance = double.MaxValue;

        foreach (var pipe in pipes)
        {
            var distance = pipe.GetDistanceTo(sprinkler.Position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPipe = pipe;
            }
        }

        if (nearestPipe is not null)
        {
            var connectionPoint = nearestPipe.GetConnectionPoint(sprinkler.Position);
            sprinkler.ConnectToPipe(nearestPipe, connectionPoint);
        }
    }
}