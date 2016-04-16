using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using System.Runtime.InteropServices;

namespace CarBidWebClient
{
	/// <summary>
	/// 截图目前就截三处地方
	/// 1. 开始拍卖
	/// 2. 重置证书
	/// 3. 窗口关闭的时候 即程序退出时（这个不太好弄吧）
	/// </summary>
    public static class ScreenShotHelper
    {
		public static void qrsyncFile()
		{
			try
			{
				System.Diagnostics.Process exep = new System.Diagnostics.Process();
				exep.StartInfo.FileName = "qrsync";
				exep.StartInfo.Arguments = "qiniu.json";
				exep.StartInfo.CreateNoWindow = true;
				exep.StartInfo.UseShellExecute = false;
				exep.Start();
				exep.WaitForExit();
			}
			catch
			{
			}

		}
		
        public static void screenCapture(BIDContext ctx)
        {
        	StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(ctx.BIDNumer);
			stringBuilder.Append("_");
			stringBuilder.Append(DateTime.Now.ToString("HH_mm_ss"));
			string imageSavePath = stringBuilder.ToString();
			
            Point curPos = new Point(Cursor.Position.X, Cursor.Position.Y);
            Size curSize = new Size();
            curSize.Height = Cursor.Current.Size.Height;
            curSize.Width = Cursor.Current.Size.Width;
			Rectangle bounds = Screen.GetBounds(Screen.GetBounds(Point.Empty));
			CaptureImage(curSize, curPos, Point.Empty, Point.Empty, bounds, imageSavePath);            
        }
        
        private static void CaptureImage(Size curSize, Point curPos, Point SourcePoint, Point DestinationPoint, Rectangle SelectionRectangle, string filePath)
        {
            using (Bitmap bitmap = new Bitmap(SelectionRectangle.Width, SelectionRectangle.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(SourcePoint, DestinationPoint, SelectionRectangle.Size);
                }
				
//                bitmap.Save(filePath, ImageFormat.Jpeg);
                bitmap.Save(filePath + ".png", ImageFormat.Png);
                Thread.Sleep(500);
                qrsyncFile();
            }
        }
    }
}
