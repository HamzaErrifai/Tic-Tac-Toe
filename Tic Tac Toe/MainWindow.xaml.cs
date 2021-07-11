using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tic_Tac_Toe
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Members
        /// <summary>
        /// Holds the current result of cells in the active game 
        /// </summary>
        private MarkType[] mResults;

        /// <summary>
        /// True if it is player 1's  turn (X) or player 2's turn (0)
        /// </summary>
        private bool mP1Turn;

        /// <summary>
        /// True if the game has ended
        /// </summary>
        private bool mGameEnded;
        /// <summary>
        /// True if the computer is the second player
        /// </summary>
        private bool p2IsComputer;


        private Dictionary<string, int> Scores = new Dictionary<string, int>();
        #endregion

        #region constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            //make sure the game starts p1 vs computer
            p2IsComputer = false;
            btnP1vP2.IsChecked = true;

            //set the apropriate scores
            Scores["Tie"] = 0;
            Scores["Cross"] = -1;
            Scores["Nought"] = 1;

            NewGame();
        }
        #endregion

        /// <summary>
        /// Starts a new game and clears all values back to the start
        /// </summary>
        private void NewGame()
        {
            //Create a new blank array of free cells 
            mResults = new MarkType[9];

            //set each item in the array to "Free"
            for (var i = 0; i < mResults.Length; i++)
                mResults[i] = MarkType.Free;

            //Make sure the player 1 starts the game
            mP1Turn = true;

            //iterate every btn on the grid...
            Container.Children.Cast<Button>().ToList().ForEach(btn =>
            {
                btn.Content = string.Empty;
                btn.Background = Brushes.White;
                btn.Foreground = Brushes.Red;
            });

            // Make sure game hasn't finished
            mGameEnded = false;

            //Make sure there is no winner
            lblResult.Header = "";


        }
        /// <summary>
        /// Handles a button click event
        /// </summary>
        /// <param name="sender">The button that was clicked</param>
        /// <param name="e">The event of the click</param>
        private void BtnClick(object sender, RoutedEventArgs e)
        {

            //start a new game on the click after it finished 
            if (mGameEnded)
            {
                NewGame();
                return;
            }
            //Cast the sender into a button
            var btn = (Button)sender;

            doTheClick(btn);

            if (!mGameEnded && !mP1Turn && p2IsComputer)
            {

                btn = findButton(bestMove());
                doTheClick(btn);

            }

        }

        private void doTheClick(Button btn)
        {
            //get col and row number
            var column = Grid.GetColumn(btn);
            var row = Grid.GetRow(btn);

            //expect the index of the number in the array
            var idx = column + (row * 3);

            //don't do anything if the cell is already has a value in it
            if (mResults[idx] != MarkType.Free)
                return;

            //set the cell value based on the player
            mResults[idx] = (mP1Turn) ? MarkType.Cross : MarkType.Nought;

            //set button text
            btn.Content = (mP1Turn) ? "X" : "O";

            //set nought color to green
            if (!mP1Turn) btn.Foreground = Brushes.Green;
            //Toggle the players turns
            mP1Turn = !mP1Turn;

            //check for a winner
            checkForWinner();
        }
        /// <summary>
        /// finds the button based on the index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns>Button</returns>
        private Button findButton(int idx)
        {
            List<Button> listBtns = new List<Button>(){
                btn00,btn10,btn20,
                btn01,btn11,btn21,
                btn02,btn12,btn22
            };
            if (idx > -1)
                return listBtns[idx];
            else
                throw new Exception("The index is not valid!!");
        }


        /// <summary>
        /// returns the index of the best place to make the move
        /// </summary>
        /// <returns>index of the button (int)</returns>
        private int bestMove()
        {
            double bestScore = double.NegativeInfinity;
            int bestChoice = -1;

            for (int i = 0; i < mResults.Length; i++)
            {
                if (mResults[i] == MarkType.Free)
                {
                    mResults[i] = MarkType.Nought;
                    double score = minimax(mResults, 0, false);
                    mResults[i] = MarkType.Free;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestChoice = i;
                    }
                }
            }
            return bestChoice;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="board"></param>
        /// <param name="depth"></param>
        /// <param name="isMaximazing"></param>
        /// <returns></returns>
        private double minimax(MarkType[] board, int depth, bool isMaximazing)
        {
            string result = checkForWinner(false);
            if (result != null)
            {
                return Scores[result];
            }
            if (isMaximazing)
            {
                double bestScore = double.NegativeInfinity;
                for (int i = 0; i < board.Length; i++)
                {
                    if (board[i] == MarkType.Free)
                    {
                        board[i] = MarkType.Nought;
                        double score = minimax(board, depth + 1, false);
                        board[i] = MarkType.Free;
                        bestScore = Math.Max(score, bestScore);
                    }
                }
                return bestScore;
            }
            else
            {
                double bestScore = double.PositiveInfinity;
                for (int i = 0; i < board.Length; i++)
                {
                    if (board[i] == MarkType.Free)
                    {
                        board[i] = MarkType.Nought;
                        double score = minimax(board, depth + 1, true);
                        board[i] = MarkType.Free;
                        bestScore = Math.Min(score, bestScore);
                    }
                }
                return bestScore;
            }
        }

        /// <summary>
        /// Checks if there is a winner of a 3 line straight
        /// </summary>
        /// <param name="doFinish">True if you want to finish the game if there is a winner</param>
        /// <returns>string or null</returns>
        private string checkForWinner(bool doFinish = true)
        {
            string result = null;
            //check for horizontal wins
            if (mResults[0] != MarkType.Free && (mResults[0] & mResults[1] & mResults[2]) == mResults[0])
            {
                //End Game
                if (doFinish)
                {
                    mGameEnded = true;
                    //highlight btns
                    btn00.Background = btn10.Background = btn20.Background = Brushes.Yellow;
                }
                result = mResults[0].ToString();
            }
            else if (mResults[3] != MarkType.Free && (mResults[3] & mResults[4] & mResults[5]) == mResults[3])
            {
                //End Game
                if (doFinish)
                {
                    mGameEnded = true;
                    //highlight btns
                    btn01.Background = btn11.Background = btn21.Background = Brushes.Yellow;
                }
                result = mResults[3].ToString();
            }
            else if (mResults[6] != MarkType.Free && (mResults[6] & mResults[7] & mResults[8]) == mResults[6])
            {
                //End Game
                if (doFinish)
                {
                    mGameEnded = true;
                    //highlight btns
                    btn02.Background = btn12.Background = btn22.Background = Brushes.Yellow;
                }
                result = mResults[6].ToString();
            }

            //check for vertical wins
            else if (mResults[0] != MarkType.Free && (mResults[0] & mResults[3] & mResults[6]) == mResults[0])
            {
                //End Game
                if (doFinish)
                {
                    mGameEnded = true;
                    //highlight btns
                    btn00.Background = btn01.Background = btn02.Background = Brushes.Yellow;
                }
                result = mResults[0].ToString();
            }
            else if (mResults[1] != MarkType.Free && (mResults[1] & mResults[4] & mResults[7]) == mResults[1])
            {
                //End Game
                if (doFinish)
                {
                    mGameEnded = true;
                    //highlight btns
                    btn10.Background = btn11.Background = btn12.Background = Brushes.Yellow;
                }
                result = mResults[1].ToString();
            }
            else if (mResults[2] != MarkType.Free && (mResults[2] & mResults[5] & mResults[8]) == mResults[2])
            {
                //End Game
                if (doFinish)
                {
                    mGameEnded = true;
                    //highlight btns
                    btn20.Background = btn21.Background = btn22.Background = Brushes.Yellow;
                }
                result = mResults[2].ToString();
            }

            //check for diagonal wins
            else if (mResults[0] != MarkType.Free && (mResults[0] & mResults[4] & mResults[8]) == mResults[0])
            {
                //End Game
                if (doFinish)
                {
                    mGameEnded = true;
                    //highlight btns
                    btn00.Background = btn11.Background = btn22.Background = Brushes.Yellow;
                }
                result = mResults[0].ToString();
            }
            else if (mResults[6] != MarkType.Free && (mResults[6] & mResults[4] & mResults[2]) == mResults[6])
            {
                //End Game
                if (doFinish)
                {
                    mGameEnded = true;
                    //highlight btns
                    btn02.Background = btn11.Background = btn20.Background = Brushes.Yellow;
                }
                result = mResults[6].ToString();
            }

            //check for tie
            else if (!mResults.Any(c => c == MarkType.Free))
            {
                //End Game
                if (doFinish)
                {
                    mGameEnded = true;
                    //highlight all buttons
                    Container.Children.Cast<Button>().ToList().ForEach(btn =>
                    {
                        btn.Background = Brushes.Gray;
                    });
                }
                result = "Tie";
            }

            Dictionary<string, SolidColorBrush> Colors = new Dictionary<string, SolidColorBrush>();
            Colors["Tie"] = Brushes.Gray;
            Colors["Cross"] = Brushes.Red;
            Colors["Nought"] = Brushes.Green;


            if (doFinish && result != null)
            {
                lblResult.Header = result;
                lblResult.Foreground = Colors[result];
            }

            return result;
        }

        /// <summary>
        /// Handles the click of the MenuItem "1 vs 1"
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e">The event of the click</param>
        private void p1vp2Click(object sender, RoutedEventArgs e)
        {
            p2IsComputer = btnP1vComp.IsChecked = false;
            btnP1vP2.IsChecked = true;
            NewGame();
        }

        /// <summary>
        /// Handles the click of the MenuItem "1 vs Computer"
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e">The event of the click</param>
        private void p1vcompClick(object sender, RoutedEventArgs e)
        {
            //check the menuItem "1 vs computer"
            p2IsComputer = btnP1vComp.IsChecked = true;
            //make sure the menuItem "1 vs 1" is not checked
            btnP1vP2.IsChecked = false;
            NewGame();
        }

        /// <summary>
        /// Starts a new Game
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e">The event of the click</param>
        private void newGameClick(object sender, RoutedEventArgs e)
        {
            NewGame();
        }
    }
}
