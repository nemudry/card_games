using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDurak
{
    internal abstract class Game
    {
        internal string Name { get; set; }

        protected List<Player> Players { get; set; }

    }
}
