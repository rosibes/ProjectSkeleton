using System.Diagnostics;
using Silk.NET.SDL;

namespace TheAdventure;

public static class Program
{
    private const int CellSize = 40;
    private const double MoveIntervalMs = 150;

    public static void Main()
    {
        var sdl = new Sdl(new SdlContext());

        var sdlInitResult = sdl.Init(Sdl.InitVideo | Sdl.InitEvents | Sdl.InitTimer);
        if (sdlInitResult < 0)
        {
            throw new InvalidOperationException("Failed to initialize SDL.");
        }

        IntPtr window;
        unsafe
        {
            window = (IntPtr)sdl.CreateWindow(
                "Snake",
                Sdl.WindowposUndefined, Sdl.WindowposUndefined,
                GameState.GridSize * CellSize, GameState.GridSize * CellSize,
                (uint)WindowFlags.Shown
            );

            if (window == IntPtr.Zero)
            {
                throw new Exception(sdl.GetErrorAsException()?.Message ?? "Failed to create window.");
            }
        }

        IntPtr renderer;
        unsafe
        {
            renderer = (IntPtr)sdl.CreateRenderer((Window*)window, -1, (uint)RendererFlags.Accelerated);
            sdl.RenderSetVSync((Renderer*)renderer, 1);
        }

        if (renderer == IntPtr.Zero)
        {
            throw new Exception(sdl.GetErrorAsException()?.Message ?? "Failed to create renderer.");
        }

        var gameState = new GameState();
        Direction? pendingDirection = null;
        double moveAccumulator = 0;

        var timer = new Stopwatch();
        timer.Start();

        var ev = new Event();
        bool quit = false;

        while (!quit)
        {
            // --- INPUT ---
            while (sdl.PollEvent(ref ev) != 0)
            {
                if (ev.Type == (uint)EventType.Quit)
                {
                    quit = true;
                    break;
                }

                if (ev.Type == (uint)EventType.Keydown)
                {
                    var key = (KeyCode)ev.Key.Keysym.Scancode;

                    // R = restart dupa game over
                    if (gameState.IsGameOver && key == KeyCode.R)
                    {
                        gameState = new GameState();
                        pendingDirection = null;
                        moveAccumulator = 0;
                        continue;
                    }

                    Direction? dir = key switch
                    {
                        KeyCode.Up    => Direction.Up,
                        KeyCode.Down  => Direction.Down,
                        KeyCode.Left  => Direction.Left,
                        KeyCode.Right => Direction.Right,
                        _ => null
                    };

                    if (dir.HasValue)
                    {
                        pendingDirection = dir;
                    }
                }
            }

            // --- UPDATE ---
            var elapsed = timer.Elapsed;
            timer.Restart();

            if (!gameState.IsGameOver)
            {
                moveAccumulator += elapsed.TotalMilliseconds;

                if (moveAccumulator >= MoveIntervalMs)
                {
                    moveAccumulator -= MoveIntervalMs; // NU resetam la zero, scadem intervalul

                    if (pendingDirection.HasValue)
                    {
                        gameState.HandleInput(pendingDirection.Value);
                        pendingDirection = null;
                    }

                    gameState.Update();
                }
            }

            // --- RENDER ---
            unsafe
            {
                var r = (Renderer*)renderer;

                sdl.SetRenderDrawColor(r, 30, 30, 30, 255);
                sdl.RenderClear(r);

                // capul e verde deschis, corpul e verde inchis
                var body = gameState.Snake.Body;
                for (int i = 0; i < body.Count; i++)
                {
                    if (i == 0)
                        sdl.SetRenderDrawColor(r, 100, 230, 100, 255);
                    else
                        sdl.SetRenderDrawColor(r, 50, 180, 50, 255);

                    FillCell(sdl, r, body[i].X * CellSize, body[i].Y * CellSize, CellSize);
                }

                sdl.SetRenderDrawColor(r, 220, 50, 50, 255);
                FillCell(sdl, r, gameState.Food.Position.X * CellSize, gameState.Food.Position.Y * CellSize, CellSize);

                sdl.RenderPresent(r);
            }

            // titlul ferestrei cu scorul
            var title = gameState.IsGameOver
                ? $"GAME OVER | Score: {gameState.Score} | Best: {gameState.HighScore} | R = restart"
                : $"Snake | Score: {gameState.Score} | Best: {gameState.HighScore}";

            unsafe
            {
                sdl.SetWindowTitle((Window*)window, title);
            }
        }

        unsafe
        {
            sdl.DestroyRenderer((Renderer*)renderer);
            sdl.DestroyWindow((Window*)window);
        }

        sdl.Quit();
    }

    private static unsafe void FillCell(Sdl sdl, Renderer* r, int x, int y, int cellSize)
    {
        int padding = 1;
        int px = x + padding;
        int py = y + padding;
        int size = cellSize - padding * 2;

        for (int row = 0; row < size; row++)
        {
            sdl.RenderDrawLine(r, px, py + row, px + size, py + row);
        }
    }
}