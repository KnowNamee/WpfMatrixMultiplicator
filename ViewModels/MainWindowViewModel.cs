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

        public string matrixAPath
        {
            get => _matrixAPath;
            set => Set(ref _matrixAPath, value ?? _noFile);
        }

        public string matrixBPath
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

        private void OnOpenMatrixCommandExecuted(object obj)
        {
            try
            {
                switch ((string)obj)
                {
                    case "A":
                        {
                            matrixAPath = GetPathFromFileDialog();
                            _matrixA = matrixAPath.Equals(_noFile) ? null : new Matrix(matrixAPath);
                            statusText = $"File opened : {matrixAPath}";
                            break;
                        }
                    case "B":
                        {
                            matrixBPath = GetPathFromFileDialog();
                            _matrixB = matrixBPath.Equals(_noFile) ? null : new Matrix(matrixBPath);
                            statusText = $"File opened : {matrixBPath}";
                            break;
                        }
                    default: throw new ArgumentException("Unknown indentifier");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Warning!!!");
            }
        }

        #endregion

        #region ClearCommand

        public ICommand ClearCommand { get; }

        private void OnClearCommandExecuted(object obj)
        {
            try
            {
                switch ((string)obj)
                {
                    case "A": matrixAPath = null; break;
                    case "B": matrixBPath = null; break;
                    default: throw new ArgumentException("Unknown identifier");
                }
                statusText = $"Matrix {(string)obj} cleared.";
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Warning!!!");
            }
        }

        #endregion

        #region MultiplyCommand

        public ICommand MultiplyCommand { get; }

        private void OnMultiplyCommandExecuted(object obj)
        {
            // !!! Blocking Multiply Call
            statusText = "Multiplying . . .";
            try
            {
                _result = MatrixService.Multiply(_matrixA, _matrixB, Environment.ProcessorCount);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Warning!!!");
            }
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
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = _openFileFormats;
            if ((bool)dialog.ShowDialog())
            {
                return dialog.FileName;               
            }
            return null;
        }
    }
}
