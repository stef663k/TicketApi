using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketApi.Models;

namespace TicketApi.DTO;

public class LoginDTO
{
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}