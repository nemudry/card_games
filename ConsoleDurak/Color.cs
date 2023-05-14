namespace ConsoleDurak
{
    internal static class Color
    {
        //Методы для окрашивания шрифта. С пустой сторокой и без
        internal static void Red(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(text);
            Console.ResetColor();
            Console.WriteLine();
        }

        internal static void RedShort(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(text);
            Console.ResetColor();
        }

        internal static void Green(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(text);
            Console.ResetColor();
            Console.WriteLine();
        }

        internal static void GreenShort(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(text);
            Console.ResetColor();
        }

        internal static void Cyan(string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(text);
            Console.ResetColor();
            Console.WriteLine();
        }

        internal static void CyanShort(string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(text);
            Console.ResetColor();
        }
    }
}