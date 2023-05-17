
namespace ConsoleDurak
{
    internal class DurakPerevod : Durak
    {
        internal override string Name { get; }

        internal statusPerevod PerevodGame { get; set; }
        internal countPerevod CountPerevod { get; set; }
        internal enum countPerevod
        {
            БезПеревода,
            Переведено1Раз,
            Переведено2Раз,
            Переведено3Раз,
        }
        internal enum statusPerevod
        {
            МожноПеревести,
            БитьПеревод,
            НельзяПеревести,
        }

        internal DurakPerevod()
        {
            Name = "Дурак переводной";
        }

        internal DurakPerevod(string playerName, int amountOfPlayers, int botLevel)
            :base(playerName, amountOfPlayers, botLevel)
        {
            Name = "Дурак переводной";
            PerevodGame = statusPerevod.МожноПеревести;
            CountPerevod = countPerevod.БезПеревода;
        }


        public override void StartGame()
        {
            try
            {
                //Очередность игроков в игре
                PlayersInfo();

                //Перемешать колоду
                ShuffleDeck();

                //раздать карты
                PassOutCards();

                //козырь в игре
                SetKosyr();

                //кто первый ходит
                WhoFirstHod(Players, Kozyr, out Card firstHod, out Player firstHodPlayer);
                Thread.Sleep(2500);
                Console.Clear();

                //определение защищающегося игрока
                NextPlayer(firstHodPlayer).PlayerStatus = Player.status.Защищающийся;

                do
                {
                    Color.Cyan($"Начало хода.");
                    DurakStatus = statusGame.НачалоХода;
                    Console.WriteLine();
                    foreach (Player player in Players)
                    {
                        if (player.PlayerStatus == Player.status.Атакующий)
                        {
                            Color.CyanShort($"Атакует игрок ");
                            Color.Green($"{player.Name}.");
                        }

                        if (player.PlayerStatus == Player.status.Защищающийся)
                        {
                            Color.CyanShort($"Защищается игрок ");
                            Color.Red($"{player.Name}.");
                        }
                    }
                    Console.WriteLine();
                    Thread.Sleep(1000);

                    //Фаза игрового хода. Игроки атакуют, защищающийся защищается
                    bool whoWinGameHod = GameHod();
                    Thread.Sleep(1500);
                    Console.Clear();

                    //Донабор карт. если в колоде есть карты
                    if (Desk.Cards.Any())
                    {
                        DonaborKart();
                        Console.WriteLine();
                        Thread.Sleep(1500);
                    }

                    //смена защищающегося и атакующего игрока     
                    ChangeAttackPlayerInGame(whoWinGameHod);

                    //Выход игрока из игры
                    ExitPlayer();

                    //Инфо об игре. Очередность игроков в игре и количество карт в игре         
                    PlayersInfo();
                    if (Desk.Cards.Any())
                    {
                        Color.CyanShort($"Количество карт в колоде: ");
                        Color.Red($"{Desk.Cards.Count()}");
                        Console.WriteLine();
                    }
                    Thread.Sleep(2500);
                    Console.Clear();

                    //Проверка. Конец игры
                    if (EndofGame()) break;

                    // выход из игры                          
                    if (DurakStatus == statusGame.КонецИгры) break;

                } while (true);

                Color.Red("Конец игры.");
            }
            catch (Exception x)
            {
                Console.WriteLine($"Ошибка {x.Message}");
            }
        }

