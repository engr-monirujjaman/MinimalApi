using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Settings
{
    public class TokenBuilder
    {
        private string _issuer = null!;
        private string _audience = null!;
        private DateTime _expires;
        private SigningCredentials _credentials = null!;
        private SymmetricSecurityKey _key = null!;
        private List<Claim>? _claims;

        public TokenBuilder AddClaims(List<Claim> claims)
        {
            if (_claims is null)
                _claims = claims;
            else
                _claims.AddRange(claims);

            return this;
        }

        public TokenBuilder AddClaim(Claim claim)
        {
            if (_claims == null)
                _claims = new List<Claim>() { claim };
            else
                _claims.Add(claim);

            return this;
        }

        public TokenBuilder AddIssuer(string issuer)
        {
            _issuer = issuer;
            return this;
        }

        public TokenBuilder AddAudience(string audience)
        {
            _audience = audience;
            return this;
        }

        public TokenBuilder AddExpiry(TimeSpan timeSpan)
        {
            _expires = DateTime.UtcNow.Add(timeSpan);
            return this;
        }

        public TokenBuilder AddKey(string key)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            _credentials = new SigningCredentials(_key, SecurityAlgorithms.Sha256);
            return this;
        }

        public JwtSecurityToken Build()
        {
            return new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: _claims,
                expires: _expires,
                signingCredentials: _credentials
            );
        }
    }
}