using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMatrixMultiplicator.Services;

namespace WpfMatrixMultiplicator.Models
{
    internal class Matrix
    {
        private int _sizeI;
        private int _sizeJ;
        private int[,] _matrix;

        public int sizeI
        {
            get => _sizeI;
            set => _sizeI = value;
        }
        public int sizeJ
        {
            get => _sizeJ;
            set => _sizeJ = value;
        }
        public int[,] matrix
        {
            get => _matrix;
            set => _matrix = value;
        }

        public Matrix(int sizeI, int sizeJ)
        {
            _sizeI = sizeI;
            _sizeJ = sizeJ;
            _matrix = new int[_sizeI, _sizeJ];
        }

        public Matrix(string filePath)
        {
            MatrixService.Read(this, filePath);
        }
    }
}
