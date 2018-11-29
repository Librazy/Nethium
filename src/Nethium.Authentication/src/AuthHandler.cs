using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Nethium.Abstraction;

namespace Nethium.Authentication
{
    public class AuthHandler : IAuthHandler
    {
        private readonly JwtHeader _jwtHeader;

        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        private readonly IServerIdentifier _server;

        private readonly TokenValidationParameters _validationParameters;

        public AuthHandler(JwtHeader jwtHeader, JwtSecurityTokenHandler jwtSecurityTokenHandler,
            IServerIdentifier serverId, TokenValidationParameters validationParameters)
        {
            _jwtHeader = jwtHeader;
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
            _server = serverId;
            _validationParameters = validationParameters;
        }

        public string CreateToken(IEnumerable<Claim> claims, DateTime? notBefore = null, DateTime? expires = null) =>
            _jwtSecurityTokenHandler.WriteToken(new JwtSecurityToken(_jwtHeader,
                new JwtPayload(_server.ServerId, "orig", claims, notBefore, expires)));

        public string ProxyToken(string token, string to)
        {
            var jwtSecurityToken = _jwtSecurityTokenHandler.ReadJwtToken(token);
            return ProxyToken(jwtSecurityToken, to);
        }

        public string ProxyToken(JwtSecurityToken token, string to)
        {
            var claims = token.Claims;
            var claimList = claims.ToList();
            var actor = (from c in claimList
                            where c.Type == "actor"
                            select c.Value).Single() ?? token.Audiences.Single();
            var currentProxy = new Claim(_server.ServerId, actor);
            var actorClaim = new Claim("act", _server.ServerId);
            return _jwtSecurityTokenHandler.WriteToken(new JwtSecurityToken(_jwtHeader,
                new JwtPayload(_server.ServerId, to, claimList.Append(currentProxy).Append(actorClaim), token.ValidFrom,
                    token.ValidTo)));
        }

        public ClaimsPrincipal ValidateToken(string token, out SecurityToken jwt) =>
            _jwtSecurityTokenHandler.ValidateToken(token, _validationParameters, out jwt);

        public ClaimsPrincipal ValidateToken(string token) => ValidateToken(token, out _);
    }
}