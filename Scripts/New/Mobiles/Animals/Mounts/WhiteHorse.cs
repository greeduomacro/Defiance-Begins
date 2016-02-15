using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a white horse corpse" )]
	public class WhiteHorse : BaseMount
	{
		public override string DefaultName{ get{ return "a rare white horse"; } }

		[Constructable]
		public WhiteHorse() : base( 0xBE, 0x3E9E, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			BaseSoundID = 0xA8;
			Hue = 1150;

			SetStr( 376, 400 );
			SetDex( 91, 120 );
			SetInt( 291, 300 );

			SetHits( 226, 240 );

			SetDamage( 11, 30 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Cold, 80 );

			SetResistance( ResistanceType.Physical, 30, 40 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 70, 80 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 100.0, 120.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 100.0 );

			Fame = 20000;
			Karma = -20000;

			Tamable = true;
			ControlSlots = 3;
			MinTameSkill = 100.0;
		}

		public override void GenerateLoot()
		{
			PackItem( new Sapphire( Utility.RandomMinMax( 16, 16 ) ) );
		}

		public override bool HasBreath{ get{ return true; } } // cold breath enabled
		public override int BreathFireDamage{ get{ return 0; } }
		public override int BreathColdDamage{ get{ return 100; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Daemon | PackInstinct.Equine; } }

		public WhiteHorse( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}