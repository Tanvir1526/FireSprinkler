using FireSprinklerDesign.Domain.Entities;

namespace FireSprinklerDesign.Domain.Abstractions;

public interface IPipeConnectionService
{
    /// <summary>
    /// Connects each sprinkler to its nearest pipe.
    /// </summary>
    void ConnectSprinklersToPipes(IReadOnlyList<Sprinkler> sprinklers, IReadOnlyList<Pipe> pipes);
}