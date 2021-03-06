﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SudokuGame
{
    class Sudoku
    {
        #region Fields

        private GameForm form;
        private PictureBox canvas;
        private ComboBox numbers;
        private Square clicked;

        private static Random random = new Random();

        private bool displayed;

        private Difficulty diff;
        private Board board;
        private Square[,] squares;

        #endregion

        #region Properties

        public Difficulty Difficulty { get { return diff; } set { diff = value; } }

        #endregion

        #region Constructors

        public Sudoku(GameForm form, PictureBox canvas, Difficulty diff)
        {
            this.form = form;
            this.diff = diff;
            this.canvas = canvas;

            displayed = false;

            this.board = new Board(diff);
            this.board.Shuffle();

            squares = new Square[Settings.Rows, Settings.Cols];

            CreateButtonsAndDisplayBoard(form, canvas);
            numbers = new ComboBox();
            numbers.DropDownStyle = ComboBoxStyle.DropDownList;
            numbers.Items.AddRange(new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            numbers.Font = new Font("Arial", Settings.FontSize);
            numbers.Visible = true;
            numbers.Enabled = true;
            numbers.SelectedValueChanged += Numbers_SelectedValueChanged;
        }

        private void Numbers_SelectedValueChanged(object sender, EventArgs e)
        {
            clicked.Text = numbers.SelectedItem.ToString();
            if (!board.CheckSpot(clicked.Row, clicked.Column))
                clicked.BackColor = Color.Red;
        }

        #endregion

        #region Public Methods

        public void RefreshBoardAndClearHidden()
        {
            int[,] gameBoard = board.GetGameBoard();
            int[,] hidden = board.GetHiddenGameBoard();

            for (int i = 0; i < Settings.Rows; i++)
            {
                for (int j = 0; j < Settings.Cols; j++)
                {
                    squares[i, j].Text = gameBoard[i, j].ToString();
                    hidden[i, j] = gameBoard[i, j];
                }
            }
        }

        public void DisplayEmptyHoles()
        {
            int[,] gameBoard = board.GetGameBoard();

            for (int i = 0; i < Settings.Rows; i++)
            {
                for (int j = 0; j < Settings.Cols; j++)
                {
                    if (squares[i, j].Text == "")
                        squares[i, j].Text = gameBoard[i, j].ToString();
                }
            }
        }

        public void Hide()
        {
            int[,] hidden = board.GetHiddenGameBoard();

            for (int i = 0; i < Settings.Rows; i++)
            {
                for (int j = 0; j < Settings.Cols; j++)
                {
                    if (hidden[i, j] < Settings.HiddenThreshold)
                        squares[i, j].Text = "";
                }
            }
        }

        public bool IsDisplayed()
        {
            for (int i = 0; i < Settings.Rows; i++)
            {
                for (int j = 0; j < Settings.Cols; j++)
                {
                    if (squares[i, j].Text == "")
                    {
                        displayed = false;
                        return false;
                    }
                }
            }

            displayed = true;
            return true;
        }

        public void DiluteBoard(int row, int col)
        {
            board.SetEmpty(random.Next(row), random.Next(col));
        }

        #endregion

        #region Private Methods

        private void CreateButtonsAndDisplayBoard(GameForm form, PictureBox parent)
        {
            int[,] gb = board.GetGameBoard();
            for (int i = 0; i < Settings.Rows; i++)
            {
                for (int j = 0; j < Settings.Cols; j++)
                {
                    squares[i, j] = new Square(Settings.Size, i, j);
                    int x = 5 + (Settings.Size + 1) * j + (j / 3) * 3;
                    int y = 5 + (Settings.Size + 1) * i + (i / 3) * 3;
                    Point p = new Point(x, y);
                    squares[i, j].Font = new Font("Arial", Settings.FontSize, FontStyle.Bold);
                    squares[i, j].Location = p;
                    squares[i, j].Visible = true;
                    squares[i, j].Name = String.Format("{0},{1}", i, j);
                    parent.Controls.Add(squares[i, j]);
                    squares[i, j].MouseClick += OnClick;
                    squares[i, j].Text = gb[i, j].ToString();
                    squares[i, j].BackColor = Color.LightGray;
                }
            }

            AdjustFormSize(form, parent);
            DiluteBoard(Settings.Rows, Settings.Cols);
        }
        private void AdjustFormSize(GameForm form, PictureBox board)
        {
            //int width = board.Width;
            int width = Settings.Rows * (Settings.Size + 1) + 6 + 10;
            int wDelta = width - board.Width;
            board.Width = width;
            form.Width += wDelta;
            int height = Settings.Cols * (Settings.Size + 1) + 6 + 10;
            int hDelta = height - board.Height;
            board.Height = height;
            form.Height += hDelta;

        }

        #endregion

        #region OnClick

        private void OnClick(object sender, MouseEventArgs e)
        {
            clicked = (Square)sender;

            if (clicked.Text == "")
            {
                canvas.Controls.Add(numbers);
                numbers.Location = clicked.Location;
                numbers.BringToFront();
            }
        }

        #endregion

        #region Get/Set

        public void SetDisplayed(bool displayed) { this.displayed = displayed; }

        public Board GetBoard() { return board; }
        public Square[,] GetSquares() { return squares; }

        #endregion
    }
}
