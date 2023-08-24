using System;
using TeamControlV2.DTO.HelperModels.Jwt;

namespace TeamControlV2.Services.Interface
{
    public interface IJwtHandler
    {
        JwtResponse CreateToken(JwtCustomClaims claims);
    }
}
