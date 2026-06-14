namespace TheAdventure;

public class Snake
{
    public List<Point> Body { get; }
    public Direction CurrentDirection { get; private set; }

    private bool _growPending;
    private Direction _lastMovedDirection; // Direction of the last actual move

    public Snake(Point startPosition, Direction startDirection)
    {
        Body = new List<Point> { startPosition };
        CurrentDirection = startDirection;
        _lastMovedDirection = startDirection;
    }

    public Point Head => Body[0];

    public bool SetDirection(Direction newDirection)
    {
        // Check against the direction of the LAST ACTUAL MOVE, not CurrentDirection.
        // This prevents bypassing the opposite check by pressing two keys quickly
        // between move ticks (e.g. Right→Down→Left would reverse into the body).
        if (Body.Count > 1 && newDirection == _lastMovedDirection.Opposite())
        {
            return false;
        }

        CurrentDirection = newDirection;
        return true;
    }

    public void Grow()
    {
        _growPending = true;
    }

    public void Move()
    {
        _lastMovedDirection = CurrentDirection;

        var delta = CurrentDirection.ToVector();
        var newHead = new Point(Head.X + delta.X, Head.Y + delta.Y);

        Body.Insert(0, newHead);

        if (_growPending)
        {
            _growPending = false;
        }
        else
        {
            Body.RemoveAt(Body.Count - 1);
        }
    }

    public bool CollidesWithSelf()
    {
        return Body.Skip(1).Any(segment => segment == Head);
    }

    public bool IsOutOfBounds(int gridSize)
    {
        return Head.X < 0 || Head.Y < 0 || Head.X >= gridSize || Head.Y >= gridSize;
    }
}
