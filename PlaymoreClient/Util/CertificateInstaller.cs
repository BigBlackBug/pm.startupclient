using NotMissing.Logging;
using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace PlaymoreClient.Util
{
	public class CertificateInstaller
	{
		public X509Certificate2[] Certificates
		{
			get;
			set;
		}

		public bool IsInstalled
		{
			get
			{
				bool flag;
				try
				{
					X509Store x509Store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
					x509Store.Open(OpenFlags.MaxAllowed);
					X509Certificate2[] certificates = this.Certificates;
					int num = 0;
					while (num < (int)certificates.Length)
					{
						X509Certificate2 x509Certificate2 = certificates[num];
						if (x509Store.Certificates.Contains(x509Certificate2))
						{
							num++;
						}
						else
						{
							flag = false;
							return flag;
						}
					}
					x509Store.Close();
					flag = true;
				}
				catch (SecurityException securityException)
				{
					PlaymoreClient.Gui.MainForm.LOGGER.Warn(securityException);
					flag = false;
				}
				return flag;
			}
		}

		public CertificateInstaller(X509Certificate2[] certs)
		{
			this.Certificates = certs;
		}

		public bool Install()
		{
			try
			{
				X509Store x509Store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
				x509Store.Open(OpenFlags.MaxAllowed);
				X509Certificate2[] certificates = this.Certificates;
				for (int i = 0; i < (int)certificates.Length; i++)
				{
					X509Certificate2 x509Certificate2 = certificates[i];
					if (!x509Store.Certificates.Contains(x509Certificate2))
					{
						x509Store.Add(x509Certificate2);
					}
				}
				x509Store.Close();
				return true;
			}
			catch (SecurityException securityException)
			{
				PlaymoreClient.Gui.MainForm.LOGGER.Warn(securityException);
			}
			catch (Exception exception)
			{
				PlaymoreClient.Gui.MainForm.LOGGER.Error(string.Concat("Failed to install ", exception));
			}
			return false;
		}

		public bool Uninstall()
		{
			try
			{
				X509Store x509Store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
				x509Store.Open(OpenFlags.MaxAllowed);
				X509Certificate2[] certificates = this.Certificates;
				for (int i = 0; i < (int)certificates.Length; i++)
				{
					X509Certificate2 x509Certificate2 = certificates[i];
					if (x509Store.Certificates.Contains(x509Certificate2))
					{
						x509Store.Remove(x509Certificate2);
					}
				}
				x509Store.Close();
				return true;
			}
			catch (SecurityException securityException)
			{
				PlaymoreClient.Gui.MainForm.LOGGER.Warn(securityException);
			}
			catch (Exception exception)
			{
				PlaymoreClient.Gui.MainForm.LOGGER.Error(string.Concat("Failed to uninstall ", exception));
			}
			return false;
		}
	}
}