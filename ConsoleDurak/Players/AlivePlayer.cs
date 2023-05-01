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
            : base()
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
                    Color.Cyan("Ваш ход для атаки. Выберите карту:");
                    Thread.Sleep(1000);
                    ShowCardsInHand(kozyr);

                    Color.CyanShort("Ваш ответ: ");
                    int.TryParse(Console.ReadLine(), out answerAttack);
                    Console.WriteLine();

                    if (answerAttack > PlayerKoloda.Count() || answerAttack < 1) // ввод неверного числа 
                    {
                        Color.Red("Введенное значение неверно. Выбранного номера нет в вашей колоде.");
                        Console.WriteLine();
                        Thread.Sleep(1000);
                    }

                } while (answerAttack > PlayerKoloda.Count() || answerAttack < 1);

                //получение карты и ее удаление из руки
                attack = PlayerKoloda[answerAttack - 1];
                PlayerKoloda.Remove(attack);
                return attack;
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
                    Color.Cyan("Ваш ход для атаки. Выберите карту:");
                    Thread.Sleep(1000);
                    ShowCardsInHand(kozyr);

                    Console.WriteLine($"-1. Не атаковать.");
                    Console.WriteLine();

                    Color.CyanShort("Ваш ответ: ");
                    int.TryParse(Console.ReadLine(), out answerAttack);
                    Console.WriteLine();

                    // Не атаковать
                    if (answerAttack == -1)
                    {
                        return attack;
                    }

                    // ввод неверного числа
                    if (answerAttack > PlayerKoloda.Count() || answerAttack < 1)
                    {
                        Color.Red("Введенное значение неверно. Выбранного номера нет в вашей колоде.");
                        Console.WriteLine();
                        Thread.Sleep(1000);
                    }


                    // соответствие выбранного номинала картам в игре
                    else
                    {
                        // можно ли кидать данную карту в атаку
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
            attack = PlayerKoloda[answerAttack - 1];
            PlayerKoloda.Remove(attack);
            return attack;
        }



        public override Card Defend(Card kozyr, List<Card> cardsInGame)
        {
            //карта для атаки
            Card defend = null;
            int answerDefend = 0;
            PlayerKoloda.Sort(Card.SortByNominal); // сортировка карт по номиналу


            //  ввод номера карты
            do
            {
                Color.Cyan("Ваш ход для защиты. Выберите карту:");
                Thread.Sleep(1000);
                ShowCardsInHand(kozyr); // показать карты в руке

                Console.WriteLine($"-1. Не защищаться.");
                Console.WriteLine();

                Color.CyanShort("Ваш ответ: ");
                int.TryParse(Console.ReadLine(), out answerDefend);
                Console.WriteLine();

                // Не защищаться
                if (answerDefend == -1)
                {
                    return defend;
                }

                // ввод неверного числа 
                if (answerDefend > PlayerKoloda.Count() || answerDefend < 1)
                {
                    Color.Red("Введенное значение неверно. Выбранного номера нет в вашей колоде.");
                    Console.WriteLine();
                    Thread.Sleep(1000);
                }


                //условия защиты согласно правилам игры
                else
                {
                    if (DefendCheck()) break;
                }

            } while (true);

            //получение карты и ее удаление из руки
            defend = PlayerKoloda[answerDefend - 1];
            PlayerKoloda.Remove(defend);
            return defend;


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
                        Color.Red("Введенное значение неверно. Козырь нужно бить козырем.");
                        Console.WriteLine();
                        Thread.Sleep(1000);
                        return false;
                    }

                    // козырь нужно бить старшим козырем
                    bigKoz = PlayerKoloda[answerDefend - 1].GetMast == kozyr.GetMast
                        && PlayerKoloda[answerDefend - 1].GetNominal > cardsInGame.Last().GetNominal;
                    if (!bigKoz)
                    {
                        Color.Red("Введенное значение неверно. Нужно бить старшим козырем.");
                        Console.WriteLine();
                        Thread.Sleep(1000);
                        return false;
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
                            Color.Red("Введенное значение неверно. Нужно бить той же мастью.");
                            Console.WriteLine();
                            Thread.Sleep(1000);
                            return false;
                        }

                        // масть должна соответствовать и номинал должен быть выше 
                        bigCard = PlayerKoloda[answerDefend - 1].GetMast == cardsInGame.Last().GetMast
                            && PlayerKoloda[answerDefend - 1].GetNominal > cardsInGame.Last().GetNominal;
                        if (!bigCard)
                        {
                            Color.Red("Введенное значение неверно. Нужно бить старший картой.");
                            Console.WriteLine();
                            Thread.Sleep(1000);
                            return false;
                        }
                    }

                    // если отбиваешься козырем
                    if (PlayerKoloda[answerDefend - 1].GetMast == kozyr.GetMast) return true;
                }


                return true;
            }

        }



        //показать карты в руке у игрока
        public void ShowCardsInHand(Card kozyr)
        {
            Color.Cyan($"Карты в руке игрока {Name}.");
            foreach (var card in PlayerKoloda)
            {
                Console.Write($"Карта №{PlayerKoloda.IndexOf(card) + 1} - {card.GetNominal}, {card.GetMast}.");
                if (kozyr.GetMast == card.GetMast) Color.Green(" Козырь.");
                else Console.WriteLine();
            }
        }

    }
}
