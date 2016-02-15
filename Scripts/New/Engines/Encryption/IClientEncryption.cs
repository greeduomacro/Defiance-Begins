namespace Scripts.Engines.Encryption
{
	public interface IClientEncryption
	{
		// Encrypt outgoing data
		void ServerEncrypt(ref byte[] buffer, int length);

		// Decrypt incoming data
		void ClientDecrypt(ref byte[] buffer, int length);
	}
}
