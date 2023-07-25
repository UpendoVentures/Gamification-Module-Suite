using System.Collections;
using System.Collections.Generic;

namespace HCC.CommunityMetrics
{
    public interface IActivity
	{
		MetricTypeEnum MetricType { get; }
		Hashtable GetSettings();
		List<UserActivityInfo> GetUserActivity(ActivityInfo objActivity, System.DateTime ExecutionDate);
	}
}
