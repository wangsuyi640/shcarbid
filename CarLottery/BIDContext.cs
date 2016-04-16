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
						
		// ��֤������� �Ƿ��˻س�
		public bool BIDAimedFlag;   
		
		// 4���ؼ���ʱ���
        public DateTime bidStartTime;
        
        public DateTime bidMidTime;
        
        public DateTime bidAimedTime;
        
        public DateTime bidEndTime;
		
		// �Ƿ��Ѿ��Զ��������֤��
		public bool imageReqFlag;

        public bool bidCompleteFlag;

        // ȷ��������֤��
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
