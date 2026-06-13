namespace TheAdventure;

public static class HighScoreStore
{
    private const string FilePath = "highscore.txt";

    public static int Load()
    {
        try
        {
            var text = File.ReadAllText(FilePath);
            return int.Parse(text);
        }
        catch (FileNotFoundException)
        {
            return 0;
        }
        catch (FormatException)
        {
            return 0;
        }
    }

    public static void Save(int score)
    {
        File.WriteAllText(FilePath, score.ToString());
    }
}
