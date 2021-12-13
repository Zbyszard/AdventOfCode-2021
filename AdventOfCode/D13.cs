using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode;

public static class D13
{
    private static (IEnumerable<Point> Sheet, IEnumerable<Fold> folds) GetInput()
    {
        string[] strs = File.ReadAllText("./Inputs/D13.txt").Split("\r\n\r\n");
        var folds = strs[1].Split("\r\n")
            .Select(s =>
            {
                int eqIndex = s.IndexOf('=');
                return new Fold(Along: s[eqIndex - 1], Value: int.Parse(s[(eqIndex + 1)..]));
            }).ToList();
        var sheet = strs[0].Split("\r\n")
            .Select(s =>
            {
                string[] xy = s.Split(',');
                return new Point(int.Parse(xy[0]), int.Parse(xy[1]));
            }).ToList();
        return (sheet, folds);
    }

    private static IEnumerable<Point>
        PerformFolds(IEnumerable<Point> sheet, IEnumerable<Fold> folds, int stopAfter = 0)
    {
        int i = 0;
        foreach (var fold in folds)
        {
            IEnumerable<Point> pointsToFold = fold.Along switch
            {
                'x' => sheet.Where(p => p.X > fold.Value),
                _ => sheet.Where(p => p.Y > fold.Value)
            };
            foreach (var point in pointsToFold)
            {
                if (fold.Along == 'x')
                    point.X = 2 * fold.Value - point.X;
                else
                    point.Y = 2 * fold.Value - point.Y;
            }
            sheet = sheet.Distinct();
            if (stopAfter == 0)
                continue;
            if (++i >= stopAfter)
                break;
        }
        return sheet;
    }

    public static int SolveA()
    {
        var (sheet, folds) = GetInput();
        return PerformFolds(sheet, folds, stopAfter: 1).Count();
    }

    private static void Print(IEnumerable<Point> sheet)
    {
        Console.BackgroundColor = ConsoleColor.White;
        var sortedPoints = sheet.OrderBy(p => p.X).ThenBy(p => p.Y);
        int xMax = sortedPoints.Max(p => p.X);
        int yMax = sortedPoints.Max(p => p.Y);
        for (int i = -1; i < yMax + 2; i++)
        {
            for (int j = -1; j < xMax + 2; j++)
            {
                var p = sortedPoints.SingleOrDefault(p => p.X == j && p.Y == i);
                if (p is not null)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(' ');
                    Console.BackgroundColor = ConsoleColor.White;
                }
                else
                    Console.Write(' ');
            }
            Console.WriteLine();
        }
        Console.ResetColor();
    }

    public static void SolveB()
    {
        var (sheet, folds) = GetInput();
        var code = PerformFolds(sheet, folds);
        Print(code);
    }

    private record struct Fold(char Along, int Value);

    private record Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}