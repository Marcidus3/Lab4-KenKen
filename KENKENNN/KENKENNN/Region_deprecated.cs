using System;
using System.Collections.Generic;
using System.Linq;
using static KENKENNN.EnumUtils;

namespace KENKENNN
{
    // not used.
    public class Region_deprecated
    {
        public Operator Operation { get; set; }
        public List<Cell> Cells { get; set; } = new List<Cell>();
        public int RegionValue { get; set; }
        public HashSet<int> RegionCandidates { get; set; } = new HashSet<int>();

        private readonly List<int> defaultCandidates =
            new List<int> { 1, 2, 3, 4 };
        private int debugCount = 0;

        public Region_deprecated() { }
        public Region_deprecated(int regionValue, Operator operation, List<Cell> neighbors)
        {
            RegionValue = regionValue;
            Operation = operation;
            Cells = neighbors;
            FindCandidates();
            Cells.ForEach(cell => cell.Candidates = RegionCandidates.ToList());
            //Cells.ForEach(cell => cell.Candidates = GetCandidates());
        }

        public void FindCandidates()
        {
            var arr = new int[Cells.Count];

            Func<int[], int> operationCallback;
            switch(Operation)
            {
                case Operator.Add:
                    operationCallback = CalculateSum;
                    break;
                case Operator.Mul:
                    operationCallback = CalculateMultiplication;
                    break;
                case Operator.Sub:
                    operationCallback = CalculateSubstraction;
                    break;
                case Operator.Div:
                default:
                    operationCallback = (arg) => { return -1; };
                    break;
            }

            #region Внимание, костыли!
            if (RegionValue == operationCallback(arr))
            {
                PutCandidatesToSet(arr);
                //Console.Write("Bingo!\t");
                //PrintArray(arr);
            }
            //PrintArray(arr);
            GenerateCandidatesRecursively(arr, 0, operationCallback);
            #endregion
        }

        #region Внимание, костыли!
        public void GenerateCandidatesRecursively(int[] arr, int ix, Func<int[], int> callback)
        {
            if (ix == arr.Length)
            {
                return;
            }
            else
            {
                for (int i = 0; i < Constants.MapSize; i++)
                {
                    GenerateCandidatesRecursively(arr, ix + 1, callback);
                    arr[ix] += 1;

                    if (!arr.Contains(Constants.MapSize))
                    {
                        if (RegionValue == callback(arr))
                        {
                            PutCandidatesToSet(arr);
                            //Console.Write("Bingo!\t");
                            //PrintArray(arr);
                        }
                        //PrintArray(arr);
                    }
                }
                arr[ix] = 0;
            }
        }

        private void PutCandidatesToSet(int[] candidates)
        {
            foreach (var candidate in candidates)
            {
                RegionCandidates.Add(candidate + 1);
            }
        }

        private int CalculateSum(int[] arr)
        {
            var result = 0;
            foreach (var element in arr)
            {
                result += (element + 1);
            }

            return result;
        }

        private int CalculateMultiplication(int[] arr)
        {
            var result = 1;
            foreach (var element in arr)
            {
                result *= (element + 1);
            }

            return result;
        }

        private int CalculateSubstraction(int[] arr)
        {
            var reversedList = arr.ToList();
            reversedList.Sort();
            reversedList.Reverse();

            var result = reversedList.First() + 1;
            foreach (var elem in reversedList.Skip(1))
            {
                if (result < (elem + 1))
                {
                    break;
                }
                result -= (elem + 1);
            }

            return result;
        }

        private void PrintArray(int[] arr)
        {
            debugCount++;
            var output = string.Empty;
            for (int i = 0; i < arr.Length; i++)
            {
                output += $"{arr[i] + 1} ";
            }
            Console.WriteLine($"{debugCount}:\t{output}");
        }
        #endregion

        private List<int> GetCandidates()
        {
            var candidates = defaultCandidates;
            switch (Operation)
            {
                case Operator.Add:
                    candidates = GetAdditionCandidates();
                    break;
                case Operator.Sub:
                    candidates = GetSubtractionCandidates();
                    break;
                case Operator.Mul:
                    candidates = GetMultiplicationCandidates();
                    break;
                case Operator.Div:
                    break;
                default:
                    break;
            }
            return candidates;
        }

        private List<int> GetSubtractionCandidates()
        {
            return defaultCandidates;
        }

        private List<int> GetAdditionCandidates()
        {
            return defaultCandidates;
        }

        private List<int> GetMultiplicationCandidates()
        {
            var excludedCandidates = defaultCandidates
                .Where(candidate => RegionValue % candidate > 0);

            return defaultCandidates.Except(excludedCandidates).ToList();
        }
    }
}
