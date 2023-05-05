using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDurak
{
    internal abstract class Game
    {
        internal virtual string Name { get; }

        protected virtual List<Player> Players { get; }

    }
}
