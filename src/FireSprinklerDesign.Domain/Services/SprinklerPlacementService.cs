using FireSprinklerDesign.Domain.Abstractions;
using FireSprinklerDesign.Domain.Entities;
using FireSprinklerDesign.Domain.ValueObjects;

namespace FireSprinklerDesign.Domain.Services;

public class SprinklerPlacementService(IGeometryService geometryService) : ISprinklerPlanner
{
    private readonly IGeometryService geometryService = geometryService ?? throw new ArgumentNullException(nameof(geometryService));

    public IReadOnlyList<Sprinkler> PlanSprinklers(Room room, double wallOffset, double spacing)
    {
        ArgumentNullException.ThrowIfNull(room);

        if (wallOffset <= 0)
            throw new ArgumentOutOfRangeException(nameof(wallOffset), "Wall offset must be positive.");

        if (spacing <= 0)
            throw new ArgumentOutOfRangeException(nameof(spacing), "Spacing must be positive.");

        // Create inset polygon (shrink by wall offset)
        var insetPolygon = geometryService.InsetPolygon(room.CeilingCorners, wallOffset);

        if (insetPolygon.Count < 3)
            return Array.Empty<Sprinkler>();

        // Get bounding box for grid generation
        var (min, max) = geometryService.GetBoundingBox(insetPolygon);

        // Calculate the average Z height for sprinkler placement
        var ceilingZ = room.GetAverageCeilingHeight();

        // Generate grid points
        var sprinklers = new List<Sprinkler>();
        int sprinklerId = 1;

        // Start from min + offset to ensure proper spacing from walls
        for (double x = min.X; x <= max.X; x += spacing)
        {
            for (double y = min.Y; y <= max.Y; y += spacing)
            {
                var candidatePoint = new Point3D(x, y, ceilingZ);

                if (geometryService.IsPointInsidePolygon(candidatePoint, insetPolygon))
                {
                    sprinklers.Add(new Sprinkler(sprinklerId++, candidatePoint));
                }
            }
        }

        return sprinklers.AsReadOnly();
    }
}