using System.Collections;
using System.Collections.Generic;
using WebMatrix.Data;

namespace HCC.CommunityMetrics
{
    public class DatabaseActivity : IActivity
	{
		public MetricTypeEnum MetricType {
			get { return MetricTypeEnum.Undefined; }
		}

		public Hashtable GetSettings()
		{
			Hashtable objSettings = new Hashtable();
			objSettings.Add("Connection", "Your Database Connection Name ( ie. SiteSqlServer )");
			objSettings.Add("Query", "select UserId, count(*) as Count from [Table] where where convert(date,[DateField]) = convert(date,@0) group by UserId");
			return objSettings;
		}

		public List<UserActivityInfo> GetUserActivity(ActivityInfo objActivity, System.DateTime ExecutionDate)
		{
			List<UserActivityInfo> objUserActivities = new List<UserActivityInfo>();
            using (var db = Database.Open(objActivity.Settings["Connection"].ToString()))
            {
                foreach (var obj in db.Query(objActivity.Settings["Query"].ToString(), ExecutionDate))
                {
                    UserActivityInfo objUserActivity = new UserActivityInfo();
                    objUserActivity.UserId = obj.UserId;
                    objUserActivity.Count = obj.Count;
                    objUserActivities.Add(objUserActivity);
                }
            }
			return objUserActivities;
		}
	}
}