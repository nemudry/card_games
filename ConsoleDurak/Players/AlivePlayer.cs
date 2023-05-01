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

                    Color.CyanShort("Ваш ответ: ");
                    int.TryParse(Console.ReadLine(), out answerAttack);
                    Console.WriteLine();

                    //проверка условий
                    if (CheckСonditions(answerAttack, PlayerKoloda.Count)) break;

                } while (true);

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
                    //показать руку
                    HandInfo(kozyr);

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


                    if (!CheckСonditions(answerAttack, PlayerKoloda.Count))
                    {
                        Color.Red("Выбранного номера нет в вашей колоде.");
                        Console.WriteLine();
                        break;
                    }

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
                HandInfo(kozyr);

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
                if (!CheckСonditions(answerDefend, PlayerKoloda.Count))
                {
                    Color.Red("Выбранного номера нет в вашей колоде.");
                    Console.WriteLine();
                    break;
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



        private bool CheckСonditions(int answerInput, int range)
        {
            if (answerInput != -1)
            {
                if (answerInput > range && answerInput < 1)
                {
                    Color.Red("Введенное значение неверно.");
                    return false;
                }
            }
            return true;
        }

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

    }
}
