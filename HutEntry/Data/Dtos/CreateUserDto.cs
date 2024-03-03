using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HutEntry.Data.Dtos
{
    public class CreateUserDto
    {
        required public string Username { get; set; }

        [DataType(DataType.Password)]
        required public string Password { get; set; }
        [DataType(DataType.Password)]
        required public string ConfirmPassword { get; set; }

        [DataType(DataType.EmailAddress)]
        required public string Email { get; set; }
        required public string FirstName { get; set; }
        required public string LastName { get; set; }
    }
}
