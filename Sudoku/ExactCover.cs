using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Sudoku
{
    public class ExactCover
    {
        private class Cell {
            public Cell Right;
            public Cell Left;
            public Cell Up;
            public Cell Down;

            public int ColumnName; //For debugging
            public int RowName;
        }

        private class ColumnHeader : Cell
        {
            public int Size;
        }

        private ColumnHeader Root;
        private Stack<int> Stack;
        private List<HashSet<int>> Solutions;

        //Constraints go in rows, not columns when formatting for input!
        //But in the code, constraints are columns. First dimension.
        //Constraint columns cannot be all zero otherwise that constraint cannot be satisfied by any row.
        //But rows can be all zero, they will be ignored.
        private void Initialise(byte[,] input)
        {
            Root = new ColumnHeader();

            int columns = input.GetLength(0);
            int rows = input.GetLength(1);
            ColumnHeader[] columnHeaders = new ColumnHeader[columns];
            for (int i = 0; i < columns; i++)
            {
                var columnHeader = new ColumnHeader();
                columnHeader.ColumnName = i;
                columnHeaders[i] = columnHeader;
            }

            Cell[,] cells = new Cell[columns, rows];
            for (int i = 0; i < columns; i++)
            {
                var columnHeader = columnHeaders[i];
                if (i == 0) columnHeader.Left = Root; 
                else columnHeader.Left = columnHeaders[i - 1];
                if (i == (columns - 1)) columnHeader.Right = Root;
                else columnHeader.Right = columnHeaders[i + 1];

                //Establish columns
                Cell previousCell = columnHeader;
                for (int j = 0; j < rows; j++)
                {
                    if (input[i, j] != 0)
                    {
                        columnHeader.Size++;
                        Cell cell = new Cell();
                        cell.ColumnName = i;
                        cell.RowName = j;
                        cells[i, j] = cell;
                        cell.Up = previousCell;
                        previousCell = cell;
                    }
                }
                columnHeader.Up = previousCell;

                //Add down links.
                columnHeader.Up.Down = columnHeader;
                Cell current = columnHeader.Up;
                while (current != columnHeader)
                {
                    current.Up.Down = current;
                    current = current.Up;
                }
            }

            Root.Right = columnHeaders[0];
            Root.Left = columnHeaders[columns - 1];

            for (int j = 0; j < rows; j++)
            {
                Cell firstCellInRow = null;
                Cell previousCell = null;
                for (int i = 0; i < columns; i++)
                {
                    if (input[i, j] != 0)
                    {
                        if (firstCellInRow == null)
                        {
                            firstCellInRow = cells[i, j];
                        }
                        else
                        {
                            cells[i, j].Left = previousCell;
                        }
                        previousCell = cells[i, j];
                    }
                }
                if (firstCellInRow != null) //Row could be empty
                {
                    firstCellInRow.Left = previousCell;

                    //Add right links.
                    firstCellInRow.Left.Right = firstCellInRow;
                    Cell current = firstCellInRow.Left;
                    while (current != firstCellInRow)
                    {
                        current.Left.Right = current;
                        current = current.Left;
                    }
                }
            }
        }

        public bool MoreThanOneSolution(byte[,] input)
        {
            Initialise(input);
            Search(0, checkForMultipleSolutions: true);
            return Solutions.Count > 1;
        }

        public HashSet<int> GetFirstSolution(byte[,] input)
        {
            Initialise(input);
            Search(0, getFirstSolution: true);
            return Solutions[0];
        }

        public bool CheckExactlyOneSolution(byte[,] input)
        {
            if (MoreThanOneSolution(input)) return false;

            Initialise(input);
            Search(0);
            return Solutions.Count == 1;
        }

        public List<HashSet<int>> GetAllSolutions(byte[,] input)
        {
            Initialise(input);
            Search(0);
            return Solutions;
        }

        private void Search(int n, bool getFirstSolution = false, bool checkForMultipleSolutions = false)
        {
            if (n == 0)
            {
                Solutions = new List<HashSet<int>>();
                Stack = new Stack<int>();
            }

            if (getFirstSolution && Solutions.Count > 0) return;
            if (checkForMultipleSolutions && Solutions.Count > 1) return;

            //PrintState();
            if (Root.Right == Root) //No columns remain to be covered
            {
                Solutions.Add(Stack.ToHashSet());
                return;
            }

            ColumnHeader column = SmallestColumn(); //This biases the ordering of solutions but is essential for speed.
            CoverColumn(column);
            foreach (var cell in GetCellsInColumn(column))
            {
                Stack.Push(cell.RowName);

                var otherCellsInRow = GetOtherCellsInRow(cell);
                foreach (var cellInRow in otherCellsInRow)
                {
                    CoverColumn(cellInRow);
                }
                Search(n + 1, getFirstSolution, checkForMultipleSolutions);
                foreach (var cellInRow in otherCellsInRow.Reverse())
                {
                    UncoverColumn(cellInRow);
                }

                Stack.Pop();
            }
            UncoverColumn(column);
        }

        private IEnumerable<Cell> GetCellsInColumn(ColumnHeader columnHeader)
        {
            return GetCellsInColumnDownwards(columnHeader);
        }

        private IEnumerable<Cell> GetCellsInColumnDownwards(ColumnHeader columnHeader)
        {
            Cell cell = columnHeader.Down;
            while (cell != columnHeader)
            {
                yield return cell;
                cell = cell.Down;
            }
        }

        private IEnumerable<Cell> GetCellsInColumnUpwards(ColumnHeader columnHeader)
        {
            Cell cell = columnHeader.Up;
            while (cell != columnHeader)
            {
                yield return cell;
                cell = cell.Up;
            }
        }

        private ColumnHeader SmallestColumn()
        {
            ColumnHeader smallestColumn = (ColumnHeader)Root.Right;
            ColumnHeader column = (ColumnHeader)Root.Right;
            while (column != Root)
            {
                if (column.Size < smallestColumn.Size) smallestColumn = column;
                column = (ColumnHeader)column.Right;
            }
            return smallestColumn;
        }

        private ColumnHeader GetColumnHeader(Cell cell)
        {
            while (!(cell is ColumnHeader))
            {
                cell = cell.Up;
            }
            return (ColumnHeader)cell;
        }

        private void CoverColumn(Cell cell)
        {
            CoverColumn(GetColumnHeader(cell));
        }

        private void UncoverColumn(Cell cell)
        {
            UncoverColumn(GetColumnHeader(cell));
        }

        private void CoverColumn(ColumnHeader columnHeader)
        {
            columnHeader.Right.Left = columnHeader.Left;
            columnHeader.Left.Right = columnHeader.Right;
            foreach (var cell in GetCellsInColumnDownwards(columnHeader))
            {
                foreach (var rowCell in GetOtherCellsInRow(cell))
                {
                    rowCell.Down.Up = rowCell.Up;
                    rowCell.Up.Down = rowCell.Down;
                    GetColumnHeader(rowCell).Size--;
                }
            }
        }

        private void UncoverColumn(ColumnHeader columnHeader)
        {

            foreach (var cell in GetCellsInColumnUpwards(columnHeader))
            {
                foreach (var rowCell in GetOtherCellsInRow(cell))
                {
                    GetColumnHeader(rowCell).Size++;
                    rowCell.Down.Up = rowCell;
                    rowCell.Up.Down = rowCell;
                }
            }
            columnHeader.Right.Left = columnHeader;
            columnHeader.Left.Right = columnHeader;
        }

        private IEnumerable<Cell> GetOtherCellsInRow(Cell cell)
        {
            //Do not return the cell itself.
            Cell next = cell.Right;
            while(next != cell)
            {
                yield return next;
                next = next.Right;
            }
        }

        private void PrintState()
        {
            Console.WriteLine("Current state:");
            
            for(int j=-1; j<6; j++)
            {
                ColumnHeader column = (ColumnHeader)Root.Right;
                while (column != Root)
                {
                    if (j==-1) Console.Write(column.ColumnName);
                    else
                    {
                        bool present = false;
                        foreach (var cell in GetCellsInColumnDownwards(column))
                        {
                            if (cell.RowName == j) present = true;
                        }
                        if (present) Console.Write("1"); else Console.Write("0");
                    }

                    column = (ColumnHeader)column.Right;
                }
                Console.WriteLine();
            }

            Console.WriteLine("#######");
        }
    }
}
