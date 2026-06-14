namespace TheAdventure;

public class GameState
{
    public const int GridSize = 20;

    public Snake Snake { get; }
    public Food Food { get; private set; }
    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public bool IsGameOver { get; private set; }

    public GameState()
    {
        var startPosition = new Point(GridSize / 2, GridSize / 2);
        Snake = new Snake(startPosition, Direction.Right);
        Food = new Food(GridSize, Snake.Body);
        HighScore = HighScoreStore.Load();
    }

    public void HandleInput(Direction direction)
    {
        Snake.SetDirection(direction);
    }

    public void Update()
    {
        if (IsGameOver)
        {
            throw new InvalidMoveException("Cannot update the game after it is over.");
        }

        Snake.Move();

        if (Snake.IsOutOfBounds(GridSize) || Snake.CollidesWithSelf())
        {
            IsGameOver = true;

            if (Score > HighScore)
            {
                HighScore = Score;
                HighScoreStore.Save(HighScore);
            }

            return;
        }

        if (Snake.Head == Food.Position)
        {
            Snake.Grow();
            Score++;
            Food.Respawn(GridSize, Snake.Body);
        }
    }
}