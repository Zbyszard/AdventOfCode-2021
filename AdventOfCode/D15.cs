using System.Text.RegularExpressions;

namespace AdventOfCode;

public static class D15
{
    private static Point[,] GetInput()
    {
        string input = File.ReadAllText("./Inputs/D15.txt");
        int xLen = input.IndexOf(Environment.NewLine);
        int yLen = Regex.Matches(input, @"(?m)^").Count;

        string digits = input.Replace(Environment.NewLine, "");

        Point[,] points = new Point[xLen, yLen];
        for (int i = 0, k = 0; i < xLen; i++)
            for (int j = 0; j < yLen; j++, k++)
                points[i, j] = new Point(int.Parse(digits[k..(k + 1)]));
        return points;
    }

    // from D11
    private static int[] GetNeighbouringIndexes(int i, int dimLength) =>
        (i == 0, i == dimLength - 1) switch
        {
            (true, _) => new[] { i, i + 1 },
            (_, true) => new[] { i - 1, i },
            _ => new[] { i - 1, i, i + 1 }
        };

    private static void AssignNeighbours(Point[,] points)
    {
        int xLen = points.GetLength(0);
        int yLen = points.GetLength(1);
        for (int i = 0; i < xLen; i++)
            for (int j = 0; j < yLen; j++)
            {
                int[] iNeighbours = GetNeighbouringIndexes(i, xLen);
                int[] jNeighbours = GetNeighbouringIndexes(j, yLen);
                foreach (int ii in iNeighbours)
                    foreach (int jj in jNeighbours)
                    {
                        if (i == ii && j == jj
                            || Math.Sqrt(Math.Pow(i - ii, 2) + Math.Pow(j - jj, 2)) == Math.Sqrt(2))
                            continue;
                        points[i, j].Neighbours.Add(points[ii, jj]);
                    }
            }
    }

    private static int DijkstraA(IEnumerable<Point> points, Point initialPoint, Point endPoint)
    {
        HashSet<Point> unvisited = new(points);
        initialPoint.RiskFromStart = 0;
        var current = initialPoint;
        while(unvisited.Any())
        {
            foreach (var point in current.Neighbours)
            {
                int risk = current.RiskFromStart + point.Risk;
                if (risk < point.RiskFromStart)
                    point.RiskFromStart = risk;
            }
            if (current == endPoint)
                break;
            unvisited.Remove(current);
            current = unvisited.MinBy(p => p.RiskFromStart)!;
        }
        return endPoint.RiskFromStart;
    }

    public static int SolveA()
    {
        var points = GetInput();
        AssignNeighbours(points);
        var iePoints = points.Cast<Point>();
        return DijkstraA(iePoints, iePoints.First(), iePoints.Last());
    }

    private static Point[,] GetLargerMap(Point[,] points, int scale)
    {
        int xLen = points.GetLength(0);
        int yLen = points.GetLength(1);
        Point[,] newMap = new Point[xLen * scale, yLen * scale];
        for (int i = 0; i < xLen; i++)
            for (int j = 0; j < yLen; j++)
                for (int ii = 0, inc = 0; ii < scale; ii++, inc -= 4)
                    for (int jj = 0; jj < scale; jj++, inc++)
                    {
                        int risk = points[i, j].Risk + inc;
                        while (risk > 9)
                            risk -= 9;
                        newMap[i + xLen * ii, j + yLen * jj] = new Point(risk);
                    }
        return newMap;
    }

    private static int DijkstraB(IEnumerable<Point> points, Point initialPoint, Point endPoint)
    {
        initialPoint.RiskFromStart = 0;
        PriorityQueue<Point, int> unvisited = new(points.Select(p => (p, p.RiskFromStart)));
        var current = unvisited.Dequeue();
        while (unvisited.Count > 0)
        {
            foreach (var point in current.Neighbours)
            {
                int risk = current.RiskFromStart + point.Risk;
                if (risk < point.RiskFromStart)
                {
                    point.RiskFromStart = risk;
                    unvisited.Enqueue(point, point.RiskFromStart);
                }
            }
            if (current == endPoint)
                break;
            current = unvisited.Dequeue();
        }
        return endPoint.RiskFromStart;
    }

    public static int SolveB()
    {
        var points = GetLargerMap(GetInput(), scale: 5);
        AssignNeighbours(points);
        var iePoints = points.Cast<Point>();
        return DijkstraB(iePoints, iePoints.First(), iePoints.Last());
    }

    private class Point
    {
        public int Risk { get; init; }
        public ICollection<Point> Neighbours { get; init; }
        public Point? NextOnShortestPath { get; set; }
        public int RiskFromStart { get; set; }
        public bool OnShortestPath { get; set; }

        private Point()
        {
            Neighbours = new List<Point>();
            RiskFromStart = int.MaxValue;
        }

        public Point(int risk) : this()
        {
            Risk = risk;
        }
    };
}