using ConsoleDurak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDurak
{
    public class Card
    {
        public Mast GetMast { get; }
        public Nominal GetNominal { get; }

        public Card(Nominal nominal, Mast mast)
        {
            GetMast = mast;
            GetNominal = nominal;
        }



        //доступные номиналы карт в игре
        public enum Nominal
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
        public enum Mast
        {
            буби = 1,
            черви,
            вини,
            крести
        }


        //cтатическое свойство для cортировки карт по номиналу в руке живого игрока
        public static IComparer<Card> SortByNominal
        {
            get { return new CardComparer(); }
        }
    }
}


//сортировщик для cортировки карт по номиналу в руке живого игрока
public class CardComparer : IComparer<Card>
{
    int IComparer<Card>.Compare(Card one, Card two)
    {
        return one.GetNominal.CompareTo(two.GetNominal);
    }
}