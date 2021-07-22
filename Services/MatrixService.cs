using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using WpfMatrixMultiplicator.Models;

namespace WpfMatrixMultiplicator.Services
{
    internal class MatrixService
    {
        public static void Read(Matrix matrix, string path)
        {
            string extension = Path.GetExtension(path);
            switch (extension)
            {
                case ".txt": ReadTxt(matrix, path); break;
                case ".csv": ReadCsv(matrix, path); break;
                default: throw new ArgumentException($"Invalid argument : Unknown extension ({extension})"); 
            }
        }

        private static void ReadCsv(Matrix matrix, string path)
        {
            ReadWithSplitter(matrix, path, ',');
        }

        private static void ReadTxt(Matrix matrix, string path)
        {
            ReadWithSplitter(matrix, path, ' ');
        }

        private static void ReadWithSplitter(Matrix matrix, string path, char splitter)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (StreamReader reader = new StreamReader(fs))
            {
                string[] numbers = reader.ReadLine().Split(splitter);
                matrix.sizeI = int.Parse(numbers[0]);
                matrix.sizeJ = int.Parse(numbers[1]);
                matrix.matrix = new int[matrix.sizeI, matrix.sizeJ];
                for (int i = 0; i < matrix.sizeI; ++i)
                {
                    numbers = reader.ReadLine().Split(splitter);
                    for (int j = 0; j < matrix.sizeJ; ++j)
                    {
                        matrix.matrix[i, j] = int.Parse(numbers[j]);
                    }
                }
            }
            fs.Close();
        }

        public static Matrix Multiply(Matrix lhs, Matrix rhs, int concurrency = 1)
        {
            if (lhs == null || rhs == null)
            {
                throw new ArgumentNullException("Error: No operand");
            }
            if (concurrency < 1)
            {
                throw new ArgumentException("Error : Concurrency < 1");
            }
            if (lhs.sizeJ != rhs.sizeI)
            {
                throw new ArgumentException($"Error: Operand sizes doesn't match :" +
                    $" ({lhs.sizeI},{lhs.sizeJ}) and ({rhs.sizeI},{rhs.sizeJ})");
            }

            concurrency = Math.Min(concurrency, lhs.sizeI);
            int threadWorkSize = (lhs.sizeI + concurrency - 1) / concurrency;

            Matrix result = new Matrix(lhs.sizeI, rhs.sizeJ);
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < concurrency; ++i)
            {
                var threadData = new Tuple<Matrix, Matrix, Matrix, int, int>(
                                    result, lhs, rhs,
                                    threadWorkSize * i, Math.Min(threadWorkSize * (i + 1), lhs.sizeI));
                threads.Add(new Thread(new ParameterizedThreadStart(Multiply)));
                threads.Last().Start(threadData);
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            return result;
        }

        private static void Multiply(object obj)
        {
            var parameters = (Tuple<Matrix, Matrix, Matrix, int, int>)obj;
            Matrix result = parameters.Item1;
            Matrix lhs = parameters.Item2;
            Matrix rhs = parameters.Item3;
            int start = parameters.Item4;
            int end = parameters.Item5;

            for (int i = start; i < end; ++i)
            {
                for (int k = 0; k < lhs.sizeJ; ++k)
                {
                    int value = lhs.matrix[i, k];
                    for (int j = 0; j < rhs.sizeJ; ++j)
                    {
                        result.matrix[i, j] += value * rhs.matrix[k, j];
                    }
                }
            }
        }
    }
}
