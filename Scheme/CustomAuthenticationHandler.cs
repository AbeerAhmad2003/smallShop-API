﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using smallShop.Data;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace smallShop.Scheme
{
    public class CustomAuthenticationHandler: AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly AppDbContext context;
        public CustomAuthenticationHandler
            (IOptionsMonitor<AuthenticationSchemeOptions> options
            , ILoggerFactory logger,
            UrlEncoder encoder,
            AppDbContext context) : base(options, logger, encoder)
        {
            this.context = context;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var headerToken = Request.Headers.Authorization.FirstOrDefault();
            if (string.IsNullOrEmpty(headerToken))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }
            try
            {
                string token =Encoding.UTF8.GetString(Convert.FromBase64String(headerToken));
                int id=int.Parse(token.Split(".")[2].Trim());
                var getRole=await context.UserRoles.FirstOrDefaultAsync(r=>r.AppUserId == id);
                var getUser=await context.AppUsers.FirstOrDefaultAsync(u=>u.Id == id);
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier,id.ToString()),
                    new Claim(ClaimTypes.Name,getUser!.Name!),
                    new Claim(ClaimTypes.Role,getRole!.Role!)
                };
                var identity=new ClaimsIdentity(claims,Scheme.Name);
                var principal=new ClaimsPrincipal(identity);
                var ticket=new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);

            }
            catch
            {
                return AuthenticateResult.Fail("Invaild authrization Header");
            }

        }
    }
}
