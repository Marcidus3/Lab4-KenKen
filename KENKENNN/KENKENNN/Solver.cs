using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static KENKENNN.EnumUtils;

namespace KENKENNN
{
    public class KenKenBoard
    {
        public int[,] State { get; private set; } = new int[Constants.MapSize, Constants.MapSize];

        // These two variables track which numbers exist in a row/column.
        bool[,] rowHash = new bool[Constants.MapSize, Constants.MapSize];
        bool[,] colHash = new bool[Constants.MapSize, Constants.MapSize];

        public bool CanPlace(Point p, int val)
        {
            int valMinusOne = val - 1;
            return !rowHash[p.Y, valMinusOne] && !colHash[p.X, valMinusOne];
        }

        public void Print()
        {
            for (int y = 0; y < Constants.MapSize; y++)
            {
                for (int x = 0; x < Constants.MapSize; x++)
                {
                    Console.Write("{0} ", State[x, y]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void UnsetPlace(Point p)
        {
            int valMinusOne = State[p.X, p.Y] - 1;
            rowHash[p.Y, valMinusOne] = false;
            colHash[p.X, valMinusOne] = false;
            State[p.X, p.Y] = 0;
        }

        public bool TryPlace(Point p, int val)
        {
            int valMinusOne = val - 1;
            if (!CanPlace(p, val))
                return false;

            rowHash[p.Y, valMinusOne] = true;
            colHash[p.X, valMinusOne] = true;
            State[p.X, p.Y] = val;

            return true;
        }
    }

    // регион с ячейками
    public class Region
    {
        public Operator Operator { get; set; }

        // value of a region.
        public int Value { get; set; }

        // ячейки в регионе
        public List<Point> Cells { get; private set; } = new List<Point>();

        // Solutions of a region.
        public List<List<int>> Solutions { get; private set; }

        // Constructor
        public Region(Operator op, int val)
        {
            Operator = op;
            Value = val;
        }

        public override string ToString()
        {
            return $"[SubProblem: Operator={Operator}, Value={Value}, Squares={Cells}]";
        }

        // Reduce this solution set given a board state.
        public Region Prune(KenKenBoard board)
        {
            var result = new Region(Operator, Value);
            
            result.Solutions = new List<List<int>>();
            result.Cells = Cells;

            // Iterate through every solution.
            foreach (var solution in Solutions)
            {
                bool success = true;
                // See if we can place this solution on the board.
                for (int i = 0; i < Cells.Count; i++)
                {
                    if (!board.CanPlace(Cells[i], solution[i]))
                    {
                        success = false;
                        break;
                    }
                }

                if (success)
                    result.Solutions.Add(solution);
            }

            // No solutions means fail
            return Solutions.Any() ? result : null;
        }

        public static bool IsValid(int val)
        {
            return val > 0 && val <= Constants.MapSize;
        }

        bool AddExplode(List<int> solutionCandidates, int currentVal)
        {
            if (currentVal > Value)
                return true;

            if (solutionCandidates == null)
                solutionCandidates = new List<int>();

            if (solutionCandidates.Count == Cells.Count - 1)
            {
                int val = Value - currentVal;
                if (IsValid(val))
                {
                    List<int> copy = new List<int>(solutionCandidates);
                    copy.Add(val);
                    Solutions.Add(copy);

                    return true;
                }
                return val <= Constants.MapSize;
            }

            for (int i = Constants.MapSize; i >= 1; i--)
            {
                solutionCandidates.Add(i);
                bool ret = AddExplode(solutionCandidates, currentVal + i);
                solutionCandidates.RemoveAt(solutionCandidates.Count - 1);
                if (!ret)
                    break;
            }
            return true;
        }

        bool MultExplode(List<int> solutionCandidates, int currentVal)
        {
            if (currentVal > Value)
                return true;

            if (solutionCandidates == null)
                solutionCandidates = new List<int>();

            if (solutionCandidates.Count == Cells.Count - 1)
            {
                int val = Value / currentVal;
                if (IsValid(val))
                {
                    if (Value % currentVal != 0)
                        return true;
                    List<int> copy = new List<int>(solutionCandidates);
                    copy.Add(val);
                    Solutions.Add(copy);

                    return true;
                }
                return val <= Constants.MapSize;
            }

            for (int i = Constants.MapSize; i >= 1; i--)
            {
                solutionCandidates.Add(i);
                bool ret = MultExplode(solutionCandidates, currentVal * i);
                solutionCandidates.RemoveAt(solutionCandidates.Count - 1);
                if (!ret)
                    break;
            }
            return true;
        }

        // Generate the possible solutions, given a bunch of squares, operator, and a final value.
        public void GenerateSolutions()
        {
            if (Solutions != null)
                return;

            Solutions = new List<List<int>>();

            switch (Operator)
            {
                case Operator.Add:
                    // get all possible combinations for addition operation
                    AddExplode(null, 0);
                    break;
                case Operator.Sub:
                    // get all possible combinations for subtraction operation
                    for (int a = 1; a <= Constants.MapSize; a++)
                    {
                        int b = Math.Abs(Value - a);
                        if (IsValid(b))
                        {
                            Solutions.Add(new List<int> { a, b });
                            Solutions.Add(new List<int> { b, a });
                        }
                    }
                    break;
                case Operator.Div:
                    for (int a = 1; a <= Constants.MapSize; a++)
                    {
                        int b = a / Value;
                        if (IsValid(b) &&
                            (a % Value == 0))
                        {
                            Solutions.Add(new List<int> { a, b });
                            Solutions.Add(new List<int> { b, a });
                        }
                    }
                    break;
                case Operator.Mul:
                    MultExplode(null, 1);
                    break;
                case Operator.Const:
                    // only one solution is possible
                    Solutions.Add(new List<int> { Value });
                    break;
            }
        }
    }

    public class Problem
    {
        List<Region> AllRegions = new List<Region>();

        public Region[,] Regions { get; } =
            new Region[Constants.MapSize, Constants.MapSize];

        // Filter out solutions that are no longer valid given the current board.
        public Problem Prune(KenKenBoard board)
        {
            var result = new Problem();

            for (int i = 1; i < AllRegions.Count; i++)
            {
                var region = AllRegions[i];
                var resultRegion = region.Prune(board);

                // If there are no solutions for this pruned Region,
                // this Problem failed.
                if (resultRegion == null)
                    return null;

                result.AllRegions.Add(resultRegion);
            }

            return result;
        }

        public KenKenBoard Solve(KenKenBoard board)
        {
            if (!AllRegions.Any())
                return board;

            // Get the next SubProblem that we want to try.
            var region = AllRegions[0];
            var cells = region.Cells;

            // Iterate through all the solutions.
            foreach (var solution in region.Solutions)
            {
                for (int i = 0; i < solution.Count; i++)
                {
                    // No need to check the result, since the solution
                    // set is pruned to only valid solutions.
                    board.TryPlace(cells[i], solution[i]);
                }
                // Prune the board with this solution.
                var newProb = Prune(board);
                if (newProb != null)
                {
                    // Resort based on the new pruned solutions.
                    newProb.Sort();

                    var ret = newProb.Solve(board);
                    if (ret != null)
                        return ret;
                }
                // Undo board state change.
                foreach (Point p in cells)
                    board.UnsetPlace(p);
            }
            // No solution in this branch!
            return null;
        }

        public KenKenBoard Solve()
        {
            // Just do some sanity checking to make sure every cell is part of a subproblem.
            for (int x = 0; x < Constants.MapSize; x++)
            {
                for (int y = 0; y < Constants.MapSize; y++)
                {
                    if (Regions[x, y] == null)
                        Console.WriteLine($"Empty subproblem at: [{x}, {y}]");

                    Regions[x, y].Cells.Add(new Point(x, y));
                }
            }

            // Generate the solution set for each SubProblem.
            for (int x = 0; x < Constants.MapSize; x++)
            {
                for (int y = 0; y < Constants.MapSize; y++)
                {
                    Regions[x, y].GenerateSolutions();
                }
            }

            // Grab all the unique subproblems from the [Constants.MapSize x Constants.MapSize] board.
            var regions = new Dictionary<Region, int>();
            Region tempSub;
            for (int x = 0; x < Constants.MapSize; x++)
            {
                for (int y = 0; y < Constants.MapSize; y++)
                {
                    tempSub = Regions[x, y];
                    if (!regions.ContainsKey(tempSub))
                    {
                        regions.Add(tempSub, tempSub.Cells.Count);
                    }
                }
            }

            // Add only unique subproblems to a list.
            foreach (var val in regions.Keys)
            {
                // Filter out invalid solutions, ie, 6+ = 2 2 2 would never be valid.
                foreach (var solution in new List<List<int>>(val.Solutions))
                {
                    var tempBoard = new KenKenBoard();
                    for (int i = 0; i < solution.Count; i++)
                    {
                        if (!tempBoard.TryPlace(val.Cells[i], solution[i]))
                        {
                            val.Solutions.Remove(solution);
                            break;
                        }
                    }
                }
                AllRegions.Add(val);
            }
            // Sort the list in order of increasing number of possible solutions.
            Sort();

            // Attempt to solve with a empty KenKenBoard.
            return Solve(new KenKenBoard());
        }

        void Sort()
        {
            // Sort the subproblems by lowest number of valid solutions, to highest.
            // Prioritizing subproblems with less solutions prunes the tree faster.
            AllRegions.Sort(
                new Comparison<Region>((x, y) =>
                {
                    return x.Solutions.Count.CompareTo(y.Solutions.Count);
                }
            ));
        }
    }
}
