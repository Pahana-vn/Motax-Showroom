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
            if (result?.Principal == null || result.Principal.Identities == null || !result.Principal.Identities.Any())
            {
                // Handle the case where authentication failed or no identities were found
                TempData["error"] = "Authentication failed.";
                return RedirectToAction("Login", "Auth");
            }

            var identity = result.Principal.Identities.FirstOrDefault();
            if (identity == null)
            {
                TempData["error"] = "No identity found.";
                return RedirectToAction("Login", "Auth");
            }

            var claims = identity.Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            }).ToList();

            var email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var externalId = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await db.Accounts.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                user = new Account
                {
                    Username = name ?? string.Empty,
                    Email = email ?? string.Empty,
                    ExternalId = externalId, // Store as string
                    RoleId = 2, // Default role
                };
                db.Accounts.Add(user);
                await db.SaveChangesAsync();
            }

            var userClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        new Claim(ClaimTypes.Role, user.Role?.Title ?? "user")
    };

            var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }


        [Route("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync("Google");
            if (result?.Principal == null || result.Principal.Identities == null || !result.Principal.Identities.Any())
            {
                // Handle the case where authentication failed or no identities were found
                TempData["error"] = "Authentication failed.";
                return RedirectToAction("Login", "Auth");
            }

            var identity = result.Principal.Identities.FirstOrDefault();
            if (identity == null)
            {
                TempData["error"] = "No identity found.";
                return RedirectToAction("Login", "Auth");
            }

            var claims = identity.Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            }).ToList();

            var email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var externalId = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            var user = await db.Accounts.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                user = new Account
                {
                    Username = name ?? string.Empty,
                    Email = email ?? string.Empty,
                    ExternalId = externalId, // Store as string
                    RoleId = 2, // Default role
                };
                db.Accounts.Add(user);
                await db.SaveChangesAsync();
            }

            var userClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        new Claim(ClaimTypes.Role, user.Role?.Title ?? "user")
    };

            var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

    }
}