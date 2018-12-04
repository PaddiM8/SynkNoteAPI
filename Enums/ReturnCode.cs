using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SynkNote
{
    public enum ReturnCode
    {
        Success = 0,
        EmailExists = 1,
        IncorrectPassword = 2,
        UserNotFound = 3,
        InvalidToken = 4,
        NoteNotFound = 5,
        PermissionDenied = 6,
        InvalidInput = 7,
        NotebookNotFound = 8
    }
}
