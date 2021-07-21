using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (StreamReader reader = new StreamReader(fs))
            {
                string[] numbers = reader.ReadLine().Split(',');
                matrix.sizeI = int.Parse(numbers[0]);
                matrix.sizeJ = int.Parse(numbers[1]);
                matrix.matrix = new int[matrix.sizeI, matrix.sizeJ];
                for (int i = 0; i < matrix.sizeI; ++i)
                {
                    numbers = reader.ReadLine().Split(',');
                    for (int j = 0; j < matrix.sizeJ; ++j)
                    {
                        matrix.matrix[i, j] = int.Parse(numbers[j]);
                    }
                }
            }
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
                    numbers = reader.ReadLine().Split(' ');
                    for (int j = 0; j < matrix.sizeJ; ++j)
                    {
                        matrix.matrix[i, j] = int.Parse(numbers[j]);
                    }
                }
            }
        }
    }
}
