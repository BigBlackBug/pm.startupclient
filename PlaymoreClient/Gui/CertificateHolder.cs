using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace PlaymoreClient.Gui
{
	public class CertificateHolder
	{
		public X509Certificate2 Certificate
		{
			get;
			set;
		}

		public string Domain
		{
			get;
			set;
		}

		public CertificateHolder(string domain, byte[] bytes)
		{
			this.Domain = domain;
			this.Certificate = new X509Certificate2(bytes, "");
		}
	}
}