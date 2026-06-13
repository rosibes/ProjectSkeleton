namespace TheAdventure;

public enum Direction
{
    Up,
    Down,
    Left,
    Right,
}

public static class DirectionExtensions
{
    public static Direction Opposite(this Direction direction)
    {
        return direction switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => throw new ArgumentOutOfRangeException(nameof(direction)),
        };
    }

    public static Point ToVector(this Direction direction)
    {
        return direction switch
        {
            Direction.Up => new Point(0, -1),
            Direction.Down => new Point(0, 1),
            Direction.Left => new Point(-1, 0),
            Direction.Right => new Point(1, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(direction)),
        };
    }
}
