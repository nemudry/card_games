using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDurak
{
    public class AlivePlayer : Player
    {
        public AlivePlayer(string name)
        {
            Name = name;
            PlayerStatus = Status.Neitral;

            PlayerKoloda = new List<Card>();

        }




        public override Card Attack(Card kozyr, List<Card> cardsInGame)
        {


            Card attack = null;//карта для атаки
            int answerAttack; // номер карты для атаки
            PlayerKoloda.Sort(Card.SortByNominal); // сортировка карт по номиналу


            // если в игре нет карт - первая атака
            if (cardsInGame.Count == 0)
            {
                do
                {
                    //показать руку
                    HandInfo(kozyr);

                    //ответ игрока
                    answerAttack = Table.PlayerAnswer();

                    //проверка условий
                    if (Table.CheckСonditions(answerAttack, PlayerKoloda.Count, 1)) break;

                } while (true);

                //получение нужной карты и удаление ее из руки
                return GetCard(answerAttack);
            }


            //если в игре уже есть карты - обычная атака
            else
            {
                // номиналы карт в игре
                var nominals = from card in cardsInGame
                               let nominal = card.GetNominal
                               select nominal;


                // подходит ли номинал выбранной карты для атаки
                bool isHaveNominal = false;

                //ввод номера карты для атаки
                do
                {
                    //показать руку
                    HandInfo(kozyr);
                    Console.WriteLine($"-1. Не атаковать.");
                    Console.WriteLine();

                    //ответ игрока
                    answerAttack = Table.PlayerAnswer();

                    // -1. Не атаковать
                    if (answerAttack == -1)
                    {
                        return attack;
                    }

                    if (!Table.CheckСonditions(answerAttack, PlayerKoloda.Count, 1)) break;

                    // соответствие выбранного номинала картам в игре
                    else
                    {
                        // да, если в картах в игре есть номинал выбранной карты
                        isHaveNominal = nominals.Contains(PlayerKoloda[answerAttack - 1].GetNominal);

                        if (!isHaveNominal)
                        {
                            Color.Red("Введенное значение неверно. Данную карту нельзя ввести в игру.");
                            Console.WriteLine();
                            Thread.Sleep(1000);
                        }

                        else break;
                    }

                } while (true);

            }

            //получение карты и ее удаление из руки
            return GetCard(answerAttack);
        }



        public override Card Defend(Card kozyr, List<Card> cardsInGame)
        {
            //карта для атаки
            Card defend = null;
            int answerDefend = 0;
            PlayerKoloda.Sort(Card.SortByNominal); // сортировка карт по номиналу


            do
            {
                //показать руку
                HandInfo(kozyr);
                Console.WriteLine($"[-1]. Не защищаться.");
                Console.WriteLine();


                //ответ игрока
                answerDefend = Table.PlayerAnswer();

                // Не защищаться
                if (answerDefend == -1)
                {
                    return defend;
                }

                // ввод неверного числа 
                if (!Table.CheckСonditions(answerDefend, PlayerKoloda.Count, 1)) break;


                //условия защиты согласно правилам игры
                else
                {
                    if (DefendCheck()) break;
                }

            } while (true);

            //получение карты и ее удаление из руки
            return GetCard(answerDefend); 


            //условия защиты согласно правилам игры
            bool DefendCheck()
            {
                //Условия для защиты
                bool kozOnKoz = false;
                bool bigKoz = false;
                bool bigCard = false;
                bool noMast = false;

                // если атакуют козырем
                if (cardsInGame.Last().GetMast == kozyr.GetMast)
                {
                    // козырь нужно бить козырем
                    kozOnKoz = PlayerKoloda[answerDefend - 1].GetMast == kozyr.GetMast;
                    if (!kozOnKoz)
                    {
                        Color.Red("Козырь нужно бить козырем.");
                        return NoCheckDefend ();
                    }

                    // козырь нужно бить старшим козырем
                    bigKoz = PlayerKoloda[answerDefend - 1].GetMast == kozyr.GetMast
                        && PlayerKoloda[answerDefend - 1].GetNominal > cardsInGame.Last().GetNominal;
                    if (!bigKoz)
                    {
                        Color.Red("Нужно бить старшим козырем.");
                        return NoCheckDefend(); ;
                    }
                }



                // если атакуют не козырем
                else
                {
                    // и если отбиваешься некозырем
                    if (PlayerKoloda[answerDefend - 1].GetMast != kozyr.GetMast)
                    {
                        // масть должна соответствовать
                        noMast = PlayerKoloda[answerDefend - 1].GetMast == cardsInGame.Last().GetMast;
                        if (!noMast)
                        {
                            Color.Red("Нужно бить той же мастью.");
                            return NoCheckDefend();
                        }

                        // масть должна соответствовать и номинал должен быть выше 
                        bigCard = PlayerKoloda[answerDefend - 1].GetMast == cardsInGame.Last().GetMast
                            && PlayerKoloda[answerDefend - 1].GetNominal > cardsInGame.Last().GetNominal;
                        if (!bigCard)
                        {
                            Color.Red("Нужно бить старшей картой.");
                            return NoCheckDefend();
                        }
                    }

                    // если отбиваешься козырем
                    if (PlayerKoloda[answerDefend - 1].GetMast == kozyr.GetMast) return true;
                }


                return true;
            }

            bool NoCheckDefend()
            {
                Color.Red("Введенное значение неверно.");
                Console.WriteLine();
                Thread.Sleep(1000);
                return false;
            }
        }




        //показать карты в руке игрока
        private void HandInfo(Card kozyr)
        {
            Color.Cyan("Ваш ход. Выберите карту:");
            Thread.Sleep(1000);

            Color.Cyan($"Карты в руке игрока {Name}.");
            foreach (var card in PlayerKoloda)
            {
                Console.Write($"Карта №{PlayerKoloda.IndexOf(card) + 1} - {card.GetNominal}, {card.GetMast}.");
                if (kozyr.GetMast == card.GetMast) Color.Green(" Козырь.");
                else Console.WriteLine();
            }

        }




        //получение карты и ее удаление из руки
        private Card GetCard (int c)
        {
            //получение карты и ее удаление из руки
            Card card = PlayerKoloda[c - 1];
            PlayerKoloda.Remove(card);
            return card;
        }

    }
}
