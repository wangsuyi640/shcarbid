using System;

namespace CarBidWebClient
{	
	public class BIDContext
	{
		public int BaseAmount = 82600;

		public DateTime UpdateTime = new DateTime(0L);

		public string Cookie;

		public int BIDNumer;
		
		/// <summary>
		///  START
		/// </summary>
		// for license feature
		public int BIDNumberMore;
						
		// 验证码输入后 是否按了回车
		public bool BIDAimedFlag;   
		
		// 4个关键的时间点
        public DateTime bidStartTime;
        
        public DateTime bidMidTime;
        
        public DateTime bidAimedTime;
        
        public DateTime bidEndTime;
		
		// 是否已经自动申请过验证码
		public bool imageReqFlag;

        public bool bidCompleteFlag;

        // 确认输完验证码
        public bool bidAllowFlag;
		/// <summary>
		///  END
		/// </summary>

		public string ClientID;

		public int BIDAmount;

        public int AddBIDAmount;   

        public int AheadBIDAmount;  

		public void UpdateState(int price, DateTime time)
		{
				lock (this)
				{
					this.BaseAmount = price;
					this.UpdateTime = time;
				}
		}
	}
}
