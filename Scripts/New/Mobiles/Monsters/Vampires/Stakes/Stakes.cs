using System;
using Server;

namespace Server.Items
{
	public class WoodenStake : Stake
	{
		public override double StakeChance{ get{ return 0.3; } }
		public override string DefaultName{ get{ return "wooden stake"; } }

		[Constructable]
		public WoodenStake() : this( 1 )
		{
		}

		[Constructable]
		public WoodenStake( int amount ) : base( amount )
		{
			Hue = 1810;
		}

		public WoodenStake( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			list.Add( String.Format("{0} wooden stake", Amount) );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class SilverStake : Stake
	{
		public override double StakeChance{ get{ return 0.5; } }
		public override string DefaultName{ get{ return "silver stake"; } }

		[Constructable]
		public SilverStake() : this( 1 )
		{
		}

		[Constructable]
		public SilverStake( int amount ) : base( amount )
		{
			Hue = 1072;
		}

		public SilverStake( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			list.Add( String.Format("{0} silver stake", Amount) );
		}

		public override bool Validate( Mobile from )
		{
/*
			if ( from.Skills[SkillName.Necromancy].Value >= 50.0 && from.AccessLevel < AccessLevel.GameMaster )
			{
				from.SendLocalizedMessage( 600295 );
				AOS.Damage( from, from, Utility.Random( 5, 5 ), 0, 0, 0, 0, 100, true );
			}
*/
			return true;
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class HolyStake : Stake
	{
		public override double StakeChance{ get{ return 0.8; } }
		public override string DefaultName{ get{ return "holy stake"; } }

		[Constructable]
		public HolyStake() : this( 1 )
		{
		}

		[Constructable]
		public HolyStake( int amount ) : base( amount )
		{
			Hue = 1150;
		}

		public HolyStake( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			list.Add( String.Format("{0} holy stake", Amount) );
		}

		public override bool Validate( Mobile from )
		{
/*
			if ( from.Skills[SkillName.Necromancy].Value >= 80.0 && from.AccessLevel < AccessLevel.GameMaster )
			{
				from.SendLocalizedMessage( 600296 );
				return false;
			}
*/
			return true;
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}