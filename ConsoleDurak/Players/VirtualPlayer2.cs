namespace ConsoleDurak
{
    internal class VirtualPlayer2 : Player
    {
        internal VirtualPlayer2(string name)
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

                // дефолтная атака 
                // выбор наименьшего некозыря для атаки до вальта не включая. Старшие карты не подкидываются
                foreach (var card in PlayerKoloda)
                {
                    if (card.GetMast != kozyr.GetMast)
                    {
                        if (nominals.Contains(card.GetNominal))
                        {
                            if ((int)card.GetNominal < 6)
                            {
                                attackCard = CardMin(attackCard, card);
                            }
                        }
                    }
                }

                // если после этого карты для атаки нет, а защищается игрок перед нападающим
                if (attackCard == null && Durak.PreviosPlayer(Players, this).PlayerStatus == status.Защищающийся)
                {
                    // выбор наименьшего некозыря для атаки, включая старшие карты
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

                // если карты для атаки все равно нет, а защищается игрок перед нападающим
                // можно атаковать мелкими козырями, атакует с наименьшего
                if (attackCard == null && Durak.PreviosPlayer(Players, this).PlayerStatus == status.Защищающийся)
                {
                    foreach (var card in PlayerKoloda)
                    {
                        if (card.GetMast == kozyr.GetMast)
                        {
                            if (nominals.Contains(card.GetNominal))
                            {
                                if ((int)card.GetNominal < 6)
                                {
                                    attackCard = CardMin(attackCard, card);
                                }
                            }
                        }
                    }
                }

                //если после этого нет карты для атаки
                // и если осталось два игрока - валит всеми козырями что есть
                if (attackCard == null && Players.Where(p => p.PlayerStatus != status.ВышелИзИгры).Count() == 2)
                {
                    // выбор наименьшего козыря для атаки
                    foreach (var card in PlayerKoloda)
                    {
                        if (card.GetMast == kozyr.GetMast)
                        {
                            if (nominals.Contains(card.GetNominal))
                            {
                                attackCard = CardMin(attackCard, card);
                            }
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

            // значения карт в игре
            var nominals = cardsInGame.Select(e => e.GetNominal);

            //карты с повторяющимся номиналом
            List<Card> doubleAndTripleCards = DoubleAndTripleCards();

            //если атакуют не козырем
            if (cardsInGame.Last().GetMast != kozyr.GetMast)
            {
                // сначала бить номиналом, который уже есть в игре
                foreach (var card in PlayerKoloda)
                {
                    if (nominals.Contains(card.GetNominal))
                    {
                        if (card.GetMast == cardsInGame.Last().GetMast)
                        {
                            if (card.GetNominal > cardsInGame.Last().GetNominal)
                            {
                                defendCard = CardMin(defendCard, card);
                            }
                        }
                    }
                }

                //если после этого карты для защиты нет
                if (defendCard == null)
                {
                    // выбор некозыря для защиты из повторящихся по номиналу карт
                    foreach (var card in doubleAndTripleCards)
                    {
                        if (card.GetMast == cardsInGame.Last().GetMast)
                        {
                            if (card.GetNominal > cardsInGame.Last().GetNominal)
                            {
                                defendCard = CardMin(defendCard, card);
                            }
                        }
                    }
                }

                //если после этого карты для защиты нет
                if (defendCard == null)
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
                }

                //если после этого карты для защиты нет
                if (defendCard == null)
                {
                    // бить козырем с таким же номиналом, который уже есть в игре 
                    foreach (var card in PlayerKoloda)
                    {
                        if (nominals.Contains(card.GetNominal))
                        {
                            if (card.GetMast == kozyr.GetMast)
                            {
                                defendCard = CardMin(defendCard, card);
                            }
                        }
                    }
                }

                //если после этого карты для защиты нет
                if (defendCard == null)
                {
                    // выбор козыря для защиты из повторяющихся карт
                    foreach (var card in doubleAndTripleCards)
                    {
                        if (card.GetMast == kozyr.GetMast)
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
                            defendCard = CardMin(defendCard, card);
                        }
                    }
                }
            }
            //если атака козырем
            else
            {
                //если после этого карты для защиты нет
                if (defendCard == null)
                {
                    // бить козырем с таким же номиналом, который уже есть в игре 
                    foreach (var card in PlayerKoloda)
                    {
                        if (nominals.Contains(card.GetNominal))
                        {
                            if (card.GetMast == kozyr.GetMast)
                            {
                                if (card.GetNominal > cardsInGame.Last().GetNominal)
                                {
                                    defendCard = CardMin(defendCard, card);
                                }
                            }
                        }
                    }
                }

                //если после этого карты для защиты нет
                if (defendCard == null)
                {
                    // выбор козыря для защиты из повторяющихся карт
                    foreach (var card in doubleAndTripleCards)
                    {
                        if (card.GetMast == kozyr.GetMast)
                        {
                            if (card.GetNominal > cardsInGame.Last().GetNominal)
                            {
                                defendCard = CardMin(defendCard, card);
                            }

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
                            if (card.GetNominal > cardsInGame.Last().GetNominal)
                            {
                                defendCard = CardMin(defendCard, card);
                            }
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
            //карта для подкида
            Card podkidCard = null;

            // значения карт в игре
            var nominals = cardsInGame.Select(e => e.GetNominal);
            
            //если игроков в игре много, подкидывает до десятки, не включая
            if (Players.Where(p => p.PlayerStatus != status.ВышелИзИгры).Count() == 4)
            {
                foreach (var card in PlayerKoloda)
                {
                    if (card.GetMast != kozyr.GetMast)
                    {
                        if (nominals.Contains(card.GetNominal))
                        {
                            if ((int)card.GetNominal < 5)
                            {
                                podkidCard = CardMin(podkidCard, card);
                            }
                        }
                    }
                }
            }

            //если игроков в игре 3, подкидывает до дамы, не включая
            if (Players.Where(p => p.PlayerStatus != status.ВышелИзИгры).Count() == 3 && Durak.PreviosPlayer(Players, this).PlayerStatus != status.Защищающийся)
            {
                foreach (var card in PlayerKoloda)
                {
                    if (card.GetMast != kozyr.GetMast)
                    {
                        if (nominals.Contains(card.GetNominal))
                        {
                            if ((int)card.GetNominal < 7)
                            {
                                podkidCard = CardMin(podkidCard, card);
                            }
                        }
                    }
                }
            }

            //если игроков в игре 2 - подкидывает все что есть, кроме козырей
            if (Players.Where(p => p.PlayerStatus != status.ВышелИзИгры).Count() == 2)
            {
                foreach (var card in PlayerKoloda)
                {
                    if (card.GetMast != kozyr.GetMast)
                    {
                        if (nominals.Contains(card.GetNominal))
                        {
                            podkidCard = CardMin(podkidCard, card);
                        }
                    }
                }
            }

            PlayerKoloda.Remove(podkidCard);
            return podkidCard;
        }


    }
}

