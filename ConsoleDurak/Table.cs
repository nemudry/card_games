using System;
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

                int.TryParse(Console.ReadLine(), out answerGame);

                if (answerGame < 1 || answerGame > Games.Count())
                {
                    Color.Red("Введенное значение не подходит. Попробуйте иное имя.");
                    Console.WriteLine();
                }

            } while (answerGame < 1 || answerGame > Games.Count());
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
        public void InputGameParameters(out string answerName, out int answerPlayerCount, out int answerDifficultyBot)
        {


            do
            {
                Color.Cyan("Введите ваше имя:");
                answerName = Console.ReadLine();

                if (answerName!.Length < 3 || answerName == null)
                {
                    Color.Red("Введенное значение подходит. Попробуйте иное имя.");
                    Console.WriteLine();
                }

            } while (answerName!.Length < 3 || answerName == null);
            Console.WriteLine();


            do
            {
                Color.Cyan("Допустимое количество игроков: 2-4.");
                Color.Cyan("Введите желаемое количество игроков:");
                int.TryParse(Console.ReadLine(), out answerPlayerCount);

                if (answerPlayerCount < 2 || answerPlayerCount > 4)
                {
                    Color.Red("Введенное значение неверно.");
                    Console.WriteLine();
                }

            } while (answerPlayerCount < 2 || answerPlayerCount > 4);
            Console.WriteLine();



            do
            {
                Color.Cyan("Введите уровень сложности: 1-2.");
                Color.Red("В данный момент реализован только [1] уровень сложности.");
                int.TryParse(Console.ReadLine(), out answerDifficultyBot);

                if (answerDifficultyBot < 1 || answerDifficultyBot > 2)
                {
                    Color.Red("Введенное значение неверно.");
                    Console.WriteLine();
                }

            } while (answerDifficultyBot < 1 || answerDifficultyBot > 2);
            Console.WriteLine();
            Console.Clear();
        }


    }
}
