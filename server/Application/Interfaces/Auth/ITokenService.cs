﻿using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Auth
{
    public interface ITokenService
    {
        Task<string> GenerateJwtTokenAsync(User user, IList<string> roles);
    }

}
