using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace PlaymoreClient.Proxy
{
	public class SecureProxyHost : ProxyHost
	{
		public X509Certificate Certificate
		{
			get;
			protected set;
		}

		public SecureProxyHost(int srcport, string remote, int remoteport, X509Certificate cert) : base(srcport, remote, remoteport)
		{
			this.Certificate = cert;
		}

		private bool AcceptAllCertificates(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		public override Stream GetStream(TcpClient tcp)
		{
			SslStream sslStream = new SslStream(base.GetStream(tcp), false, new RemoteCertificateValidationCallback(this.AcceptAllCertificates));
			sslStream.ReadTimeout = 50000;
			sslStream.WriteTimeout = 50000;
			return sslStream;
		}

		public override void OnConnect(ProxyClient sender)
		{
			SslStream sourceStream = (SslStream)sender.SourceStream;
			SslStream remoteStream = (SslStream)sender.RemoteStream;
			sourceStream.AuthenticateAsServer(this.Certificate, false, SslProtocols.Default, false);
			remoteStream.AuthenticateAsClient(base.RemoteAddress);
			base.OnConnect(sender);
		}
	}
}