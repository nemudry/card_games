
namespace ConsoleDurak
{
    public class Table
    {
        private readonly IReadOnlyList<Game> games;

        public Table()
        {
            games = new List<Game>
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
                int number = 0;
                Color.Cyan("Выберите игру :");
                foreach (Game game in games)
                {
                    Console.WriteLine($"[{++number}] {game.Name}.");
                }

                answerGame = PlayerAnswer();

                //проверка условий
                if (Table.CheckСonditions(answerGame, games.Count(), 1)) break;

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

                answerDifficultyBot = PlayerAnswer();

                //проверка условий
                if (Table.CheckСonditions(answerDifficultyBot, 2, 1)) break;

            } while (true);
            Console.Clear();
        }


        //Получение ответа игрока, используется внутри сборки
        internal static int PlayerAnswer()
        {
            Color.CyanShort("Ваш ответ: ");
            int.TryParse(Console.ReadLine(), out int answer);
            Console.WriteLine();
            return answer;
        }

        //проверка условий на ввод данных игроком
        internal static bool CheckСonditions(int answerInput, int MaxRange, int MinRange, params int[] exeptions)
        {
            if (!exeptions.Contains(answerInput))
            {
                if (!(answerInput >= MinRange && answerInput <= MaxRange))
                {
                    Color.Red("Введенное значение неверно.");
                    Console.WriteLine();
                    return false;
                }
            }
            return true;
        }
    }
}
