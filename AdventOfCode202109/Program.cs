using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace com.fabioscagliola.AdventOfCode202109
{
    class Program
    {
        /// <summary>
        /// Represents a low point on the heightmap 
        /// </summary>
        class LowPoint : IEquatable<LowPoint>
        {
            protected int[,] matrix;

            public int Row { get; protected set; }
            public int Col { get; protected set; }

            public int Risk => matrix[Row, Col] + 1;

            public LowPoint(int[,] matrix, int row, int col)
            {
                this.matrix = matrix;

                Row = row;
                Col = col;
            }

            /// <summary>
            /// Returns the list of the low points on the specified heightmap 
            /// </summary>
            /// <param name="matrix">The heightmap as a bidimensional array of integers</param>
            public static List<LowPoint> GetLowPoints(int[,] matrix)
            {
                List<LowPoint> lowPointList = new List<LowPoint>();

                for (int row = 0; row < matrix.GetLength(0); row++)
                {
                    for (int col = 0; col < matrix.GetLength(1); col++)
                    {
                        LowPoint lowPoint = new LowPoint(matrix, row, col);

                        int top = 9;
                        int bottom = 9;
                        int left = 9;
                        int right = 9;

                        if (row != 0)
                            top = matrix[row - 1, col];

                        if (row != matrix.GetLength(0) - 1)
                            bottom = matrix[row + 1, col];

                        if (col != 0)
                            left = matrix[row, col - 1];

                        if (col != matrix.GetLength(1) - 1)
                            right = matrix[row, col + 1];

                        if (matrix[lowPoint.Row, lowPoint.Col] < top &&
                            matrix[lowPoint.Row, lowPoint.Col] < bottom &&
                            matrix[lowPoint.Row, lowPoint.Col] < left &&
                            matrix[lowPoint.Row, lowPoint.Col] < right)
                            lowPointList.Add(lowPoint);
                    }
                }

                return lowPointList;
            }
            public bool Equals(LowPoint other)
            {
                if (other == null)
                    return false;

                return Row == other.Row && Col == other.Col;
            }

            public override string ToString()
            {
                return $"({Row}, {Col})";
            }

        }

        /// <summary>
        /// Represents a basin on the heightmap 
        /// </summary>
        class Basin
        {
            protected int[,] matrix;
            protected LowPoint lowPoint;

            private int __size = 0;

            public int Size { get { if (__size == 0) { __size = LookAround(new List<LowPoint>(), lowPoint).Count; } return __size; } }

            public Basin(int[,] matrix, LowPoint lowPoint)
            {
                this.matrix = matrix;
                this.lowPoint = lowPoint;
            }

            private List<LowPoint> LookAround(List<LowPoint> lowPointList, LowPoint lowPoint)
            {
                if (lowPointList.Find(x => x.Equals(lowPoint)) == null)
                    lowPointList.Add(lowPoint);

                List<LowPoint> neighborList = new List<LowPoint>();

                if (lowPoint.Row != 0 && matrix[lowPoint.Row - 1, lowPoint.Col] != 9)
                {
                    LowPoint neighbor = new LowPoint(matrix, lowPoint.Row - 1, lowPoint.Col);
                    if (lowPointList.Find(x => x.Equals(neighbor)) == null)
                        neighborList.Add(neighbor);
                }

                if (lowPoint.Row != matrix.GetLength(0) - 1 && matrix[lowPoint.Row + 1, lowPoint.Col] != 9)
                {
                    LowPoint neighbor = new LowPoint(matrix, lowPoint.Row + 1, lowPoint.Col);
                    if (lowPointList.Find(x => x.Equals(neighbor)) == null)
                        neighborList.Add(neighbor);
                }

                if (lowPoint.Col != 0 && matrix[lowPoint.Row, lowPoint.Col - 1] != 9)
                {
                    LowPoint neighbor = new LowPoint(matrix, lowPoint.Row, lowPoint.Col - 1);
                    if (lowPointList.Find(x => x.Equals(neighbor)) == null)
                        neighborList.Add(neighbor);
                }

                if (lowPoint.Col != matrix.GetLength(1) - 1 && matrix[lowPoint.Row, lowPoint.Col + 1] != 9)
                {
                    LowPoint neighbor = new LowPoint(matrix, lowPoint.Row, lowPoint.Col + 1);
                    if (lowPointList.Find(x => x.Equals(neighbor)) == null)
                        neighborList.Add(neighbor);
                }

                foreach (LowPoint neighbor in neighborList)
                    if (lowPointList.Find(x => x.Equals(neighbor)) == null)
                        lowPointList.Add(neighbor);

                foreach (LowPoint neighbor in neighborList)
                    foreach (LowPoint neighborOfNeighbor in LookAround(lowPointList, neighbor))
                        if (lowPointList.Find(x => x.Equals(neighborOfNeighbor)) == null)
                            lowPointList.Add(neighborOfNeighbor);

                return lowPointList;
            }

        }

        static void Main()
        {
            int[,] matrix = new int[100, 100];

            // Fill the heightmap 

            {
                Regex regex = new Regex(@"(?:\d)");
                int row = 0;
                foreach (string line in File.ReadAllLines("Input1.txt"))
                {
                    int col = 0;
                    MatchCollection matchCollection = regex.Matches(line);
                    foreach (Match match in matchCollection)
                    {
                        matrix[row, col] = int.Parse(match.Value);
                        col++;
                    }
                    row++;
                }
            }

            // PART 1 

            List<LowPoint> lowPointList = LowPoint.GetLowPoints(matrix);

            int risk = 0;

            foreach (LowPoint lowPoint in lowPointList)
                risk += lowPoint.Risk;

            Console.WriteLine($"The sum of the risk levels of all low points on the heightmap is {risk}");

            // PART 2 

            List<Basin> basinList = new List<Basin>();

            foreach (LowPoint lowPoint in lowPointList)
                basinList.Add(new Basin(matrix, lowPoint));

            basinList.Sort((a, b) => a.Size.CompareTo(b.Size) * -1);

            int size = basinList[0].Size * basinList[1].Size * basinList[2].Size;

            Console.WriteLine($"If you multiply together the sizes of the three largest basins you get {size}");
        }

    }
}

