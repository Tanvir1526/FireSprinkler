using FireSprinklerDesign.Application.UseCases;
using FireSprinklerDesign.Console.DependencyInjection;
using FireSprinklerDesign.Console.Output;
using FireSprinklerDesign.Domain.Entities;
using FireSprinklerDesign.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace FireSprinklerDesign.Console;

public class Program
{
    private const double WallOffset = 2500.0;    // Distance from walls
    private const double SprinklerSpacing = 2500.0;  // Distance between sprinklers

    public static void Main(string[] args)
    {
        // Configure dependency injection
        var services = new ServiceCollection();
        services.AddFireSprinklerServices();
        var serviceProvider = services.BuildServiceProvider();

        try
        {
            // Create room with ceiling coordinates
            var room = CreateRoom();

            // Create water pipes
            var pipes = CreatePipes();

            // Execute the use case
            var useCase = serviceProvider.GetRequiredService<CalculateSprinklerLayoutUseCase>();
            var result = useCase.Execute(room, pipes, WallOffset, SprinklerSpacing);

            // Format and display output
            serviceProvider.GetRequiredService<ConsoleOutputFormatter>();
            var output = ConsoleOutputFormatter.FormatResult(result);

            System.Console.WriteLine(output);
        }
        catch (Exception ex)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"Error: {ex.Message}");
            System.Console.ResetColor();
            Environment.Exit(1);
        }
    }

    private static Room CreateRoom()
    {
        var ceilingCorners = new List<Point3D>
        {
            new(97500.01, 34000.00, 2500.00),
            new(85647.67, 43193.61, 2500.00),
            new(91776.75, 51095.16, 2530.00),
            new(103629.07, 41901.55, 2530.00)
        };

        return new Room(ceilingCorners, "Fire Protection Zone A");
    }

    private static IReadOnlyList<Pipe> CreatePipes()
    {
        return new List<Pipe>
        {
            new(1,
                new Point3D(98242.11, 36588.29, 3000.00),
                new Point3D(87970.10, 44556.09, 3000.00)),

            new(2,
                new Point3D(99774.38, 38563.68, 3000.00),
                new Point3D(89502.37, 46531.47, 3000.00)),

            new(3,
                new Point3D(101306.65, 40539.07, 3000.00),
                new Point3D(91034.63, 48507.01, 3000.00))
        }.AsReadOnly();
    }
}