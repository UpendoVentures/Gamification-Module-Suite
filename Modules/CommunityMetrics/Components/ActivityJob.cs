using System;
namespace HCC.CommunityMetrics
{
    public class ActivityJob : DotNetNuke.Services.Scheduling.SchedulerClient
	{
		public ActivityJob(DotNetNuke.Services.Scheduling.ScheduleHistoryItem objScheduleHistoryItem) : base()
		{
			ScheduleHistoryItem = objScheduleHistoryItem;
		}

		public override void DoWork()
		{
			try
            {
				Progressing();
				string strMessage = ProcessActivities();
				ScheduleHistoryItem.Succeeded = true;
				ScheduleHistoryItem.AddLogNote("Successful. " + strMessage);
			}
            catch (Exception exc)
            {
				ScheduleHistoryItem.Succeeded = false;
				ScheduleHistoryItem.AddLogNote("Failed. " + exc.ToString());
				Errored(ref exc);
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
			}
		}

		public string ProcessActivities()
		{
			string strMessage = "<br/>";
            int intDays = 0;
            System.DateTime datDate = default(System.DateTime);
			int intUsers = 0;

			// iterate through activities
			ActivityController objActivities = new ActivityController();
			foreach (ActivityInfo objActivity in objActivities.GetActivities())
            {
				if (objActivity.IsActive)
                {
					strMessage += "<br/>Activity: " + objActivity.ActivityName + "<br/>";
                    intDays = (DateTime.Now - objActivity.LastExecutionDate).Days;
                    datDate = objActivity.LastExecutionDate;
                    // iterate through all days from LastExecutionDate to Today
                    for (int intDay = 1; intDay <= intDays; intDay++)
                    {
						intUsers = 0;
						objActivity.LastExecutionDate = datDate.AddDays(1);
						strMessage += "Date: " + objActivity.LastExecutionDate.ToString("yyyyMMdd");
						try
                        {
							foreach (UserActivityInfo objUserActivity in objActivities.GetUserActivity(objActivity, datDate))
                            {
                                if (string.IsNullOrWhiteSpace(objActivity.UserFilter) || ("," + objActivity.UserFilter + ",").IndexOf("," + objUserActivity.UserId.ToString() + ",") == -1 )
                                {
                                    if (objActivity.MinDaily > 0 && objUserActivity.Count < objActivity.MinDaily)
                                    {
                                        objUserActivity.Count = 0;
                                    }
                                    if (objActivity.MaxDaily > 0 && objUserActivity.Count > objActivity.MaxDaily)
                                    {
                                        objUserActivity.Count = objActivity.MaxDaily;
                                    }
                                    if (objUserActivity.Count > 0)
                                    {
                                        objActivities.UpdateUserActivity(objActivity.ActivityId, objUserActivity.UserId, datDate, objUserActivity.Count, "");
                                        intUsers += 1;
                                    }
                                }
                            }
							strMessage += " Users: " + intUsers.ToString() + "<br/>";
							objActivities.UpdateActivity(objActivity);
						}
                        catch (Exception ex)
                        {
							strMessage += ex.ToString() + "<br/>";
						}
						datDate = objActivity.LastExecutionDate;
					}
				}
			}
			return strMessage;
		}
	}
}