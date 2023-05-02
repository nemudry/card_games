using ConsoleDurak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleDurak.Card;

namespace ConsoleDurak
{

    public class Koloda
    {
        //колода в игре
        public Stack<Card> GetKoloda { get; set; }


        // создание новой колоды на 36 карт
        public Koloda(int amountOfCards)
        {
            GetKoloda = new Stack<Card>();

            if (amountOfCards == 36)
            { 
                // создание новой колоды на 36 карт
                for (Card.Nominal nominal = Card.Nominal.Шесть; nominal <= Card.Nominal.Туз; nominal++)
                {
                    for (Card.Mast mast = Card.Mast.буби; mast <= Card.Mast.крести; mast++)
                    {
                        GetKoloda.Push(new Card(nominal, mast));
                    }
                }
            }

            if (amountOfCards == 52)
            {
                // создание новой колоды на 52 карты
                for (Card.Nominal nominal = Card.Nominal.Два; nominal <= Card.Nominal.Туз; nominal++)
                {
                    for (Card.Mast mast = Card.Mast.буби; mast <= Card.Mast.крести; mast++)
                    {
                        GetKoloda.Push(new Card(nominal, mast));
                    }
                }
            }

        }

    }

}


