using System;
using Server;
using Server.Gumps;

namespace Server.Multis
{
	public class InfoGump : GumpPlusLight
	{
		private int m_Width, m_Height;
		private string m_Text;
		private bool m_Scroll;

		public InfoGump( Mobile m, int width, int height, string text, bool scroll ) : base( m, 100, 100 )
		{
			m_Width = width;
			m_Height = height;
			m_Text= text;
			m_Scroll = scroll;

			NewGump();
		}

		protected override void BuildGump()
		{
			AddBackground( 0, 0, m_Width, m_Height, 0x13BE );

			AddHtml( 20, 20, m_Width-40, m_Height-40, HTML.White + m_Text, false, m_Scroll );
		}
	}
}