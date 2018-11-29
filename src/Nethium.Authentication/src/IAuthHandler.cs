using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Nethium.Authentication
{
    public interface IAuthHandler
    {
        string CreateToken(IEnumerable<Claim> claims, DateTime? notBefore = null, DateTime? expires = null);
        string ProxyToken(string token, string to);
        string ProxyToken(JwtSecurityToken token, string to);
        ClaimsPrincipal ValidateToken(string token, out SecurityToken jwt);
        ClaimsPrincipal ValidateToken(string token);
    }

    public class StubAuthHandler : IAuthHandler
    {
        public string CreateToken(IEnumerable<Claim> claims, DateTime? notBefore = null, DateTime? expires = null) =>
            throw new NotImplementedException();

        public string ProxyToken(string token, string to) => token;

        public string ProxyToken(JwtSecurityToken token, string to) => token.ToString();

        public ClaimsPrincipal ValidateToken(string token, out SecurityToken jwt) =>
            throw new NotImplementedException();

        public ClaimsPrincipal ValidateToken(string token) => throw new NotImplementedException();
    }
}