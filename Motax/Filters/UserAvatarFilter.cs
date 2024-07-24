using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Motax.Models;
using System.Security.Claims;

namespace Motax.Filters
{
    public class UserAvatarFilter : IActionFilter
    {
        private readonly MotaxContext db;

        public UserAvatarFilter(MotaxContext context)
        {
            db = context;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as Controller;
            if (controller != null && controller.User.Identity.IsAuthenticated)
            {
                var userId = controller.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userId, out int intUserId))
                {
                    var user = db.Accounts.Find(intUserId);
                    if (user != null)
                    {
                        controller.ViewData["UserAvatar"] = user.Image;
                    }
                }
                else
                {
                    var user = db.Accounts.FirstOrDefault(u => u.ExternalId == userId);
                    if (user != null)
                    {
                        controller.ViewData["UserAvatar"] = user.Image;
                    }
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}