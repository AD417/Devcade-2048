using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Devcade2048.App;

// Code shamelessly improvised from Gabriele Cirulli's 2048. 
[Serializable]
public class Grid {
    private readonly int size;
    private readonly Tile[,] cells;

    public int Size => size;
    public Tile[,] Cells => cells;

    private static Random RNG = new Random();
    private static readonly int DEFAULT_DIMENSIONS = 4;

    public Grid(int size, Tile[,] previousState) {
        this.size = size;
        this.cells = FromState(previousState);
    }

    public Grid(int size) {
        this.size = size;
        this.cells = Empty();
    }

    public Grid() : this(DEFAULT_DIMENSIONS) {}

    private Tile[,] Empty() {
        Tile[,] cells = new Tile[size,size];
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                // TODO: Verify that x,y vs y,x is not an issue here.
                cells[x,y] = null;
            }
        }
        return cells;
    }

    private Tile[,] FromState(Tile[,] state) {
        Tile[,] cells = new Tile[size, size];
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                Tile tile = state[x,y];
                cells[x,y] = (tile != null) ? new Tile(tile.Position, tile.TextureId, tile.Value) : null;
            }
        }
        return cells;
    }

    public Vector2? RandomAvailableCell() {
        List<Vector2> cells = AvailableCells();
        if (cells.Count == 0) return null;
        return cells[RNG.Next() % cells.Count];
    }

    public List<Vector2> AvailableCells() {
        List<Vector2> cells = new List<Vector2>();

        void gatherAvailableCells(int x, int y, Tile tile) {
            if (tile == null) {
                cells.Add(new Vector2(x,y));
            }
        }

        EachCell(gatherAvailableCells);

        return cells;
    }

    public void EachCell(Action<int, int, Tile> callback) {
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                callback(x, y, cells[x,y]);
            }
        }
    }

    public bool CellsAvailable() {
        return AvailableCells().Count > 0;
    }

    public bool CellAvailable(Vector2 cell) {
        return !CellOccupied(cell);
    }

    public bool CellOccupied(Vector2 cell) {
        return CellContent(cell) != null;
    }

    public Tile CellContent(Vector2 cell) {
        if (!WithinBounds(cell)) return null;
        return cells[(int)cell.X,(int)cell.Y];
    }

    public bool WithinBounds(Vector2 position) {
        return 
            position.X >= 0
         && position.X < size
         && position.Y >= 0
         && position.Y < size
        ;
    }

    public void InsertTile(Tile tile) {
        Vector2 pos = tile.Position;
        cells[(int)pos.X,(int)pos.Y] = tile;
    }

    public void RemoveTile(Tile tile) {
        Vector2 pos = tile.Position;
        cells[(int)pos.X,(int)pos.Y] = null;
    }
}