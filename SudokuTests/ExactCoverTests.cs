using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sudoku;

namespace ExactCoverTests
{
    public class ExactCoverTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Solve()
        {
            ExactCover exactCover = new ExactCover();
            var input = new byte[,] {
                { 0, 1, 0, 1, 0, 0 },
                { 0, 0, 1, 0, 1, 0 },
                { 1, 0, 1, 0, 0, 0 },
                { 0, 1, 0, 1, 0, 1 },
                { 1, 0, 0, 0, 0, 1 },
                { 1, 0, 1, 0, 0, 0 },
                { 0, 1, 0, 0, 1, 1 },
            };

            var results = exactCover.GetAllSolutions(input);
            Assert.AreEqual(1, results.Count);
            var result = results[0];
            Assert.AreEqual(3, result.Count);
            Assert.True(result.Contains(0));
            Assert.True(result.Contains(3));
            Assert.True(result.Contains(4));
        }

        [Test]
        public void Solve_2()
        {
            ExactCover exactCover = new ExactCover();
            var input = new byte[,] {
                { 1, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1 },
                { 0, 0, 0, 1, 1, 0 },
                { 1, 1, 1, 0, 0, 0 },
                { 0, 0, 1, 1, 0, 0 },
                { 0, 0, 0, 1, 1, 0 },
                { 1, 0, 1, 0, 1, 1 },
            };

            var results = exactCover.GetAllSolutions(input);
            Assert.AreEqual(1, results.Count);
            var result = results[0];
            Assert.AreEqual(3, result.Count);
            Assert.True(result.Contains(1));
            Assert.True(result.Contains(3));
            Assert.True(result.Contains(5));
        }

        [Test]
        public void Solve_RowAllZero()
        {
            ExactCover exactCover = new ExactCover();
            var input = new byte[,] {
                { 0, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1 },
                { 0, 0, 0, 1, 1, 0 },
                { 0, 1, 1, 0, 0, 0 },
                { 0, 0, 1, 1, 0, 0 },
                { 0, 0, 0, 1, 1, 0 },
                { 0, 0, 1, 0, 1, 1 },
            };
            
            var results = exactCover.GetAllSolutions(input);
            Assert.AreEqual(1, results.Count);
            var result = results[0];
            Assert.AreEqual(3, result.Count);
            Assert.True(result.Contains(1));
            Assert.True(result.Contains(3));
            Assert.True(result.Contains(5));
        }
        
        [Test]
        public void Solve_NoResults()//Because empty column cannot be satisfied
        {
            ExactCover exactCover = new ExactCover();
            var input = new byte[,] {
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 1 },
                { 0, 0, 0, 1, 1, 0 },
                { 1, 1, 1, 0, 0, 0 },
                { 0, 0, 1, 1, 0, 0 },
                { 0, 0, 0, 1, 1, 0 },
                { 1, 0, 1, 0, 1, 1 },
            };

            var results = exactCover.GetAllSolutions(input);
            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public void Solve_MultipleResults()
        {
            ExactCover exactCover = new ExactCover();
            var input = new byte[,] {
                { 1, 0, 1, 0 },
                { 1, 0, 1, 0 },
                { 1, 0, 0, 1 },
                { 1, 0, 0, 1 },
                { 0, 1, 1, 0 },
                { 0, 1, 1, 0 },
                { 0, 1, 0, 1 },
            };

            var results = exactCover.GetAllSolutions(input);
            Assert.AreEqual(2, results.Count);
            var result1 = results[0];
            Assert.AreEqual(2, result1.Count);
            Assert.True(result1.Contains(0));
            Assert.True(result1.Contains(1));
            var result2 = results[1];
            Assert.AreEqual(2, result2.Count);
            Assert.True(result2.Contains(2));
            Assert.True(result2.Contains(3));
        }


        [Test]
        public void GetFirstSolution()
        {
            ExactCover exactCover = new ExactCover();
            var input = new byte[,] {
                { 1, 0, 1, 0 },
                { 1, 0, 1, 0 },
                { 1, 0, 0, 1 },
                { 1, 0, 0, 1 },
                { 0, 1, 1, 0 },
                { 0, 1, 1, 0 },
                { 0, 1, 0, 1 },
            };

            var result = exactCover.GetFirstSolution(input);
            Assert.True(result.Contains(0));
            Assert.True(result.Contains(1));
        }

        [Test]
        public void CheckMultipleSolutions_True()
        {
            ExactCover exactCover = new ExactCover();
            var input = new byte[,] {
                { 1, 0, 1, 0 },
                { 1, 0, 1, 0 },
                { 1, 0, 0, 1 },
                { 1, 0, 0, 1 },
                { 0, 1, 1, 0 },
                { 0, 1, 1, 0 },
                { 0, 1, 0, 1 },
            };
            
            var result = exactCover.MoreThanOneSolution(input);
            Assert.True(result);
        }

        [Test]
        public void CheckMultipleSolutions_False()
        {
            ExactCover exactCover = new ExactCover();
            var input = new byte[,] {
                { 1, 0, 1, 0 },
                { 1, 0, 1, 0 },
                { 1, 0, 0, 1 },
                { 1, 0, 0, 1 },
                { 0, 1, 0, 0 },
                { 0, 1, 1, 0 },
                { 0, 1, 0, 1 },
            };

            var result = exactCover.MoreThanOneSolution(input);
            Assert.False(result);
        }
    }
}