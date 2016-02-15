using System;

namespace Scripts.Engines.Encryption
{
	public class LoginKey
	{
		private uint m_Key1;
		private uint m_Key2;
		private string m_Name;

		public uint Key1{ get{ return m_Key1; } }
		public uint Key2{ get{ return m_Key2; } }
		public string Name{ get{ return m_Name; } }

		public LoginKey( string name, uint key1, uint key2 )
		{
			m_Key1 = key1;
			m_Key2 = key2;
			m_Name = name;
		}

		public static LoginKey[] Keys = new LoginKey[]
		{
			new LoginKey("7.0.23", 0x2A9F868D, 0xA0437E7F),
			new LoginKey("7.0.22", 0x2B406C9D, 0xA0A1227F),
			new LoginKey("7.0.21", 0x2B08D6AD, 0xA0875E7F),
			new LoginKey("7.0.20", 0x2BF084BD, 0xA0FD127F),
			new LoginKey("7.0.19", 0x2BB976CD, 0xA0DBDE7F),
			new LoginKey("7.0.18", 0x2C612CDD, 0xA328227F ),
			new LoginKey("7.0.17", 0x2C29E6ED, 0xA30EFE7F ),
			new LoginKey("7.0.16", 0x2C11A4FD, 0xA313527F ),
			new LoginKey("7.0.15", 0x2CDA670D, 0xA3723E7F ),
			new LoginKey("7.0.14", 0x2C822D1D, 0xA35DA27F ),
			new LoginKey("7.0.13", 0x2D4AF72D, 0xA3B71E7F ),
			new LoginKey("7.0.12", 0x2D32853D, 0xA38A127F ),
			new LoginKey("7.0.11", 0x2FABA7ED , 0xA2C17E7F ),
			new LoginKey("7.0.10", 0x1f9c9575, 0x1bd26d6b ),
			new LoginKey("7.0.11", 0x2FABA7ED, 0xA2C17E7F),
			new LoginKey("7.0.6.5", 0x2EC3ED9D, 0xA274227F),
			new LoginKey("7.0.4", 0x2FABA7ED, 0xA2C17E7F),
			new LoginKey("7.0.3", 0x2FABA7ED, 0xA2C17E7F),
			new LoginKey("7.0.2", 0x2FABA7ED, 0xA2C17E7F),
			new LoginKey("7.0.1.1", 0x2FABA7ED, 0xA2C17E7F),
			new LoginKey("7.0.1", 0x2FABA7ED , 0xA2C17E7F ),
			new LoginKey("7.0.0", 0x2F93A5FD , 0xA2DD527F ),
			new LoginKey("6.0.14", 0x2C022D1D, 0xA31DA27F ),
			new LoginKey("6.0.13", 0x2DCAF72D, 0xA3F71E7F ),
			//new LoginKey("6.0.12", 0x2DB2853D, 0xA3CA127F ),
			//new LoginKey("6.0.11", 0x2D7B574D, 0xA3AD9E7F ),
			//new LoginKey("6.0.10", 0x2D236D5D, 0xA380A27F ),
			//new LoginKey("6.0.9", 0x2EEB076D, 0xA263BE7F ),
			//new LoginKey("6.0.8", 0x2ED3257D, 0xA27F527F ),
			//new LoginKey("6.0.7", 0x2E9BC78D, 0xA25BFE7F ),
			//new LoginKey("6.0.6", 0x2E43ED9D, 0xA234227F ),
			//new LoginKey("6.0.5", 0x2E0B97AD, 0xA210DE7F ),
			//new LoginKey("6.0.4", 0x2FF385BD, 0xA2ED127F ),
			//new LoginKey("6.0.3", 0x2FBBB7CD, 0xA2C95E7F ),
			//new LoginKey("6.0.2", 0x2F63ADDD, 0xA2A5227F),
			//new LoginKey("6.0.1", 0x2F2BA7ED, 0xA2817E7F ),
			//new LoginKey("6.0.0", 0x2f13a5fd, 0xa29d527f ),
			//new LoginKey("5.0.9", 0x2F6B076D, 0xA2A3BE7F ),
			//new LoginKey("5.0.8", 0x2F53257D, 0xA2BF527F ),
			//new LoginKey("5.0.7", 0x10140441, 0xA29BFE7F ),
			//new LoginKey("5.0.6", 0x2fc3ed9c, 0xa2f4227f ),
			//new LoginKey("5.0.5", 0x2f8b97ac, 0xa2d0de7f ),
			//new LoginKey("5.0.4", 0x2e7385bc, 0xa22d127f ),
			//new LoginKey("5.0.3", 0x2e3bb7cc, 0xa2095e7f ),
			//new LoginKey("5.0.2", 0x2ee3addc, 0xa265227f ),
			//new LoginKey("5.0.1", 0x2eaba7ec, 0xa2417e7f ),
			//new LoginKey("5.0.0", 0x2E93A5FC, 0xA25D527F ),
			//new LoginKey("4.0.11", 0x2C7B574C, 0xA32D9E7F )
		};
	}
}
