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
        internal abstract Card Attack(Card kozyr, List<Card> cardsInGame);


        //защита
        internal abstract Card Defend(Card kozyr, List<Card> cardsInGame);



    }
}