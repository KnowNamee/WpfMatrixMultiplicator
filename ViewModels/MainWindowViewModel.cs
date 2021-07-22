using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfMatrixMultiplicator.Commands.Base;
using WpfMatrixMultiplicator.Models;
using WpfMatrixMultiplicator.ViewModels.Base;
using WpfMatrixMultiplicator.Services;

namespace WpfMatrixMultiplicator.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private static readonly string _noFile = "No file";
        private static readonly string _openFileFormats = "txt files (*.txt)|*.txt|csv files (*.csv)|*.csv";
        private static readonly string _startStatusText = "Waiting for opened files.";

        private string _matrixAPath = _noFile;
        private string _matrixBPath = _noFile;
        private string _statusText = _startStatusText;

        private Matrix _matrixA;
        private Matrix _matrixB;
        private Matrix _result;

        public string MatrixAPath
        {
            get => _matrixAPath;
            set => Set(ref _matrixAPath, value ?? _noFile);
        }

        public string MatrixBPath
        {
            get => _matrixBPath;
            set => Set(ref _matrixBPath, value ?? _noFile);
        }

        public string statusText
        {
            set => Set(ref _statusText, value);
            get => _statusText;
        }

        #region Commands

        #region OpenMatrixCommand
        public ICommand OpenMatrixCommand { get; }

        private void OnOpenMatrixCommandExecuted(object Obj)
        {
            try
            {
                string MatrixIdentifier = (string)Convert.ChangeType(Obj, typeof(string));
                if (MatrixIdentifier.Equals("A"))
                {
                    MatrixAPath = GetPathFromFileDialog();
                    _matrixA = MatrixAPath.Equals(_noFile) ? null : new Matrix(MatrixAPath);
                    statusText = $"File opened : {MatrixAPath}";
                }
                else
                {
                    MatrixBPath = GetPathFromFileDialog();
                    _matrixB = MatrixBPath.Equals(_noFile) ? null : new Matrix(MatrixBPath);
                    statusText = $"File opened : {MatrixBPath}";
                }
            }
            catch
            {
                Console.Error.WriteLine("Error: OnOpenMatrixCommandExecuted");
            }
        }

        #endregion

        #region ClearCommand

        public ICommand ClearCommand { get; }

        private void OnClearCommandExecuted(object Obj)
        {
            try
            {
                string MatrixIdentifier = (string)Convert.ChangeType(Obj, typeof(string));
                
                if (MatrixIdentifier.Equals("A"))
                {
                    MatrixAPath = null;
                }
                else
                {
                    MatrixBPath = null;    
                }
                statusText = $"Matrix {MatrixIdentifier} cleared.";
            }
            catch
            {
                Console.Error.WriteLine("Error: OnClearCommandExecuted");
            }
        }

        #endregion

        #region MultiplyCommand

        public ICommand MultiplyCommand { get; }

        private void OnMultiplyCommandExecuted(object obj)
        {
            // !!! Blocking Multiply Call
            statusText = "Multiplying . . .";
            _result = MatrixService.Multiply(_matrixA, _matrixB, Environment.ProcessorCount);
            statusText = "Multiplying finished.";
        }

        #endregion

        #region SaveCommand

        public ICommand SaveCommand { get; }

        private void OnSaveCommandExecuted(object obj)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if ((bool)dialog.ShowDialog() && _result != null)
            {
                MatrixService.Save(_result, dialog.FileName);
                statusText = "Result saved successfully.";
            }
        }

        #endregion

        #endregion

        public MainWindowViewModel()
        {
            #region Commands

            OpenMatrixCommand = new LambdaCommand(OnOpenMatrixCommandExecuted);
            ClearCommand = new LambdaCommand(OnClearCommandExecuted);
            MultiplyCommand = new LambdaCommand(OnMultiplyCommandExecuted);
            SaveCommand = new LambdaCommand(OnSaveCommandExecuted);

            #endregion
        }

        private string GetPathFromFileDialog()
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.Filter = _openFileFormats;
            if ((bool)Dialog.ShowDialog())
            {
                return Dialog.FileName;               
            }
            return null;
        }
    }
}
