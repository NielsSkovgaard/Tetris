﻿using System.Windows;
using System.Windows.Input;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Official guidelines for building Tetris:
            // http://tetris.wikia.com/wiki/Tetris_Guideline
            // https://en.wikipedia.org/wiki/Tetromino

            // TODO: Organize project as MVVM: http://www.markwithall.com/programming/2013/03/01/worlds-simplest-csharp-wpf-mvvm-example.html

            // TODO: This is just test code
            int[,] staticBlocks = GameBoard1.StaticBlocks;
            staticBlocks[19, 0] = 1;
            staticBlocks[19, 1] = 2;
            staticBlocks[19, 2] = 3;
            staticBlocks[19, 3] = 4;
            staticBlocks[19, 4] = 5;
            staticBlocks[19, 5] = 6;
            staticBlocks[19, 6] = 7;
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            GameBoard1.KeyPressed(e.Key);
        }
    }
}
