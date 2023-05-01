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
                        if (card.GetNominal <= attack.GetNominal)
                        {
                            attack = card;
                        }
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
                            if (card.GetNominal <= attack.GetNominal)
                            {
                                attack = card;
                            }
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
                            if (attack == null)
                            {
                                attack = card;
                            }

                            if (attack!.GetNominal > card.GetNominal)
                            {
                                attack = card;
                            }

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
                            if (defend == null)
                            {
                                defend = card;
                            }

                            if (defend!.GetNominal > card.GetNominal)
                            {
                                defend = card;
                            }
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
                            if (defend == null)
                            {
                                defend = card;
                            }

                            if (card.GetNominal < defend!.GetNominal) //выбирается меньший некозырь по номиналу
                            {
                                defend = card;
                            }
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
                            if (defend == null)
                            {
                                defend = card;
                            }

                            if (card.GetNominal < defend!.GetNominal) //выбирается меньший некозырь по номиналу
                            {
                                defend = card;
                            }
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

