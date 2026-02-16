using FireSprinklerDesign.Application.Strategies;
using FireSprinklerDesign.Application.UseCases;
using FireSprinklerDesign.Console.Output;
using FireSprinklerDesign.Domain.Abstractions;
using FireSprinklerDesign.Domain.Services;
using FireSprinklerDesign.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FireSprinklerDesign.Console.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFireSprinklerServices(this IServiceCollection services)
    {
        // Infrastructure Services
        services.AddSingleton<IGeometryService, GeometryService>();

        // Domain Services
        services.AddSingleton<ISprinklerPlanner, SprinklerPlacementService>();
        services.AddSingleton<IPipeConnectionService, PipeConnectionService>();

        // Strategies
        services.AddSingleton<IPlacementStrategy, GridPlacementStrategy>();
        services.AddSingleton<PlacementStrategyFactory>();

        // Use Cases
        services.AddSingleton<CalculateSprinklerLayoutUseCase>();

        // Output
        services.AddSingleton<ConsoleOutputFormatter>();

        return services;
    }
}