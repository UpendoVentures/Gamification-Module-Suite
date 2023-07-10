using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace HCC.Showcase
{ 
	public class WebsiteThumbnail
	{
		protected string url;
		protected int width;
		protected int height;
		protected int thumbWidth;
		protected int thumbHeight;
		protected Bitmap bmp;

		public static Bitmap GetThumbnail(string URL, int Width, int Height, int ThumbWidth, int ThumbHeight)
		{
			WebsiteThumbnail Thumbnail = new WebsiteThumbnail(URL, Width, Height, ThumbWidth, ThumbHeight);
			return Thumbnail.GetThumbnail();
		}

		protected WebsiteThumbnail(string URL, int Width, int Height, int ThumbWidth, int ThumbHeight)
		{
			url = URL;
			width = Width;
			height = Height;
			thumbWidth = ThumbWidth;
			thumbHeight = ThumbHeight;
		}

		protected Bitmap GetThumbnail()
		{
			Thread objThread = new Thread(new ThreadStart(GetThumbnailWorker));
			objThread.SetApartmentState(ApartmentState.STA);
			objThread.Start();
			objThread.Join();
			return bmp.GetThumbnailImage(thumbWidth, thumbHeight, null, IntPtr.Zero) as Bitmap;
		}

		protected void GetThumbnailWorker()
		{
			using (WebBrowser objBrowser = new WebBrowser()) {
				objBrowser.ClientSize = new Size(width, height);
				objBrowser.ScrollBarsEnabled = false;
				objBrowser.ScriptErrorsSuppressed = true;
				objBrowser.Navigate(url);

				while (objBrowser.ReadyState != WebBrowserReadyState.Complete) {
					System.Windows.Forms.Application.DoEvents();
				}
                System.Threading.Thread.Sleep(1000);

                bmp = new Bitmap(width, height);
				objBrowser.DrawToBitmap(bmp, new Rectangle(0, 0, width, height));
			}
		}

	}

}
