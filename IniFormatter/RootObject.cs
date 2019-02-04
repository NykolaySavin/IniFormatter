using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IniFormatter
{
    public class Action
    {
        public int firstSymbolIndexInT { get; set; }
        public int secondSymbolIndexInT { get; set; }
        public int firstSymbolIndexInC { get; set; }
        public int secondSymbolIndexInC { get; set; }
    }

    public class RootObject
    {
        public string file { get; set; }
        public List<int> numberOfDigits { get; set; }
        public List<Action> actions { get; set; }
    }
}
