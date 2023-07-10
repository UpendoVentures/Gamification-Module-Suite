using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using WebMatrix.Data;

namespace HCC.CommunityMetrics
{
    public class TwitterActivity : IActivity
	{
		public MetricTypeEnum MetricType {
			get { return MetricTypeEnum.Daily; }
		}

		public Hashtable GetSettings()
		{
			Hashtable objSettings = new Hashtable();
			objSettings.Add("Connection", "Your Database Connection Name ( ie. SiteSqlServer )");
			objSettings.Add("Consumer Key", "Consumer Key From Twitter http://apps.twitter.com");
            objSettings.Add("Consumer Secret", "Consumer Secret From Twitter http://apps.twitter.com");
            objSettings.Add("Access Token", "Access Token From Twitter http://apps.twitter.com");
            objSettings.Add("Access Secret", "Access Secret From Twitter http://apps.twitter.com");
            objSettings.Add("Query", "Twitter Search Query ( ie. #twitter OR @twitter )");
			objSettings.Add("Profile", "The Name Of The User Profile Field For Twitter Accounts In Your Site");
			return objSettings;
		}

		/// <summary>
        /// 
        /// </summary>
        /// <param name="objActivity"></param>
        /// <param name="ExecutionDate"></param>
        /// <returns></returns>
        public List<UserActivityInfo> GetUserActivity(ActivityInfo objActivity, System.DateTime ExecutionDate)
		{
			dynamic db = Database.Open(objActivity.Settings["Connection"].ToString());
			string strSQL = "";

            Dictionary<int, int> arrUsers = new Dictionary<int, int>();

            TwitterAPI api = new TwitterAPI(
                objActivity.Settings["Access Token"].ToString(),
                objActivity.Settings["Access Secret"].ToString(),
                objActivity.Settings["Consumer Key"].ToString(),
                objActivity.Settings["Consumer Secret"].ToString());

            foreach (JSONObject json in api.Get("search/tweets.json", new Parameters { { "q", objActivity.Settings["Query"].ToString() } }))
            {
                foreach (JSONObject status in json.GetList<JSONObject>("statuses"))
                {
                    DateTime CreatedDate = DateTime.ParseExact(status.Get("created_at").ToString(), "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture);
                    string ScreenName = status.Get("user.screen_name").ToString();

                    if (CreatedDate.ToString("yyyy-MMM-dd") == ExecutionDate.ToString("yyyy-MMM-dd"))
                    {
                        strSQL = "select UserID ";
                        strSQL += "from UserProfile up ";
                        strSQL += "inner join ProfilePropertyDefinition ppd on up.PropertyDefinitionID = ppd.PropertyDefinitionID ";
                        strSQL += "where ppd.PropertyName = @0 ";
                        strSQL += "and up.PropertyValue like '%' + @1 + '%'";
                        var obj = db.QuerySingle(strSQL, objActivity.Settings["Profile"], ScreenName);
                        if ((obj != null))
                        {
                            if (!arrUsers.ContainsKey(obj.UserId))
                            {
                                arrUsers.Add(obj.UserId, 1);
                            }
                            else
                            {
                                arrUsers[obj.UserId] += 1;
                            }
                        }
                    }
                }
            }

            List<UserActivityInfo> objUserActivities = new List<UserActivityInfo>();
			foreach (KeyValuePair<int, int> kvp in arrUsers) {
				UserActivityInfo objUserActivity = new UserActivityInfo();
				objUserActivity.UserId = kvp.Key;
				objUserActivity.Count = kvp.Value;
				objUserActivities.Add(objUserActivity);
			}

			return objUserActivities;
		}
	}
}
