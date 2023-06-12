using System;

namespace Devcade2048.App;

public static class DebugRender {
    public static void Write(Grid grid) {
        Console.WriteLine();

        String[] rows = new String[grid.Size];
        void AddContent(int x, int y, Tile t) {
            if (t != null) rows[y] += t.Value.ToString().PadLeft(4) + "  ";
            else rows[y] += "  --  ";
        }
        grid.EachCell(AddContent);

        foreach (String row in rows) {
            Console.WriteLine(row);
            Console.WriteLine();
        }
    }
}