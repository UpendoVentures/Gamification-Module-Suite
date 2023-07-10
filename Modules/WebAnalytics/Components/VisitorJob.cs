using System;

namespace HCC.WebAnalytics
{
    public class VisitorJob : DotNetNuke.Services.Scheduling.SchedulerClient
	{
        public VisitorJob(DotNetNuke.Services.Scheduling.ScheduleHistoryItem objScheduleHistoryItem) : base()
		{
			ScheduleHistoryItem = objScheduleHistoryItem;
		}

		public override void DoWork()
		{
			try {
				string strMessage = Processing();
				ScheduleHistoryItem.Succeeded = true;
                ScheduleHistoryItem.AddLogNote("Successful. " + strMessage);
			} catch (Exception exc) {
				ScheduleHistoryItem.Succeeded = false;
				ScheduleHistoryItem.AddLogNote("Failed. " + exc.Message);
				Errored(ref exc);
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
            }
        }

		public string Processing()
		{
			string Message = "";

            VisitorController objVisitors = new VisitorController();
            objVisitors.WriteVisits();
            objVisitors.PurgeVisits();

            return Message;
		}

	}

}