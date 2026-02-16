using FireSprinklerDesign.Domain.Abstractions;
using FireSprinklerDesign.Domain.Entities;

namespace FireSprinklerDesign.Application.Strategies;

public class GridPlacementStrategy(ISprinklerPlanner sprinklerPlanner) : IPlacementStrategy
{
    private readonly ISprinklerPlanner sprinklerPlanner = sprinklerPlanner ?? throw new ArgumentNullException(nameof(sprinklerPlanner));

    public string Name => "Grid Placement";

    public IReadOnlyList<Sprinkler> PlaceSprinklers(Room room, double wallOffset, double spacing)
    {
        return sprinklerPlanner.PlanSprinklers(room, wallOffset, spacing);
    }
}