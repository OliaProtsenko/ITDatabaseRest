using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ITDatabaseRest
{
    public abstract class Column
    {
        public string name { get; private set; }
        public abstract string type { get; }
        public Column(string name)
        {
            this.name = name;
        }
        public abstract bool Validate(string value);
    }

    public class IntColumn : Column
    {
        public override string type { get; } = "INT";
        public IntColumn(string name) : base(name) { }

        public override bool Validate(string value) => int.TryParse(value, out _);
    }

    public class RealColumn : Column
    {
        public override string type { get; } = "REAL";
        public RealColumn(string name) : base(name) { }

        public override bool Validate(string value) => double.TryParse(value, out _);
    }

    public class CharColumn : Column
    {
        public override string type { get; } = "CHAR";
        public CharColumn(string name) : base(name) { }

        public override bool Validate(string value) => char.TryParse(value, out _);
    }

    public class StringColumn : Column
    {
        public override string type { get; } = "STRING";
        public StringColumn(string name) : base(name) { }

        public override bool Validate(string value) => true;
    }

    public class ColorColumn : Column
    {
        public override string type { get; } = "COLOR";
        public ColorColumn(string name) : base(name) { }

        public override bool Validate(string value) => Regex.IsMatch(value, "^#([a-fA-F0-9]{6})");
    }

    public class ColorIntervalColumn : Column
    {
        public override string type { get; } = "COLOR INTERVAL";
        public ColorIntervalColumn(string name) : base(name) { }

        public override bool Validate(string value)
        {
            string[] buf = value.Replace(" ", "").Split('-');
            return buf.Length == 2 && Regex.IsMatch(buf[0], "^#([a-fA-F0-9]{6})") && Regex.IsMatch(buf[1], "^#([a-fA-F0-9]{6})");
        }
    }

}
