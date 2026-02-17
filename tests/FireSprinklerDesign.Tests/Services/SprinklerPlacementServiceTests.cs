using FireSprinklerDesign.Domain.Entities;
using FireSprinklerDesign.Domain.Services;
using FireSprinklerDesign.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FireSprinklerDesign.Tests.Services;

public class SprinklerPlacementServiceTests
{
    private readonly SprinklerPlacementService _sut = new();

    [Fact]
    public void PlanSprinklers_NullRoom_ThrowsArgumentNullException()
    {
        var act = () => _sut.PlanSprinklers(null!, 1.0, 10.0);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("room");
    }

    [Fact]
    public void PlanSprinklers_ZeroWallOffset_ThrowsArgumentOutOfRangeException()
    {
        var room = CreateTestRoom();

        var act = () => _sut.PlanSprinklers(room, 0, 10.0);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("wallOffset");
    }

    [Fact]
    public void PlanSprinklers_NegativeWallOffset_ThrowsArgumentOutOfRangeException()
    {
        var room = CreateTestRoom();

        var act = () => _sut.PlanSprinklers(room, -5, 10.0);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("wallOffset");
    }

    [Fact]
    public void PlanSprinklers_ZeroSpacing_ThrowsArgumentOutOfRangeException()
    {
        var room = CreateTestRoom();

        var act = () => _sut.PlanSprinklers(room, 1.0, 0);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("spacing");
    }

    [Fact]
    public void PlanSprinklers_NegativeSpacing_ThrowsArgumentOutOfRangeException()
    {
        var room = CreateTestRoom();

        var act = () => _sut.PlanSprinklers(room, 1.0, -10);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("spacing");
    }

    [Fact]
    public void PlanSprinklers_AxisAlignedRoom_Returns15Sprinklers()
    {
        var room = CreateTestRoom(length: 150, width: 100);

        var sprinklers = _sut.PlanSprinklers(room, 25, 25);

        sprinklers.Should().HaveCount(15);
    }

    [Fact]
    public void PlanSprinklers_RotatedRoom_Returns15Sprinklers()
    {
        // The actual fire sprinkler room (~15000×10000), rotated ~37°
        var room = CreateActualRoom();

        var sprinklers = _sut.PlanSprinklers(room, 2500, 2500);

        sprinklers.Should().HaveCount(15);
    }

    [Fact]
    public void PlanSprinklers_SprinklersHaveUniqueIds()
    {
        var room = CreateTestRoom(length: 150, width: 100);

        var sprinklers = _sut.PlanSprinklers(room, 25, 25);

        sprinklers.Select(s => s.Id).Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void PlanSprinklers_SprinklersAtAverageCeilingHeight()
    {
        var room = CreateTestRoom(length: 150, width: 100, ceilingHeight: 12.0);

        var sprinklers = _sut.PlanSprinklers(room, 25, 25);

        sprinklers.Should().AllSatisfy(s =>
            s.Position.Z.Should().BeApproximately(12.0, 1e-10));
    }

    [Fact]
    public void PlanSprinklers_WallOffsetTooLarge_ReturnsEmpty()
    {
        // 100×100 room with 60mm offset → effective = -20 → no sprinklers
        var room = CreateTestRoom(length: 100, width: 100);

        var sprinklers = _sut.PlanSprinklers(room, 60, 10);

        sprinklers.Should().BeEmpty();
    }


    private static Room CreateTestRoom(double length = 100, double width = 100, double ceilingHeight = 10.0)
    {
        var corners = new List<Point3D>
        {
            new(0, 0, ceilingHeight),
            new(length, 0, ceilingHeight),
            new(length, width, ceilingHeight),
            new(0, width, ceilingHeight)
        };
        return new Room(corners, "Test Room");
    }

    private static Room CreateActualRoom()
    {
        var corners = new List<Point3D>
        {
            new(97500.01, 34000.00, 2500.00),
            new(85647.67, 43193.61, 2500.00),
            new(91776.75, 51095.16, 2530.00),
            new(103629.07, 41901.55, 2530.00)
        };
        return new Room(corners, "Fire Protection Zone A");
    }
}