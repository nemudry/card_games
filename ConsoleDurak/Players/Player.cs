using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleDurak.Card;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleDurak
{
    public abstract class Player
    { 

        internal string Name { get; set; }
        internal List<Card> PlayerKoloda { get; set; }
        internal Status PlayerStatus { get; set; }


        internal enum Status
        {
            Neitral,
            Attacking,            
            Defending,
            QuitTheGame
        }

        //атака 
        internal abstract Card Attack(Card kozyr, List<Card> cardsInGame);


        //защита
        internal abstract Card Defend(Card kozyr, List<Card> cardsInGame);



    }
}