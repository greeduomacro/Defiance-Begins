using System;
using System.Collections;
using Server.Network;

namespace Scripts.Engines.Encryption
{
	public class LoginEncryption : IClientEncryption
	{
		private string m_Name;
		private uint m_Key1;
		private uint m_Key2;
		private uint m_Table1;
		private uint m_Table2;

		public string Name{ get{ return m_Name; } }
		public uint Key1{ get{ return m_Key1; } }
		public uint Key2{ get{ return m_Key2; } }

		// Try to initialize the login encryption with the given buffer
		public bool Initialize( uint seed, byte[] buffer, int offset, int length, NetState ns )
		{
			if (length < 62) // includes the 4 byte header
				return false;

			// Try to find a valid key
			byte[] packet = new byte[62];

			// Initialize our tables (cache them, they will be modified)
			uint orgTable1 = ( ( ( ~seed ) ^ 0x00001357 ) << 16 ) | ( ( seed ^ 0xffffaaaa ) & 0x0000ffff );
			uint orgTable2 = ( ( seed ^ 0x43210000 ) >> 16 ) | ( ( ( ~seed ) ^ 0xabcdffff ) & 0xffff0000 );

			for (int i = 0; i < LoginKey.Keys.Length; ++i)
			{
				// Check if this key works on this packet
				Buffer.BlockCopy( buffer, offset, packet, 0, 62 );
				m_Table1 = orgTable1;
				m_Table2 = orgTable2;
				m_Key1 = LoginKey.Keys[i].Key1;
				m_Key2 = LoginKey.Keys[i].Key2;

				ClientDecrypt( ref packet, packet.Length );

				// Check if it decrypted correctly
				if ( packet[0] == 0x80 && packet[30] == 0x00 && packet[60] == 0x00 )
				{
					// Reestablish our current state
					m_Table1 = orgTable1;
					m_Table2 = orgTable2;
					m_Key1 = LoginKey.Keys[i].Key1;
					m_Key2 = LoginKey.Keys[i].Key2;
					m_Name = LoginKey.Keys[i].Name;
					return true;
				}
			}

			return false;
		}

		public void Initialize( uint seed, uint key1, uint key2 )
		{
			m_Table1 = ( ( ( ~seed ) ^ 0x00001357 ) << 16 ) | ( ( seed ^ 0xffffaaaa ) & 0x0000ffff );
			m_Table2 = ( ( seed ^ 0x43210000 ) >> 16 ) | ( ( ( ~seed ) ^ 0xabcdffff ) & 0xffff0000 );
			m_Key1 = key1;
			m_Key2 = key2;
		}

		public void ServerEncrypt( ref byte[] buffer, int length )
		{
			// There is no server->client encryption in the login stage
		}

		public void ClientDecrypt( ref byte[] buffer, int length )
		{
			uint eax, ecx, edx, esi;
			for ( int i = 0; i < length; ++i )
			{
				buffer[i] = (byte)(buffer[i] ^ (byte) ( m_Table1 & 0xFF ));
				edx = m_Table2;
				esi = m_Table1 << 31;
				eax = m_Table2 >> 1;
				eax |= esi;
				eax ^= m_Key1 - 1;
				edx <<= 31;
				eax >>= 1;
				ecx = m_Table1 >> 1;
				eax |= esi;
				ecx |= edx;
				eax ^= m_Key1;
				ecx ^= m_Key2;
				m_Table1 = ecx;
				m_Table2 = eax;
			}
		}
	}
}
