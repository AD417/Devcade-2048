using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Devcade2048.App;

// Code shamelessly improvised from Gabriele Cirulli's 2048. 
#nullable enable
public class Grid {
    public int Size { get; }
    public Tile?[,] Cells { get; }

    private static Random RNG = new Random();

    public Grid(int size, Tile[,] previousState) {
        Size = size;
        Cells = FromState(previousState);
    }

    public Grid(int size) {
        Size = size;
        Cells = Empty();
    }

    public Grid() : this(4) {}

    private Tile?[,] Empty() {
        Tile?[,] cells = new Tile[Size,Size];
        for (int x = 0; x < Size; x++) {
            for (int y = 0; y < Size; y++) {
                // TODO: Verify that x,y vs y,x is not an issue here.
                cells[x,y] = null;
            }
        }
        return cells;
    }

    private Tile?[,] FromState(Tile[,] state) {
        Tile?[,] cells = new Tile[Size, Size];
        for (int x = 0; x < Size; x++) {
            for (int y = 0; y < Size; y++) {
                Tile tile = state[x,y];
                cells[x,y] = (tile != null) ? new Tile(tile.Position, tile.Value) : null;
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

        void gatherAvailableCells(int x, int y, Tile? tile) {
            if (tile == null) {
                cells.Add(new Vector2(x,y));
            }
        }

        EachCell(gatherAvailableCells);

        return cells;
    }

    public void EachCell(Action<int, int, Tile?> callback) {
        for (int x = 0; x < Size; x++) {
            for (int y = 0; y < Size; y++) {
                callback(x, y, Cells[x,y]);
            }
        }
    }

    public bool CellsAvailable() {
        return (AvailableCells().Count > 0);
    }

    public bool CellAvailable(Vector2 cell) {
        return !CellOccupied(cell);
    }

    public bool CellOccupied(Vector2 cell) {
        return (CellContent(cell) != null);
    }

    public Tile? CellContent(Vector2 cell) {
        if (!WithinBounds(cell)) return null;
        return Cells[(int)cell.X,(int)cell.Y];
    }

    public bool WithinBounds(Vector2 position) {
        return (
            position.X >= 0
         && position.X < Size
         && position.Y >= 0
         && position.Y < Size
        );
    }

    public void InsertTile(Tile tile) {
        Vector2 pos = tile.Position;
        Console.WriteLine(pos);
        Cells[(int)pos.X,(int)pos.Y] = tile;
    }

    public void RemoveTile(Tile tile) {
        Vector2 pos = tile.Position;
        Cells[(int)pos.X,(int)pos.Y] = null;
    }
}