        //Фаза игрового хода. Игроки атакуют, один игрок защищается
        protected override bool GameHod()
        {
            Player attackingPlayer = AttackPlayer();
            Player defendingPlayer = DefendPlayer();
            List<Player> AlreadyAttackedPlayer = new List<Player>();
            Card attackCard; //карта для атаки
            Card defendCard; // карта для защиты
            Card perevodCard;// карта для перевода
            bool whoWinHod = true; // если успешно отзащищался - тру, неуспешно - фолс                                                               

            do
            {
                do
                {
                    if (CountPerevod == countPerevod.БезПеревода && PerevodGame != statusPerevod.БитьПеревод)
                    {
                        // получение карты для атаки 
                        attackCard = attackingPlayer.Attack(Kozyr, CardsInGame, Players);

                        //если карты для атаки нет
                        if (attackCard == null)
                        {
                            Color.Red($"Игроку {attackingPlayer.Name} нечем атаковать. Атаковать пробуют другие игроки.");
                            AlreadyAttackedPlayer.Add(attackingPlayer);
                            Console.WriteLine();
                            Thread.Sleep(1500);
                            break;
                        }

                        //если карта для атаки есть 
                        else
                        {
                            CardsInGame.Add(attackCard); // она добавляется в карты на столе
                            AlreadyAttackedPlayer.Clear();
                            Color.Green($"{attackingPlayer.Name} атакует с карты {attackCard.GetNominal}, {attackCard.GetMast}.");
                            Console.WriteLine();
                            Thread.Sleep(1000);
                        }
                    }
                    //Показ карт в игре
                    ShowCardsInGameWithPerevod();
                    Console.WriteLine();
                    Thread.Sleep(2000);


                    //Перевод карт
                    if (PerevodGame == statusPerevod.МожноПеревести && CardsInGame.Count() < NextPlayer(defendingPlayer).PlayerKoloda.Count() +1 )
                    {
                        if (defendingPlayer.PlayerKoloda.Select(e => e.GetNominal).Contains(CardsInGame.Last().GetNominal))
                        {
                            int answerPerevod = 1;
                            if (defendingPlayer is AlivePlayer)
                            {
                                do
                                {
                                    Color.Green($"Есть карта для перевода. Перевести? \n[1] Да. \n[2] Нет.");

                                    //ответ игрока
                                    answerPerevod = Table.PlayerAnswer();

                                    //проверка условий
                                    if (Table.CheckСonditions(answerPerevod, 2, 1)) break;

                                } while (true);
                            }

                            if (answerPerevod == 1)
                            {
                                // получение карты для перевода 
                                perevodCard = defendingPlayer.Perevod(Kozyr, CardsInGame, Players);
                                if (perevodCard != null)
                                {                                    
                                    Color.Green($"{defendingPlayer.Name} переводит картой {perevodCard.GetNominal}, {perevodCard.GetMast}.");
                                    Console.WriteLine();
                                    Thread.Sleep(2000);

                                    CountPerevod = ++CountPerevod;
                                    PerevodChangePlayer();
                                    defendingPlayer = DefendPlayer();
                                    attackingPlayer = AttackPlayer();

                                    CardsInGame.Add(perevodCard);
                                    break;
                                }
                            }
                        }
                    }

                    // получение карты для защиты 
                    defendCard = defendingPlayer.Defend(Kozyr, CardsInGame);

                    //если карты для защиты нет
                    if (defendCard == null)
                    {
                        DurakStatus = statusGame.КонецХода;
                        whoWinHod = false; // защищающийся проиграл ход

                        Color.Red($"Игроку {defendingPlayer.Name} нечем защищаться. Он забирает карты со стола, остальные игроки по желанию додают карты.");
                        Console.WriteLine();
                        Thread.Sleep(1500);
                    }
                    //если карта для защиты есть
                    else
                    {
                        Color.Green($"{defendingPlayer.Name} защищается картой {defendCard.GetNominal}, {defendCard.GetMast}.");
                        Console.WriteLine();
                        Thread.Sleep(2000);
                        CardsInGame.Add(defendCard);

                        //Перенос переведенной карты на последнее место, чтобы ее можно было побить
                        if (CountPerevod != countPerevod.БезПеревода)
                        {
                            Card cardToReplace;
                            cardToReplace = CardsInGame[0];
                            CardsInGame.Remove(cardToReplace);
                            CardsInGame.Add(cardToReplace);
                        }

                        if (CountPerevod != countPerevod.БезПеревода) CountPerevod = --CountPerevod;
                        if (CountPerevod != countPerevod.БезПеревода) PerevodGame = statusPerevod.БитьПеревод;
                        else
                        {
                            if (CardsInGame.Count() % 2 != 0) PerevodGame = statusPerevod.БитьПеревод;
                            else PerevodGame = statusPerevod.НельзяПеревести;
                        }
                    }

                    //Показ карт в игре
                    ShowCardsInGameWithPerevod();
                    Console.WriteLine();

                    //подкидывание карт
                    if (whoWinHod == false)
                    {
                        AlreadyAttackedPlayer.Clear();
                        do
                        {
                            Podkid(attackingPlayer);
                            AlreadyAttackedPlayer.Add(attackingPlayer);
                            ChangeAttackPlayerInHod();
                            if (AlreadyAttackedPlayer.Distinct().Count() == Players.Count()) break;

                        } while (true);

                        // Проигравший забирает в руку все карты со стола 
                        foreach (Card card in CardsInGame) defendingPlayer.PlayerKoloda.Add(card);
                        CardsInGame.Clear(); //карты со стола очищаются
                        break;
                    }

                    //проверка на макс.количество сыгранных карт и наличия карт у защищающегося
                    if (MaxCardCheck()) break;

                } while (true);


                // выбор следующего атакующего игрока
                if (DurakStatus != statusGame.КонецХода && PerevodGame == statusPerevod.НельзяПеревести)
                {
                    ChangeAttackPlayerInHod();
                }

                //проверка на завершение хода
                if (EndHodCheck()) break;

            } while (true);

            Color.Cyan("Передача хода!");
            Console.WriteLine();
            Thread.Sleep(500);
            return whoWinHod;

            //проверка на макс.количество сыгранных карт и наличия карт у защищающегося
            bool MaxCardCheck()
            {
                //если сыграно максимальное количество карт в ходу
                if (CardsInGame.Count() == 12)
                {
                    Color.Cyan($"Сыграно максимальное количество карт.");
                    Console.WriteLine();
                    Thread.Sleep(1500);
                    DurakStatus = statusGame.КонецХода;
                    whoWinHod = true; // защищающийся выиграл ход
                    return true;
                }

                //если у защищавшегося закончились карты
                if (defendingPlayer.PlayerKoloda.Count() == 0)
                {
                    Color.Cyan($"У игрока {defendingPlayer.Name} закончились карты.");
                    Console.WriteLine();
                    Thread.Sleep(1500);
                    DurakStatus = statusGame.КонецХода;
                    whoWinHod = true;// защищающийся выиграл ход
                    return true;
                }
                return false;
            }

            //меняет атакующего игрока в ходу
            void ChangeAttackPlayerInHod()
            {
                foreach (Player player in Players)
                {
                    if (player.PlayerStatus == Player.status.ВышелИзИгры) AlreadyAttackedPlayer.Add(player);
                    if (player.PlayerStatus == Player.status.Защищающийся) AlreadyAttackedPlayer.Add(player);
                }

                //если проатаковали не все               
                if (AlreadyAttackedPlayer.Distinct().Count() != Players.Count())
                {
                    do
                    {
                        attackingPlayer = NextPlayer(attackingPlayer);

                        if (attackingPlayer.PlayerStatus != Player.status.ВышелИзИгры && attackingPlayer.PlayerStatus != Player.status.Защищающийся)
                            break;

                    } while (true);
                }
                else
                {
                    if (DurakStatus != statusGame.КонецХода)
                    {
                        whoWinHod = true;
                        DurakStatus = statusGame.КонецХода;
                    }
                }
            }

            //проверка на завершение хода
            bool EndHodCheck()
            {
                if (DurakStatus == statusGame.КонецХода) //если ход окончен
                {
                    PerevodGame = statusPerevod.МожноПеревести;
                    CountPerevod = countPerevod.БезПеревода;

                    //если защита проиграла
                    if (!whoWinHod) return true;
                    //если защита выиграла
                    else
                    {
                        Color.Green($"Игрок {defendingPlayer.Name} успешно отзащищался!");
                        Thread.Sleep(1000);
                        Console.WriteLine();
                        CardsInGame.Clear();
                        return true;
                    }
                }
                return false;
            }
        }

