using System;
using System.IO;
using System.Net.Sockets;

namespace PlaymoreClient.Proxy
{
	public interface IProxyHost
	{
		Stream GetStream(TcpClient tcp);

		void OnConnect(ProxyClient sender);

		void OnException(ProxyClient sender, Exception ex);

		void OnReceive(ProxyClient sender, byte[] buffer, int idx, int len);

		void OnSend(ProxyClient sender, byte[] buffer, int idx, int len);
	}
}