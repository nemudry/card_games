using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static ConsoleDurak.Card;
using static ConsoleDurak.Koloda;

namespace ConsoleDurak
{
    internal class Durak : Game
    {
        private Koloda Desk { get; }
        private Card Kozyr { get; set; }

        internal override string Name { get; }
        protected override List<Player> Players { get; }

        internal Durak()
        {
            Name = "Дурак подкидной";
        }

        internal Durak(string playerName, int amountOfPlayers, int botLevel)
        {

            Name = "Дурак подкидной";
            Players = new List<Player>();
            Players.Add(new AlivePlayer(playerName));  // Живой игрок

            //ввод виртуальных игроков в список игроков
            switch (amountOfPlayers)
            {
                case 2:
                    Players.Add(new VirtualPlayer("Мед из сала"));
                    break;
                case 3:
                    Players.Add(new VirtualPlayer("Пидор из дерева"));
                    Players.Add(new VirtualPlayer("Мед из сала"));
                    break;
                case 4:
                    Players.Add(new VirtualPlayer("Пидор из дерева"));
                    Players.Add(new VirtualPlayer("Мед из сала"));
                    Players.Add(new VirtualPlayer("Дельфин из океана"));
                    break;
                default: break;
            };

            Desk = new Koloda(36);
            Kozyr = null;

        }




        Player defendPlayer, attackPlayer; //атакующий и защищающийся игроки
    





