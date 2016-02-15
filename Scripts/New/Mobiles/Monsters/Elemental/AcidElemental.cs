using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an acid elemental corpse" )]
	public class AcidElemental : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 85.0; } }
		public override string DefaultName{ get{ return "a toxic water elemental"; } }

		[Constructable]
		public AcidElemental() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 0x9E;
			Hue = 471;
			BaseSoundID = 278;

			SetStr( 675, 790 );
			SetDex( 66, 85 );
			SetInt( 202, 250 );

			SetHits( 535, 675 );

			SetDamage( 10, 13 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 10, 25 );
			SetResistance( ResistanceType.Cold, 10, 25 );
			SetResistance( ResistanceType.Poison, 100, 100 );
			SetResistance( ResistanceType.Energy, 5, 10 );

			SetSkill( SkillName.EvalInt, 89.1, 99.0 );
			SetSkill( SkillName.Magery, 99.1, 105.0 );
			SetSkill( SkillName.MagicResist, 125.1, 140.0 );
			SetSkill( SkillName.Tactics, 89.1, 99.0 );
			SetSkill( SkillName.Wrestling, 76.1, 90.0 );

			Fame = 6500;
			Karma = -6500;

			VirtualArmor = 55;
			CanSwim = true;

			PackItem( new BlackPearl( 9 ) );
			PackItem( new Nightshade( 10 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Meager );
			AddLoot( LootPack.Potions );
		}

		public override bool BleedImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 3; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int DefaultBloodHue{ get{ return 471; } }

		public AcidElemental( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}