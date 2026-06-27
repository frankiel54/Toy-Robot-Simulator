using Xunit;

namespace ToyRobot.Tests
{
    public class DirectionExtensionsTests
    {
        [Theory]
        [MemberData(nameof(TurnLeftData))]
        public void TurnLeft_Should_Return_Correct_Direction(Direction input, Direction expected)
        {
            Assert.Equal(expected, input.TurnLeft());
        }

        public static TheoryData<Direction, Direction> TurnLeftData() => new()
        {
            { Direction.North, Direction.West  },
            { Direction.West,  Direction.South },
            { Direction.South, Direction.East  },
            { Direction.East,  Direction.North },
            { Direction.Unset, Direction.Unset },
        };

        [Theory]
        [MemberData(nameof(TurnRightData))]
        public void TurnRight_Should_Return_Correct_Direction(Direction input, Direction expected)
        {
            Assert.Equal(expected, input.TurnRight());
        }

        public static TheoryData<Direction, Direction> TurnRightData() => new()
        {
            { Direction.North, Direction.East  },
            { Direction.East,  Direction.South },
            { Direction.South, Direction.West  },
            { Direction.West,  Direction.North },
            { Direction.Unset, Direction.Unset },
        };

    }
}
