using System;
using Server;
using Server.Items;

namespace Server.Multis
{
	public class RentalLicense : Item
	{
		private Mobile m_Owner;

		public Mobile Owner{ get{ return m_Owner; } set{ m_Owner = value; InvalidateProperties(); } }

		public RentalLicense() : base( 0x14F0 )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			if ( m_Owner != null )
				list.Add( "a renter's license belonging to " + m_Owner.Name );
			else
				list.Add( "a renter's license" );
		}

		public override void OnDoubleClick( Mobile m )
		{
			if ( m_Owner == null )
				m_Owner = m;
		}

		public RentalLicense( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_Owner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Owner = reader.ReadMobile();
		}
	}
}