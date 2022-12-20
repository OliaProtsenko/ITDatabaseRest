using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace ITDatabaseRest
{
    public class Manager
    {

        private const char separator = '%';
        private const char space = '\t';
        private const string _bannedChars = "\\/\"|:<> ";
        private Database _database;
        private static Manager _instance;

        public static Manager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Manager();
                }

                return _instance;
            }
        }

        private Manager() { }

        public Database Database { get => _database; set => _database = value; }

        public bool CreateDB(string name)
        {
            if (name.IndexOfAny(_bannedChars.ToCharArray()) != -1)
            {
                return false;
            }

            _database = new Database(name);
            return true;
        }

        public bool SaveDB(string path)
        {
            try
            {
                var streamWriter = new StreamWriter(path);

                streamWriter.WriteLine(_database.name);
                WriteTablesToFile(streamWriter);
                streamWriter.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ImportData(string path)
        {
            try
            {
                var streamReader = new StreamReader(path);
                string file = streamReader.ReadToEnd();
                string[] parts = file.Split(separator);
                _database = new Database(parts[0]);

                ReadTables(parts);
                streamReader.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ReadTables(string[] parts)
        {
            for (int i = 1; i < parts.Length; i++)
            {
                parts[i] = parts[i].Replace("\r\n", "\r");
                var buf = parts[i].Split('\r').ToList();

                buf.RemoveAt(0);
                buf.RemoveAt(buf.Count - 1);

                if (buf.Count > 0)
                {
                    _database.tables.Add(new Table(buf[0]));
                }

                if (buf.Count > 2)
                {
                    ReadColumnsFromFile(buf, i);
                    ReadRowsFromFile(buf, i);
                }
            }
        }

        private void ReadColumnsFromFile(List<string> buf, int tableIndex)
        {
            string[] columnNames = buf[1].Split(space);
            string[] columnTypes = buf[2].Split(space);

            for (int j = 0; j < columnNames.Length - 1; j++)
            {
                _database.tables[tableIndex - 1].columns.Add(ColumnFromString(columnNames[j], columnTypes[j]));
            }
        }

        private void ReadRowsFromFile(List<string> buf, int tableIndex)
        {
            for (int j = 3; j < buf.Count; j++)
            {
                _database.tables[tableIndex - 1].rows.Add(new Row());

                var values = buf[j].Split(space).ToList();
                values.RemoveAt(values.Count - 1);
                _database.tables[tableIndex - 1].rows.Last().values.AddRange(values);
            }
        }

        private void WriteTablesToFile(StreamWriter streamWriter)
        {
            foreach (var table in _database.tables)
            {
                streamWriter.WriteLine(separator);
                streamWriter.WriteLine(table.name);

                WriteColumnsToFile(streamWriter, table);
                WriteRowsToFile(streamWriter, table);
            }
        }

        private static void WriteColumnsToFile(StreamWriter streamWriter, Table table)
        {
            foreach (var column in table.columns)
            {
                streamWriter.Write(column.name + space);
            }

            streamWriter.WriteLine();

            foreach (var column in table.columns)
            {
                var t = column.type;
                streamWriter.Write(column.type + space);
            }

            streamWriter.WriteLine();
        }

        private static void WriteRowsToFile(StreamWriter streamWriter, Table table)
        {
            foreach (var row in table.rows)
            {
                foreach (string value in row.values)
                {
                    streamWriter.Write(value.ToString() + space);
                }

                streamWriter.WriteLine();
            }
        }

        public bool DeleteDB()
        {
            try
            {
                _database = null;
                return true;

            }
            catch
            {
                return false;
            }
        }


        public bool AddTable(string name)
        {
            if (GetTableNames().Contains(name))
            {
                return false;
            }

            _database.tables.Add(new Table(name));
            return true;
        }

        public Table GetTable(int index) => _database.tables[index];

        public List<string> GetTableNames() => _database.tables.Select(t => t.name).ToList();

        public List<string> GetColumnNames(int tableIndex) => _database.tables[tableIndex].columns.Select(c => c.name).ToList();

        public void DeleteTable(int index)
        {
            _database.tables.RemoveAt(index);
        }

        public bool AddColumn(int tableIndex, Column column)
        {
            if (GetColumnNames(tableIndex).Contains(column.name))
            {
                return false;
            }

            _database.tables[tableIndex].columns.Add(column);

            foreach (var row in _database.tables[tableIndex].rows)
            {
                row.values.Add("");
            }

            return true;
        }

        public void DeleteColumn(int tableIndex, int columnIndex)
        {
            var table = _database.tables[tableIndex];

            table.columns.RemoveAt(columnIndex);

            foreach (var row in table.rows)
            {
                row.values.RemoveAt(columnIndex);
            }

            if (table.columns.Count == 0)
            {
                table.rows.Clear();
            }
        }

        public bool AddRow(int tableIndex)
        {
            if (_database == null || _database.tables.Count == 0 || _database.tables[tableIndex].columns.Count == 0)
            {
                return false;
            }

            var row = new Row();

            foreach (var _ in _database.tables[tableIndex].columns)
            {
                row.values.Add("");
            }

            _database.tables[tableIndex].rows.Add(row);

            return true;
        }

        public void DeleteRow(int tableIndex, int rowIndex)
        {
            _database.tables[tableIndex].rows.RemoveAt(rowIndex);
        }


        public bool ChangeCellValue(string newValue, int tableIndex, int columnIndex, int rowIndex)
        {
            if (_database.tables[tableIndex].columns[columnIndex].Validate(newValue))
            {
                _database.tables[tableIndex].rows[rowIndex][columnIndex] = newValue;

                return true;
            }

            return false;
        }
        public Table Search(int tableIndex, int columnIndex, String pattern)
        {
            if (_database == null || _database.tables.Count == 0 || _database.tables[tableIndex].columns.Count == 0)
            {
                return null;
            }
            var resRows = new List<Row>();

            foreach (var row in _database.tables[tableIndex].rows)
            {
                if (Regex.IsMatch(row[columnIndex], pattern))
                {
                    resRows.Add(row);
                }
            }
            return new Table("")
            {
                columns = _database.tables[tableIndex].columns,
                rows = resRows
            };

        }



        public static Column ColumnFromString(string name, string type)
        {
            switch (type)
            {
                case "INT": return new IntColumn(name);
                case "REAL": return new RealColumn(name);
                case "CHAR": return new CharColumn(name);
                case "STRING": return new StringColumn(name);
                case "COLOR": return new ColorColumn(name);
                case "COLOR INVL": return new ColorIntervalColumn(name);
                default: return null;

            }

        }
    }
}

