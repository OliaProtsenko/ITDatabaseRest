using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITDatabaseRest
{
    public class Row
    {

        public List<String> values = new List<String>();

        public string this[int i]
        {
            get => values[i];
            set => values[i] = value;
        }
    }
}
