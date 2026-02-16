namespace FireSprinklerDesign.Application.Strategies;

public class PlacementStrategyFactory(IEnumerable<IPlacementStrategy> strategies)
{
    private readonly IEnumerable<IPlacementStrategy> strategies = strategies ?? throw new ArgumentNullException(nameof(strategies));

    public IPlacementStrategy GetStrategy(string name)
    {
        var strategy = strategies.FirstOrDefault(s =>
            s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        return strategy ?? throw new ArgumentException($"Unknown strategy: {name}", nameof(name));
    }

    public IPlacementStrategy GetDefaultStrategy()
    {
        return strategies.First();
    }
}