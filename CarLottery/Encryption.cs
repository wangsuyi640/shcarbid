using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Reflection;

namespace CarBidWebClient
{
    public static class EncryptionHelper
    {
    	public static Tuple<string, int> ParserLicense(string encryString)
    	{
    		string licenseContent = DecryptString(encryString);
    		if (licenseContent == null)
    		{
    			// license文件无法解析
    			return Tuple.Create<string, int>("license文件无法解析", 44444444);
    		}
    		//时间上比较的话 比较下当前时间的月份和license文件中的月份即可 相等就可以搞
    		string[] sArray=Regex.Split(licenseContent, "\n", RegexOptions.IgnoreCase);
    		
    		// BIDNumber解析
    		string[] bidNumberArray=Regex.Split(sArray[0], "=", RegexOptions.IgnoreCase);
    		int bidNumberMore = Convert.ToInt32(bidNumberArray[1]);
    		
    		//过期时间解析
    		string[] expireArray=Regex.Split(sArray[4], "=", RegexOptions.IgnoreCase);
    		string expireTime = expireArray[1];
    		
    		// license是否过期判定
    		DateTime expireDt = Convert.ToDateTime(expireTime);    		
    		if (DateTime.Compare(DateTime.Now, expireDt) > 0)
    		{
    			return Tuple.Create<string, int>("license已经过期", 44444444);
    		}
    		
    		return Tuple.Create<string, int>("0", bidNumberMore);
    	}
    	
        private static string DecryptString(string encryString)
        {
	        try
	        {	            
	            string privateKeyXML = BIDWebConfig.privateKeyXML;
	            var rsa = new RSACryptoServiceProvider();
	            rsa.FromXmlString(privateKeyXML);
	
	            // Decrypt string, convert to correct encoding
	            byte[] encryptedBytes = Convert.FromBase64String(encryString);
	            var decryptedBytes = rsa.Decrypt(encryptedBytes, false);
	            var decryptedString = Encoding.UTF8.GetString(decryptedBytes);
	            return decryptedString;
	        }
	        catch (Exception ex)
	        {
	        	BIDLogger.GetLogger(null).Append("解析license文件出错" + ex.ToString());
	        	return null;
	        }
        }
    }
}
