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

        public string Name { get; set; } 

        public List<Card> PlayerKoloda { get; set; }
        public Status PlayerStatus { get; set; }


        public enum Status
        {
            Neitral,
            Attacking,            
            Defending,
            QuitTheGame
        }

        //атака 
        public abstract Card Attack(Card kozyr, List<Card> cardsInGame);


        //защита
        public abstract Card Defend(Card kozyr, List<Card> cardsInGame);



    }
}