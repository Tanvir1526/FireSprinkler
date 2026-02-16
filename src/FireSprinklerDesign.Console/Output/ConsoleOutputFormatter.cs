using FireSprinklerDesign.Application.DTOs;
using FireSprinklerDesign.Domain.Entities;
using System.Text;

namespace FireSprinklerDesign.Console.Output;

public class ConsoleOutputFormatter
{
    private const string Separator = "════════════════════════════════════════════════════════════════";
    private const string SubSeparator = "────────────────────────────────────────────────────────────────";

    public static string FormatResult(SprinklerLayoutResult result)
    {
        var sb = new StringBuilder();

        // Header
        sb.AppendLine();
        sb.AppendLine(Separator);
        sb.AppendLine("           FIRE SPRINKLER DESIGN CALCULATION RESULTS ");
        sb.AppendLine(Separator);
        sb.AppendLine();

        // Input Summary
        FormatInputSummary(sb, result);

        // Room Information
        FormatRoomInfo(sb, result.Room);

        // Pipes Information
        FormatPipesInfo(sb, result.Pipes);

        // Sprinkler Results
        FormatSprinklerResults(sb, result);

        // Summary Statistics
        FormatSummaryStatistics(sb, result);

        return sb.ToString();
    }

    private static void FormatInputSummary(StringBuilder sb, SprinklerLayoutResult result)
    {
        sb.AppendLine(" INPUT PARAMETERS");
        sb.AppendLine(SubSeparator);
        sb.AppendLine($"   Wall Offset:     {result.WallOffset:F2} mm");
        sb.AppendLine($"   Sprinkler Spacing: {result.Spacing:F2} mm");
        sb.AppendLine();
    }

    private static void FormatRoomInfo(StringBuilder sb, Room room)
    {
        sb.AppendLine(" ROOM CEILING COORDINATES");
        sb.AppendLine(SubSeparator);

        for (int i = 0; i < room.CeilingCorners.Count; i++)
        {
            var corner = room.CeilingCorners[i];
            sb.AppendLine($"   Corner {i + 1}: ({corner.X:F2}, {corner.Y:F2}, {corner.Z:F2})");
        }
        sb.AppendLine();
    }

    private static void FormatPipesInfo(StringBuilder sb, IReadOnlyList<Pipe> pipes)
    {
        sb.AppendLine(" WATER PIPES");
        sb.AppendLine(SubSeparator);

        foreach (var pipe in pipes)
        {
            sb.AppendLine($"   Pipe {pipe.Id}:");
            sb.AppendLine($"      Start: ({pipe.Segment.Start.X:F2}, {pipe.Segment.Start.Y:F2}, {pipe.Segment.Start.Z:F2})");
            sb.AppendLine($"      End:   ({pipe.Segment.End.X:F2}, {pipe.Segment.End.Y:F2}, {pipe.Segment.End.Z:F2})");
        }
        sb.AppendLine();
    }

    private static void FormatSprinklerResults(StringBuilder sb, SprinklerLayoutResult result)
    {
        sb.AppendLine(" SPRINKLER POSITIONS AND CONNECTIONS");
        sb.AppendLine(SubSeparator);
        sb.AppendLine();

        foreach (var sprinkler in result.Sprinklers)
        {
            sb.AppendLine($"   Sprinkler #{sprinkler.Id}");
            sb.AppendLine($"      Position:         ({sprinkler.Position.X:F2}, {sprinkler.Position.Y:F2}, {sprinkler.Position.Z:F2})");

            if (sprinkler.IsConnected)
            {
                sb.AppendLine($"      Connected to:     Pipe {sprinkler.ConnectedPipe!.Id}");
                sb.AppendLine($"      Connection Point: ({sprinkler.ConnectionPoint!.Value.X:F2}, {sprinkler.ConnectionPoint.Value.Y:F2}, {sprinkler.ConnectionPoint.Value.Z:F2})");
                sb.AppendLine($"      Connection Distance: {sprinkler.ConnectionDistance:F2} mm");
            }
            sb.AppendLine();
        }
    }

    private static void FormatSummaryStatistics(StringBuilder sb, SprinklerLayoutResult result)
    {
        sb.AppendLine(" SUMMARY STATISTICS");
        sb.AppendLine(SubSeparator);
        sb.AppendLine($"   Total Sprinklers:     {result.TotalSprinklers}");
        sb.AppendLine($"   Connected Sprinklers: {result.ConnectedSprinklers}");
        sb.AppendLine($"   Available Pipes:      {result.Pipes.Count}");

        // Group by pipe
        var sprinklersByPipe = result.Sprinklers
            .Where(s => s.IsConnected)
            .GroupBy(s => s.ConnectedPipe!.Id)
            .OrderBy(g => g.Key);

        sb.AppendLine();
        sb.AppendLine("   Sprinklers per Pipe:");
        foreach (var group in sprinklersByPipe)
        {
            sb.AppendLine($"      Pipe {group.Key}: {group.Count()} sprinklers");
        }

        // Calculate total pipe length needed
        var totalPipeLength = result.Sprinklers
            .Where(s => s.IsConnected)
            .Sum(s => s.ConnectionDistance ?? 0);

        sb.AppendLine();
        sb.AppendLine($"   Total Connection Length: {totalPipeLength:F2} mm ({totalPipeLength / 1000:F2} m)");
        sb.AppendLine();
    }
}