using System.Collections;
using System.Collections.Generic;
using WebMatrix.Data;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace HCC.CommunityMetrics
{
    public class StackOverflowActivity : IActivity
	{
		public MetricTypeEnum MetricType {
			get { return MetricTypeEnum.Cumulative; }
		}

		public Hashtable GetSettings()
		{
			Hashtable objSettings = new Hashtable();
			objSettings.Add("Connection", "Your Database Connection Name ( ie. SiteSqlServer )");
            objSettings.Add("Tag", "StackOverflow Tag ( ie. asp.net )");
			objSettings.Add("Profile", "The Name Of The User Profile Field For StackOverflow User Ids In Your Site");
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

            // returns the top 30 users in the past month who have used a specified tag in their answers on StackOverflow 
            var apiUrl = ("http://api.stackexchange.com/2.2/tags/" + objActivity.Settings["Tag"].ToString() + "/top-answerers/all_time?site=stackoverflow");
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            httpWebRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            httpWebRequest.Method = "GET";
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string responseText;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                responseText = streamReader.ReadToEnd();
            }
            var result = (Result)new JavaScriptSerializer().Deserialize(responseText, typeof(Result));
            foreach (TagScore item in result.items)
            {
                strSQL = "select UserID ";
                strSQL += "from UserProfile up ";
                strSQL += "inner join ProfilePropertyDefinition ppd on up.PropertyDefinitionID = ppd.PropertyDefinitionID ";
                strSQL += "where ppd.PropertyName = @0 ";
                strSQL += "and up.PropertyValue = @1";
                var obj = db.QuerySingle(strSQL, objActivity.Settings["Profile"], item.user.user_id.ToString());
                if ((obj != null))
                {
                    arrUsers.Add(obj.UserId, item.post_count);
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

    class Result
    {
        public List<TagScore> items { get; set; }
    }
    class TagScore
    {
        public ShallowUser user { get; set; }
        public int post_count { get; set; }
        public int score { get; set; }
    }
    class ShallowUser
    {
        public int reputation { get; set; }
        public int user_id { get; set; }
        public string user_type { get; set; }
        public string profile_image { get; set; }
        public string display_name { get; set; }
        public string link { get; set; }
    }
}
