
namespace TheAdventure;

public class Food
{
    public Point Position { get; private set; }

    private readonly Random _random = new();

    public Food(int gridSize, IEnumerable<Point> occupied)
    {
        Position = FindFreeSpot(gridSize, occupied);
    }

    public void Respawn(int gridSize, IEnumerable<Point> occupied)
    {
        Position = FindFreeSpot(gridSize, occupied);
    }

    private Point FindFreeSpot(int gridSize, IEnumerable<Point> occupied)
    {
        Point candidate;
        do
        {
            candidate = new Point(_random.Next(gridSize), _random.Next(gridSize));
        }
        while (occupied.Contains(candidate));

        return candidate;
    }
}
