using System.Collections;
using System.Collections.Generic;
using Octokit;
using WebMatrix.Data;

namespace HCC.CommunityMetrics
{
    public class GitHubWatchersActivity : IActivity
	{
		public MetricTypeEnum MetricType
        {
			get { return MetricTypeEnum.Once; }
		}

		public Hashtable GetSettings()
		{
			Hashtable objSettings = new Hashtable();
			objSettings.Add("Connection", "Your Database Connection Name ( ie. SiteSqlServer )");
			objSettings.Add("Credentials", "A Personal Acccess Token You Create In Your GitHub Account");
			objSettings.Add("Organization", "Your GitHub Organization Name");
			objSettings.Add("Profile", "The Name Of The User Profile Field You Created For GitHub Accounts In Your Site");
            objSettings.Add("Repositories", "Comma Delimited List Of Repositories You Would Like To Include (Optional)");
            return objSettings;
		}

		public List<UserActivityInfo> GetUserActivity(ActivityInfo objActivity, System.DateTime ExecutionDate)
		{
			dynamic db = Database.Open(objActivity.Settings["Connection"].ToString());
			string strSQL = "";

			Dictionary<int, int> arrUsers = new Dictionary<int, int>();
			GitHubClient objGitHub = new GitHubClient(new ProductHeaderValue("HCC.CommunityActivity"));
			objGitHub.Credentials = new Credentials(objActivity.Settings["Credentials"].ToString());

            IReadOnlyList<Octokit.Repository> objRepositories = objGitHub.Repository.GetAllForOrg(objActivity.Settings["Organization"].ToString()).Result;
			foreach (Octokit.Repository objRepository in objRepositories)
            {
                if (objActivity.Settings["Repositories"].ToString() == "" || ("," + objActivity.Settings["Repositories"].ToString() + ",").ToLower().IndexOf("," + objRepository.Name.ToLower() + ",") != -1)
                {
                    IReadOnlyList<Octokit.User> objUsers = objGitHub.Activity.Watching.GetAllWatchers(objRepository.Id).Result;
                    foreach (Octokit.User objUser in objUsers)
                    {
                        strSQL = "select UserID ";
                        strSQL += "from UserProfile up ";
                        strSQL += "inner join ProfilePropertyDefinition ppd on up.PropertyDefinitionID = ppd.PropertyDefinitionID ";
                        strSQL += "where ppd.PropertyName = @0 ";
                        strSQL += "and up.PropertyValue like '%' + @1 + '%'";
                        var obj = db.QuerySingle(strSQL, objActivity.Settings["Profile"].ToString(), objUser.Login);
                        if ((obj != null))
                        {
                            arrUsers.Add(obj.UserId, 1);
                        }
                    }
                }
            }

			List<UserActivityInfo> objUserActivities = new List<UserActivityInfo>();
			foreach (KeyValuePair<int, int> kvp in arrUsers)
            {
				UserActivityInfo objUserActivity = new UserActivityInfo();
				objUserActivity.UserId = kvp.Key;
				objUserActivity.Count = kvp.Value;
				objUserActivities.Add(objUserActivity);
			}

			return objUserActivities;
		}
	}
}
