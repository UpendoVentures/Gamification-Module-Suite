using System;

namespace HCC.WebAnalytics
{
    public class VisitorInfo
    {

	    public VisitorInfo()
	    {
	    }
        public int PortalId { get; set; }

        public DateTime Date { get; set; }

        public int VisitorId { get; set; }

        public object TabId { get; set; }

        public object UserId { get; set; }

        public string IP { get; set; }

        public string Country { get; set; }

	    public string Region { get; set; }

        public string City { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Language { get; set; }

	    public string Domain { get; set; }

        public string URL { get; set; }

        public string UserAgent { get; set; }

        public string DeviceType { get; set; }

        public string Device { get; set; }

        public string Platform { get; set; }

        public string Browser { get; set; }

        public string ReferrerDomain { get; set; }

        public string ReferrerURL { get; set; }

        public string Server { get; set; }

        public string Activity { get; set; }

        public string Campaign { get; set; }

        public Guid SessionId { get; set; }

        public Guid RequestId { get; set; }

        public object LastRequestId { get; set; }

    }

}