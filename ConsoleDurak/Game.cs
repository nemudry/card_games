using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDurak
{
    public abstract class Game
    {
        public string Name { get; set; }

        public List<Player> Players { get; set; }

    }
}
