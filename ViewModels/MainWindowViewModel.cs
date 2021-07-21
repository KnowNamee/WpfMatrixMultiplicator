﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfMatrixMultiplicator.Commands.Base;
using WpfMatrixMultiplicator.ViewModels.Base;

namespace WpfMatrixMultiplicator.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private static string _noFile = "No file";
        private static string _openFileFormats = "txt files (*.txt)|*.txt|csv files (*.csv)|*.csv";

        private string _matrixAPath = _noFile;
        private string _matrixBPath = _noFile;

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
                }
                else
                {
                    MatrixBPath = GetPathFromFileDialog();
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
            }
            catch
            {
                Console.Error.WriteLine("Error: OnClearCommandExecuted");
            }
        }

        #endregion

        #endregion

        public MainWindowViewModel()
        {
            #region Commands

            OpenMatrixCommand = new LambdaCommand(OnOpenMatrixCommandExecuted);
            ClearCommand = new LambdaCommand(OnClearCommandExecuted);

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
