using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Server
{
	[Parsable]
	public class BaseInstanceMap : Map, IComparable<BaseInstanceMap>, ISerializable
	{
		internal int m_TypeRef;
		int ISerializable.TypeReference{ get{ return m_TypeRef; } }
		int ISerializable.SerialIdentity{ get{ return Serial; } }

		private bool m_Deleted;
		public bool Deleted{ get{ return m_Deleted; } set{ m_Deleted = value; } }

		public Serial Serial{ get{ return MapIndex; } set{ MapIndex = value; } }
		public virtual bool Decays{ get{ return false; } }

		//private Point3D m_EjectPoint; //Where do all the Players go?
		//public Point3D EjectPoint{ get{ return m_EjectPoint; } set{ m_EjectPoint = value; } }

		public BaseInstanceMap( Map model, string name, MapRules rules ) : this( model.MapID, model.FileIndex, model.Width, model.Height, model.Season, name, rules )
		{
		}

		public BaseInstanceMap( int mapID, int fileIndex, int width, int height, int season, string name, MapRules rules ) : base( mapID, Serial.NewMap, fileIndex, width, height, season, name, rules )
		{
			Type ourType = this.GetType();
			m_TypeRef = World.m_InstanceMapTypes.IndexOf( ourType );

			if ( m_TypeRef == -1 )
			{
				World.m_InstanceMapTypes.Add( ourType );
				m_TypeRef = World.m_InstanceMapTypes.Count - 1;
			}

			Register();
		}

		public BaseInstanceMap( Serial serial ) : base( serial )
		{
			Type ourType = this.GetType();
			m_TypeRef = World.m_InstanceMapTypes.IndexOf( ourType );

			if ( m_TypeRef == -1 )
			{
				World.m_InstanceMapTypes.Add( ourType );
				m_TypeRef = World.m_InstanceMapTypes.Count - 1;
			}

			Register();
		}

		public virtual void EjectPlayers()
		{
			foreach ( Mobile m in World.Mobiles.Values )
				if ( m.Player && m.Map == this )
					m.Map = Map.Maps[MapID];
		}

		public virtual void Register()
		{
			Map.AllMaps.Add( this );
			World.AddMap( this );
		}

		public virtual void Delete()
		{
			if ( m_Deleted || !World.OnDelete( this ) )
				return;

			Map.AllMaps.Remove( this );

			EjectPlayers();

			OnDelete();

			m_Deleted = true;
			World.RemoveMap( this );

			OnAfterDelete();
		}

		public virtual bool OnDecay()
		{
			return true;
		}

		public virtual void OnDelete()
		{
		}

		public virtual void OnAfterDelete()
		{
		}

		public virtual void Serialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( 0 ); // version

			writer.Write( MapID );
			//writer.Write( MapIndex ); - serialized in idx file
			writer.Write( FileIndex );
			writer.Write( Width );
			writer.Write( Height );
			writer.Write( Season );
			writer.Write( Name );
			writer.Write( (int)Rules );
		}

		public virtual void Deserialize( GenericReader reader )
		{
			int version = reader.ReadEncodedInt();

			MapID = reader.ReadInt();
			//MapIndex = reader.ReadInt(); - deserialized in ctor
			FileIndex = reader.ReadInt();
			Width = reader.ReadInt();
			Height = reader.ReadInt();
			Season = reader.ReadInt();
			Name = reader.ReadString();
			Rules = (MapRules)reader.ReadInt();

			InitializeSectors();
		}

		public int CompareTo( BaseInstanceMap other )
		{
			if ( other == null )
				return -1;

			return Serial.CompareTo( other.Serial );
		}

		public override string ToString()
		{
			return String.Format( "{0}-{1:D}", Name, MapIndex );
		}
	}
}