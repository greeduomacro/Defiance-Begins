using System;
using Server;
using Server.Gumps;

namespace Server.Multis
{
	public class ButtonPlus : GumpButton
	{
		private string m_Name;
		private object m_Callback;
		private object m_Param;

		public string Name{ get{ return m_Name; } }

		public ButtonPlus( int x, int y, int normalID, int pressedID, int buttonID, string name, GumpCallback back ) : base( x, y, normalID, pressedID, buttonID, GumpButtonType.Reply, 0 )
		{
			m_Name = name;
			m_Callback = back;
			m_Param = "";
		}

		public ButtonPlus( int x, int y, int normalID, int pressedID, int buttonID, string name, GumpStateCallback back, object param ) : base( x, y, normalID, pressedID, buttonID, GumpButtonType.Reply, 0 )
		{
			m_Name = name;
			m_Callback = back;
			m_Param = param;
		}

		public void Invoke()
		{
			if ( m_Callback is GumpCallback )
				((GumpCallback)m_Callback)();
			else if ( m_Callback is GumpStateCallback )
				((GumpStateCallback)m_Callback)( m_Param );
		}
	}
}