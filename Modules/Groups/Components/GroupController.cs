using System.Collections.Generic;
using WebMatrix.Data;

namespace HCC.Groups
{

    public class GroupController
	{
        public IEnumerable<dynamic> GetGroups(int PortalId)
        {
            IEnumerable<dynamic> objGroups;
            using (var db = Database.Open("SiteSqlServer"))
            {
                objGroups = db.Query("exec hccm_GetGroups @0", PortalId);
            }
            return objGroups;
        }

        public dynamic GetGroupByUserId(int PortalId, int UserId)
		{
            dynamic objGroup;
            using (var db = Database.Open("SiteSqlServer"))
            {
                objGroup = db.QuerySingle("exec hccm_GetGroupByUserId @0, @1", PortalId, UserId);
            }
            return objGroup;
		}

		public int UpdateGroup(int PortalId, string GroupName, string URL, string City, string Region, string Latitude, string Longitude, bool IsActive, int UserId)
		{
            int intGroupId;
            using (var db = Database.Open("SiteSqlServer"))
            {
                intGroupId = db.QuerySingle("exec hccm_UpdateGroup @0, @1, @2, @3, @4, @5, @6, @7, @8", PortalId, GroupName, URL, City, Region, Latitude, Longitude, IsActive, UserId).GroupId;
            }
			return intGroupId;
		}
	}
}