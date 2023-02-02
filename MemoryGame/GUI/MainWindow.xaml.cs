/*
    Class name: MainWindow.xaml.cs
    Authors:    Danielle Miike, Priscilla Peron, Renato Medeiros
    Date:       April/7/2022
 */

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ServiceModel;  // WCF Types
using System.Threading;     // Thread class
using MemoryGameLibrary;

namespace GUI
{

    public partial class MainWindow : Window, ICallBack
    {
        private IMemoryGame mg = null;
        Dictionary<int, string> chart = new Dictionary<int, string>() { { 0, "a0"}, {1, "a1" }, {2, "a2"}, { 3, "a3" }, { 4, "a4" }, { 5, "b0" }, { 6, "b1" }, { 7, "b2" }, { 8, "b3" }, { 9, "b4" },
          {10, "c0"},{11, "c1"},{12, "c2"},{13, "c3"},{14, "c4"},{15, "d0"},{16, "d1"},{17, "d2"},{18, "d3"},{19, "d4"}};
        private int ID = 0;
        public MainWindow()
        {
            InitializeComponent();
            
            try
            {
                // Connect to the Shoe service and subscribe to the callbacks 
                DuplexChannelFactory<IMemoryGame> channel = new DuplexChannelFactory<IMemoryGame>(this, "MemoryGameEndpoint");
                mg = channel.CreateChannel();
                mg.RegisterForCallbacks();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //Method:    UpdateGrid.
        //Arguments: An integer representing the number of players and a second integer representing the player ID.
        //Purpose:   Helper method to show grid according to the number of players connected to the server (max 4).
        //return:    Void
        public void UpdateGrid(int numOfPlayers, int playerId)
        {
            
            if(ID == 0)
            {
                ID = playerId;
            }
            if(ID==1)
            {
                submit.Visibility = Visibility.Visible;
            }
            switch (numOfPlayers)
            {
                case 1:
                    player1grid.Visibility = Visibility.Visible;
                    player2grid.Visibility = Visibility.Hidden;
                    player3grid.Visibility = Visibility.Hidden;
                    player4grid.Visibility = Visibility.Hidden;
                    break;
                case 2:
                    player1grid.Visibility = Visibility.Visible;
                    player2grid.Visibility = Visibility.Visible;
                    player3grid.Visibility = Visibility.Hidden;
                    player4grid.Visibility = Visibility.Hidden;
                    break;
                case 3:
                    player1grid.Visibility = Visibility.Visible;
                    player2grid.Visibility = Visibility.Visible;
                    player3grid.Visibility = Visibility.Visible;
                    player4grid.Visibility = Visibility.Hidden;
                    break;
                case 4:
                    player1grid.Visibility = Visibility.Visible;
                    player2grid.Visibility = Visibility.Visible;
                    player3grid.Visibility = Visibility.Visible;
                    player4grid.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }
        //Method:    UpdateGui.
        //Arguments: A CallBackInfo object with information from the server.
        //Purpose:   Update the GUI with relevant information to maintain the game functionality.
        //return:    Void
        public void UpdateGui(CallBackInfo info)
        {
            if (info.isGameOver == true)
            {
                MessageBox.Show($"A player left the server. Game is over!");
                System.Environment.Exit(0);
            }
            if (info.isGameOver== false &&(Thread.CurrentThread == this.Dispatcher.Thread))
            {
                //Enable the Submit button only for player who plays on this turn.
                if(info.playerTurn==ID)
                {
                    submit.Visibility = Visibility.Visible;

                }
                else
                {
                    submit.Visibility = Visibility.Hidden;
                }
                //Display a marker to show the player's turn.
                changeTurn(info.playerTurn);
                //Update the score for each player
                score1.Content = info.playersScore[0].ToString();
                if(info.numOfPlayers>1)
                {
                    score2.Content = info.playersScore[1].ToString();
                }
                if(info.numOfPlayers>2)
                {
                    score3.Content = info.playersScore[2].ToString();
                }
                if(info.numOfPlayers>3)
                {
                    score4.Content = info.playersScore[3].ToString();
                }
                //Reveal the block chosen by the player
                getLabel(info.guess[0]).Content = info.guessValue[0];
                getLabel(info.guess[1]).Content = info.guessValue[1];
                //If it is a match, remove the blocks from the reference dictionary. (used to check which blocks must be removed)
                if(info.guessValue[1]==info.guessValue[0])
                {
                    chart.Remove(info.guess[1]);
                    chart.Remove(info.guess[0]);
                }
                //chart is the reference Dictionary, if it is equals to 0, game is over.
                if (chart.Count == 0)
                {

                    int index = 0;
                    int score = info.playersScore[0];
                    for (int i = 1; i < info.playersScore.Length; i++)
                    {
                        if (info.playersScore[i] > score)
                        {
                            score = info.playersScore[i];
                            index = i;
                        }
                    }
                    MessageBox.Show($"Game is Over. Player {index + 1} won!");
                }
                //Hide the previous chosen blocks if there was a match or remove them as options.
                if (info.lastBlocks!=null)
                {
                    if (info.lastWasMatch)
                    {
                        getLabel(info.lastBlocks[0]).Content = "";
                        getLabel(info.lastBlocks[1]).Content = "";
                    }
                    else
                    {
                        if(!(info.lastBlocks[0]==info.guess[0] || info.lastBlocks[0] == info.guess[1]))
                        {
                            getLabel(info.lastBlocks[0]).Content = getLabel(info.lastBlocks[0]).Name;
                        }
                        if (!(info.lastBlocks[1] == info.guess[0] || info.lastBlocks[1] == info.guess[1]))
                        {
                            getLabel(info.lastBlocks[1]).Content = getLabel(info.lastBlocks[1]).Name;
                        }  
                    }
                }
            }
            else
            {
                Action<CallBackInfo> updateDelegate = UpdateGui;
                this.Dispatcher.BeginInvoke(updateDelegate, info);
            }
            
        }
        //Submit button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int gs1=-1;
            int gs2=-1;
            char[] result= { };
            //Use the chart dictionary to translate the user's guess to its respective index.          
            foreach (var item in chart)
            {
                if(item.Value==guess1.Text)
                {
                    gs1 = item.Key;
                }
                if(item.Value==guess2.Text)
                {
                    gs2 = item.Key;
                }
                
            }
            if(gs1!=-1 && gs2!=-1&& chart.ContainsValue(guess1.Text) && chart.ContainsValue(guess2.Text))
            {
                //**Send the player's guess to the server side with 2 indexes.**
                 mg.makeAGuess(gs1, gs2);
            }
            else
            {
                MessageBox.Show("Invalid input. Type a valid label #");
            }
        }

        //Method:    changeTurn.
        //Arguments: an integer representing the current turn.
        //Purpose:   Helper method to update player turn marker.
        //return:    Void
        private void changeTurn(int playerTurn)
        {
            switch (playerTurn)
            {
                case 1:
                    turn1.Visibility = Visibility.Visible;
                    turn2.Visibility = Visibility.Hidden;
                    turn3.Visibility = Visibility.Hidden;
                    turn4.Visibility = Visibility.Hidden;
                    break;
                case 2:
                    turn2.Visibility = Visibility.Visible;
                    turn3.Visibility = Visibility.Hidden;
                    turn4.Visibility = Visibility.Hidden;
                    turn1.Visibility = Visibility.Hidden;
                    break;
                case 3:
                    turn3.Visibility = Visibility.Visible;
                    turn1.Visibility = Visibility.Hidden;
                    turn4.Visibility = Visibility.Hidden;
                    turn2.Visibility = Visibility.Hidden;
                    break;
                case 4:
                    turn4.Visibility = Visibility.Visible;
                    turn3.Visibility = Visibility.Hidden;
                    turn1.Visibility = Visibility.Hidden;
                    turn2.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;

            }
        }
        //Method:    getLabel.
        //Arguments: an integer representing the title index.
        //Purpose:   Helper method to find a Label using its index representation.
        //Return:    A label from a respective index.
        private Label getLabel(int index)
        {
            switch (index)
            {
                case 0:
                    return a0;
                case 1:
                    return a1;
                case 2:
                    return a2;
                case 3:
                    return a3;
                case 4:
                    return a4;
                case 5:
                    return b0;
                case 6:
                    return b1;
                case 7:
                    return b2;
                case 8:
                    return b3;
                case 9:
                    return b4;
                case 10:
                    return c0;
                case 11:
                    return c1;
                case 12:
                    return c2;
                case 13:
                    return c3;
                case 14:
                    return c4;
                case 15:
                    return d0;
                case 16:
                    return d1;
                case 17:
                    return d2;
                case 18:
                    return d3;
                case 19:
                    return d4;
                default:
                    return null;

            }
        }
        //Method:    InstructionsClick.
        //Arguments: Internal arguments.
        //Purpose:   Display the instructions Window.
        //Return:    Void.
        private void InstructionsClick(object sender, RoutedEventArgs e)
        {
            Instructions instuc = new Instructions();
            instuc.Show();
        }
        //Method:    Window_Closing.
        //Arguments: Internal arguments.
        //Purpose:   Inform the server side that a player quit.
        //Return:    Void.
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mg?.UnregisterFromCallbacks();
        }
    }
}
