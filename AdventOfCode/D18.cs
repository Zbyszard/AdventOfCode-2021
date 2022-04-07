using System.Text;

namespace AdventOfCode;

public static class D18
{
    private static IEnumerable<string> GetInputLines() =>
        File.ReadAllLines("./Inputs/D18.txt");

    private static IEnumerable<Number> GetInput() =>
        GetInputLines().Select(ParseNumber);

    private static void AssignNextValue(Number number, int value)
    {
        var v = new Value { Literal = value };
        AssignNextValue(number, v);
    }

    private static void AssignNextValue(Number number, Number value)
    {
        var v = new Value { Complex = value };
        v.Complex.ContainingValue = v;
        AssignNextValue(number, v);
    }

    private static void AssignNextValue(Number number, Value value)
    {
        if (!number.Left.Assigned)
        {
            number.Left = value;
            value.ContainingNumber = number;
            return;
        }
        number.Right = value;
        value.ContainingNumber = number;
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
                    AssignNextValue(current, ParseNumber(str, ref i));
                    break;
                case '[':
                    stack.Push(current);
                    current = new();
                    current.Parent = stack.Peek();
                    current.Depth = current.Parent.Depth + 1;
                    break;
                case ']':
                    AssignNextValue(stack.Peek(), current);
                    current = stack.Pop();
                    break;
            }
        }
        return current;

        int ParseNumber(string str, ref int index)
        {
            StringBuilder digits = new();
            while (index < str.Length)
            {
                if (str[index] is < '0' or > '9')
                    break;

                digits.Append(str[index++]);
            }
            index--;
            return int.Parse(digits.ToString());
        }
    }

    private static Number? NumberToExplode(Number? num)
    {
        if (num is null) return null;
        if (num.Depth == 4) return num;

        Number? leftToExplode;
        leftToExplode = NumberToExplode(num.Left.Complex);

        if (leftToExplode is not null) return leftToExplode;

        return NumberToExplode(num.Right.Complex);
    }

    private static void AddToUpperLiteral(Number startingNum, Direction dir, int valueToAdd)
    {
        var parent = startingNum.Parent;
        if (parent is null) 
            return;

        var child = dir switch
        {
            Direction.Left => parent.Left,
            _ => parent.Right
        };

        if (!child.IsComplex)
        {
            child.Literal += valueToAdd;
            return;
        }

        if (child.Complex != startingNum)
        {
            AddToLowerLiteral(child.Complex!, dir == Direction.Left ? Direction.Right : Direction.Left, valueToAdd);
            return;
        }

        AddToUpperLiteral(parent, dir, valueToAdd);
    }

    private static void AddToLowerLiteral(Number startingNum, Direction dir, int valueToAdd)
    {
        var child = dir switch
        {
            Direction.Left => startingNum.Left,
            _ => startingNum.Right
        };

        if (!child.IsComplex)
        {
            child.Literal += valueToAdd;
            return;
        }

        AddToLowerLiteral(child.Complex!, dir, valueToAdd);
    }

    private static void Explode(Number num)
    {
        AddToUpperLiteral(num, Direction.Left, num.Left.Literal);
        AddToUpperLiteral(num, Direction.Right, num.Right.Literal);

        var value = num.ContainingValue!;
        value.Complex = null;
    }

    private static Value? ValueToSplit(Number startingNumber)
    {
        if (startingNumber.Left is { IsComplex: false, Literal: >= 10 })
            return startingNumber.Left;

        if (startingNumber.Left.IsComplex)
        {
            var lowerLeft = ValueToSplit(startingNumber.Left.Complex!);
            if (lowerLeft is not null)
                return lowerLeft;
        }

        if (startingNumber.Right is { IsComplex: false, Literal: >= 10 })
            return startingNumber.Right;

        if (startingNumber.Right.IsComplex)
        {
            var lowerRight = ValueToSplit(startingNumber.Right.Complex!);
            if (lowerRight is not null)
                return lowerRight;
        }

        return null;
    }

    private static void Split(Value value)
    {
        int left = (int)Math.Floor(value.Literal / 2.0);
        int right = (int)Math.Ceiling(value.Literal / 2.0);

        value.Literal = 0;
        value.Complex = new()
        {
            ContainingValue = value,
            Depth = value.ContainingNumber!.Depth + 1,
            Parent = value.ContainingNumber
        };
        value.Complex.Left = new()
        {
            ContainingNumber = value.Complex,
            Literal = left
        };
        value.Complex.Right = new()
        {
            ContainingNumber = value.Complex,
            Literal = right
        };
    }

    private static bool TryExplode(Number num)
    {
        var toExplode = NumberToExplode(num);
        if (toExplode is null)
            return false;

        Explode(toExplode);
        return true;
    }

    private static bool TrySplit(Number num)
    {
        var toSplit = ValueToSplit(num);
        if (toSplit is null)
            return false;

        Split(toSplit);
        return true;
    }

    private static void RecalculateDepth(Number num)
    {
        if (num.Parent is null)
            num.Depth = 0;
        else
            num.Depth = num.Parent.Depth + 1;

        if (num.Left.IsComplex)
            RecalculateDepth(num.Left.Complex!);

        if (num.Right.IsComplex)
            RecalculateDepth(num.Right.Complex!);
    }

    private static void Reduce(Number sum)
    {
        while (true)
        {
            if (TryExplode(sum))
                continue;

            if (TrySplit(sum))
                continue;

            break;
        }
    }

    private static Number Add(Number first, Number second)
    {
        Number sum = new()
        {
            Left = new Value { Complex = first },
            Right = new Value { Complex = second }
        };

        first.Parent = sum;
        second.Parent = sum;
        first.ContainingValue = sum.Left;
        second.ContainingValue = sum.Right;

        sum.Left.ContainingNumber = sum;
        sum.Right.ContainingNumber = sum;

        RecalculateDepth(sum);
        Reduce(sum);

        return sum;
    }

    private static int CalculateMagnitute(Number num)
    {
        int left = num.Left.IsComplex ? CalculateMagnitute(num.Left.Complex!) : num.Left.Literal;
        int right = num.Right.IsComplex ? CalculateMagnitute(num.Right.Complex!) : num.Right.Literal;

        return 3 * left + 2 * right;
    }

    public static int SolveA()
    {
        List<Number> nums = GetInput().ToList();
        var sum = nums.Skip(1)
            .Aggregate(nums[0], (partialSum, currentNum) => Add(partialSum, currentNum));
        Console.WriteLine(sum);
        return CalculateMagnitute(sum);
    }

    private static List<(Number first, Number second)> AllPairs(List<string> numbers)
    {
        List<(Number first, Number second)> result = new();
        foreach (var fst in numbers)
            foreach (var snd in numbers)
            {
                if (fst == snd)
                    continue;
                result.Add((ParseNumber(fst), ParseNumber(snd)));
            }
        return result;
    }

    public static int SolveB()
    {
        List<string> nums = GetInputLines().ToList();
        List<(Number first, Number second)> pairs = AllPairs(nums);
        List<Number> sums = pairs.Select(p => Add(p.first, p.second)).ToList();
        List<(Number sum, int magnitute)> magnitudes = sums.Select(s => (s, CalculateMagnitute(s))).ToList();

        return pairs.Select(p => CalculateMagnitute(Add(p.first, p.second))).Max();
    }

    private class Value
    {
        public bool Assigned { get; set; }
        public int Literal { get; set; }
        public bool IsComplex => Complex is not null;
        public Number? ContainingNumber { get; set; }
        public Number? Complex { get; set; }

        public override string ToString() =>
            Complex is null ? Literal.ToString() : Complex.ToString();
    }

    private class Number
    {
        private Value _left;
        private Value _right;
        public Value? ContainingValue { get; set; }
        public int Depth { get; set; }
        public Number? Parent { get; set; }
        public Value Left
        {
            get => _left;
            set => SetValue(ref _left, value);
        }
        public Value Right
        {
            get => _right;
            set => SetValue(ref _right, value);
        }

        public Number()
        {
            _left = new();
            _right = new();
        }

        public override string ToString()
        {
            string left = Left.ToString();
            string right = Right.ToString();
            return $"[{left},{right}]";
        }

        private void SetValue(ref Value target, Value v)
        {
            v.Assigned = true;
            target = v;
        }
    }
    private enum Direction
    {
        Left,
        Right
    }
}
