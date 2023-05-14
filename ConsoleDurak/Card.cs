namespace ConsoleDurak
{
    internal class Card
    {
        internal Mast GetMast { get; }
        internal Nominal GetNominal { get; }

        internal Card(Nominal nominal, Mast mast)
        {
            GetMast = mast;
            GetNominal = nominal;
        }

        //доступные номиналы карт в игре
        internal enum Nominal
        {
            Два = 1,
            Три,
            Четыре,
            Пять,
            Шесть,
            Семь,
            Восемь,
            Девять,
            Десять,
            Валет,
            Дама,
            Король,
            Туз
        }

        //доступные масти карт в игре
        internal enum Mast
        {
            буби = 1,
            черви,
            вини,
            крести
        }

        //cтатическое свойство для cортировки карт по номиналу в руке живого игрока
        internal static IComparer<Card> SortByNominal
        {
            get { return new CardComparer(); }
        }
    }

    //сортировщик для cортировки карт по номиналу в руке живого игрока
    internal class CardComparer : IComparer<Card>
    {
        int IComparer<Card>.Compare(Card one, Card two)
        {
            return one.GetNominal.CompareTo(two.GetNominal);
        }
    }
}