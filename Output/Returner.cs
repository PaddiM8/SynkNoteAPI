using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynkNote.Output
{
    public static class Returner
    {
        public static string Custom(string name, string value)
        {
            return "{\n\t" + $"'{name}': '{value}'" + "\n}";
        }

        public static string Success()
        {
            return "{\n'result': 'success'\n}";
        }
    }
}
