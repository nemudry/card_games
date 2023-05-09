namespace ConsoleDurak
{

    internal class VirtualPlayer : Player
    {
        internal VirtualPlayer(string name)
        {
            Name = name;
            PlayerStatus = status.Нейтральный;
            PlayerKoloda = new List<Card>();
        }



        internal override Card Attack(Card kozyr, List<Card> cardsInGame)
        {
            //карта для атаки
            Card attack = null;


            // если в игре нет карт - первая атака
            if (cardsInGame.Count == 0)
            {

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
                            //выбирается наименьший некозырь
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

                            attack = CardMin(attack, card);

                        }
                    }
                }
            }


            PlayerKoloda.Remove(attack);
            return attack;
        }


        internal override Card Defend(Card kozyr, List<Card> cardsInGame)
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




        //выбирается наименьшая карта по номиналу 
        private Card CardMin(Card card, Card x)
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
    }
}

