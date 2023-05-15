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

        internal override Card Attack(Card kozyr, List<Card> cardsInGame, List<Player> Players)
        {
            //карта для атаки
            Card attackCard = null;

            // если в игре нет карт - первая атака
            if (cardsInGame.Count == 0)
            {
                foreach (var card in PlayerKoloda)
                {
                    // если в руке одни козыри
                    if (PlayerKoloda.All(card => card.GetMast == kozyr.GetMast))
                    {
                        //выбирается наименьший козырь
                        attackCard = CardMin(attackCard, card);
                    }
                    else
                    {
                        //выбор некозыря
                        if (card.GetMast != kozyr.GetMast)
                        {
                            //выбирается наименьший некозырь
                            attackCard = CardMin(attackCard, card);
                        }
                    }
                }

                //удаление карты из руки
                PlayerKoloda.Remove(attackCard);
                return attackCard;
            }
            // если в игре есть карты - обычная атака
            else
            {
                // номиналы карт в игре
                var nominals = cardsInGame.Select(e => e.GetNominal);

                // выбор наименьшего некозыря для атаки
                foreach (var card in PlayerKoloda)
                {
                    if (card.GetMast != kozyr.GetMast)
                    {
                        if (nominals.Contains(card.GetNominal))
                        {
                            attackCard = CardMin(attackCard, card);
                        }
                    }
                }
            }
            PlayerKoloda.Remove(attackCard);
            return attackCard;
        }

        internal override Card Defend(Card kozyr, List<Card> cardsInGame)
        {
            //карта для защиты
            Card defendCard = null;

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
                            defendCard = CardMin(defendCard, card);
                        }
                    }
                }

                //если после этого карты для защиты нет
                if (defendCard == null)
                {
                    // выбор наименьшего козыря для защиты
                    foreach (var card in PlayerKoloda)
                    {
                        if (card.GetMast == kozyr.GetMast)
                        {
                            //выбирается меньший некозырь по номиналу
                            defendCard = CardMin(defendCard, card);
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
                            defendCard = CardMin(defendCard, card);
                        }
                    }
                }
            }

            //удаление карты из руки        
            PlayerKoloda.Remove(defendCard);
            return defendCard;
        }

        internal override Card Podkid(Card kozyr, List<Card> cardsInGame, List<Player> Players)
        {
            return Attack(kozyr, cardsInGame, Players);
        }
    }
}

