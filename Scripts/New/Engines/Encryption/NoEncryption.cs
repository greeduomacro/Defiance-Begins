namespace Scripts.Engines.Encryption
{
	public class NoEncryption : IClientEncryption
	{
		public void ServerEncrypt(ref byte[] buffer, int length)
		{
		}

		public void ClientDecrypt(ref byte[] buffer, int length)
		{
		}
	}
}
