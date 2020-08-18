using NUnit.Framework;

namespace SudokuTests
{
    public class SudokuTests
    {
        Sudoku.Sudoku Sudoku;

        [SetUp]
        public void Setup()
        {
            Sudoku = new Sudoku.Sudoku();
        }


        [Test]
        public void Example_1()
        {
            byte[,] input = new byte[9, 9]
            {
                { 6, 2, 0, 3, 1, 0, 0, 9, 8 },
                { 0, 0, 0, 0, 0, 0, 0, 1, 0 },
                { 1, 0, 5, 8, 0, 2, 6, 3, 0 },
                { 0, 8, 2, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 3, 0, 0, 9, 4, 8, 0 },
                { 5, 1, 0, 7, 0, 0, 0, 2, 0 },
                { 0, 0, 0, 0, 5, 1, 0, 0, 0 },
                { 0, 3, 0, 0, 7, 0, 0, 5, 9 },
                { 4, 0, 0, 2, 0, 3, 0, 6, 0 },
            };

            Sudoku.Solve(input);
        }

        [Test]
        public void Example_2()
        {
            byte[,] input = new byte[9, 9]
            {
                { 8, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 3, 6, 0, 0, 0, 0, 0 },
                { 0, 7, 0, 0, 9, 0, 2, 0, 0 },
                { 0, 5, 0, 0, 0, 7, 0, 0, 0 },
                { 0, 0, 0, 0, 4, 5, 7, 0, 0 },
                { 0, 0, 0, 1, 0, 0, 0, 3, 0 },
                { 0, 0, 1, 0, 0, 0, 0, 6, 8 },
                { 0, 0, 8, 5, 0, 0, 0, 1, 0 },
                { 0, 9, 0, 0, 0, 0, 4, 0, 0 },
            };

            Sudoku.Solve(input);
        }

        [Test]
        public void Random()
        {
            Sudoku.GenerateRandomCompleteGrid();
        }

        [Test]
        public void GeneratePuzzle()
        {
            Sudoku.GeneratePuzzleWithClues(30);//3,26,197 attempts
            //Sudoku.GeneratePuzzleWithClues(28); //947
        }
    }
}