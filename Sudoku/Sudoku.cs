using System;
using System.Collections.Generic;

namespace Sudoku
{
    public class Sudoku
    {
        public byte[,] Solve(byte[,] input)
        {
            byte[,] exactCoverProblem = GenerateExactCoverProblem(input);

            var exactCover = new ExactCover();
            var results = exactCover.GetAllSolutions(exactCoverProblem);
            if (results.Count > 1) throw new ArgumentException("More than one solution exists.");
            else return ExactCoverResultToSudoku(results[0]);
            //foreach (var rowNumber in result) Console.WriteLine(rowNumber);
        }

        public byte[,] GenerateRandomCompleteGrid()
        {
            byte[,] exactCoverProblem = GenerateExactCoverProblem(new byte[9,9] {
            {0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0},
            });
            List<int> rowsRandomOrder;
            (exactCoverProblem, rowsRandomOrder) = RandomiseMatrix(exactCoverProblem);

            var exactCover = new ExactCover();
            var result = exactCover.GetFirstSolution(exactCoverProblem);
            return ExactCoverResultToSudoku(result, rowsRandomOrder);
        }

        public byte[,] GeneratePuzzleWithClues(int n)
        {
            if (n < 17) Console.WriteLine("Not possible!");
            var exactCover = new ExactCover();
            byte[,] completedSudoku;
            byte[,] cluesRemoved;

            int attempts = 0;

            do
            {
                completedSudoku = GenerateRandomCompleteGrid();
                cluesRemoved = RemoveClues(completedSudoku, 81-n);
                attempts++;
            }
            while (!exactCover.CheckExactlyOneSolution(GenerateExactCoverProblem(cluesRemoved)));

            Console.WriteLine("Attempts: " + attempts);

            Print(completedSudoku);
            Console.WriteLine("#########");
            Print(cluesRemoved);

            return cluesRemoved;
        }

        private byte[,] RemoveClues(byte[,] input, int numberOfCluesToRemove)
        {
            byte[,] output = new byte[9, 9];
            List<(int, int)> clues = new List<(int, int)>();
            for (int i = 0; i < 9; i++) for (int j = 0; j < 9; j++) clues.Add((i, j));
            var random = new Random();
            while(numberOfCluesToRemove > 0)
            {
                var r = random.Next(0, clues.Count);
                clues.RemoveAt(r);
                numberOfCluesToRemove--;
            }
            foreach(var clue in clues)
            {
                output[clue.Item1, clue.Item2] = input[clue.Item1, clue.Item2];
            }
            return output;
        }

        private byte[,] GenerateExactCoverProblem(byte[,] input) {
            byte[,] exactCoverProblem = new byte[324, 729];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    for (int z = 0; z < 9; z++)
                    {
                        bool setValue = false;
                        if (input[i, j] == 0) setValue = true;
                        else if (input[i, j] == (z + 1)) setValue = true;

                        if (setValue)
                        {
                            var row = (i * 81) + (j * 9) + z;

                            //Columns
                            //Cell constraints
                            exactCoverProblem[(i * 9) + j, row] = 1;

                            //Row constraints
                            exactCoverProblem[81 + (i * 9) + z, row] = 1;

                            //Column constraints
                            exactCoverProblem[162 + (j * 9) + z, row] = 1;

                            //Box constraints
                            exactCoverProblem[243 + (27 * (i / 3)) + (9 * (j / 3)) + z, row] = 1;

                        }
                    }
                }
            }

            return exactCoverProblem;
        }

        private byte[,] ExactCoverResultToSudoku(HashSet<int> exactCoverResult,
            List<int> rowsRandomOrder = null)
        {
            var sudoku = new byte[9, 9];

            foreach (int resultRowName in exactCoverResult)
            {
                int rowName;
                if (rowsRandomOrder == null) 
                    rowName = resultRowName;
                else 
                    rowName = rowsRandomOrder[resultRowName];

                int i = rowName / 81;
                int j = (rowName % 81) / 9;
                byte value = (byte)((rowName % 9) + 1);
                sudoku[i, j] = value;
            }

            return sudoku;
        }

        public (byte[,], List<int>) RandomiseMatrix(byte[,] input)
        {
            var x = input.GetLength(0);
            var y = input.GetLength(1);
            byte[,] output = new byte[x, y];

            var random = new Random();
            List<int> xHeaders = new List<int>();
            List<int> yHeaders = new List<int>();
            for(var n = 0; n < x; n++)
            {
                var r = random.Next(0, n + 1);
                xHeaders.Insert(r, n);
            }
            for (var n = 0; n < y; n++)
            {
                var r = random.Next(0, n + 1);
                yHeaders.Insert(r, n);
            }

            for(int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    output[i, j] = input[xHeaders[i], yHeaders[j]];
                }
            }

            return (output, yHeaders);
        }

        private void Print(byte[,] sudoku)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write(sudoku[i, j]);
                }
                Console.WriteLine();
            }
        }
    }
}
