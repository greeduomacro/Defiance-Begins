using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a mongbat corpse" )]
	public class Mongbat : BaseCreature
	{
		public override string DefaultName{ get{ return "a mongbat"; } }

		[Constructable]
		public Mongbat() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 39;
			BaseSoundID = 422;

			SetStr( 6, 10 );
			SetDex( 26, 38 );
			SetInt( 6, 14 );

			SetHits( 4, 6 );
			SetMana( 0 );

			SetDamage( 1, 2 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 5, 10 );

			SetSkill( SkillName.MagicResist, 5.1, 14.0 );
			SetSkill( SkillName.Tactics, 5.1, 10.0 );
			SetSkill( SkillName.Wrestling, 5.1, 10.0 );

			Fame = 150;
			Karma = -150;

			VirtualArmor = 10;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = -18.9;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Poor );
		}

		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }

		public override void OnCarve( Mobile from, Corpse corpse, Item with )
		{
			if ( !(Summoned || IsBonded || corpse.Animated) )
				corpse.AddCarvedItem( new BatWing( 2 ), from );

			base.OnCarve( from, corpse, with );
		}

		public Mongbat( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}