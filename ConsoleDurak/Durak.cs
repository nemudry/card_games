namespace ConsoleDurak
{
    internal class Durak : Game
    {
        internal override string Name { get; }
        private int BotLevel { get; }
        private Koloda Desk { get; }
        private List<Card> CardsInGame { get; }

        private Card Kozyr { get; set; }
        protected override List<Player> Players { get; }
        internal statusGame DurakStatus { get; set; }
        internal enum statusGame
        {
            НачалоХода,
            КонецХода,
            КонецИгры,
        }

        internal Durak()
        {
            Name = "Дурак подкидной";
        }

        internal Durak(string playerName, int amountOfPlayers, int botLevel)
        {
            Name = "Дурак подкидной";
            BotLevel = botLevel;
            DurakStatus = statusGame.НачалоХода;
            Desk = new Koloda(36);
            Kozyr = null;
            CardsInGame = new List<Card>(12); 

            Players = new List<Player>();
            Players.Add(new AlivePlayer(playerName));  // Живой игрок

            //ввод виртуальных игроков в список игроков
            switch (amountOfPlayers)
            {
                case 2:
                    Players.Add(new VirtualPlayer("Сало"));
                    break;
                case 3:
                    Players.Add(new VirtualPlayer("Дерево"));
                    Players.Add(new VirtualPlayer("Сало"));
                    break;
                case 4:
                    Players.Add(new VirtualPlayer("Дерево"));
                    Players.Add(new VirtualPlayer("Сало"));
                    Players.Add(new VirtualPlayer("Дельфин"));
                    break;
                default: break;
            };
        }
        public void RunGame()
        {
            try
            {
                //Очередность игроков в игре
                PlayersInfo();
                //Перемешать колоду
                Random random = new Random();
                var kolodaforShuffle = Desk.Cards.ToList<Card>();
                Desk.Cards.Clear();
                for (int card; kolodaforShuffle.Count() > 0; kolodaforShuffle.Remove(kolodaforShuffle[card]))
                {
                    card = random.Next(0, kolodaforShuffle.Count());
                    Desk.Cards.Push(kolodaforShuffle[card]);// создание перемешанной колоды    
                }

                //раздача карт
                foreach (Player player in Players)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        player.PlayerKoloda.Add(Desk.Cards.Pop());  
                    }
                }

                //козырь в игре
                Kozyr = Desk.Cards.ToArray<Card>().Last();
                Color.Cyan($"Козырь в игре - {Kozyr.GetNominal}, {Kozyr.GetMast}");
                Thread.Sleep(1000);

                //кто первый ходит
                WhoFirstHod(Players, Kozyr, out Card firstHod, out Player firstHodPlayer);
                firstHodPlayer.PlayerStatus = Player.status.Атакующий;
                Color.GreenShort($"Наименьший козырь {firstHod.GetNominal}, {firstHod.GetMast}. ");
                Color.Cyan($"Первым ходит {firstHodPlayer.Name}.");
                Console.WriteLine();
                Thread.Sleep(2500);
                Console.Clear();                

                //определение защищающегося игрока
                NextPlayer(firstHodPlayer).PlayerStatus = Player.status.Защищающийся;

                StartGame();

                Color.Red("Конец игры.");
            }
            catch(Exception x)
            {
                Console.WriteLine($"Ошибка {x.Message}");
            }
        }

        //Очень громоздкий метод
        //Не понятно зачем разделил на Run и Start
        //Такие действия как: перемешать колоду, определить козырь и кто первый ходит выносим отдельными функциями(пригодятся в переводном например)
        private void StartGame()
        {
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
                    //Фаза донабора карт из колоды
                    DonaborKart();
                    Console.WriteLine();
                    Thread.Sleep(1500);
                }

                //смена защищающегося и атакующего игрока     
                ChangeAttackPlayerInGame();

                //Выход игрока из игры
                ExitPlayer();
                
                //Инфо об игре. Очередность игроков в игре и количество карт в игре и на руках              
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

                //смена защищающегося и атакующего игрока   
                void ChangeAttackPlayerInGame()
                {
                    //если ранее игрок успешно отзащищался
                    if (whoWinGameHod)
                    {
                        Player attack = AttackPlayer();
                        attack.PlayerStatus = Player.status.Нейтральный;
                        Player defend = DefendPlayer();
                        defend.PlayerStatus = Player.status.Атакующий;
                        Player next = NextPlayer(defend);
                        next.PlayerStatus = Player.status.Защищающийся;
                    }

                    //если игрок не защитился
                    else
                    {
                        Player attack = AttackPlayer();
                        attack.PlayerStatus = Player.status.Нейтральный;
                        Player defend = DefendPlayer();
                        defend.PlayerStatus = Player.status.Нейтральный;
                        Player next = NextPlayer(defend);
                        next.PlayerStatus = Player.status.Атакующий;
                        Player next2 = NextPlayer(next);
                        next2.PlayerStatus = Player.status.Защищающийся;
                    }
                }

                //Выход игрока из игры
                void ExitPlayer()
                {
                    foreach (Player player in Players)
                    {
                        if (player.PlayerStatus != Player.status.ВышелИзИгры)
                        {
                            //если после донабора, у игрока нет на руках карт
                            if (player.PlayerKoloda.Count() == 0)
                            {
                                if (player.PlayerStatus == Player.status.Атакующий)
                                {
                                    Player next = NextPlayer(player);
                                    next.PlayerStatus = Player.status.Атакующий;
                                    Player next2 = NextPlayer(next);
                                    next2.PlayerStatus = Player.status.Защищающийся;
                                    player.PlayerStatus = Player.status.ВышелИзИгры;

                                    Color.Green($"Игрок {player.Name} вышел из игры.");
                                    Console.WriteLine();
                                    Thread.Sleep(1500);

                                }

                                if (player.PlayerStatus == Player.status.Защищающийся)
                                {
                                    Player next = NextPlayer(player);
                                    next.PlayerStatus = Player.status.Защищающийся;
                                    player.PlayerStatus = Player.status.ВышелИзИгры;

                                    Color.Green($"Игрок {player.Name} вышел из игры.");
                                    Console.WriteLine();
                                    Thread.Sleep(1500);
                                }


                                if (player.PlayerStatus == Player.status.Нейтральный)
                                {
                                    player.PlayerStatus = Player.status.ВышелИзИгры;
                                    Color.Green($"Игрок {player.Name} вышел из игры.");
                                    Console.WriteLine();
                                    Thread.Sleep(1500);
                                }

                            }
                        }
                    }
                }

                //Проверка. Конец игры
                bool EndofGame()
                {
                    var LastPlayer = Players.Where(e => e.PlayerStatus != Player.status.ВышелИзИгры);

                    if (LastPlayer.Count() == 1)
                    {
                        Color.Cyan($"Игрок {LastPlayer.First().Name} проиграл. {LastPlayer.First().Name} - дурак!");
                        Console.WriteLine();
                        DurakStatus = statusGame.КонецИгры;
                        Thread.Sleep(1500);

                        return true;
                    }

                    return false;
                }

            } while (true);
        }

        // кто первый ходит
        private void WhoFirstHod(List<Player> players, Card kozyr, out Card firstHod, out Player firstHodPlayer)
        {
            //наименьший козырь в игре, и игрок у которого этот козырь
            firstHod = null;
            firstHodPlayer = null;

            // перебор козырей
            foreach (Player player in players)
            {
                foreach (var card in player.PlayerKoloda)
                {
                    if (card.GetMast == kozyr.GetMast)
                    {
                        //если козырь ноль, то сперва любой козырь 
                        if (firstHod == null)
                        {
                            firstHod = card;
                            firstHodPlayer = player;
                        }

                        //дальше козырь с наименьшим номиналом
                        if (firstHod!.GetNominal > card.GetNominal)
                        {
                            firstHod = card;
                            firstHodPlayer = player;
                        }
                    }
                }
            }

            // если козырей нет ни у кого на руках - выбирается самый старший некозырь
            if (firstHod == null)
            {
                // перебор  карт
                foreach (Player player in players)
                {
                    foreach (var card in player.PlayerKoloda)
                    {
                        //первая карта для сравнения
                        if (firstHod == null)
                        {
                            firstHod = card;
                            firstHodPlayer = player;
                        }
                        //карта с наибольшим номиналом
                        if (card.GetNominal > firstHod.GetNominal)
                        {
                            firstHod = card;
                            firstHodPlayer = player;
                        }

                        //карта с наибольшей мастью
                        if (card.GetMast > firstHod.GetMast)
                        {
                            firstHod = card;
                            firstHodPlayer = player;
                        }
                    }
                }
            }
        }

        //Фаза игрового хода. Игроки атакуют, один игрок защищается
        private bool GameHod()
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
                    attackCard = attackingPlayer.Attack(Kozyr, CardsInGame);

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


                        //Фаза подкидывания карт
                        //   PhasePodkid(players, attackPlayer, defendPlayer, cardsInGame, desk, kozyr);
                       // Thread.Sleep(1000);


                        // Проигравший забирает в руку все карты со стола 
                        foreach (Card card in CardsInGame) defendingPlayer.PlayerKoloda.Add(card);
                        CardsInGame.Clear(); //карты со стола очищаются
                        break;
                    }

                    //если карта для защиты есть
                    else
                    {
                        Color.Green($"{defendingPlayer.Name} защищается картой {defendCard.GetNominal}, {defendCard.GetMast}.");
                        Console.WriteLine();
                        Thread.Sleep(2000);
                        CardsInGame.Add(defendCard);
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
                    whoWinHod = true;
                    DurakStatus = statusGame.КонецХода;
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



        //Фаза донабора карт из колоды
        public void DonaborKart()
        {

            Color.Cyan($"Игроки добирают карты из колоды.");
            Thread.Sleep(1500);

            
            bool isDeskEmpty = false; //пуста ли колода
            Card newCard;//новая карта, взятая из колоды
            Player donaborPlayer = AttackPlayer();
            List<Player> donaboredPlayers = new List<Player>(); //список игроков, которые уже брали карты из колоды

            do
            {

                if (donaborPlayer.PlayerKoloda.Count() < 6 && Desk.Cards.Any())
                {
                    Color.Green($"Игрок {donaborPlayer.Name} берет карты из колоды.");
                    Thread.Sleep(1000);
                }
                if (donaborPlayer.PlayerKoloda.Count() > 5)
                {
                    Color.Green($"Игроку {donaborPlayer.Name} не нужно брать карты из колоды.");
                    Thread.Sleep(1000);
                }


                //игрок берет из колоды карты
                donaboredPlayers.Add(donaborPlayer); // пытающийся брать из колоды добавляется в список
                while (donaborPlayer.PlayerKoloda.Count() < 6 && !isDeskEmpty)
                {
                    if (!isDeskEmpty) // если колода не пуста, игрок берет карты пока в руке не окажется 6 карт
                    {
                        Desk.Cards.TryPop(out newCard);
                        donaborPlayer.PlayerKoloda.Add(newCard); //добавление карты в руку

                        //если в колоде кончились карты
                        if (Desk.Cards.Count() == 0)
                        {
                            Color.Cyan($"Игрок забирает последнюю карту из колоды - козырь {newCard.GetNominal}, {newCard.GetMast}.");
                            isDeskEmpty = true;
                            break;
                        }
                    }
                }


                //Проверка. выходы из цикла донабора, если колода пуста или брали все игроки
                if (CannotDonabor()) break;


                //Смена игрока для донабора карт
                ChangeDonaborPlayer ();

            } while (true);


            //Проверка. выходы из цикла донабора, если колода пуста или брали все игроки
            bool CannotDonabor()
            {
                //выход из цикла, если колода пуста.
                if (isDeskEmpty)
                {
                    Color.Red($"В колоде нет карт.");
                    Thread.Sleep(2000);
                    return true;
                }

                //Если количество пытавшихся брать карты равно количеству игроков
                if (donaboredPlayers.Count() == Players.Count())
                {
                    return true;
                }

                return false;
            }


            //Смена игрока для донабора карт
            void ChangeDonaborPlayer()
            {

                foreach (Player player in Players)
                {
                    if (player.PlayerStatus == Player.status.ВышелИзИгры) donaboredPlayers.Add(player);
                }

                //Если количество пытавшихся брать карты равно количеству атакующих и нейтралов, берет защищающийся
                if (donaboredPlayers.Count() == (Players.Count() - 1))
                {
                    donaborPlayer = DefendPlayer();
                    return;
                }


                do
                {
                    donaborPlayer = NextPlayer(donaborPlayer);

                    if (donaborPlayer.PlayerStatus != Player.status.ВышелИзИгры && donaborPlayer.PlayerStatus != Player.status.Защищающийся)
                        break;

                } while (true);

            }
        }




        /*

        //Фаза подкидывания карт
        public void PhasePodkid(List<Player> players, Player attackPlayer, Player defendPlayer,
            List<Card> cardsInGame, Koloda desk, Card kozyr)
        {

            bool canPodkid = true; //можно ли подкидывать
            Card podkid = null; //карта для подкида
            List<Player> podkidPlayers = new List<Player>(); //список уже подкидывавших игроков


            //вычисление количества карт, которые можно подкинуть
            int countPodkid = 6 - ((cardsInGame.Count() / 2) + 1); // максимум подкидывать остаток от шести атакующих карт
            if ((defendPlayer.PlayerKoloda.Count() - 1) < countPodkid) 
                countPodkid = (defendPlayer.PlayerKoloda.Count() - 1); //но не больше, чем есть на руках у защитника


            do
            {
                //если количество пытавшихся подкинуть равно количеству атакующих игроков (то есть пробовали подкинуть все)
                if (podkidPlayers.Count() == players.Count() - 1)
                {
                    Color.Red($"У игроков отсутствуют карты для подкида.");
                    Thread.Sleep(1500);
                    Console.WriteLine();
                    canPodkid = false;
                    break; //выход из цикла
                }


                //игрок добавляеся в список пытавшихся подкинуть карты
                podkidPlayers.Add(attackPlayer);

                if (countPodkid != 0) // пока подкидывать можно, игрок пытается подкинуть
                {
                    do
                    {
                        //один и тот же игрок подкидывает карту, пока есть карты для подкида. В зависимости от уровня сложности
                        switch (botLevel)
                        {
                            case 1:
                                podkid = attackPlayer.PodkidLevelOne(players, cardsInGame, desk, kozyr);
                                break;
                            case 2:
                                podkid = attackPlayer.PodkidLevelTwo(players, cardsInGame, desk, kozyr);
                                break;
                            default:
                                podkid = null;
                                break;
                        }


                        if (podkid != null)
                        {
                            //если карта для подкида есть, она подкидывается, а счетчик уменьшается
                            cardsInGame.Add(podkid);
                            countPodkid--;
                        }

                        //если один и тот же игрок не может подкинуть карту - выход, и смена подкидывающего игрока
                        else
                        {
                            break;
                        }

                    } while (countPodkid != 0);
                }


                if (countPodkid == 0)
                {
                    Color.Red($"Максимальное количество карт подкинуто. Больше подкидывать нельзя!");
                    Thread.Sleep(1000);
                    Console.WriteLine();
                    canPodkid = false;
                    break;
                }


                // выбор следующего подкидывающего игрока
                int currentIndexOfAttackPlayer = players.IndexOf(attackPlayer);
                if (canPodkid)
                {
                    foreach (var player in players)
                    {
                        //если игрок не защищающийся, и если еще не подкидывал
                        if (player != defendPlayer && !podkidPlayers.Contains(player))
                        {
                            try
                            {
                                attackPlayer = players[++currentIndexOfAttackPlayer];
                                if (attackPlayer == defendPlayer)
                                {
                                    ++currentIndexOfAttackPlayer;
                                    attackPlayer = players[currentIndexOfAttackPlayer];
                                }
                            }
                            catch (ArgumentOutOfRangeException x)
                            {
                                currentIndexOfAttackPlayer = 0;
                                attackPlayer = players[currentIndexOfAttackPlayer];
                                if (attackPlayer == defendPlayer)
                                {
                                    ++currentIndexOfAttackPlayer;
                                    attackPlayer = players[currentIndexOfAttackPlayer];
                                }
                            }
                            /// Color.Green($"Смена подкидывающего игрока. Подкидывает {attackPlayer.Name}");
                            break;

                        }
                    }
                }

            } while (canPodkid);
        }
        */







        //Очередность игроков в игре
        private void PlayersInfo()
        {
            Color.Cyan("Очередность игроков в игре:");
            int playerNumber = 0;
            foreach (Player player in Players)
            {
                if (player.PlayerStatus != Player.status.ВышелИзИгры)
                {
                    Console.WriteLine($"{++playerNumber}. {player.Name}.");
                }
            }
            Console.WriteLine();
            Thread.Sleep(1000);
        }


        //показать карты в игре
        private void ShowCardsInGame(List<Card> cardsInGame, Card kozyr)
        {
            Color.Cyan($"Карты в игре:");

            int j = 1;
            for (int i = 0; i < cardsInGame.Count; i += 2)
            {
                Console.Write($"{cardsInGame[i].GetNominal}, {cardsInGame[i].GetMast}");
                if (kozyr.GetMast == cardsInGame[i].GetMast) Color.GreenShort(" (козырь)");

                // если карта в игре нечетная - конец вывода
                if (i % 2 != 0)
                {
                    Console.Write(".");
                    Console.WriteLine();

                }

                // если карта в игре четная - выводится карта, которой она бита
                if (i % 2 == 0)
                {
                    for (; j < cardsInGame.Count;)
                    {
                        Console.Write($" - бита картой {cardsInGame[j].GetNominal}, {cardsInGame[j].GetMast}");
                        if (kozyr.GetMast == cardsInGame[j].GetMast) Color.GreenShort(" (козырь).");
                        else Console.Write(".");
                        Console.WriteLine();
                        j += 2;
                        break;
                    }
                }
            }

            Console.WriteLine();
        }


        //следующий игрок по списку игроков
        private Player NextPlayer(Player player)
        {

            int currentIndexOfPlayer = Players.IndexOf(player);//индекс игрока
            Player nextPlayer;
            try
            {
                do
                {
                    if (Players[++currentIndexOfPlayer].PlayerStatus != Player.status.ВышелИзИгры)
                    {
                        nextPlayer = Players[currentIndexOfPlayer];
                        break;
                    }

                } while (true);

            }
            catch (ArgumentOutOfRangeException x)
            {
                currentIndexOfPlayer = -1;
                do
                {
                    if (Players[++currentIndexOfPlayer].PlayerStatus != Player.status.ВышелИзИгры)
                    {
                        nextPlayer = Players[currentIndexOfPlayer];
                        break;
                    }
                } while (true);

            }
            return nextPlayer;
        }

        //текущий атакующий игрок
        private Player AttackPlayer()
        {
            foreach (Player player in Players)
            {
                if (player.PlayerStatus == Player.status.Атакующий)
                {
                    return player;
                }
            }
            return null;
        }

        //текущий защищающийся игрок
        private Player DefendPlayer()
        {
            foreach (Player player in Players)
            {
                if (player.PlayerStatus == Player.status.Защищающийся)
                {
                    return player;
                }
            }
            return null;
        }

    }
}
