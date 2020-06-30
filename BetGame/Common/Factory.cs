using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetGame.Common
{
    public static class Factory
    {
        public static Punter GetAPunter(int num)
        {
            if( num == 1)
            {
                return new Joe() { Name = "Joe", Cash = 50 };
            }
            else if( num == 2 )
            {
                return new Bob() { Name = "Bob", Cash = 50 };
            }
            else
            {
                return new AI() { Name = "AI", Cash = 50 };
            }
        }
    }
}
