using FireSprinklerDesign.Domain.Entities;

namespace FireSprinklerDesign.Domain.Abstractions;

public interface ISprinklerPlanner
{
    /// <summary>
    /// Calculates the positions of sprinklers within the room.
    /// </summary>
    IReadOnlyList<Sprinkler> PlanSprinklers(Room room, double wallOffset, double spacing);
}