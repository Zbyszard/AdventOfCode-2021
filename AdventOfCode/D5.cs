using System.Text.RegularExpressions;

namespace AdventOfCode;

public static class D5
{
    private static IEnumerable<Line> GetInput() => 
        new Regex(@"[^0-9\r\n]+")
        .Replace(File.ReadAllText("./Inputs/D5.txt"), " ")
        .Split("\r\n")
        .Select(s => s.Split(" ").Select(int.Parse).ToArray())
        .Select(nums => new Line(A: new Point(X: nums[0], Y: nums[1]), B: new Point(X: nums[2], Y: nums[3])));

    private static IEnumerable<Point> GetPoints(this Line line)
    {
        yield return line.A;

        if ((line.IsVertical || line.IsHorizontal) && line.Length > 1 || line.Length > Math.Sqrt(2))
        {
            int xDir = -Math.Sign(line.A.X - line.B.X);
            int yDir = -Math.Sign(line.A.Y - line.B.Y);
            for (int lastX = line.A.X + xDir, lastY = line.A.Y + yDir;
                lastX != line.B.X || lastY != line.B.Y;
                lastX += xDir, lastY += yDir)
                yield return new Point(lastX, lastY);
        }

        yield return line.B;
    }

    private static int Solve(IEnumerable<Line> lines)
    {
        Dictionary<Point, int> pointOccurrences = new();
        foreach (var line in lines)
        {
            foreach (var point in GetPoints(line))
            {
                if (pointOccurrences.ContainsKey(point))
                    pointOccurrences[point]++;
                else
                    pointOccurrences.Add(point, 1);
            }
        }
        return pointOccurrences.Count(po => po.Value > 1);
    }

    public static int SolveA() => Solve(GetInput().Where(l => l.IsVertical || l.IsHorizontal));
    public static int SolveB() => Solve(GetInput());

    public record struct Point(int X, int Y);
    public record struct Line(Point A, Point B)
    {
        public bool IsHorizontal => A.Y == B.Y;
        public bool IsVertical => A.X == B.X;
        public double Length => Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2));
    };
}
