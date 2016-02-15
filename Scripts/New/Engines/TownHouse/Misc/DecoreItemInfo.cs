using System;
using Server;

namespace Server.Multis
{
	public class DecoreItemInfo
	{
		private string m_TypeString;
		private string m_Name;
		private int m_ItemID;
		private int m_Hue;
		private Point3D m_Location;
		private Map m_Map;

		public string TypeString{ get{ return m_TypeString; } }
		public string Name{ get{ return m_Name; } }
		public int ItemID{ get{ return m_ItemID; } }
		public int Hue{ get{ return m_Hue; } }
		public Point3D Location{ get{ return m_Location; } }
		public Map Map{ get{ return m_Map; } }

		public DecoreItemInfo()
		{
		}

		public DecoreItemInfo( string typestring, string name, int itemid, int hue, Point3D loc, Map map )
		{
			m_TypeString = typestring;
			m_ItemID = itemid;
			m_Location = loc;
			m_Map = map;
		}

		public void Save( GenericWriter writer )
		{
			writer.Write( (int)1 ); // Version

			// Version 1
			writer.Write( m_Hue );
			writer.Write( m_Name );

			writer.Write( m_TypeString );
			writer.Write( m_ItemID );
			writer.Write( m_Location );
			writer.Write( m_Map );
		}

		public void Load( GenericReader reader )
		{
			int version = reader.ReadInt();

			if ( version >= 1 )
			{
				m_Hue = reader.ReadInt();
				m_Name = reader.ReadString();
			}

			m_TypeString = reader.ReadString();
			m_ItemID = reader.ReadInt();
			m_Location = reader.ReadPoint3D();
			m_Map = reader.ReadMap();
		}
	}
}