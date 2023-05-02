namespace ConsoleDurak
{

    public class VirtualPlayer : Player
    {
        public VirtualPlayer(string name)
        {
            Name = name;
            PlayerStatus = Status.Neitral;

            PlayerKoloda = new List<Card>();
        }


        //выбирается наименьшая карта по номиналу 
        private Card CardMin(Card card, Card x)
        {
            if (x.GetNominal <= card!.GetNominal)
            {
                card = x;
            }
            return card;
        }

        private Card CardNull (Card card, Card x)
        {
            if (card == null)
            {
                card = x;
            }
            return card;
        }

        public override Card Attack(Card kozyr, List<Card> cardsInGame)
        {
            //карта для атаки
            Card attack = null;


            // если в игре нет карт - первая атака
            if (cardsInGame.Count == 0)
            {
                //карта для атаки, выбирается одна для сравнения с остальными
                attack = PlayerKoloda.First();

                foreach (var card in PlayerKoloda)
                {
                    // если в руке одни козыри
                    if (PlayerKoloda.All(card => card.GetMast == kozyr.GetMast))
                    {
                        //выбирается наименьший козырь
                        attack = CardMin(attack, card);
                    }

                    else
                    {
                        //выбор некозыря
                        if (card.GetMast != kozyr.GetMast)
                        {
                            //на случай, если первая карта в руке козырь, она сбрасывается на первый некозырь
                            if (attack.GetMast == kozyr.GetMast)
                            {
                                attack = card;
                            }

                            //затем выбирается наименьший некозырь
                            attack = CardMin(attack, card);
                        }
                    }
                }

                //удаление карты из руки
                PlayerKoloda.Remove(attack);
                return attack;
            }


            // если в игре есть карты - обычная атака
            else
            {
                // номиналы карт в игре
                var nominals = from card in cardsInGame
                               let nominal = card.GetNominal
                               select nominal;


                // выбор наименьшего некозыря для атаки
                foreach (var card in PlayerKoloda)
                {
                    if (card.GetMast != kozyr.GetMast)
                    {
                        if (nominals.Contains(card.GetNominal))
                        {

                            attack = CardNull(attack, card);

                            attack = CardMin(attack, card);

                        }
                    }
                }
            }


            PlayerKoloda.Remove(attack);
            return attack;
        }


        public override Card Defend(Card kozyr, List<Card> cardsInGame)
        {
            //карта для защиты
            Card defend = null;


            //если атакуют не козырем
            if (cardsInGame.Last().GetMast != kozyr.GetMast)
            {

                // выбор наименьшего некозыря для защиты
                foreach (var card in PlayerKoloda)
                {
                    if (card.GetMast == cardsInGame.Last().GetMast)
                    {
                        if (card.GetNominal > cardsInGame.Last().GetNominal)
                        {
                            defend = CardNull(defend, card);

                            defend = CardMin(defend, card);
                        }
                    }
                }

                //если после этого карты для защиты нет
                if (defend == null)
                {
                    // выбор наименьшего козыря для защиты
                    foreach (var card in PlayerKoloda)
                    {

                        if (card.GetMast == kozyr.GetMast)
                        {
                            defend = CardNull(defend, card);

                            //выбирается меньший некозырь по номиналу
                            defend = CardMin(defend, card);
                        }
                    }
                }
            }


            //если атака козырем
            else
            {
                // выбор наименьшего козыря для защиты
                foreach (var card in PlayerKoloda)
                {
                    if (card.GetMast == kozyr.GetMast)
                    {
                        if (card.GetNominal > cardsInGame.Last().GetNominal)
                        {
                            defend = CardNull(defend, card);

                            //выбирается меньший некозырь по номиналу
                            defend = CardMin(defend, card);
                        }
                    }
                }

            }

            //удаление карты из руки        
            PlayerKoloda.Remove(defend);
            return defend;
        }


    }
}

