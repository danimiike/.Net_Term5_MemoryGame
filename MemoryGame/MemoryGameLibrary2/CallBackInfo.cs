/*
    Class name: CallBackInfo.cs
    Authors:    Danielle Miike, Priscilla Peron, Renato Medeiros
    Date:       April/7/2022
    Purpose:    An object used to send relevant information between server and client
 */
using System.Runtime.Serialization;
namespace MemoryGameLibrary
{
    [DataContract]
    public class CallBackInfo
    {
        
        [DataMember]
        public int[] playersScore;
        [DataMember]
        public int[] lastBlocks;
        
        [DataMember]
        public int[] guess;
        [DataMember]
        public char[] guessValue;
        [DataMember]
        public int playerTurn;
        [DataMember]
        public bool lastWasMatch;
        [DataMember]
        public int numOfPlayers;
        [DataMember]
        public bool isGameOver;
        
        public CallBackInfo(int[] score, int[] gss, char[] gssVal, int playerTurn, int[] lstBlocks, bool lwm, int numOfPlayers, bool isGameOver)
        {
            this.playersScore = score;
            this.guess = gss;
            this.guessValue = gssVal;
            this.playerTurn = playerTurn;
            this.lastBlocks = lstBlocks;
            this.lastWasMatch = lwm;
            this.numOfPlayers = numOfPlayers;
            this.isGameOver = isGameOver;
        }

    }
}
