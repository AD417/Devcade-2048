using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Devcade2048.App.Render;
using Devcade;
using System.Text.Json;

namespace Devcade2048.App;

[Serializable]
public class Manager {
    private readonly int size;
    private int score;
    [NonSerialized]
    private int scoreDelta;
    [NonSerialized]
    private Grid grid;

    public int Size => size;
    public Grid Grid { get => grid; }
    public int Score { get => score; }

    // TODO: remove this and make it local to 
    public int ScoreDelta { get => scoreDelta; }

    // TODO: figure out how to handle InMenu. 
    [NonSerialized]
    public GameState State;
    private static readonly Random RNG = new();

    public enum Direction { 
        Up,
        Down, 
        Left, 
        Right, 
        None 
    }

    internal class MoveData {
        public Vector2 Farthest { get; }
        public Vector2 Next { get; }

        public MoveData(Vector2 farthest, Vector2 next) {
            Farthest = farthest;
            Next = next;
        }
    }

    public Manager(int size) {
        this.size = size;
        // Setup();
        State = GameState.Playing;
    }

    public void Continue() {
        State = GameState.Continuing;
    }

    public bool IsGameTerminated() {
        return State == GameState.Lost || State == GameState.Won;
    }

    public void NewGame() {

        grid = new Grid(size);
        score = 0;
        AddStartTiles();
        scoreDelta = 0;
        State = GameState.Playing;
    }

    public void LoadGame() {
        try {
            Manager man = Persistence.LoadSync<ManagerSerializable>("2blob48", "grid", new JsonSerializerOptions())
                .ToManager();

            grid = man.grid;
            score = man.score;

        } catch (Exception e) {
            Console.Error.WriteLine("Whoops! Can't load the game!");
            Console.Error.WriteLine(e);
            NewGame();
        }
        State = GameState.Playing;
    }

    public void Setup() {
        if (Config.LoadGame) {
        }

        if (!Config.LoadGame) {
            grid = new Grid(size);
            score = 0;
            AddStartTiles();
        }
        scoreDelta = 0;
        State = GameState.Playing;
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
        if (RNG.NextSingle() < Config.FourChance) {
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
        // Nothing...?
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

    public bool Move(Direction direction) {
        if (IsGameTerminated()) return false;
        if (direction == Direction.None) return false;

        Tile tile;

        scoreDelta = 0;
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
                Tile merged = new Tile(positions.Next, tile.TextureId + 1, tile.Value * 2) {
                    MergedFrom = new Tile[] { tile, next }
                };

                Grid.InsertTile(merged);
                Grid.RemoveTile(tile);

                tile.UpdatePosition(positions.Next);

                score += merged.Value;
                scoreDelta += merged.Value;

                if (merged.Value == 2048 && State == GameState.Playing) State = GameState.Won;
            } else {
                MoveTile(tile, positions.Farthest);
            }

            if (cell != tile.Position) moved = true;
            HighScoreTracker.Update(Score);
        }

        if (moved) {
            AddRandomTile();

            if (!MovesAvailable()) {
                State = GameState.Lost;
                HighScoreTracker.Save();
            }
            if (scoreDelta > 0) ScoreContainer.Add(scoreDelta);
            Actuate();
        }
        return moved;
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
        int[] xOrder = new int[size];
        int[] yOrder = new int[size];

        for (int i = 0; i < size; i++) {
            xOrder[i] = vector.X == 1 ? (size - 1 - i) : i;
            yOrder[i] = vector.Y == 1 ? (size - 1 - i) : i;
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

    public bool MovesAvailable() {
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
                Tile other = Grid.CellContent(cell + vector);
                if (other != null && other.Value == tile.Value) {
                    return true;
                }
            }
        }
        return false;
    }

    [Serializable]
    private class ManagerSerializable {
        public int Score { get; set; }
        public int[] Tiles { get; set; }

        public ManagerSerializable(Manager man) {
            Score = man.score;
            Tiles = new int[16];
            for (int i = 0; i < 16; i++) {
                int x = i % 4;
                int y = i / 4;
                Tiles[i] = man.grid.CellContent(new Vector2(x, y))?.TextureId ?? -1;
            }
        }

        public ManagerSerializable(int score, int[] tiles) {
            Score = score;
            Tiles = tiles;
        }

        public ManagerSerializable() {
            Score = 0;
            Tiles = new int[0];
        }

        public Manager ToManager() {
            Manager man = new Manager(4);
            Tile[,] tiles = new Tile[4,4];

            for (int i = 0; i < 16; i++) {
                int x = i % 4;
                int y = i / 4;
                if (Tiles[i] == -1) {
                    tiles[x,y] = null;
                    continue;
                }
                tiles[x,y] = new Tile(new Vector2(x, y), Tiles[i], 2 << Tiles[i]);
            }

            man.grid = new Grid(4, tiles);
            man.score = Score;
            return man;
        }
    }

    public void Export() {
        ManagerSerializable export = new(this);
        _ = Persistence.SaveSync<ManagerSerializable>("2blob48", "grid", export, new JsonSerializerOptions());
        Persistence.Flush().Wait();
    }
}