using FireSprinklerDesign.Domain.Entities;

namespace FireSprinklerDesign.Application.Strategies;

public interface IPlacementStrategy
{
    string Name { get; }

    IReadOnlyList<Sprinkler> PlaceSprinklers(Room room, double wallOffset, double spacing);
}