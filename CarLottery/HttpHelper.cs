using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading;

namespace CarBidWebClient
{
    public static class HttpHelper
    {
        public static LoginInfo GetLogInfo()
        {
	        try
	        {
	        	using (WebClient webClient = new WebClient())
	        	{
                    string logInfoUrl = "http://" + BIDWebConfig.yayaServer + ":8080/get_login_info";
                    var jsonInfo = webClient.DownloadString(logInfoUrl);
                    return JsonConvert.DeserializeObject<LoginInfo>(jsonInfo.ToString());
	        	}	
	        }
	        catch (Exception)
	        {
	        	return null;
	        }	
        }

        public static TestInfo GetTestInfo()
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string testInfoUrl = "http://" + BIDWebConfig.yayaServer + ":8080/get_test_info";
                    var jsonInfo = webClient.DownloadString(testInfoUrl);
                    return JsonConvert.DeserializeObject<TestInfo>(jsonInfo.ToString());
                }
            }
            catch (Exception)
            {
                return null;
            }
        }    	

    	public static Bitmap DownloadImage(string imageUrl)
    	{
    		int retries = 6;
    		Bitmap bitmap = null;
    		while ((bitmap == null) && (retries-- > 0))
    		{
    			try
    			{
    				WebClient client = new WebClient();
    				Stream stream = client.OpenRead(imageUrl);
    				bitmap = new Bitmap(stream);
    				stream.Flush();
					stream.Close();    				
    			}
    			catch (Exception)
    			{
    				BIDLogger.GetLogger(null).Append("DownloadImage retry");
    				
    				//这个WebClient应该自己默认有超时时间的 1秒吧
//    				Thread.Sleep(500);
    				continue;
    			}
    		}
    		return bitmap;
    	}
    }
}
