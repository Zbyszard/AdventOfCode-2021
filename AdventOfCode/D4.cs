using System.Text.RegularExpressions;

namespace AdventOfCode;

public static class D4
{
    private static (int[] numbers, Board[] boards) GetInput()
    {
        string str = File.ReadAllText("./Inputs/D4.txt");
        int fstLineBreak = str.IndexOf("\r\n");
        string numsStr = str[..fstLineBreak];
        string boardsStr = str[(fstLineBreak + 1)..].Trim();

        int[] nums = numsStr.Split(',').Select(int.Parse).ToArray();
        Board[] boards = boardsStr.Split("\r\n\r\n")
            .Select(s => new Regex(@"\s+").Replace(s.Trim(), " ").Split(' ')
                .Select(int.Parse).ToArray()
            ).Select(values => new Board(values))
            .ToArray();

        return (nums, boards);
    }

    public static int SolveA()
    {
        (int[] numbers, Board[] boards) = GetInput();
        int markedNumbers = 0;
        foreach(int num in numbers)
        {
            markedNumbers++;
            foreach(var board in boards)
            {
                board.Mark(num);
                if (markedNumbers >= 5 && board.CheckWin())
                    return board.UnmarkedSum() * num;
            }
        }
        return 0;
    }

    public static int SolveB()
    {
        (int[] numbers, Board[] boards) = GetInput();
        int markedNumbers = 0;
        foreach (int num in numbers)
        {
            markedNumbers++;
            foreach (var board in boards.Where(b => !b.Won))
            {
                board.Mark(num);
                if (markedNumbers >= 5 && board.CheckWin() && !boards.Any(b => !b.Won))
                    return board.UnmarkedSum() * num;
            }
        }
        return 0;
    }

    private class Board
    {
        private Field[,] Fields { get; init; }
        public bool Won { get; private set; }

        public Board(int[] values)
        {
            Fields = new Field[5, 5];
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    Fields[i, j] = new Field { Value = values[i * 5 + j] };
        }

        public void Mark(int number)
        {
            var field = Fields.Cast<Field>().FirstOrDefault(f => f.Value == number);
            if (field is not null)
                field.Marked = true;
        }

        public int UnmarkedSum() => Fields.Cast<Field>()
            .Where(f => f.Marked == false)
            .Sum(f => f.Value);

        public bool CheckWin()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (!Fields[i, j].Marked) 
                        break;
                    if (j == 4)
                    {
                        Won = true;
                        return true;
                    }
                }
            }
            for (int j = 0; j < 5; j++)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (!Fields[i, j].Marked)
                        break;
                    if (i == 4)
                    {
                        Won = true;
                        return true;
                    }
                }
            }
            return false;
        }

        public class Field
        {
            public int Value { get; init; }
            public bool Marked { get; set; }
        }
    }
}

