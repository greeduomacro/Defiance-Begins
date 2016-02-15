using System;
using Server;
using System.Collections.Generic;

namespace Server.Items
{
	public class VampireCorpse : Corpse
	{
		private bool m_Staked;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Staked
		{
			get{ return m_Staked; }
			set{ m_Staked = value; }
		}

		public VampireCorpse( Mobile owner, HairInfo hair, FacialHairInfo facialhair, List<Item> equipItems ) : base( owner, hair, facialhair, equipItems )
		{
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
				list.Add( "a vampire corpse" );
		}

		public override void Open( Mobile from, bool checkSelfLoot )
		{
			if ( m_Staked )
				//from.SendMessage(  ); //You cannot loot from what is not dead.
			//else
				base.Open( from, checkSelfLoot );
		}

		public override void Carve( Mobile from, Item item )
		{
			Mobile dead = Owner;

			if ( Staked )
				base.Carve( from, item );
			else
				from.SendLocalizedMessage( 600278 );
		}

		public VampireCorpse( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}