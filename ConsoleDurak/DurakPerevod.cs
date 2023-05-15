namespace ConsoleDurak
{
    internal class DurakPerevod : Durak
    {
        internal override string Name { get; }
        internal DurakPerevod()
        {
            Name = "Дурак переводной";
        }

        internal DurakPerevod(string playerName, int amountOfPlayers, int botLevel)
            :base(playerName, amountOfPlayers, botLevel)
        {
            Name = "Дурак переводной";
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
        protected virtual bool GameHod()
        {
            Player attackingPlayer = AttackPlayer();
            Player defendingPlayer = DefendPlayer();
            List<Player> AlreadyAttackedPlayer = new List<Player>();
            Card attackCard; //карта для атаки
            Card defendCard; // карта для защиты
            bool whoWinHod = true; // если успешно отзащищался - тру, неуспешно - фолс                                                               

            do
            {
                do
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

                    //Показ карт в игре
                    ShowCardsInGame(CardsInGame, Kozyr);
                    Console.WriteLine();
                    Thread.Sleep(2000);

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
                    }

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

                    //Показ карт в игре
                    ShowCardsInGame(CardsInGame, Kozyr);

                    //проверка на макс.количество сыгранных карт и наличия карт у защищающегося
                    if (MaxCardCheck()) break;

                } while (true);


                // выбор следующего атакующего игрока
                if (DurakStatus != statusGame.КонецХода)
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
    }
}
