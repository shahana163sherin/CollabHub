using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollabHub.Application.Interfaces.Auth
{
    public interface IHashPassword
    {
        string HashPassword(string password);
        bool verifyPassword(string hashedPassword, string password);
    }
}
