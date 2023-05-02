﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDurak
{
    public class Table
    {
        public List<Game> Games { get; set; }

        public Table()
        {
            Games = new List<Game>
            {
                new Durak ()
            };
        }



        public void Start()
        {
            Color.Cyan("Добро пожаловать за стол.");
            Console.WriteLine();

            int answerGame;
            do
            {
                Color.Cyan("Выберите игру :");
                foreach (Game game in Games)
                {
                    Console.WriteLine($"[{Games.IndexOf(game)+1}] {game.Name}.");
                }

                answerGame = PlayerAnswer();

                //проверка условий
                if (Table.CheckСonditions(answerGame, Games.Count(), 1)) break;


            } while (true);
            Console.WriteLine();


            //Ввод имени игрока, количества игроков, сложности бота
            InputGameParameters(out string playerName, out int amountOfPlayers, out int botLevel);


            switch (answerGame)
            {
                case 1:
                    Color.Cyan("Добро пожаловать в карточную игру \"Дурак\".");
                    Console.WriteLine();
                    Durak durak = new Durak(playerName, amountOfPlayers, botLevel);
                    durak.StartGame();
                    break;
                default: break;
            }        
        }






        //Ввод имени и количества игроков
        private void InputGameParameters(out string answerName, out int answerPlayerCount, out int answerDifficultyBot)
        {


            do
            {
                Color.Cyan("Введите ваше имя:");
                answerName = Console.ReadLine();

                //проверка условий
                if (Table.CheckСonditions(answerName!.Length, 30, 3)) break;

            } while (true);
            Console.WriteLine();


            do
            {
                Color.Cyan("Допустимое количество игроков: 2-4.");
                Color.Cyan("Введите желаемое количество игроков:");

                answerPlayerCount = PlayerAnswer();

                //проверка условий
                if (Table.CheckСonditions(answerPlayerCount, 4, 2)) break;


            } while (true);
            Console.WriteLine();



            do
            {
                Color.Cyan("Введите уровень сложности: 1-2.");
                Color.Red("В данный момент реализован только [1] уровень сложности.");

                answerDifficultyBot = PlayerAnswer();

                //проверка условий
                if (Table.CheckСonditions(answerDifficultyBot, 2, 1)) break;

            } while (true);
            Console.Clear();
        }



        //Получение ответа игрока
        public static int PlayerAnswer()
        {
            Color.CyanShort("Ваш ответ: ");
            int.TryParse(Console.ReadLine(), out int answer);
            Console.WriteLine();
            return answer;
        }

        //проверка условий на ввод данных игроком
        public static bool CheckСonditions(int answerInput, int MaxRange, int MinRange)
        {
            if (answerInput != -1)
            {
                if (answerInput > MaxRange && answerInput < MinRange)
                {
                    Color.Red("Введенное значение неверно.");
                    return false;
                }
            }
            return true;
        }
    }
}
