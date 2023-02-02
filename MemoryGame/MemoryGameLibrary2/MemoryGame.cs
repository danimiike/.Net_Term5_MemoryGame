/*
    Class name:  MemoryGame.cs
    Authors:     Danielle Miike, Priscilla Peron, Renato Medeiros
    Date:        April/7/2022
    Description: MemoryGame library.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
namespace MemoryGameLibrary
{
    public interface ICallBack
    {
        [OperationContract(IsOneWay = true)]
        void UpdateGui(CallBackInfo info);
        [OperationContract(IsOneWay = true)]
        void UpdateGrid(int numOfPlayers, int playerID);
    }

    [ServiceContract(CallbackContract = typeof(ICallBack))]
    public interface IMemoryGame
    {
        [OperationContract(IsOneWay = true)]
        void makeAGuess(int guess1, int guess2);
        int PlayerTurn { [OperationContract] get; }
        [OperationContract]
        int[] playerScore();
        [OperationContract(IsOneWay = true)]
        void RegisterForCallbacks();
        [OperationContract(IsOneWay = true)]
        void UnregisterFromCallbacks();
    }
    /*
    Class name:  Player.cs
    Description: Player object.
    */
    public class Player
    {
        private int score { get; set; }
        public Player()
        {
            score = 0;
        }
        public void increaseScore(){ score++; }
        public int getScore() { return score; }
        public void resetScore() { score = 0; } 
        
    }
   
    
    
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class MemoryGame : IMemoryGame
    {
        private List<Player> playersList;
        private char[] charsArray = new char[] { '!','@', '#', '$','%','^','&','*','<','+'}; // 10 chars max
        private List<char> blocksList;
        private int playerTurn;
        private int pairsFound;
        private HashSet<ICallBack> callbacks = new HashSet<ICallBack>();
        private int[] lastBlocks = null;
        private bool lastWasAmatch = false;
        private int playerID;
        public MemoryGame()
        {
            blocksList = new List<char>();
            for (int i = 0; i < charsArray.Length; i++) //add a pair of each char
            {
                blocksList.Add(charsArray[i]);
                blocksList.Add(charsArray[i]);
            }
            //shuffle the order
            Random rand = new Random();
            blocksList = blocksList.OrderBy(card => rand.Next()).ToList();
            playerID = 1;
            //generate the playersList
            playersList = new List<Player>();
            playerTurn = 1;
            pairsFound = 0;
        }//end constructor

        //Method:    makeAGuess.
        //Arguments: An integer representing the first guess and a second integer representing second guess.
        //Purpose:   Method which handle most part of the game logic. Check the user's guess, scores if the guess is right and update all the clients.
        //return:    Void
        public void makeAGuess ( int guess1, int guess2)
        {
            if(blocksList[guess1] == blocksList[guess2])//Match found
            {
                playersList[playerTurn - 1].increaseScore();
                pairsFound++;
            }
            playerTurn++;//increases the turn of the game
            if (playerTurn > playersList.Count)//loop the turn into the number of players
            {
                playerTurn = 1;
            }
            //Call the UpdateAllClients method with the necessary arguments.
            UpdateAllClients(new int[] { guess1, guess2 }, new char[] { blocksList[guess1], blocksList[guess2] }, lastBlocks, lastWasAmatch, playersList.Count, false);
            lastWasAmatch = false;
            lastBlocks = new int[] { guess1, guess2 };
            if(blocksList[guess1]==blocksList[guess2])//if it is a match, keeps a refence bool to disable the match blocks
            {
                lastWasAmatch = true;
            }
        }
       
        public int PlayerTurn
        {
            get { return playerTurn; }
        }
        //Method:    playerScore.
        //Arguments: none.
        //Purpose:   Builds an array with all players' score.
        //return:    An array of integers with the score of all players ordered by its indexes.
        public int[] playerScore()
        {
            int[] returnScore = new int[playersList.Count];
            for (int i = 0; i < playersList.Count; i++)
            {
                returnScore[i] = playersList[i].getScore();
            }
            return returnScore;
        }
        //Method:    RegisterForCallbacks.
        //Arguments: none.
        //Purpose:   Register a new client into the server to send that client updates.
        //return:    Void.
        public void RegisterForCallbacks()
        {
            if(playersList.Count<4)
            { 
                playersList.Add(new Player());
                ICallBack cb = OperationContext.Current.GetCallbackChannel<ICallBack>();
                if (!callbacks.Contains(cb))
                {
                    callbacks.Add(cb);
                    NewPlayerJoined(playersList.Count);
                }
            }
        }
        //Method:    RegisterForCallbacks.
        //Arguments: none.
        //Purpose:   Register a new client into the server to send that client updates.
        //return:    Void.
        public void UnregisterFromCallbacks()
        {
            ICallBack cb = OperationContext.Current.GetCallbackChannel<ICallBack>();
            if (callbacks.Contains(cb))
            {
                callbacks.Remove(cb);
                UpdateAllClients(null, null, null, false, 0, true);
            }
        }
        //Method:    UpdateAllClients.
        //Arguments: Int[] pair of indexes, char[] pair of values, int[] last pair of indexes, bool last was a match, int number of players, bool games is over.
        //Purpose:   Send an update to all clients registred into the server.
        //return:    Void.
        private void UpdateAllClients(int[] pairOfIndexes, char[] pairOfValues, int[] lastPairOfIndexes, bool lwm, int numOfPlayers, bool gameOver)
        {
            CallBackInfo info = new CallBackInfo(playerScore(), pairOfIndexes, pairOfValues, playerTurn, lastPairOfIndexes, lwm, numOfPlayers, gameOver);

            foreach (ICallBack cb in callbacks)
            {
                cb.UpdateGui(info);
            }

        }
        //Method:    NewPlayerJoined.
        //Arguments: An integer representing the new number of players.
        //Purpose:   Send an update to all clients informing that a new player registred into the server.
        //return:    Void.
        private void NewPlayerJoined(int numOfPlayers)
        {

            foreach (ICallBack cb in callbacks)
            {
                cb.UpdateGrid(numOfPlayers, playerID);
            }
            playerID++;
        }
    }
}
