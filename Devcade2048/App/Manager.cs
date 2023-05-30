using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Devcade2048.App;

public class Manager {
    public int Size { get; }
    public Grid Grid { get; private set; }
    public int Score { get; private set; }

    public int ScoreDelta { get; private set; }

    private static Random RNG = new Random();
    // TODO: figure out how to handle InMenu. 

    public GameState State;

    public enum Direction { Up, Down, Left, Right, None }

    internal class MoveData {
        public Vector2 Farthest { get; }
        public Vector2 Next { get; }

        public MoveData(Vector2 farthest, Vector2 next) {
            Farthest = farthest;
            Next = next;
        }
    }

    public Manager(int size) {
        Size = size;
        Setup();
    }

    public void Restart() {
        // Render.ContinueGame();
        State = GameState.Continuing;
    }

    public bool IsGameTerminated() {
        return (State == GameState.Lost || State == GameState.Won);
    }

    public void Setup() {
        Grid = new Grid(Size);
        Score = 0;
        ScoreDelta = 0;
        State = GameState.Playing;
        AddStartTiles();

        Actuate();
    }

    public void AddStartTiles() {
        for (int i = 0; i < Config.StartTiles; i++) {
            AddRandomTile();
        }
    }

    public void AddRandomTile() {
        if (!Grid.CellsAvailable()) return;
        int value, id;
        if (RNG.NextDouble() < Config.FourChance) {
            value = 4;
            id = 1;
        } else {
            value = 2;
            id = 0;
        }
        Vector2? available = Grid.RandomAvailableCell();
        if (available != null) {
            Tile tile = new Tile((Vector2)available, id, value);
            Grid.InsertTile(tile);
        }
    }

    public void Actuate() {
        // TODO
    }

    public void PrepareTiles() {
        void prepareTile(int x, int y, Tile tile) {
            if (tile == null) return;
            tile.MergedFrom = null;
            tile.SavePosition();
        }

        Grid.EachCell(prepareTile);
    }

    public void MoveTile(Tile tile, Vector2 cell) {
        Grid.Cells[(int)tile.Position.X,(int)tile.Position.Y] = null;
        Grid.Cells[(int)cell.X,(int)cell.Y] = tile;
        tile.UpdatePosition(cell);
    }

    public void Move(Direction direction) {
        if (IsGameTerminated()) return;
        if (direction == Direction.None) return;

        Tile tile;

        ScoreDelta = 0;
        Vector2 vector = GetVector(direction);
        List<Vector2> traversal = BuildTraversals(vector);
        bool moved = false;

        PrepareTiles();

        foreach (Vector2 cell in traversal) {
            tile = Grid.CellContent(cell);
            if (tile == null) continue;

            MoveData positions = FindFarthestPosition(cell, vector);
            Tile next = Grid.CellContent(positions.Next);

            if (next != null && next.Value == tile.Value && next.MergedFrom == null) {
                Tile merged = new Tile(positions.Next, tile.TextureId + 1, tile.Value * 2);
                merged.MergedFrom = new Tile[] {tile, next};

                Grid.InsertTile(merged);
                Grid.RemoveTile(tile);

                tile.UpdatePosition(positions.Next);

                Score += merged.Value;
                ScoreDelta += merged.Value;

                if (merged.Value == 2048) State = GameState.Won;
            } else {
                MoveTile(tile, positions.Farthest);
            }

            if (cell != tile.Position) moved = true;
            HighScoreTracker.Update(Score);
        }

        if (moved) {
            AddRandomTile();

            if (!this.MovesAvailable()) {
                State = GameState.Lost;
                HighScoreTracker.Save();
            }

            Actuate();
        }
    }

    public Vector2 GetVector(Direction direction) {
        switch (direction) {
            case Direction.Up:      return new Vector2(0, -1);
            case Direction.Down:    return new Vector2(0, 1);
            case Direction.Left:    return new Vector2(-1, 0);
            case Direction.Right:   return new Vector2(1, 0);
            case Direction.None:   return new Vector2(0, 0);
        }
        throw new IndexOutOfRangeException();
    }

    private List<Vector2> BuildTraversals(Vector2 vector) {
        List<Vector2> output = new List<Vector2>();
        int[] xOrder = new int[Size];
        int[] yOrder = new int[Size];

        for (int i = 0; i < Size; i++) {
            xOrder[i] = vector.X == 1 ? (Size - 1 - i) : i;
            yOrder[i] = vector.Y == 1 ? (Size - 1 - i) : i;
        }

        foreach (int x in xOrder) {
            foreach(int y in yOrder) {
                output.Add(new Vector2(x,y));
            }
        }

        return output;
    }

    private MoveData FindFarthestPosition(Vector2 cell, Vector2 vector) {
        Vector2 previous;

        do {
            previous = cell;
            cell += vector;
        } while (Grid.WithinBounds(cell) && Grid.CellAvailable(cell));

        return new MoveData(previous, cell);
    }

    private bool MovesAvailable() {
        return Grid.CellsAvailable() || TileMatchesAvailable();
    }

    private bool TileMatchesAvailable() {
        Tile tile;
        foreach (Vector2 cell in BuildTraversals(new Vector2(1, 0))) {
            tile = Grid.CellContent(cell);
            if (tile == null) continue;
            foreach (Direction dir in Enum.GetValues(typeof(Direction))) {
                if (dir == Direction.None) continue;
                Vector2 vector = GetVector(dir);
                Tile other = this.Grid.CellContent(cell + vector);
                if (other != null && other.Value == tile.Value) {
                    System.Console.Write(cell);
                    System.Console.WriteLine(vector);
                    return true;
                }
            }
        }
        return false;
    }
}