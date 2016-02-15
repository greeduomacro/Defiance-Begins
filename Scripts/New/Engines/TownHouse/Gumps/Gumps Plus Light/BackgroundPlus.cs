using System;
using Server;
using Server.Gumps;

namespace Server.Multis
{
	public class BackgroundPlus : GumpBackground
	{
		private bool m_Override;

		public bool Override{ get{ return m_Override; } set{ m_Override = value; } }

		public BackgroundPlus( int x, int y, int width, int height, int back ) : base( x, y, width, height, back )
		{
			m_Override = true;
		}

		public BackgroundPlus( int x, int y, int width, int height, int back, bool over ) : base( x, y, width, height, back )
		{
			m_Override = over;
		}
	}
}