        protected void PerevodChangePlayer()
        {
            Player attack = AttackPlayer();
            attack.PlayerStatus = Player.status.Нейтральный;
            Player defend = DefendPlayer();
            defend.PlayerStatus = Player.status.Атакующий;
            Player next = NextPlayer(defend);
            next.PlayerStatus = Player.status.Защищающийся;

            Color.Green($"Защищается игрок {next.Name}.");
            Console.WriteLine();
        }

        //показать карты в игре
        protected void ShowCardsInGameWithPerevod()
        {
            if (CountPerevod == countPerevod.БезПеревода) ShowCardsInGame();

            if (CountPerevod == countPerevod.Переведено1Раз)
            {
                Color.Cyan($"Карты в игре:");

                if (CardsInGame.Count() % 2 != 0)
                {
                    for (int i = 0; i < CardsInGame.Count; ++i)
                    {
                        ShowCardsInGame();
                    }
                }
                else
                {
                    if (CardsInGame.Count == 2)
                    {
                        Show1AttackCard(0);
                        Show1AttackCard(1);
                    }

                    if (CardsInGame.Count == 4)
                    {
                        Show1AttackCard(0);
                        Show1AttackCardShort(1);
                        Show1DefendCard(2);
                        Show1AttackCard(3);
                    }
                }
            }   

            if (CountPerevod == countPerevod.Переведено2Раз)
            {
                Color.Cyan($"Карты в игре:");

                if (CardsInGame.Count == 3)
                {
                    for (int i = 0; i < CardsInGame.Count; ++i)
                    {
                        Show1AttackCard(i);
                    }
                }

                if (CardsInGame.Count == 4)
                {
                    Show1AttackCardShort(0);
                    Show1DefendCard(1);
                    Show1AttackCard(2);
                    Show1AttackCard(3);
                }

            }

            if (CountPerevod == countPerevod.Переведено3Раз)
            {
                Color.Cyan($"Карты в игре:");

                if (CardsInGame.Count == 4)
                {
                    for (int i = 0; i < CardsInGame.Count; ++i)
                    {
                        Show1AttackCard(i);
                    }
                }

                if (CardsInGame.Count == 5)
                {
                    Show1AttackCardShort(0);
                    Show1DefendCard(1);
                    Show1AttackCard(2);
                    Show1AttackCard(3);
                    Show1AttackCard(4);
                }

                if (CardsInGame.Count == 6)
                {
                    Show1AttackCardShort(0);
                    Show1DefendCard(1);
                    Show1AttackCardShort(2);
                    Show1DefendCard(3);
                    Show1AttackCard(4);
                    Show1AttackCard(5);
                }

            }

            void Show1AttackCardShort (int i)
            {
                Console.Write($"{CardsInGame[i].GetNominal}, {CardsInGame[i].GetMast}");
                if (Kozyr.GetMast == CardsInGame[i].GetMast) Color.GreenShort(" (козырь)");
            }
            void Show1AttackCard(int i)
            {
                Console.Write($"{CardsInGame[i].GetNominal}, {CardsInGame[i].GetMast}");
                if (Kozyr.GetMast == CardsInGame[i].GetMast) Color.Green(" (козырь).");
                else
                {
                    Console.Write(".");
                    Console.WriteLine();
                }
            }
            void Show1DefendCard(int i)
            {
                Console.Write($" - бита картой {CardsInGame[i].GetNominal}, {CardsInGame[i].GetMast}");
                if (Kozyr.GetMast == CardsInGame[i].GetMast) Color.GreenShort(" (козырь).");
                else
                {
                    Console.Write(".");
                    Console.WriteLine();
                }
            }
        }
    }


    
}
