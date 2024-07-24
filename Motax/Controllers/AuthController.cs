using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Motax.Models;
using Microsoft.EntityFrameworkCore;

namespace Motax.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly MotaxContext db;

        public AuthController(MotaxContext context)
        {
            db = context;
        }

        [Route("signin-facebook")]
        public IActionResult SignInFacebook()
        {
            var redirectUri = Url.Action("FacebookResponse", "Auth");
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, "Facebook");
        }

        [Route("signin-google")]
        public IActionResult SignInGoogle()
        {
            var redirectUri = Url.Action("GoogleResponse", "Auth");
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, "Google");
        }

        [Route("facebook-response")]
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync("Facebook");
            var claims = result.Principal.Identities
                .FirstOrDefault().Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                });

            var email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var externalId = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await db.Accounts.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                user = new Account
                {
                    Username = name,
                    Email = email,
                    ExternalId = externalId,
                    RoleId = 2,// Default role
                };
                db.Accounts.Add(user);
                await db.SaveChangesAsync();
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        [Route("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync("Google");
            var claims = result.Principal.Identities
                .FirstOrDefault().Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                });

            var email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var externalId = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await db.Accounts.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                user = new Account
                {
                    Username = name,
                    Email = email,
                    ExternalId = externalId,
                    RoleId = 2, // Default role
                };
                db.Accounts.Add(user);
                await db.SaveChangesAsync();
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }
    }
}
