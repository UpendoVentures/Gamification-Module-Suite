using DotNetNuke.UI.Modules;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Permissions;
using System.Web;
using FiftyOne.Foundation.Mobile.Detection;
using WebMatrix.Data;
using MaxMind.GeoIP2;
using System;

namespace HCC.Personalization
{
    public class ContextInjectionFilter : IModuleInjectionFilter
    {           
        public bool CanInjectModule(ModuleInfo objModule, PortalSettings objPortalSettings)
        {
            bool InjectModule = true;
            if (objModule.ModuleSettings["personalization"] != null)
            {
                //  if user does not have EDIT permissions
                if (ModulePermissionController.HasModulePermission(objModule.ModulePermissions, "EDIT") == false)
                {
                    //  get personalization rules
                    string[] arrRules = objModule.ModuleSettings["personalization"].ToString().Split('|');
                    foreach (string strRule in arrRules)
                    {
                        if (strRule != "")
                        {
                            string[] arrAttributes = strRule.Split(',');
                            if (arrAttributes.Length == 3)
                            {
                                InjectModule = EvaluateProperty(arrAttributes[0], arrAttributes[1], arrAttributes[2], objPortalSettings);
                                if (InjectModule == false)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return InjectModule;
        }
        
        private bool EvaluateProperty(string strAttribute, string strCondition, string strValue, PortalSettings objPortalSettings)
        {
            bool Evaluate = true;
            string strType = "";
            string strKey = "";

            HttpCookie objVisitorCookie = HttpContext.Current.Request.Cookies["HCCVISITOR"];

            if (strAttribute.IndexOf(":") != -1)
            {
                strType = strAttribute.Substring(0, strAttribute.IndexOf(":")).Trim();
                strKey = strAttribute.Substring(strAttribute.IndexOf(":") + 1).Trim();
            }
            else
            {
                strType = strAttribute.Trim();
                strKey = strAttribute.Trim();
            }

            try
            {
                switch (strType.ToUpper())
                {
                    case "DEVICE":
                        var objDevice = WebProvider.ActiveProvider.Match(HttpContext.Current.Request.UserAgent);
                        if (objDevice != null && objDevice[strKey] != null)
                        {
                            Evaluate = CheckValue(strValue, strCondition, objDevice[strKey].ToString());
                        }
                        else
                        {
                            Evaluate = false;
                        }

                        break;
                    case "USERAGENT":
                        Evaluate = CheckValue(strValue, strCondition, HttpContext.Current.Request.UserAgent);
                        break;
                    case "LANGUAGE":
                        Evaluate = CheckValue(strValue, strCondition, DotNetNuke.Services.Localization.Localization.GetPageLocale(objPortalSettings).Name);
                        break;
                    case "LOCATION":
                        using (var objGeoIP2DB = new DatabaseReader(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\GeoIP2-City.mmdb")))
                        {
                            try
                            {
                                var objGeoIP2 = objGeoIP2DB.City(HttpContext.Current.Request.UserHostAddress);
                                switch (strKey.ToUpper())
                                {
                                    case "CONTINENT":
                                        if (objGeoIP2.Continent.Name != null)
                                        {
                                            Evaluate = CheckValue(strValue, strCondition, objGeoIP2.Continent.Name);
                                        }
                                        else
                                        {
                                            Evaluate = false;
                                        }
                                        break;
                                    case "COUNTRY":
                                        if (objGeoIP2.Country.Name != null)
                                        {
                                            Evaluate = CheckValue(strValue, strCondition, objGeoIP2.Country.Name);
                                        }
                                        else
                                        {
                                            Evaluate = false;
                                        }
                                        break;
                                    case "REGION":
                                        if (objGeoIP2.MostSpecificSubdivision.Name != null)
                                        {
                                            Evaluate = CheckValue(strValue, strCondition, objGeoIP2.MostSpecificSubdivision.Name);
                                        }
                                        else
                                        {
                                            Evaluate = false;
                                        }
                                        break;
                                    case "CITY":
                                        if (objGeoIP2.City.Name != null)
                                        {
                                            Evaluate = CheckValue(strValue, strCondition, objGeoIP2.City.Name);
                                        }
                                        else
                                        {
                                            Evaluate = false;
                                        }
                                        break;
                                    case "LATITUDE":
                                        if (objGeoIP2.Location.Latitude != null)
                                        {
                                            Evaluate = CheckValue(strValue, strCondition, objGeoIP2.Location.Latitude.ToString());
                                        }
                                        else
                                        {
                                            Evaluate = false;
                                        }
                                        break;
                                    case "LONGITUDE":
                                        if (objGeoIP2.Location.Longitude != null)
                                        {
                                            Evaluate = CheckValue(strValue, strCondition, objGeoIP2.Location.Longitude.ToString());
                                        }
                                        else
                                        {
                                            Evaluate = false;
                                        }
                                        break;
                                }
                            }
                            catch
                            {
                                // IP address cannot be resolved
                                Evaluate = false;
                            }
                        }
                        break;
                    case "IP":
                        Evaluate = CheckValue(strValue, strCondition, HttpContext.Current.Request.UserHostAddress);
                        break;
                    case "PAGE":
                        string Page = "";
                        if (!(objVisitorCookie == null))
                        {
                            dynamic objVisit;
                            using (var db = Database.Open("SiteSqlServer"))
                            {
                                objVisit = db.QuerySingle("select top 1 TabName from hccm_Visits inner join Tabs on hccm_Visits.TabId = Tabs.TabId where VisitorId = @0 and TabName = @1", objVisitorCookie.Value, strValue);
                            }
                        }
                        Evaluate = CheckValue(strValue, strCondition, Page);
                        break;
                    case "REFERRER":
                        string Referrer = "";
                        if (!(objVisitorCookie == null))
                        {
                            dynamic objVisit;
                            using (var db = Database.Open("SiteSqlServer"))
                            {
                                objVisit = db.QuerySingle("select top 1 ReferrerDomain from hccm_Visits where VisitorId = @0 and ReferrerDomain = @1", objVisitorCookie.Value, strValue);
                            }
                            if (objVisit != null)
                            {
                                Referrer = objVisit.ReferrerDomain;
                            }
                        }
                        Evaluate = CheckValue(strValue, strCondition, Referrer);
                        break;
                }
            }
            catch
            {
                // ignore
            }

            return Evaluate;
        }

        private bool CheckValue(string strValue1, string strOperator, string strValue2)
        {
            bool Check = true;
            double dblValue1 = 0;
            double dblValue2 = 0;
            switch (strOperator) {
                case "!=":
                    Check = (strValue1.Trim().ToUpper() != strValue2.Trim().ToUpper());
                    break;
                case "=":
                    Check = (strValue1.Trim().ToUpper() == strValue2.Trim().ToUpper());
                    break;
                case "<":
                    if (double.TryParse(strValue1, out dblValue1) && double.TryParse(strValue2, out dblValue2))
                    {
                        Check = (double.Parse(strValue1.Trim().ToUpper()) < double.Parse(strValue2.Trim().ToUpper()));
                    }
                    else
                    {
                        Check = false;
                    }
                    break;
                case ">":
                    if (double.TryParse(strValue1, out dblValue1) && double.TryParse(strValue2, out dblValue2))
                    {
                        Check = (double.Parse(strValue1.Trim().ToUpper()) > double.Parse(strValue2.Trim().ToUpper()));
                    }
                    else
                    {
                        Check = false;
                    }
                    break;
                case "!~":
                    Check = (strValue2.Trim().ToUpper().IndexOf(strValue1.Trim().ToUpper()) == -1);
                    break;
                case "~":
                    Check = (strValue2.Trim().ToUpper().IndexOf(strValue1.Trim().ToUpper()) != -1);
                    break;
                case "[]":
                    Check = (strValue1.Trim().ToUpper().IndexOf(strValue2.Trim().ToUpper()) == -1);
                    break;
                case "[!]":
                    Check = (strValue1.Trim().ToUpper().IndexOf(strValue2.Trim().ToUpper()) != -1);
                    break;
            }
            return Check;
        }                    
    }
}