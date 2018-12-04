using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynkNote
{
    public static class ErrorReturner
    {
        public static string Make(ReturnCode returnCode)
        {
            return "[Error] " + (int)returnCode;
        }
    } 
}