        public void StartGame()
        {
            try
            {
                //Очередность игроков в игре
                Color.Cyan("Очередность игроков в игре:");
                foreach (Player player in Players)
                {
                    Console.WriteLine($"{Players.IndexOf(player) + 1}. {player.Name}.");
                }
                Console.WriteLine();
                Thread.Sleep(1000);


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
                WhoFirstHod(Players, Kozyr, out Card firstHod, out attackPlayer);
                Color.GreenShort($"Наименьший козырь {firstHod.GetNominal}, {firstHod.GetMast}. ");
                Color.Cyan($"Первым ходит {attackPlayer.Name}.");
                Console.WriteLine();
                Thread.Sleep(2500);
                Console.Clear();




                //определение защищающегося игрока
                int currentIndexOfAttackPlayer = Players.IndexOf(attackPlayer);//индекс атакующего игрока
                try
                {
                    defendPlayer = Players[currentIndexOfAttackPlayer + 1]; //защищающийся игрок следующий по списку после атакующегося
                }
                catch (ArgumentOutOfRangeException x)
                {
                    defendPlayer = Players[0]; // если список закончился, то защищается первый игрок в списке
                }


                /// Показ карт на руках у всех игроков
                /*
                 foreach (Player player in players)
                 {
                     player.ShowCardsInHand(kozyr);
                     Console.WriteLine();
                 }
                 */


                bool isEndOfGameDurak = false; //игра закончилась
                do
                {
                    Color.Cyan($"Начало хода.");
                    Console.WriteLine();
                    Thread.Sleep(2000);

                    Color.CyanShort($"Атакует игрок ");
                    Color.GreenShort($"{attackPlayer.Name}");
                    Color.CyanShort($", защищается игрок ");
                    Color.Red($"{defendPlayer.Name}");
                    Console.WriteLine();
                    Thread.Sleep(1000);



                    //Фаза игрового хода. Игроки атакуют, защищающийся защищается
                    bool whoWinGame = PhaseGameHod(Players, attackPlayer, defendPlayer, Desk, Kozyr, 1);
                    Thread.Sleep(1500);
                    Console.Clear();


                    //Войсы.
                   // VoiceOfPlayers();


                    //Донабор карт. если в колоде есть карты
                    if (Desk.Cards.Any())
                    {
                        //Фаза донабора карт из колоды
                        PhaseDonaborKart(Players, attackPlayer, defendPlayer, Desk, Kozyr);
                        Console.WriteLine();
                        Thread.Sleep(1500);
                    }

                    ///Показ карт на руках у всех игроков
                    /*
                    foreach (Player player in players)
                    {
                        player.ShowCardsInHand(kozyr);
                        Console.WriteLine();
                    }
                    */


                    //Фаза выхода игрока из игры
                    ExitPlayer();


                    //Инфо об игре. Очередность игроков в игре и количество карт в игре и на руках
                    ProcessGameInfo();


                    //Проверка. Конец игры
                    if (EndofGame()) break;


                    //смена защищающегося и атакующего игрока                                
                    if (!isEndOfGameDurak)
                    {
                        ChangeAttackPlayerInGame();
                    }


                    /*
                    //Войсы.
                    void VoiceOfPlayers()
                    {
                        //Войсы. Если отзащищался успешно
                        if (whoWinGame)
                        {
                            //войсы. Игрок произносит речь
                            defendPlayer.GoodVoice();
                            attackPlayer.BadVoice();
                        }
                        //Если отзащищался неуспешно
                        else
                        {
                            //войсы. Игрок произносит речь
                            defendPlayer.BadVoice();
                            attackPlayer.GoodVoice();
                        }
                        Console.Clear();
                    }
                    */

                    //Фаза выхода игрока из игры
                    void ExitPlayer()
                    {
                        List<Player> ExitPlayers = new List<Player>();
                        foreach (Player player in Players)
                        {
                            //если после донабора, у игрока нет на руках карт
                            if (player.PlayerKoloda.Count() == 0)
                            {
                                ExitPlayers.Add(player);
                                Color.Green($"Игрок {player.Name} вышел из игры.");
                                Console.WriteLine();
                                Thread.Sleep(1500);

                                //довольный войс
                                //player.GoodVoice();
                                Thread.Sleep(1000);
                            }
                        }
                        foreach (Player player in ExitPlayers)
                        {
                            Players.Remove(player); // удаление игрока
                        }
                    }

                    //Очередность игроков в игре и количество карт
                    void ProcessGameInfo ()
                    {
                        //Очередность игроков в игре и количество карт
                        Color.Cyan("Очередность игроков в игре:");
                        foreach (Player player in Players)
                        {
                            Console.WriteLine($"{Players.IndexOf(player) + 1}. {player.Name}, количество карт на руках - {player.PlayerKoloda.Count()}.");
                        }
                        Console.WriteLine();
                        Thread.Sleep(1000);

                        //Количество карт в колоде, если там в принципе есть карты
                        if (Desk.Cards.Any())
                        {
                            Color.CyanShort($"Количество карт в колоде: ");
                            Color.Red($"{Desk.Cards.Count()}");
                            Console.WriteLine();
                        }
                        Thread.Sleep(2500);
                        Console.Clear();
                    }

                    //смена защищающегося и атакующего игрока   
                    void ChangeAttackPlayerInGame()
                    {

                        //если ранее игрок успешно отзащищался
                        if (whoWinGame)
                        {
                            //начинает атаковать тот, кто успешно ранее отзащищался
                            try
                            {
                                //если игрок ранее отзащищался, у него закончились карты и он вышел из игры - смена логики смены игрока
                                if (!Players.Contains(defendPlayer)) throw new Exception();


                                //начинает атаковать тот, кто ранее успешно отзащищался
                                attackPlayer = defendPlayer;
                                currentIndexOfAttackPlayer = Players.IndexOf(attackPlayer);
                                /// Color.Green($"1 Игрок {attackPlayer.Name} {currentIndexOfAttackPlayer} атакует.");


                                //выбор нового защищающегося игрока. Защищается следущий игрок
                                try
                                {
                                    defendPlayer = Players[currentIndexOfAttackPlayer + 1];
                                }
                                catch (ArgumentOutOfRangeException x)
                                {
                                    defendPlayer = Players[0];
                                }

                            }

                            catch (Exception y)
                            {
                                //если успешно защитившийся игрок вышел из игры
                                try
                                {
                                    // тогда атакует следующий после вышедшего игрока
                                    currentIndexOfAttackPlayer = Players.IndexOf(attackPlayer) + 1;
                                    attackPlayer = Players[currentIndexOfAttackPlayer];
                                    ///  Color.Green($"2 Игрок {attackPlayer.Name} {currentIndexOfAttackPlayer} атакует.");


                                    //выбор нового защищающегося игрока. Защищается следущий игрок
                                    try
                                    {
                                        defendPlayer = Players[currentIndexOfAttackPlayer + 1];
                                    }
                                    catch (ArgumentOutOfRangeException x)
                                    {
                                        defendPlayer = Players[0];
                                    }
                                }


                                catch (ArgumentOutOfRangeException x)
                                {
                                    //когда атакует следующий игрок, но вылетает исключение конца списка для перебора, атакует первый игрок в списке
                                    currentIndexOfAttackPlayer = 0;
                                    attackPlayer = Players[currentIndexOfAttackPlayer];
                                    ///   Color.Green($"3 Игрок {attackPlayer.Name} {currentIndexOfAttackPlayer} атакует.");
                                    ///   Color.Green($"3 Игрок  атакует.");


                                    //выбор нового защищающегося игрока. Защищается следущий игрок
                                    try
                                    {
                                        defendPlayer = Players[currentIndexOfAttackPlayer + 1];
                                    }
                                    catch (ArgumentOutOfRangeException z)
                                    {
                                        defendPlayer = Players[0];
                                    }
                                }
                            }
                        }



                        //если игрок не защитился
                        else
                        {
                            try
                            {
                                //То атакует следующий после защищавшегося
                                attackPlayer = Players[Players.IndexOf(defendPlayer) + 1];
                                currentIndexOfAttackPlayer = Players.IndexOf(attackPlayer);

                                //выбор нового защищающегося игрока. Защищается следущий игрок
                                try
                                {
                                    defendPlayer = Players[currentIndexOfAttackPlayer + 1];
                                }
                                catch (ArgumentOutOfRangeException x)
                                {
                                    defendPlayer = Players[0];
                                }

                            }

                            catch (ArgumentOutOfRangeException x)
                            {
                                //когда атакует следующий игрок, но вылетает исключение конца списка для перебора, атакует первый игрок в списк
                                currentIndexOfAttackPlayer = 0;
                                attackPlayer = Players[currentIndexOfAttackPlayer];

                                //выбор нового защищающегося игрока. Защищается следущий игрок
                                try
                                {
                                    defendPlayer = Players[currentIndexOfAttackPlayer + 1];
                                }
                                catch (ArgumentOutOfRangeException y)
                                {
                                    defendPlayer = Players[0];
                                }
                            }
                        }
                    }

                    //Проверка. Конец игры
                    bool EndofGame ()
                    {
                        // если в игре остался один игрок - конец игре
                        foreach (Player player in Players)
                        {
                            if (Players.Count() == 1)
                            {
                                Color.Cyan($"Игрок {player.Name} проиграл. {player.Name} - дурак!");
                                Console.WriteLine();
                                isEndOfGameDurak = true;
                                Thread.Sleep(1500);

                               // player.BadVoice();
                                Thread.Sleep(1000);
                                return true;
                            }
                        }
                        return false;
                    }

                } while (!isEndOfGameDurak);

                Color.Red("Конец игры.");
            }
            catch(Exception x)
            {
                Console.WriteLine($"Ошибка {x.Message}");
            }



        }







 



