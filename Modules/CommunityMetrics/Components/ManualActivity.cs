using System;
using System.Collections;
using System.Collections.Generic;

namespace HCC.CommunityMetrics
{
    public class ManualActivity : IActivity
    {
        public MetricTypeEnum MetricType
        {
            get { return MetricTypeEnum.Daily; }
        }

        public Hashtable GetSettings()
        {
            return new Hashtable();
        }

        public List<UserActivityInfo> GetUserActivity(ActivityInfo objActivity, DateTime ExecutionDate)
        {
            return new List<UserActivityInfo>();
        }
    }
}