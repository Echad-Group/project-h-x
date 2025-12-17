using Microsoft.AspNetCore.Identity;

namespace NewKenyaAPI.Models
{
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Volunteer = "Volunteer";
        public const string User = "User";
        public const string Moderator = "Moderator";
        public const string TeamLead = "TeamLead";
    }
}
