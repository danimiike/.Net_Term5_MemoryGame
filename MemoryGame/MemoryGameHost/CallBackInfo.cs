using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MemoryGameHost
{
    [DataContract]
    public class CallBackInfo
    {
        [DataMember]
        int players1Score;
        [DataMember]
        int players2Score;
        [DataMember]
        int players3Score;
        [DataMember]
        int players4Score;
        [DataMember]
        char[] guess;
        [DataMember]
        int[] guessIndexes;
        public CallBackInfo(int p1, int p2, int p3, int p4, char[] gss,int[] gssIdx)
        {
            players1Score = p1;
            players2Score = p2;
            players3Score = p3;
            players4Score = p4;
            guess = gss;
            guessIndexes = gssIdx;
        }

    }
}
