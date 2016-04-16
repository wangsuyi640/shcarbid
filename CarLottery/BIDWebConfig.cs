using System;

namespace CarBidWebClient
{
	public class BIDWebConfig
	{
        public static string yayaServer = "114.215.179.168";
        //public static string yayaServer = "192.168.3.104";
        
		public static string UrlTest = "http://test.alltobid.com/";

        public static string UrlLogin = "https://paimai2.alltobid.com/bid/2015112101/login.htm";

        public static string UrlBid = "https://paimai2.alltobid.com/bid/2015112101/bid.htm";

		public static string[] TradeServers = new string[]
		{
			"222.73.120.236:8300"
		};

		public static string WebServer = "http://paimai.alltobid.com";

		public static string VersionHTTP = "1.0";

		public static string VersionSocket = "1.0";

		public static bool IsEncryptHTTP = false;

		public static string EncryptKeyHTTP = "shcarbid";

		public static bool IsEncryptSocket = true;

		public static string EncryptKeySocket = "@g*p%";

		public static bool IsEncryptCookie = false;

		public static string EncryptKeyCookie = "shcarbid";

		public static string Base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
		
		public static string privateKeyXML = @"<RSAKeyValue><Modulus>svq6x6/dKDPoWOEazxJkl554s/Wtkds68h3+qcpLBOkJQf7NCDv7/NXN+xN4UNzBajm2h7gxrLilJPMt/fEIgXqUAB3iyblfMX8KGOU8STRlmwXsIpzNPCIwN2xKA+tDcTIQLg9Se9M4xnrJrr3VQVcDV0mbih0cQ8wJX0eFj1R2lb3b/Y5IXz4pT5yiUcuhyjthL31bSKRL/PGGC1TOA+njZMySz+xyeclylfGVQO1QWS2/fasV74HB9NhTXJxMs+1GRNf6WfSaQCn9HLlJA6Wob6Gii7aUamEXHS9PKw0ri/LyQn34nmjmb21eJEbaAZtxSx5rmZHUEmjEgRKtMeRnTrhPl3Cq/av1FLpiZxKUr9bewMkzFo80LBAK0kRDTDxryFi2yz8tGw3RGrEACDq3dwsp9Ml6YELZCdjmGaGpPhDp3saZ5XsredUW+fs89bbaLnpQ+Q9uKDmOCACQE4kqXJrT70t4CJWyxIsl+N0dSH0wmVxEpt+bZMJ+77YZxjDB3cmRvFyYlbIp9AtCTUazXfC6easeNiXZ7nlFXE+vbWRSVnPWfXIzXv1YaoChfi0D15V68tFq1Vlo6jBREeD2eI8UJwwDAzAlfVaeT9ZgEZTjQnoNj33BWwDPquLZrYKjCtdkYDAfWm8V2sBZN9QUM4MvsiV+NgPIqdG5vck=</Modulus><Exponent>AQAB</Exponent><P>2HoINes4J/sehngNcvq5G/yEaBBHgVFjOG2IHjUVcZEk3JQYmKrLl9bTaBFkz6TL2uG1en5ey/ZTXSndw4HwH1AmDk7Mfgi86Aulh8Yz8i/S2vhHsvQuYexw2B3hdIzagbwYVTNZrto5blmZrAW9fILTNjREuNpUVSIIREvxkfYT/q+hvW2SngnMD6V8A0dVcCN72Rc66+ww4VaqP+SJQdLViZk8tFa+TQIJ9pqOK9M2QgUENv0ILqb02vp7pQINfxtblMdj/s9UBKVzRu16ey9M6xWWAOaWC4KTZXds+HLDMd40fTX7dIRt5Ftxprc1UPTEm5Rdcq3iZK+jAjhlmw==</P><Q>06gZ7s9eh9H7F13yjzhY+0XTwvdgkpN1jacqknGowBnemjWS6v0hlfdWFyd9nNd8CcSfa524/hZsVxdnY6s3WwQVmHOujMJAuLtPsmz5A4r5IYOuYoOI3K4ZDmf9peRPoP289Do+TJbBwKXiPtI52t1F00CqHb3ZcbiTcsJdQ9hwnuaLzZ25F5BD1dmdgFwX+LR5/wym6dHQcqXB3DZlpbPsdXgIFXRGrW8j/KGVQv/Vx+oAbSv4h02aeDjzO6gQhy3Ex2Bju7IPjP4wYqCZpgyfgvIp9R0xRRJpRxlcHPLN2IYBn3iT6TU9rMIfNzABocXloR55GEZ+KcRW1V0yaw==</Q><DP>ZwtPtTrOdFmY4jFrVXv7eVLuRArbvrbbIBUjYh2qo3CPt1/XhXRnjVN5NmRxNFYho4MlO7XDshsC+6neIh5MCtvAAbeIp8XJBjvlA4hZWcNh/1GH76tEGODPplAy7aPOr0q12nijDBRcmiaUe8oPtKEhoP+oZsWmtw+IF9p+lxgn1GgN+Xma/Rd0x03ihj0CcmqxnVjtdzfq3JjAWcGThH2zn9gNiVpxI38A9x4C5uGXRj3VD8fKiUp363SoPpeHjSWcpmlzM+vE9qE7ClwonVKarpyL1snsSnZRa08509ntCGPZYHC9zWzsUnMk2lMDUhHxNI2fHeOfjIX+diQaWQ==</DP><DQ>eMN0MHbvN82OV3HKIP+EfbIEtjxRRLCQJQeZWb8yCpF4GkKxFhTh4eAipclpeWDVq/kdANkYIRmILVT0L8EUL4EwFTEECdoX9Y8iVOqqVeQoa3/nstvb7hYDw91/svSGnpPICQ1mWGq4GAROB4wJaKBR48wEhD27YJBwyMyvTDOlZnSileoFHrW4jr7Ah+mD3+qRpP2CCu0k4xVCfDQpYRc8tnWLHzqdzP0Z6l4DfRvdzVftcFzjS8fyd9/+oC4vaAGsQhn0LbbV+ptZ3hxGLqELLjqfx/QlFYUAvolwri+3VXB4YpX4xtoS24xTJTjl5Xi1oW1yqHAoMgqNhOXNCw==</DQ><InverseQ>YHgkWidspMb72U+BgRqZ//D/Ytlm8yGra15BV3XBCNHI44LbOAdsXgf5BoDgO1yjojp26JjxpXvVKhSIFRQZg6+A9vs1zfmApADkB4IihWTQAqk5LBrO6nOosfijLwYhRvbTZvTx+OWwJe8t7ZBK+rfIZU7bkISZRL8Sp2+XrNOKwNW4c2W6AgdBgaEJ0bTzcSslyIeS3X7MFwyqPAa4a0m4pCeRLndFz5arhfOSWNHQ8TqTsHGfXQIuc492Wvrm1IL5dPs04y8TLq3RB2k3hP/d24geFDSIrRBTbY4yuy0KAUn87zYpY42kZpafWZOXFmr5d31yXa2EiYmiD6NxQg==</InverseQ><D>fy/GA5lp4+PfPSnaT0SWz1gCFoCDs57DKMM/8ZCb9wVTWt6i/TMDQwz8r2uKmIwHtmEE4TV3t3IPiU52ZHsgvEJLYjnaq9dbWmUY29ArIypk+eEfiQk8n3r8IKaHgnlxF8On09N3nh7fvcwL/m1QWnpnGWRpb6B4MYWRM3Az7ULsckTVTD4MGAKogLaictXuRzQjQAgpEloO1dUcmHjPXeCbg0oNGkzAnOMW1o8yrUz3SChGaBtoHydqmYAQ3UMFDueSg+GnOr2jz+0GvZ690CWsqIeIZxh8J4yPTGIONF3LkEw49B9oyclp1800lTr0OSgpPmSDukHzHpHs8nw/LoEORzspK7vi8ulGFQc+O3nNbhfjZt+Qlj+SakHrcIg2R6CKL0sIoa5edfILIhEuP0PqMQV78IxjbzFlkJVrBEoHEww0qZ8QzZpYzBL1xwAfG2AhDJo9+ZNYjq4aolXFqb7uQHsL6wgnutI8nWOti09qvYnU1rzfb76zYSsMAobhc9wNs9T63JsPTC3BTEIMGLIj9Llkaslq8J4idz2VpOHb5NCziMX6GMWh7g9tQOPafSmI5s4umYoCZoiLAPH9hlTj0kV9y86z+qY9SF6/pm53HR+b5gfGm8XePbpIZVYHKSQaIjn8ckTRlY6PnpGYyh9Z7T92AuizAkm6wMNVZLE=</D></RSAKeyValue>";
	}
}
