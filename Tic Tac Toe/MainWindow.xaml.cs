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
        #endregion

        #region constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            //make sure the game starts p1 vs p2
            p2IsComputer = false;
            btnP1vP2.IsChecked = true;

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

            if (!mP1Turn && p2IsComputer)
            {
                //find the button that should be written
                btn = findButton(computerTurn());
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

        private Button findButton(int idx)
        {
            List<Button> listBtns = new List<Button>(){
                btn00,btn01,btn02,
                btn10,btn11,btn12,
                btn20,btn21,btn22
            };

            return listBtns[idx];
        }

        /// <summary>
        /// Simulates the computer turn
        /// </summary>
        /// <returns>index of button that computer choses</returns>
        private int computerTurn()
        {
            HashSet<int> possibleCells = new HashSet<int>();
            //int[] possibleCells = new int[mResults.Length - 1];
            for (int i = 0; i < mResults.Length; i++)
            {
                if (mResults[i] == MarkType.Free)
                {
                    possibleCells.Add(i);
                }
            }
            //chose a random free cell
            Random randomIndex = new Random();
            int idx = randomIndex.Next(possibleCells.Min(), possibleCells.Max());
            return idx;

        }

        /// <summary>
        /// Checks if there is a winner of a 3 line straight
        /// </summary>
        private void checkForWinner()
        {
            //check for horizontal wins
            if (mResults[0] != MarkType.Free && (mResults[0] & mResults[1] & mResults[2]) == mResults[0])
            {
                //End Game
                mGameEnded = true;
                //highlight btns
                btn00.Background = btn10.Background = btn20.Background = Brushes.Yellow;
            }
            if (mResults[3] != MarkType.Free && (mResults[3] & mResults[4] & mResults[5]) == mResults[3])
            {
                //End Game
                mGameEnded = true;
                //highlight btns
                btn01.Background = btn11.Background = btn21.Background = Brushes.Yellow;
            }
            if (mResults[6] != MarkType.Free && (mResults[6] & mResults[7] & mResults[8]) == mResults[6])
            {
                //End Game
                mGameEnded = true;
                //highlight btns
                btn02.Background = btn12.Background = btn22.Background = Brushes.Yellow;
            }

            //check for vertical wins
            if (mResults[0] != MarkType.Free && (mResults[0] & mResults[3] & mResults[6]) == mResults[0])
            {
                //End Game
                mGameEnded = true;
                //highlight btns
                btn00.Background = btn01.Background = btn02.Background = Brushes.Yellow;
            }
            if (mResults[1] != MarkType.Free && (mResults[1] & mResults[4] & mResults[7]) == mResults[1])
            {
                //End Game
                mGameEnded = true;
                //highlight btns
                btn10.Background = btn11.Background = btn12.Background = Brushes.Yellow;
            }
            if (mResults[2] != MarkType.Free && (mResults[2] & mResults[5] & mResults[8]) == mResults[2])
            {
                //End Game
                mGameEnded = true;
                //highlight btns
                btn20.Background = btn21.Background = btn22.Background = Brushes.Yellow;
            }

            //check for diagonal wins
            if (mResults[0] != MarkType.Free && (mResults[0] & mResults[4] & mResults[8]) == mResults[0])
            {
                //End Game
                mGameEnded = true;
                //highlight btns
                btn00.Background = btn11.Background = btn22.Background = Brushes.Yellow;
            }
            if (mResults[6] != MarkType.Free && (mResults[6] & mResults[4] & mResults[2]) == mResults[6])
            {
                //End Game
                mGameEnded = true;
                //highlight btns

                btn02.Background = btn11.Background = btn20.Background = Brushes.Yellow;
            }

            //check for tie
            if (!mResults.Any(c => c == MarkType.Free))
            {
                //End Game
                mGameEnded = true;
                //highlight all buttons
                Container.Children.Cast<Button>().ToList().ForEach(btn =>
                {
                    btn.Background = Brushes.Gray;

                });
            }
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
            p2IsComputer = btnP1vComp.IsChecked = true;
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
