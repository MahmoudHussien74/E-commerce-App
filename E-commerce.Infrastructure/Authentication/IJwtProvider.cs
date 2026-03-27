using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Infrastructure.Authentication;

public interface IJwtProvider
{
    (string token,int expireMinute) GenerateToken(User user);
}
