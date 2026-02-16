using FireSprinklerDesign.Application.DTOs;
using FireSprinklerDesign.Application.Strategies;
using FireSprinklerDesign.Domain.Abstractions;
using FireSprinklerDesign.Domain.Entities;

namespace FireSprinklerDesign.Application.UseCases;

public class CalculateSprinklerLayoutUseCase(PlacementStrategyFactory strategyFactory,
                                            IPipeConnectionService pipeConnectionService)
{
    private readonly PlacementStrategyFactory strategyFactory = strategyFactory ?? throw new ArgumentNullException(nameof(strategyFactory));
    private readonly IPipeConnectionService pipeConnectionService = pipeConnectionService ?? throw new ArgumentNullException(nameof(pipeConnectionService));

    public SprinklerLayoutResult Execute(Room room, IReadOnlyList<Pipe> pipes,
        double wallOffset, double spacing, string? strategyName = null)
    {
        ArgumentNullException.ThrowIfNull(room);
        ArgumentNullException.ThrowIfNull(pipes);

        // Get placement strategy
        var strategy = strategyName is not null
            ? strategyFactory.GetStrategy(strategyName)
            : strategyFactory.GetDefaultStrategy();

        // Calculate sprinkler positions
        var sprinklers = strategy.PlaceSprinklers(room, wallOffset, spacing);

        // Connect sprinklers to pipes
        pipeConnectionService.ConnectSprinklersToPipes(sprinklers, pipes);

        return new SprinklerLayoutResult
        {
            Room = room,
            Sprinklers = sprinklers,
            Pipes = pipes,
            WallOffset = wallOffset,
            Spacing = spacing
        };
    }
}