using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.Compilation;
using System.Reflection;
using WebMatrix.Data;
using DotNetNuke.Entities.Users;
using DotNetNuke.Data;
using System.Linq;
using DotNetNuke.Common.Utilities;

namespace HCC.CommunityMetrics
{
    public class ActivityController
	{
        public List<ActivityInfo> GetActivities()
		{
			List<ActivityInfo> objActivities = new List<ActivityInfo>();
            using (var db = Database.Open("SiteSqlServer"))
            {
                foreach (var obj in db.Query("exec hccm_GetActivities"))
                {
                    ActivityInfo objActivity = new ActivityInfo();
                    objActivity.ActivityId = obj.ActivityId;
                    objActivity.ActivityName = obj.ActivityName;
                    objActivity.Description = obj.Description;
                    objActivity.TypeName = obj.TypeName;
                    objActivity.Factor = obj.Factor;
                    objActivity.IsActive = obj.IsActive;
                    objActivity.LastExecutionDate = obj.LastExecutionDate;
                    objActivity.MetricType = (MetricTypeEnum)obj.MetricType;
                    objActivity.UserFilter = obj.UserFilter;
                    objActivity.MinDaily = obj.MinDaily;
                    objActivity.MaxDaily = obj.MaxDaily;

                    Hashtable objSettings = new Hashtable();
                    foreach (var objSetting in db.Query("exec hccm_GetActivitySettings @0", objActivity.ActivityId))
                    {
                        objSettings.Add(objSetting.SettingName, objSetting.SettingValue);
                    }

                    objActivity.Settings = objSettings;
                    objActivities.Add(objActivity);
                }
            }
			return objActivities;
		}

		public ActivityInfo GetActivity(int ActivityId)
		{
			ActivityInfo objActivity = new ActivityInfo();
            using (var db = Database.Open("SiteSqlServer"))
            {
                var obj = db.QuerySingle("exec hccm_GetActivity @0", ActivityId);
                if (obj != null)
                {
                    objActivity.ActivityId = obj.ActivityId;
                    objActivity.ActivityName = obj.ActivityName;
                    objActivity.Description = obj.Description;
                    objActivity.TypeName = obj.TypeName;
                    objActivity.Factor = obj.Factor;
                    objActivity.IsActive = obj.IsActive;
                    objActivity.LastExecutionDate = obj.LastExecutionDate;
                    objActivity.MetricType = (MetricTypeEnum)obj.MetricType;
                    objActivity.UserFilter = obj.UserFilter;
                    objActivity.MinDaily = obj.MinDaily;
                    objActivity.MaxDaily = obj.MaxDaily;

                    Hashtable objSettings = new Hashtable();
                    foreach (var objSetting in db.Query("exec hccm_GetActivitySettings @0", ActivityId))
                    {
                        objSettings.Add(objSetting.SettingName, objSetting.SettingValue);
                    }
                    objActivity.Settings = objSettings;
                }
            }
			return objActivity;
		}

		public int UpdateActivity(ActivityInfo objActivity)
		{
            int intActivityId;
            using (var db = Database.Open("SiteSqlServer"))
            {
                intActivityId = db.QuerySingle("exec hccm_UpdateActivity @0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11", objActivity.ActivityId, objActivity.ActivityName, objActivity.Description, objActivity.TypeName, objActivity.Factor, objActivity.IsActive, objActivity.LastExecutionDate, Convert.ToInt32(objActivity.MetricType), objActivity.UserFilter, objActivity.MinDaily, objActivity.MaxDaily, UserController.Instance.GetCurrentUserInfo().UserID).ActivityId;
            }
            return intActivityId;
		}

		public void UpdateActivitySetting(int ActivityId, string SettingName, string SettingValue)
		{
            using (var db = Database.Open("SiteSqlServer"))
            {
                db.Execute("exec hccm_UpdateActivitySetting @0, @1, @2, @3", ActivityId, SettingName, SettingValue, UserController.Instance.GetCurrentUserInfo().UserID);
            }
		}

        public UserActivityInfo GetUserActivity(int ActivityId, int UserId, DateTime Date)
        {
            UserActivityInfo objUserActivity = new UserActivityInfo();
            using (var db = Database.Open("SiteSqlServer"))
            {
                var obj = db.QuerySingle("exec hccm_GetUserActivity @0, @1, @2", ActivityId, UserId, Date);
                if (obj != null)
                {
                    objUserActivity.ActivityId = obj.ActivityId;
                    objUserActivity.UserId = obj.UserId;
                    objUserActivity.Date = obj.Date;
                    objUserActivity.Count = obj.Count;
                    objUserActivity.Notes = obj.Notes;
                }
            }
            return objUserActivity;
        }