        // кто первый ходит
        public void WhoFirstHod(List<Player> players, Card kozyr, out Card firstHod, out Player firstHodPlayer)
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
        public bool PhaseGameHod(List<Player> players, Player attackPlayer, Player defendPlayer, Koloda desk, Card kozyr, int botLevel)
        {

            bool isEndHod = false; // конец хода
            bool whoWinHod = true; // если успешно отзащищался - тру, неуспешно - фолс
            
            Card attackCard; //карта для атаки
            Card defendCard; // карта для защиты

            List<Card> cardsInGame = new List<Card>(12); // карты в игре
            List<Player> attackedPlayers = new List<Player>(); //список уже атаковавших игроков
        //    List<Card> doubleAndTripleCards = defendPlayer.DoubleAndTripleCards(); //список повторящихся карт на руке защитника
            

            do //цикл защиты-атаки, смены игрока и проверки условий победы в ходе
            {
                
                do //цикл защиты-атаки
                {
                    // получение карты для атаки в зависимости от уровня сложности
                    switch (botLevel)
                    {
                        case 1:
                            attackCard = attackPlayer.Attack(kozyr, cardsInGame);
                            break;
                        case 2:
                            attackCard = attackPlayer.Attack(kozyr, cardsInGame);
                            break;
                        default:
                            attackCard = null;
                            break;
                    }


                    //если карты для атаки нет
                    if (attackCard == null)
                    {                        
                        NoAttackCard();
                        break; // если карты для атаки нет - выход из цикла атаки-защиты для смены атакующего игрока
                    }

                    //если карта для атаки есть 
                    else
                    {
                        cardsInGame.Add(attackCard); // она добавляется в карты на столе
                        Color.Green($"{attackPlayer.Name} атакует с карты {attackCard.GetNominal}, {attackCard.GetMast}.");
                        Console.WriteLine();
                        Thread.Sleep(1000);
                    }

                    //Показ карт в игре
                    ShowCardsInGame(cardsInGame, kozyr);
                    Console.WriteLine();
                    Thread.Sleep(2000);




                    // получение карты для защиты в зависимости от уровня сложности
                    switch (botLevel)
                    {
                        case 1:
                            defendCard = defendPlayer.Defend(kozyr, cardsInGame);
                            break;
                        case 2:
                            defendCard = defendPlayer.Defend(kozyr, cardsInGame);
                            break;
                        default:
                            defendCard = null;
                            break;
                    }

                    //если карты для защиты нет
                    if (defendCard == null)
                    {
                        isEndHod = true; // конец хода
                        whoWinHod = false; // защищающийся проиграл ход
                        Color.Red($"Игроку {defendPlayer.Name} нечем защищаться. Он забирает карты со стола, остальные игроки по желанию додают карты.");
                        Console.WriteLine();
                        Thread.Sleep(1500);


                        //Фаза подкидывания карт
                     //   PhasePodkid(players, attackPlayer, defendPlayer, cardsInGame, desk, kozyr);
                        Thread.Sleep(1000);


                        // Проигравший забирает в руку все карты со стола 
                        foreach (Card card in cardsInGame) defendPlayer.PlayerKoloda.Add(card); 
                        cardsInGame.Clear(); //карты со стола очищаются
                        break;
                    }

                    //если карта для защиты есть
                    else
                    {
                        Color.Green($"{defendPlayer.Name} защищается картой {defendCard.GetNominal}, {defendCard.GetMast}.");
                        Console.WriteLine();
                        Thread.Sleep(2000);
                        cardsInGame.Add(defendCard);// она добавляется в карты на столе
                    }

                    //Показ карт в игре
                    ShowCardsInGame(cardsInGame, kozyr);
                    Thread.Sleep(1000);

                    //проверка на макс.количество сыгранных карт и наличия карт у защищающегося
                    if (MaxCardCheck()) break;

                } while (cardsInGame.Count != 12); 


                // выбор следующего атакующего игрока
                if (isEndHod == false)
                {
                    ChangeAttackPlayerInHod();
                }


                //проверка на завершение хода
                if (EndHodCheck()) break;


            } while (cardsInGame.Count != 12);


            Color.Cyan("Передача хода!");
            Console.WriteLine();
            Thread.Sleep(500);
            return whoWinHod;


            //если карты для атаки нет
            void NoAttackCard()
            {
                //номиналы карт на руке атакующего
                var nominalPlayer = from card in attackPlayer.PlayerKoloda
                                    let nominal = card.GetNominal
                                    select nominal;

                //номиналы карт в игре
                var nominalGame = from card in cardsInGame
                                  let nominal = card.GetNominal
                                  select nominal;

                //номиналы карт на руке всех атакующих
                var nominalAllAttackPlayers = from player in players
                                              where player != defendPlayer
                                              let playerKoloda = player.PlayerKoloda
                                              from card in playerKoloda
                                              let nominal = card.GetNominal
                                              select nominal;


                //пересечение номиналов в руке атакующего и в игре
                var yesNominal = nominalPlayer.Intersect(nominalGame);

                //пересечение номиналов в руке всех атакующих игроков и в игре
                var yesAllNominal = nominalAllAttackPlayers.Intersect(nominalGame);


                // Всем атакующим игрокам нечем атаковать
                if (yesAllNominal.Count() == 0)
                {
                    Console.WriteLine();
                    isEndHod = true;
                    whoWinHod = true;
                    return;
                }

                //Если по факту у атакующего игрока есть номинал для атаки, но по стратегии атаки он не подкидывает карты
                if (yesNominal.Count() != 0)
                {
                    if (!attackedPlayers.Contains(attackPlayer)) // Раньше данный игрок не атаковал
                    {
                        Color.Red($"Игроку {attackPlayer.Name} нечем атаковать. Атаковать пробуют другие игроки.");
                        Console.WriteLine();
                        Thread.Sleep(1500);
                    }

                    //такой игрок дважды заносится в список атаковавших игроков, чтобы сократить цикл перебора игроков
                    attackedPlayers.Add(attackPlayer);
                    attackedPlayers.Add(attackPlayer);
                }

                //Если у атакующего игрока реально нет карты для атаки
                else
                {
                    if (!attackedPlayers.Contains(attackPlayer)) // Раньше данный игрок не атаковал
                    {
                        Color.Red($"Игроку {attackPlayer.Name} нечем атаковать. Атаковать пробуют другие игроки.");
                        Console.WriteLine();
                        Thread.Sleep(1500);
                    }
                    //такой игрок один раз заносится в список атаковавших игроков
                    attackedPlayers.Add(attackPlayer);
                }

                return;
            }

            //проверка на макс.количество сыгранных карт и наличия карт у защищающегося
            bool MaxCardCheck()
            {
                //если сыграно максимальное количество карт в ходу
                if (cardsInGame.Count() == 12)
                {
                    Color.Cyan($"Сыграно максимальное количество карт.");
                    Console.WriteLine();
                    Thread.Sleep(1500);
                    isEndHod = true;
                    whoWinHod = true; // защищающийся выиграл ход
                    return true;
                }

                //если у защищавшегося закончились карты
                if (defendPlayer.PlayerKoloda.Count() == 0)
                {
                    Color.Cyan($"У игрока {defendPlayer.Name} закончились карты.");
                    Console.WriteLine();
                    Thread.Sleep(1500);
                    isEndHod = true;
                    whoWinHod = true;// защищающийся выиграл ход
                    return true;
                }
                return false;
            }

            //вычисляет количество попыток для атаки
            void TryToAttack()
            {
                //если игроков в игре 2 или 3
                if (players.Count() < 4)
                {
                    if (attackedPlayers.Where(x => x == attackPlayer).Count() > 2) // то дается три попытки атаковать
                    {
                        whoWinHod = true;
                        isEndHod = true;
                    }

                }
                //если игроков в игре 4
                else
                {
                    if (attackedPlayers.Where(x => x == attackPlayer).Count() > 4) // то дается 4 попытки атаковать
                                                                                   // (так как игроков в игре больше, выше вероятность
                                                                                   // атаковать на новом круге
                    {
                        whoWinHod = true;
                        isEndHod = true;
                    }
                }
            }

            //меняет атакующего игрока в ходу
            void ChangeAttackPlayerInHod()
            {
                //вычисляет количество попыток для атаки
                TryToAttack();

                // выбор следующего атакующего игрока
                int currentIndexOfAttackPlayer = players.IndexOf(attackPlayer);
                foreach (var player in players)
                {
                    if (player != defendPlayer)
                    {
                        try
                        {
                            //атакует следущий игрок
                            attackPlayer = players[++currentIndexOfAttackPlayer];
                            if (attackPlayer == defendPlayer)
                            {
                                //если следующий игрок - защитник, атакует следующий за защитником
                                ++currentIndexOfAttackPlayer;
                                attackPlayer = players[currentIndexOfAttackPlayer];
                            }
                        }
                        catch (ArgumentOutOfRangeException x)
                        {
                            //если вылетело исключение конца списка, атакует первый игрок в списка
                            currentIndexOfAttackPlayer = 0;
                            attackPlayer = players[currentIndexOfAttackPlayer];
                            if (attackPlayer == defendPlayer)
                            {
                                //если первый игрок - защитник, атакует следующий за защитником

                                ++currentIndexOfAttackPlayer;
                                attackPlayer = players[currentIndexOfAttackPlayer];
                            }
                        }

                        ///Color.Red($"Смена атакующего игрока. Атакует {attackPlayer.Name}");
                        ///attackPlayer.ShowCardsInHand(kozyr); Console.WriteLine();
                        ///defendPlayer.ShowCardsInHand(kozyr); Console.WriteLine();
                        ///ShowCardsInGame(cardsInGame, kozyr);
                        break;
                    }
                }
            }

            //проверка на завершение хода
            bool EndHodCheck()
            {
                if (isEndHod) //если ход окончен
                {
                    //если защита проиграла
                    if (!whoWinHod) return true;

                    //если защита выиграла
                    else
                    {
                        Color.Green($"Игрок {defendPlayer.Name} успешно отзащищался!");
                        Thread.Sleep(1000);
                        Console.WriteLine();
                        cardsInGame.Clear();
                        return true;
                    }

                }
                return false;

            }
        }

           

