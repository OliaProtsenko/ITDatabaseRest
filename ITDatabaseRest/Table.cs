using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITDatabaseRest
{
    public class Table
    {
        public string name;
        public List<Column> columns = new List<Column>();
        public List<Row> rows = new List<Row>();
        public Table(string name)
        {
            this.name = name;
        }
    }
}
