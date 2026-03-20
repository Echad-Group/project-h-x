using Microsoft.AspNetCore.Identity;

namespace NewKenyaAPI.Models
{
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string SuperAdmin = "SuperAdmin";
        public const string RegionalLeader = "RegionalLeader";
        public const string CountyLeader = "CountyLeader";
        public const string SubCountyLeader = "SubCountyLeader";
        public const string ConstituencyLeader = "ConstituencyLeader";
        public const string WardLeader = "WardLeader";
        public const string PollingStationAgent = "PollingStationAgent";
        public const string Volunteer = "Volunteer";
        public const string User = "User";
        public const string Moderator = "Moderator";
        public const string TeamLead = "TeamLead";
        public const string PartyLiaison = "PartyLiaison";
        public const string TribalRegionalMobilizer = "TribalRegionalMobilizer";
        public const string ReligiousAdvisoryLead = "ReligiousAdvisoryLead";
        public const string VillageElderCoordinator = "VillageElderCoordinator";

        public const string LeadershipAccess = Admin + "," + SuperAdmin + "," + RegionalLeader + "," + CountyLeader + "," + SubCountyLeader + "," + ConstituencyLeader + "," + WardLeader;

        public static readonly string[] AllRoles =
        {
            Admin,
            SuperAdmin,
            RegionalLeader,
            CountyLeader,
            SubCountyLeader,
            ConstituencyLeader,
            WardLeader,
            PollingStationAgent,
            Volunteer,
            User,
            Moderator,
            TeamLead,
            PartyLiaison,
            TribalRegionalMobilizer,
            ReligiousAdvisoryLead,
            VillageElderCoordinator
        };
    }
}
