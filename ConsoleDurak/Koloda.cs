﻿namespace ConsoleDurak
{
    internal class Koloda
    {
        //колода в игре
        internal Stack<Card> Cards { get; }

        // создание новой колоды на 36 карт
        internal Koloda(int amountOfCards)
        {
            Cards = new Stack<Card>();

            if (amountOfCards == 36)
            { 
                // создание новой колоды на 36 карт
                for (Card.Nominal nominal = Card.Nominal.Шесть; nominal <= Card.Nominal.Туз; nominal++)
                {
                    for (Card.Mast mast = Card.Mast.буби; mast <= Card.Mast.крести; mast++)
                    {
                        Cards.Push(new Card(nominal, mast));
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
                        Cards.Push(new Card(nominal, mast));
                    }
                }
            }
        }
    }
}


