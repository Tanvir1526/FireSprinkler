using FireSprinklerDesign.Domain.Entities;

namespace FireSprinklerDesign.Application.DTOs;

public sealed record SprinklerLayoutResult
{
    public required Room Room { get; init; }

    public required IReadOnlyList<Sprinkler> Sprinklers { get; init; }

    public required IReadOnlyList<Pipe> Pipes { get; init; }

    public required double WallOffset { get; init; }

    public required double Spacing { get; init; }

    public DateTime CalculatedAt { get; init; } = DateTime.UtcNow;

    public int TotalSprinklers => Sprinklers.Count;

    public int ConnectedSprinklers => Sprinklers.Count(s => s.IsConnected);
}