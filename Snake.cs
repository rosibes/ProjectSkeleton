namespace TheAdventure;

public class Snake
{
    public List<Point> Body { get; }
    public Direction CurrentDirection { get; private set; }

    private bool _growPending;

    public Snake(Point startPosition, Direction startDirection)
    {
        Body = new List<Point> { startPosition };
        CurrentDirection = startDirection;
    }

    public Point Head => Body[0];

    public void SetDirection(Direction newDirection)
    {
        if (newDirection == CurrentDirection.Opposite())
        {
            return;
        }

        CurrentDirection = newDirection;
    }

    public void Grow()
    {
        _growPending = true;
    }

    public void Move()
    {
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
