namespace AdventOfCode;

public static class D17
{
    private static Target GetInput()
    {
        string str = File.ReadAllText("./Inputs/D17.txt");
        string xStrRange = str[(str.IndexOf("x=") + 2)..str.IndexOf(",")];
        string yStrRange = str[(str.IndexOf("y=") + 2)..];
        string[] xs = xStrRange.Split("..");
        string[] ys = yStrRange.Split("..");

        var x = new Range(int.Parse(xs[0]), int.Parse(xs[1]));
        var y = new Range(int.Parse(ys[0]), int.Parse(ys[1]));
        return new(x, y);
    }

    private static IEnumerable<Point> ProbePositions(Probe probe)
    {
        List<Point> probePositions = new();
        Point first = probe.Position;
        probePositions.Add(first);

        while (probe.Position.Y > probe.Target.Y.From)
        {
            probe.Step();
            probePositions.Add(probe.Position);
        }
        return probePositions;
    }

    public static int SolveA()
    {
        Target t = GetInput();
        int vY = -t.Y.From - 1;
        return ProbePositions(new(t, 0, vY)).Max(p => p.Y);
    }

    private static IEnumerable<Velocity> FindEveryVelocity(Target t)
    {
        List<Velocity> velocities = new();
        var horDirection = t.X.From > 0 ? HorizontalDirection.Right : HorizontalDirection.Left;
        for (int vy = -t.Y.From - 1; vy >= t.Y.From; vy--)
        {
            for (int vx = horDirection == HorizontalDirection.Right ? t.X.To : t.X.From; ; vx--)
            {
                var positions = ProbePositions(new(t, vx, vy));
                if (positions.Any(t.PointInTarget))
                    velocities.Add(new (vx, vy));

                if (horDirection == HorizontalDirection.Right ?
                    positions.All(p => p.X < t.X.From) :
                    positions.All(p => p.X < t.X.To))
                    break;
            }
        }
        return velocities;
    }

    public static int SolveB()
    {
        Target t = GetInput();
        return FindEveryVelocity(t).Distinct().Count();
    }

    private enum HorizontalDirection
    {
        Left,
        Right
    }

    private class Probe
    {
        public Target Target { get; init; }
        public Point Position { get; private set; }
        public int VX { get; private set; }
        public int VY { get; private set; }

        public Probe(Target target, int vX, int vY)
        {
            Target = target;
            VX = vX;
            VY = vY;
        }

        public void Step()
        {
            Position = new(Position.X + VX, Position.Y + VY);
            VX -= VX == 0 ? 0 : Math.Sign(Position.X);
            VY--;
        }
    }

    private record struct Velocity(int VX, int VY);
    private record struct Point(int X, int Y);
    private record Target(Range X, Range Y)
    {
        public bool PointInTarget(Point p)
        {
            bool inXRange = p.X >= X.From && p.X <= X.To;
            bool inYRange = p.Y >= Y.From && p.Y <= Y.To;
            return inXRange && inYRange;
        }
    }
    private record Range(int From, int To);
}
