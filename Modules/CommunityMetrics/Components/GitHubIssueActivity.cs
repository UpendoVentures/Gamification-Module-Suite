using System.Collections;
using System.Collections.Generic;
using Octokit;
using WebMatrix.Data;

namespace HCC.CommunityMetrics
{
    public class GitHubIssueActivity : IActivity
	{
		public MetricTypeEnum MetricType {
			get { return MetricTypeEnum.Daily; }
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

			Dictionary<string, int> arrUsers = new Dictionary<string, int>();

			GitHubClient objGitHub = new GitHubClient(new ProductHeaderValue("HCC.CommunityActivity"));
			objGitHub.Credentials = new Credentials(objActivity.Settings["Credentials"].ToString());

			RepositoryIssueRequest objOptions = new RepositoryIssueRequest();
			objOptions.Filter = IssueFilter.All;
			objOptions.State = ItemStateFilter.All;

            IReadOnlyList<Octokit.Repository> objRepositories = objGitHub.Repository.GetAllForOrg(objActivity.Settings["Organization"].ToString()).Result;
			foreach (Octokit.Repository objRepository in objRepositories) {
                if (objActivity.Settings["Repositories"].ToString() == "" || ("," + objActivity.Settings["Repositories"].ToString() + ",").ToLower().IndexOf("," + objRepository.Name.ToLower() + ",") != -1)
                {
                    IReadOnlyList<Octokit.Issue> objIssues = objGitHub.Issue.GetAllForRepository(objRepository.Id, objOptions).Result;
                    foreach (Octokit.Issue objIssue in objIssues)
                    {
                        if (objIssue.PullRequest == null) // exclude issues which are automatically generated for pull requests
                        {
                            if (objIssue.CreatedAt.DateTime.ToString("yyyy-MMM-dd") == ExecutionDate.ToString("yyyy-MMM-dd"))
                            {
                                if (!arrUsers.ContainsKey(objIssue.User.Login))
                                {
                                    arrUsers.Add(objIssue.User.Login, 1);
                                }
                                else
                                {
                                    arrUsers[objIssue.User.Login] += 1;
                                }
                            }
                        }
                    }
                }
			}

			List<UserActivityInfo> objUserActivities = new List<UserActivityInfo>();
			foreach (KeyValuePair<string, int> kvp in arrUsers) {
                strSQL = "select UserID ";
                strSQL += "from UserProfile up ";
                strSQL += "inner join ProfilePropertyDefinition ppd on up.PropertyDefinitionID = ppd.PropertyDefinitionID ";
                strSQL += "where ppd.PropertyName = @0 ";
                strSQL += "and up.PropertyValue like '%' + @1 + '%'";
                var obj = db.QuerySingle(strSQL, "GitHub", kvp.Key);
                if ((obj != null))
                {
                    UserActivityInfo objUserActivity = new UserActivityInfo();
                    objUserActivity.UserId = obj.UserId;
                    objUserActivity.Count = kvp.Value;
                    objUserActivities.Add(objUserActivity);
                }
            }

            return objUserActivities;
		}
	}
}
