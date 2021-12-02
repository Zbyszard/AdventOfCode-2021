namespace AdventOfCode;

public static class D2
{
    private enum Direction
    {
        Forward,
        Up,
        Down
    }
    private static IEnumerable<Move> GetInput() =>
        File.ReadAllText("./Inputs/D2.txt")
        .Split("\r\n")
        .Select(line =>
        {
            var split = line.Split(' ');
            var direction = split[0] switch
            {
                "forward" => Direction.Forward,
                "up" => Direction.Up,
                "down" => Direction.Down,
                _ => throw new ArgumentException()
            };
            return new Move(direction, int.Parse(split[1]));
        });

    public static int SolveA()
    {
        var moves = GetInput();
        int depth = moves.Aggregate(0, (actDepth, move) => actDepth + move.Direction switch
        {
            Direction.Down => move.Value,
            Direction.Up => -move.Value,
            _ => 0
        });
        int horizontalPos = moves.Where(m => m.Direction == Direction.Forward).Sum(m => m.Value);
        return depth * horizontalPos;
    }

    public static int SolveB() => GetInput()
        .Aggregate(new Position(), (pos, move) =>
        {
            if (move.Direction == Direction.Forward)
            {
                pos.Horizontal += move.Value;
                pos.Depth += pos.Aim * move.Value;
            }
            else
                pos.Aim += move.Direction == Direction.Down ? move.Value : -move.Value;
            return pos;
        }, pos => pos.Depth * pos.Horizontal);

    private record Move(Direction Direction, int Value);

    private class Position
    {
        public int Horizontal { get; set; }
        public int Depth { get; set; }
        public int Aim { get; set; }
    }
}
