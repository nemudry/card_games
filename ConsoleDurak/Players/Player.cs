namespace ConsoleDurak
{
    public abstract class Player
    { 
        internal string Name { get; set; }
        internal List<Card> PlayerKoloda { get; set; }
        internal status PlayerStatus { get; set; }

        internal enum status
        {
            Атакующий,
            Нейтральный,
            Защищающийся,
            ВышелИзИгры
        }

        //атака 
        internal abstract Card Attack(Card kozyr, List<Card> cardsInGame, List<Player> Players);

        //защита
        internal abstract Card Defend(Card kozyr, List<Card> cardsInGame);

        //подкид
        internal abstract Card Podkid(Card kozyr, List<Card> cardsInGame, List<Player> Players);
        

        //выбирается наименьшая карта по номиналу
        internal Card CardMin(Card card, Card x)
        {
            if (card == null)
            {
                card = x;
            }

            if (x.GetNominal <= card!.GetNominal)
            {
                card = x;
            }
            return card;
        }

        //карты с повторяющимся номиналом в игре
        internal List<Card> DoubleAndTripleCards()
        {
            //карты с повторяющимся номиналом в игре
            List<Card> doubleAndTripleCards = new List<Card>();

            foreach (var card1 in PlayerKoloda)
            {
                foreach (var card2 in PlayerKoloda)
                {
                    //если номинал повторяется, карта вносится в список
                    if (card1.GetNominal == card2.GetNominal && card1.GetMast != card2.GetMast)
                    {
                        if (!doubleAndTripleCards.Contains(card1))//чтобы одна и та же карта больше не вносилась
                        {
                            doubleAndTripleCards.Add(card1);
                        }
                    }
                }
            }
            return doubleAndTripleCards;
        }
    }
}