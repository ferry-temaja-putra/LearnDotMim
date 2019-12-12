using AuthSyncSharedData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthSyncServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private JwtSettings jwtSettings;

        public AuthController(IOptions<JwtSettings> jwtSettingsOption)
        {
            jwtSettings = jwtSettingsOption.Value;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] LoginInfo loginInfo)
        {
            if (loginInfo == null || loginInfo.InputValid == false)
            {
                return Unauthorized();
            }

            if (loginInfo.UserName != "admin" || loginInfo.Password != "admin")
            {
                return Unauthorized();
            }

            var user = new User { UserName = loginInfo.UserName };
          
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            user.Token = tokenHandler.WriteToken(token);

            return Ok(user);
        }        
    }
}