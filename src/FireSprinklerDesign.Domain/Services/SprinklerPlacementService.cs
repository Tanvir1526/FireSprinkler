using FireSprinklerDesign.Domain.Abstractions;
using FireSprinklerDesign.Domain.Entities;
using FireSprinklerDesign.Domain.ValueObjects;

namespace FireSprinklerDesign.Domain.Services;

public class SprinklerPlacementService : ISprinklerPlanner
{
    public IReadOnlyList<Sprinkler> PlanSprinklers(Room room, double wallOffset, double spacing)
    {
        ArgumentNullException.ThrowIfNull(room);

        if (wallOffset <= 0)
            throw new ArgumentOutOfRangeException(nameof(wallOffset), "Wall offset must be positive.");

        if (spacing <= 0)
            throw new ArgumentOutOfRangeException(nameof(spacing), "Spacing must be positive.");

        var corners = room.CeilingCorners;

        var origin = corners[1];

        var edgeU = corners[0] - corners[1];
        var edgeV = corners[2] - corners[1];

        double lengthU = Math.Sqrt(edgeU.X * edgeU.X + edgeU.Y * edgeU.Y);
        double lengthV = Math.Sqrt(edgeV.X * edgeV.X + edgeV.Y * edgeV.Y);

        double effectiveLengthU = lengthU - 2 * wallOffset;
        double effectiveLengthV = lengthV - 2 * wallOffset;

        if (effectiveLengthU < -1e-9 || effectiveLengthV < -1e-9)
            return Array.Empty<Sprinkler>();

        int countU = (int)Math.Floor(effectiveLengthU / spacing + 1e-9) + 1;
        int countV = (int)Math.Floor(effectiveLengthV / spacing + 1e-9) + 1;

        var uUnit = new Vector3D(edgeU.X / lengthU, edgeU.Y / lengthU, 0);
        var vUnit = new Vector3D(edgeV.X / lengthV, edgeV.Y / lengthV, 0);

        double zStart = corners[1].Z;
        double zEnd = corners[2].Z;

        var sprinklers = new List<Sprinkler>(countU * countV);
        int id = 1;

        for (int i = 0; i < countU; i++)
        {
            double uDistance = wallOffset + i * spacing;

            for (int j = 0; j < countV; j++)
            {
                double vDistance = wallOffset + j * spacing;

                double worldX = origin.X + uDistance * uUnit.X + vDistance * vUnit.X;
                double worldY = origin.Y + uDistance * uUnit.Y + vDistance * vUnit.Y;

                double t = vDistance / lengthV;
                double worldZ = zStart + t * (zEnd - zStart);

                sprinklers.Add(new Sprinkler(id++, new Point3D(worldX, worldY, worldZ)));
            }
        }

        return sprinklers.AsReadOnly();
    }
}