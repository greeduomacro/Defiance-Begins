using System;
using System.Collections;
using System.Security.Cryptography;

namespace Scripts.Engines.Encryption
{
	public class GameEncryption : IClientEncryption
	{
		private TwofishEncryption m_Engine;

		private ushort m_RecvPos; // Position in our CipherTable (Recv)
		private byte m_SendPos; // Offset in our XOR Table (Send)
		private byte[] m_CipherTable;
		private byte[] m_XorData; // This table is used for encrypting the server->client stream

		public GameEncryption( uint seed )
		{
			m_CipherTable = new byte[0x100];

			// Set up the crypt key
			byte[] key = new byte[16];
			key[0] = key[4] = key[8] = key[12] = (byte)((seed >> 24) & 0xff);
			key[1] = key[5] = key[9] = key[13] = (byte)((seed >> 16) & 0xff);
			key[2] = key[6] = key[10] = key[14] = (byte)((seed >> 8) & 0xff);
			key[3] = key[7] = key[11] = key[15] = (byte)(seed & 0xff);

			byte[] iv = new byte[0];
			m_Engine = new TwofishEncryption( 128, ref key, ref iv, CipherMode.ECB, TwofishBase.EncryptionDirection.Decrypting );

			// Initialize table
			for ( int i = 0; i < 256; ++i )
				m_CipherTable[i] = (byte)i;

			m_SendPos = 0;

			// We need to fill the table initially to calculate the MD5 hash of it
			RefreshCipherTable();

			// Create a MD5 hash of the twofish crypt data and use it as a 16-byte xor table
			// for encrypting the server->client stream.
			MD5 md5 = new MD5CryptoServiceProvider();
			m_XorData = md5.ComputeHash( m_CipherTable );
		}

		private void RefreshCipherTable()
		{
			uint[] block = new uint[4];

			for ( int i = 0; i < 256; i += 16 )
			{
				Buffer.BlockCopy( m_CipherTable, i, block, 0, 16 );
				m_Engine.blockEncrypt( ref block );
				Buffer.BlockCopy( block, 0, m_CipherTable, i, 16 );
			}

			m_RecvPos = 0;
		}

		public void ServerEncrypt( ref byte[] buffer, int length )
		{
			byte[] packet = new byte[length];
			for ( int i = 0; i < length; ++i )
			{
				packet[i] = (byte)(buffer[i] ^ m_XorData[m_SendPos++]);
				m_SendPos &= 0x0F; // Maximum Value is 0xF = 15, then 0xF + 1 = 0 again
			}
			buffer = packet;
		}

		public void ClientDecrypt( ref byte[] buffer, int length )
		{
			for ( int i = 0; i < length; ++i )
			{
				// Recalculate table
				if ( m_RecvPos >= 0x100 )
				{
					byte[] tmpBuffer = new byte[0x100];
					RefreshCipherTable();
				}

				// Simple XOR operation
				buffer[i] ^= m_CipherTable[m_RecvPos++];
			}
		}
	}
}