        //Фаза донабора карт из колоды
        public void PhaseDonaborKart(List<Player> players, Player attackPlayer, Player defendPlayer,
            Koloda desk, Card kozyr)
        {

            Color.Cyan($"Игроки добирают карты из колоды.");
            Thread.Sleep(1500);


            bool isCanTakeFromDesk = true;    //если из колоды можно взять карты
            bool isDeskEmpty = false;               //пуста ли колода
            Card newCard = null;                    //новая карта, взятая из колоды
            List<Player> donaboredPlayers = new List<Player>(); //список игроков, которые уже брали карты из колоды

            do
            {

                //набор булевок. Возвращает тру, если у атакующего игрока полный набор карт
                var fullsets = from player in players
                               where player != defendPlayer
                               let isSetCards = player.PlayerKoloda.Count() >= 6 ? true : false
                               select isSetCards;


                //Если у всех атакующих полный набор карт и количество пытавшихся брать карты равно количеству атакующих, берет защищающийся
                if (fullsets.All(set => set == true) && donaboredPlayers.Count() == (players.Count() - 1))
                {
                    attackPlayer = defendPlayer;
                }



                donaboredPlayers.Add(attackPlayer); // пытающийся брать из колоды добавляется в список
                if (attackPlayer.PlayerKoloda.Count() < 6 && desk.Cards.Any())
                {
                    Color.Green($"Игрок {attackPlayer.Name} берет карты из колоды.");
                    Thread.Sleep(1000);
                }
                if (attackPlayer.PlayerKoloda.Count() > 5)
                {
                    Color.Green($"Игроку {attackPlayer.Name} не нужно брать карты из колоды.");
                    Thread.Sleep(1000);
                }


                //игрок берет из колоды карты
                while (attackPlayer.PlayerKoloda.Count() < 6 && !isDeskEmpty)
                {
                    if (!isDeskEmpty) // если колода не пуста, игрок берет карты пока в руке не окажется 6 карт
                    {
                        Desk.Cards.TryPop(out newCard);
                        attackPlayer.PlayerKoloda.Add(newCard); //добавление карты в руку

                        //если в колоде кончились карты
                        if (desk.Cards.Count() == 0)
                        {
                            Color.Cyan($"Игрок забирает последнюю карту из колоды - козырь {newCard.GetNominal}, {newCard.GetMast}.");
                            isDeskEmpty = true;
                            isCanTakeFromDesk = false;
                            break;
                        }
                    }
                }


                //Проверка. выходы из цикла донабора, если колода пуста или брали все игроки
                if (CannotDonabor()) break;


                //Смена игрока для донабора карт
                ChangeDonaborPlayer ();

            } while (isCanTakeFromDesk);

            //Проверка. выходы из цикла донабора, если колода пуста или брали все игроки
            bool CannotDonabor()
            {
                //выход из цикла, если колода пуста. В начале, после набора карт и перед сменой игрока
                if (isDeskEmpty)
                {
                    Color.Red($"В колоде нет карт.");
                    Thread.Sleep(2000);
                    return true;
                }


                //последним берет защищающийся, как только взял - выход из фазы донабора карт
                if (attackPlayer == defendPlayer)
                {
                    isCanTakeFromDesk = false;
                    return true;
                }

                return false;
            }

            //Смена игрока для донабора карт
            void ChangeDonaborPlayer()
            {
                int currentIndexOfAttackPlayer = players.IndexOf(attackPlayer);
                foreach (var player in players)
                {
                    //перебор из всех оставшихся игроков, кроме защищавшегося и тех, что уже брали
                    if (player != defendPlayer && !donaboredPlayers.Contains(player))
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

                        /// Color.Red($"Смена берущего из колоды игрока. Берет {attackPlayer.Name}.");
                        break;
                    }
                }
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

        //показать карты в игре
        public void ShowCardsInGame(List<Card> cardsInGame, Card kozyr)
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
    }
}
