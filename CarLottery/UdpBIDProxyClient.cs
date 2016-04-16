using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;

namespace CarBidWebClient
{
	public class UdpBIDProxyClient
	{
		private Thread _proxyThread;

		private BIDContext _ctx;

		public UdpBIDProxyClient(BIDContext ctx)
		{
			this._ctx = ctx;
			this._proxyThread = new Thread(new ThreadStart(this.ProxyThread));
			this._proxyThread.IsBackground = true;
			this._proxyThread.Start();
		}

		public void Stop()
		{
			if (this._proxyThread != null)
			{
				this._proxyThread.Abort();
				this._proxyThread = null;
			}
		}

		public void ProxyThread()
		{
			while (true)
			{
				UdpClient udpClient = null;
                try
                {
                    // Automatically assigned local IPv4 port
                    udpClient = new UdpClient(0);
                    IPEndPoint epUdpServer = new IPEndPoint(IPAddress.Parse(BIDWebConfig.yayaServer), 990);
					UdpState state = new UdpState(udpClient, epUdpServer);
                    
                    // UdpClient is not verify the validity of addresses
                    udpClient.Connect(epUdpServer);
                    udpClient.BeginReceive(new AsyncCallback(DataReceived), state);
                    StringBuilder msgSendStringBuilder = new StringBuilder();
                    StringBuilder msgHelloBuilder = new StringBuilder();
                    msgHelloBuilder.Append("<TYPE>CLIENT</TYPE><BIDNO>");
                    msgHelloBuilder.Append(this._ctx.BIDNumer);
                    msgHelloBuilder.Append("</BIDNO>");
                    msgHelloBuilder.Append("<VCODE>123456</VCODE>");
                    byte[] msgHello = System.Text.Encoding.Default.GetBytes(msgHelloBuilder.ToString());
                    while (true)
                    {
                        // We Must Send Hello Message First
                        udpClient.Send(msgHello, msgHello.Length);
                        Thread.Sleep(100);

                        // Send Price Update Message
                        msgSendStringBuilder.Append("<TYPE>UPDATE</TYPE><INFO>");
                        
                        // 显然此处的发送时间不应该再是本地的时间了：
                        msgSendStringBuilder.Append(this._ctx.UpdateTime.ToString("HH:mm:ss"));
                        msgSendStringBuilder.Append("^");
                        msgSendStringBuilder.Append(this._ctx.BaseAmount);
                        msgSendStringBuilder.Append("</INFO>");
                        byte[] msgSend = System.Text.Encoding.Default.GetBytes(msgSendStringBuilder.ToString());
                        udpClient.Send(msgSend, msgSend.Length);
                        msgSendStringBuilder.Clear();
                        Thread.Sleep(200);
                    }
                }
                catch (SocketException ex) 
                {
                    BIDLogger.GetLogger(null).Append(ex.ToString());
                }
                catch (Exception ex)
                {
                    BIDLogger.GetLogger(null).Append(ex.ToString());
                }
				finally
				{
					if (udpClient != null)
					{
						try
						{
							udpClient.Close();
						}
						catch
						{
						}
					}
				}
				Thread.Sleep(500);
			}
		}
		private void DataReceived(IAsyncResult ar)
        {
            UdpClient c = (UdpClient)((UdpState)ar.AsyncState).c;
            IPEndPoint wantedIpEndPoint = (IPEndPoint)((UdpState)(ar.AsyncState)).e;
            IPEndPoint receivedIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Byte[] receiveBytes = System.Text.Encoding.Default.GetBytes("".ToString());
            Regex regexMyBidInfo = new Regex(@"<MYBIDINFO>(.*)</MYBIDINFO>");
            try
            {
            	// parse UDP packages and update the price and tiem in BIDContext
                receiveBytes = c.EndReceive(ar, ref receivedIpEndPoint);
                string recvInfo = ASCIIEncoding.ASCII.GetString(receiveBytes);
                if (regexMyBidInfo.IsMatch(recvInfo))
                {
                	var myBidInfo = regexMyBidInfo.Match(recvInfo).Groups[1].ToString();
                	string[] myBidInfoArray = myBidInfo.Split(new Char[]{'^'});
                	DateTime dtUpdate = Convert.ToDateTime(myBidInfoArray[0]);
                	int price = Convert.ToInt32(myBidInfoArray[1]);

                    // 收到一个udp包就去更新 这样时间就能够保持一致
                    this._ctx.UpdateState(price, dtUpdate);
                    BIDLogger.GetLogger(null).Append("It is a new info：" + recvInfo);
                    //if (price > this._ctx.BaseAmount)
                    //{
                    //    this._ctx.UpdateState(price, dtUpdate);
                    //    BIDLogger.GetLogger(null).Append("It is a new info：" + recvInfo);
                    //}
                    //else
                    //{
                    //    BIDLogger.GetLogger(null).Append("It is an old info：" + recvInfo);
                    //}
                }    
            }
            catch (Exception)
            {
                //c.Close();
                //c.Client.Close();
                // 点多次开始拍卖此处有坑 需要做null的判定
                if (c != null && c.Client != null)
                	c.Client.Shutdown(SocketShutdown.Both);
                return;
            }

            // Check sender
            bool isRightHost = (wantedIpEndPoint.Address.Equals(receivedIpEndPoint.Address)) || wantedIpEndPoint.Address.Equals(IPAddress.Any);
            bool isRightPort = (wantedIpEndPoint.Port == receivedIpEndPoint.Port) || wantedIpEndPoint.Port == 0;
            if (isRightHost && isRightPort)
            {
                // Convert data to ASCII and print in console
                string receivedText = ASCIIEncoding.ASCII.GetString(receiveBytes);
            }

            // Restart listening for udp data packages
			c.BeginReceive(new AsyncCallback(DataReceived), ar.AsyncState);

        }
	}
}
