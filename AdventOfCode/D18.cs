namespace AdventOfCode;

public static class D18
{
    private static IEnumerable<Number> GetInput() =>
        File.ReadAllLines("./Inputs/D18.txt")
        .Select(ParseNumber);

    private static void AssignNextValue(Number number, char value)
    {
        var v = new Value { Literal = int.Parse(value.ToString()) };
        AssignNextValue(number, v);
    }

    private static void AssignNextValue(Number number, Number value)
    {
        var v = new Value { Complex = value };
        AssignNextValue(number, v);
    }

    private static void AssignNextValue(Number number, Value value)
    {
        if (!number.Left.Assigned)
        {
            number.Left = value;
            return;
        }
        number.Right = value;
    }

    private static Number ParseNumber(string str)
    {
        Stack<Number> stack = new();
        Number current = new();
        for (int i = 1; i < str.Length - 1; i++)
        {
            switch (str[i])
            {
                case >= '0' and <= '9':
                    AssignNextValue(current, str[i]);
                    break;
                case '[':
                    stack.Push(current);
                    current = new();
                    current.Parent = stack.Peek();
                    break;
                case ']':
                    AssignNextValue(stack.Peek(), current);
                    current = stack.Pop();
                    break;
            }
        }
        return current;
    }

    public static int SolveA()
    {
        var i = GetInput().ToList();
        return 0;
    }

    public static int SolveB()
    {
        return 0;
    }

    private struct Value
    {
        public bool Assigned { get; set; }
        public int Literal { get; init; }
        public Number? Complex { get; init; }

        public override string ToString() =>
            Complex is null ? Literal.ToString() : Complex.ToString();
    }

    private class Number
    {
        private Value _left;
        private Value _right;
        public int Depth { get; set; }
        public Number? Parent { get; set; }
        public Value Left
        {
            get => _left;
            set => _left = SetValue(value);
        }
        public Value Right
        {
            get => _right;
            set => _right = SetValue(value);
        }

        public override string ToString()
        {
            string left = Left.ToString();
            string right = Right.ToString();
            return $"[{left},{right}]";
        }

        private Value SetValue(Value v)
        {
            v.Assigned = true;
            return v;
        }
    }
}
