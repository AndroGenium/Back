using Backend.Enums;
using System.Security.Claims;

namespace Backend.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        }

        public static UserRole GetUserRole(this ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(roleClaim))
                return UserRole.Guest;

            if (Enum.TryParse<UserRole>(roleClaim, out var role))
                return role;

            return UserRole.Guest;
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.GetUserRole() == UserRole.Admin;
        }

        public static bool IsUser(this ClaimsPrincipal user)
        {
            return user.GetUserRole() == UserRole.User;
        }

        public static bool IsGuest(this ClaimsPrincipal user)
        {
            return user.GetUserRole() == UserRole.Guest;
        }
    }
}