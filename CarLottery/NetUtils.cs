using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace CarBidWebClient
{
	public class NetUtils
	{
		public static bool IsPortAvailableTcp(int port)
		{
			IPGlobalProperties iPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
			HashSet<int> hashSet = new HashSet<int>();
			IPEndPoint[] activeTcpListeners = iPGlobalProperties.GetActiveTcpListeners();
			for (int i = 0; i < activeTcpListeners.Length; i++)
			{
				IPEndPoint iPEndPoint = activeTcpListeners[i];
				hashSet.Add(iPEndPoint.Port);
			}
			TcpConnectionInformation[] activeTcpConnections = iPGlobalProperties.GetActiveTcpConnections();
			for (int j = 0; j < activeTcpConnections.Length; j++)
			{
				TcpConnectionInformation tcpConnectionInformation = activeTcpConnections[j];
				hashSet.Add(tcpConnectionInformation.LocalEndPoint.Port);
			}
			return hashSet.Contains(port);
		}

		public static int GetNextAvailablePortTcp(int start, int end)
		{
			IPGlobalProperties iPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
			HashSet<int> hashSet = new HashSet<int>();
			IPEndPoint[] activeTcpListeners = iPGlobalProperties.GetActiveTcpListeners();
			for (int i = 0; i < activeTcpListeners.Length; i++)
			{
				IPEndPoint iPEndPoint = activeTcpListeners[i];
				hashSet.Add(iPEndPoint.Port);
			}
			TcpConnectionInformation[] activeTcpConnections = iPGlobalProperties.GetActiveTcpConnections();
			for (int j = 0; j < activeTcpConnections.Length; j++)
			{
				TcpConnectionInformation tcpConnectionInformation = activeTcpConnections[j];
				hashSet.Add(tcpConnectionInformation.LocalEndPoint.Port);
			}
			for (int k = start; k <= end; k++)
			{
				if (!hashSet.Contains(k))
				{
					return k;
				}
			}
			return -1;
		}
	}
}