        public void UpdateUserActivity(int ActivityId, int UserId, DateTime Date, int Count, string Notes)
		{
            using (var db = Database.Open("SiteSqlServer"))
            {
                db.Execute("exec hccm_UpdateUserActivity @0, @1, @2, @3, @4", ActivityId, UserId, Date, Count, Notes);
            }
		}

		public IEnumerable<object> GetUserActivities(object UserId, object ActivityId, object StartDate, object EndDate, bool Summary, int Rows)
		{
            string cacheKey = "HCC.CommunityMetrics." + UserId.ToString() + "." + ActivityId.ToString() + "." + StartDate.ToString() + "." + EndDate.ToString() + "." + Summary.ToString() + "." + Rows.ToString();
            var args = new CacheItemArgs(cacheKey);
            IEnumerable<object> objUserActivity = CBO.GetCachedObject<IEnumerable<object>>(args,
                delegate
                {
                    using (var db = Database.Open("SiteSqlServer"))
                    {
                        objUserActivity = db.Query("exec hccm_GetUserActivities @0, @1, @2, @3, @4, @5", UserId, ActivityId, StartDate, EndDate, Summary, Rows);
                    }
                    return objUserActivity;
                }
            );
            return objUserActivity;
       }

        public IDataReader GetDailyActivity(object ActivityId, object StartDate, object EndDate)
        {
            return DataProvider.Instance().ExecuteReader("hccm_GetDailyActivity", ActivityId, StartDate, EndDate);
        }

        public List<UserActivityInfo> GetUserActivity(ActivityInfo objActivity, DateTime ExecutionDate)
		{
			IActivity objIActivity = (IActivity)Activator.CreateInstance(BuildManager.GetType(objActivity.TypeName, true));
			List<UserActivityInfo> lstUserActivities = objIActivity.GetUserActivity(objActivity, ExecutionDate);
			if ((MetricTypeEnum)objActivity.MetricType == MetricTypeEnum.Cumulative)
            {
				foreach (UserActivityInfo objUserActivity in lstUserActivities)
                {
					int intCount = GetHistoricalCount(objActivity.ActivityId, objUserActivity.UserId, ExecutionDate);
					objUserActivity.Count = objUserActivity.Count - intCount;
				}
			}
            if ((MetricTypeEnum)objActivity.MetricType == MetricTypeEnum.Once)
            {
                foreach (UserActivityInfo objUserActivity in lstUserActivities)
                {
                    int intCount = GetHistoricalCount(objActivity.ActivityId, objUserActivity.UserId, ExecutionDate);
                    if (intCount > 0) objUserActivity.Count = 0;
                }
            }
            return lstUserActivities;
		}

        private int GetHistoricalCount(int ActivityId, int UserId, DateTime CurrentDate)
        {
            return DataProvider.Instance().ExecuteScalar<int>("hccm_GetUserActivityCount", ActivityId, UserId, CurrentDate);
        }

        public Hashtable GetSettings(string TypeName)
		{
			IActivity objIActivity = (IActivity)Activator.CreateInstance(BuildManager.GetType(TypeName, true));
			Hashtable objSettings = objIActivity.GetSettings();
			return objSettings;
		}

		public MetricTypeEnum GetMetricType(string TypeName)
		{
			IActivity objIActivity = (IActivity)Activator.CreateInstance(BuildManager.GetType(TypeName, true));
			return objIActivity.MetricType;
        }

		public List<Type> GetActivityTypes()
		{
			List<Type> objTypes = new List<Type>();
			foreach (Assembly objAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
				Type[] objLoadableTypes = null;
				try
                {
					objLoadableTypes = objAssembly.GetTypes();
				} catch (ReflectionTypeLoadException e) {
					objLoadableTypes = e.Types;
				}
				foreach (Type objType in objLoadableTypes.Where(t => t != null))
                {
					if (!objType.IsInterface & typeof(HCC.CommunityMetrics.IActivity).IsAssignableFrom(objType)) {
						objTypes.Add(objType);
					}
				}
			}
			return objTypes;
		}

	}

}