namespace ConsoleDurak
{
    internal abstract class Game
    {
        internal virtual string Name { get; }

        protected virtual List<Player> Players { get; }
    }
}
