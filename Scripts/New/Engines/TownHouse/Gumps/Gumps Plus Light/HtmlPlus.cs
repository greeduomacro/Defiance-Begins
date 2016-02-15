using System;
using Server;
using Server.Gumps;

namespace Server.Multis
{
	public class HtmlPlus : GumpHtml
	{
		private bool m_Override;

		public bool Override{ get{ return m_Override; } set{ m_Override = value; } }

		public HtmlPlus( int x, int y, int width, int height, string text, bool back, bool scroll ) : base( x, y, width, height, text, back, scroll )
		{
			m_Override = true;
		}

		public HtmlPlus( int x, int y, int width, int height, string text, bool back, bool scroll, bool over ) : base( x, y, width, height, text, back, scroll )
		{
			m_Override = over;
		}
	}
}