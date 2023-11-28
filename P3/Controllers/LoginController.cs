using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using P3.Models;
using P3.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Configuration;

using Microsoft.IdentityModel.Tokens;

using P3.Models;

using P3.Repositories;

using System;

using System.IdentityModel.Tokens.Jwt;

using System.Linq;

using System.Security.Claims;

using System.Text;
 
namespace P3.Controllers

{

    [Route("api/[controller]")]

    [ApiController]

    public class LoginController : ControllerBase

    {

        private readonly MyDbContext context;

        private readonly IConfiguration configuration;

        private readonly IUsersRepo userRepo;

        public LoginController(MyDbContext _context, IUsersRepo _usersRepo, IConfiguration _configuration)

        {

            this.userRepo = _usersRepo;

            context = _context;

            configuration = _configuration;

        }

        [AllowAnonymous]

        [HttpPost]

        public IActionResult Login(UserLogin user)

        {

            var a = context.UsersTable.FirstOrDefault(x => x.Email == user.Email && x.Password == user.Password);

            if (a != null) { return Ok(Authenticate(a)); }

            return NotFound();

        }

        //private string Generate(UserDetails user)

        //{

        //    var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

        //    var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

        //    var claim = new[]

        //    {

        //        new Claim(ClaimTypes.NameIdentifier,user.Name),

        //        new Claim(ClaimTypes.Email,user.Email),

        //        new Claim(ClaimTypes.Role,user.Role),

        //        new Claim(ClaimTypes.MobilePhone,user.Phone)

        //    };

        //    var token = new JwtSecurityToken(configuration["Jwt:Issuer"],

        //        configuration["Jwt : Audience"],

        //        claim,

        //        expires: DateTime.Now.AddMinutes(30),

        //        signingCredentials: credentials);

        //    return new JwtSecurityTokenHandler().WriteToken(token);

        //}

        private string Authenticate(UserDetails user)

        {

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(configuration["JWT:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor

            {

                Subject = new ClaimsIdentity(new Claim[]

                {

                 new Claim(ClaimTypes.Name, user.Email),

            // new Claim(ClaimTypes.Role, "admin")

                }),

                Expires = DateTime.UtcNow.AddHours(3),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }


    }

}
