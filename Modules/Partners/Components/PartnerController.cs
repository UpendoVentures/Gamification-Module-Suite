using System.Collections.Generic;
using WebMatrix.Data;
using System.Drawing;
using System.IO;
using DotNetNuke.Entities.Users;
using DotNetNuke.Common.Utilities;
using System;

namespace HCC.Partners
{
    public class PartnerController
	{
        public IEnumerable<dynamic> GetPartnerOwners(int PortalId)
        {
            IEnumerable<dynamic> objUsers;
            using (var db = Database.Open("SiteSqlServer"))
            {
                objUsers = db.Query("exec hccm_GetPartnerOwners @0", PortalId);
            }
            return objUsers;
        }

        public dynamic GetPartnerByUserId(int PortalId, int UserId)
		{
            dynamic objPartner;
            using (var db = Database.Open("SiteSqlServer"))
            {
                objPartner = db.QuerySingle("exec hccm_GetPartnerByUserId @0, @1", PortalId, UserId);
            }
            return objPartner;
		}

		public dynamic GetPartner(int PartnerId)
		{
            dynamic objPartner;
            using (var db = Database.Open("SiteSqlServer"))
            {
                objPartner = db.QuerySingle("exec hccm_GetPartner @0", PartnerId);
            }
            return objPartner;
		}

		public int UpdatePartner(int PortalId, string PartnerName, string Logo, string Summary, string Description, string City, string Region, string Telephone, string URL, string Email, string Contact, string Services, bool IsApproved, int UserId)
		{
            int intPartnerId;
            using (var db = Database.Open("SiteSqlServer"))
            {
                intPartnerId = db.QuerySingle("exec hccm_UpdatePartner @0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13", PortalId, PartnerName, Logo, Summary, Description, City, Region, Telephone, URL, Email, Contact, Services, IsApproved, UserId).PartnerId;
            }
			return intPartnerId;
		}

		public IEnumerable<dynamic> GetPartnerUsers(int PartnerId)
		{
            IEnumerable<dynamic> objPartnerUsers;
            using (var db = Database.Open("SiteSqlServer"))
            {
                objPartnerUsers = db.Query("exec hccm_GetPartnerUsers @0", PartnerId);
            }
            return objPartnerUsers;
		}

		public IEnumerable<dynamic> GetUserPartners(int UserId)
		{
            IEnumerable<dynamic> objPartnerUsers;
            using (var db = Database.Open("SiteSqlServer"))
            {
                objPartnerUsers = db.Query("exec hccm_GetUserPartners @0", UserId);
            }
            return objPartnerUsers;
		}

		public void AddPartnerUser(int PartnerId, int UserId)
		{
            using (var db = Database.Open("SiteSqlServer"))
            {
                db.Execute("exec hccm_AddPartnerUser @0, @1", PartnerId, UserId);
            }
		}

		public void DeletePartnerUser(int PartnerId, int UserId)
		{
            using (var db = Database.Open("SiteSqlServer"))
            {
                db.Execute("exec hccm_DeletePartnerUser @0, @1", PartnerId, UserId);
            }
		}

        public IEnumerable<dynamic> GetPartnerActivity(int PortalId, object PartnerId, object Service, object ActivityId, object StartDate, object EndDate, int Rows, int Page, bool Summary)
		{
            string cacheKey = "HCC.Partners." + PortalId.ToString() + "." + (Convert.IsDBNull(PartnerId) ? "" : PartnerId.ToString()) + "." + (Convert.IsDBNull(Service) ? "" : Service.ToString()) + "." + (Convert.IsDBNull(ActivityId) ? "" : ActivityId.ToString()) + "." + (Convert.IsDBNull(StartDate) ? "" : StartDate.ToString()) + "." + (Convert.IsDBNull(EndDate) ? "" : EndDate.ToString()) + "." + Rows.ToString() + "." + Page.ToString() + "." + Summary.ToString();
            var args = new CacheItemArgs(cacheKey);
            IEnumerable<dynamic> objUserActivity = CBO.GetCachedObject<IEnumerable<dynamic>>(args,
                delegate
                {
                    using (var db = Database.Open("SiteSqlServer"))
                    {
                        objUserActivity = db.Query("exec hccm_GetPartnerActivity @0, @1, @2, @3, @4, @5, @6, @7, @8", PortalId, PartnerId, Service, ActivityId, StartDate, EndDate, Rows, Page, Summary);
                    }
                    return objUserActivity;
                }
            );
            return objUserActivity;
        }

        public string SaveLogo(string strFile, Size objSize, UserInfo objUser)
        {
            string strFileID = "";
            Image imgOriginal = Image.FromFile(strFile);
            int intWidth = imgOriginal.Width;
            int intHeight = imgOriginal.Height;

            // calculate aspect ratios
            double ratioX = (double)objSize.Width / (double)intWidth;
            double ratioY = (double)objSize.Height / (double)intHeight;
      
            // use whichever ratio is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // get the new height and width
            int newHeight = (int)(intHeight * ratio);
            int newWidth = (int)(intWidth * ratio);

            // calculate the X,Y position of the upper-left corner 
            int intX = (int)((objSize.Width - (intWidth * ratio)) / 2);
            int intY = (int)((objSize.Height - (intHeight * ratio)) / 2);

            // create new image
            Image objNewImage = new Bitmap(objSize.Width, objSize.Height);
            using (Graphics objGraphics = Graphics.FromImage(objNewImage))
            {
                objGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                objGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                objGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                objGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                objGraphics.DrawImage(imgOriginal, intX, intY, newWidth, newHeight);
            }

            // convert to PNG with transparent background
            Bitmap objBmp = new Bitmap(objNewImage);
            Color objColor = objBmp.GetPixel(1, 1);
            objBmp.MakeTransparent(objColor);
            strFile = Path.ChangeExtension(strFile, "png");
            strFile = strFile.Replace(".png", "_" + objSize.Width.ToString() + "px.png");
            objBmp.Save(strFile, System.Drawing.Imaging.ImageFormat.Png);
            objBmp.Dispose();

            // register logo in system
            DotNetNuke.Services.FileSystem.IFolderInfo objFolder = DotNetNuke.Services.FileSystem.FolderManager.Instance.GetUserFolder(objUser);
            using (FileStream objStream = new FileStream(strFile, FileMode.Open))
            {
                DotNetNuke.Services.FileSystem.IFileInfo objFile = DotNetNuke.Services.FileSystem.FileManager.Instance.AddFile(objFolder, "logo_" + System.DateTime.Now.ToString("yyyyMMddhhmmss") + ".png", objStream, true);
                strFileID = objFile.FileId.ToString();
            }
            File.Delete(strFile);
            return strFileID;
        }

    }
}