using System;
using Server;
using Server.Spells;
using Server.Network;

namespace Server.Items
{
	[Flipable( 0x125E, 0x1230 )]
	public class GuillotineComponent : AddonComponent
	{
		public override int LabelNumber { get { return 1024656; } } // Guillotine

		public GuillotineComponent() : base( 0x125E )
		{
		}

		public GuillotineComponent( Serial serial ) : base( serial )
		{
		}

		public void Activate( Mobile from )
		{
			if ( ItemID == 0x125E )
				ItemID = 0x1269;
			else
				ItemID = 0x1247;

			// blood
			int amount = Utility.RandomMinMax( 3, 7 );

			for ( int i = 0; i < amount; i++ )
			{
				int x = X + Utility.RandomMinMax( -1, 1 );
				int y = Y + Utility.RandomMinMax( -1, 1 );
				int z = Z;

				if ( !Map.CanFit( x, y, z, 1, false, false, true ) )
				{
					z = Map.GetAverageZ( x, y );

					if ( !Map.CanFit( x, y, z, 1, false, false, true ) )
						continue;
				}

				Blood blood = new Blood( Utility.RandomMinMax( 0x122C, 0x122F ) );
				blood.MoveToWorld( new Point3D( x, y, z ), Map );
			}

			if ( from.Female )
				from.PlaySound( Utility.RandomMinMax( 0x150, 0x153 ) );
			else
				from.PlaySound( Utility.RandomMinMax( 0x15A, 0x15D ) );

			from.LocalOverheadMessage( MessageType.Regular, 0, 501777 ); // Hmm... you suspect that if you used this again, it might hurt.
			SpellHelper.Damage( TimeSpan.Zero, from, Utility.Dice( 2, 10, 5 ) );

			Timer.DelayCall( TimeSpan.FromSeconds( 0.5 ), TimeSpan.FromSeconds( 0.5 ), 2, new TimerCallback( Deactivate ) );
		}

		private void Deactivate()
		{
			if ( ItemID == 0x1269 )
				ItemID = 0x1260;
			else if ( ItemID == 0x1260 )
				ItemID = 0x125E;
			else if ( ItemID == 0x1247 )
				ItemID = 0x1246;
			else if ( ItemID == 0x1246 )
				ItemID = 0x1230;
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

	public class GuillotineAddon : BaseAddon
	{
		public override BaseAddonDeed Deed { get { return new GuillotineDeed(); } }

		public GuillotineAddon() : base()
		{
			AddComponent( new GuillotineComponent(), 0, 0, 0 );
		}

		public GuillotineAddon( Serial serial ) : base( serial )
		{
		}

		public override void OnComponentUsed( AddonComponent c, Mobile from )
		{
			if ( from.InRange( Location, 2 ) )
			{
				if ( Utility.RandomBool() )
				{
					from.Location = Location;

					if ( c is GuillotineComponent )
						Timer.DelayCall<Mobile>( TimeSpan.FromSeconds( 0.5 ), new TimerStateCallback<Mobile>( ((GuillotineComponent)c).Activate ), from );
				}
				else
					from.LocalOverheadMessage( MessageType.Regular, 0, 501777 ); // Hmm... you suspect that if you used this again, it might hurt.
			}
			else
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
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

	public class GuillotineDeed : BaseAddonDeed
	{
		public override BaseAddon Addon { get { return new GuillotineAddon(); } }
		public override int LabelNumber { get { return 1024656; } } // Guillotine

		[Constructable]
		public GuillotineDeed() : base()
		{
			LootType = LootType.Blessed;
		}

		public GuillotineDeed( Serial serial ) : base( serial )
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