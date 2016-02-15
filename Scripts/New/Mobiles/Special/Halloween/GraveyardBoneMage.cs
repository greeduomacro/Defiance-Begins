using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a skeletal corpse" )]
	public class GraveyardBoneMage : BaseCreature
	{
		public override string DefaultName{ get{ return "a bone mage"; } }

		[Constructable]
		public GraveyardBoneMage() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = Utility.RandomList( 50, 56 );
			BaseSoundID = 451;
			Hue = Utility.RandomList( 43, 1899, 0x4001 );

			SetStr( 96, 135 );
			SetDex( 56, 75 );
			SetInt( 186, 210 );

			SetHits( 125, 165 );

			SetDamage( 6, 9 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 40 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.EvalInt, 70.1, 80.0 );
			SetSkill( SkillName.Magery, 70.1, 80.0 );
			SetSkill( SkillName.MagicResist, 55.1, 70.0 );
			SetSkill( SkillName.Tactics, 65.1, 70.0 );
			SetSkill( SkillName.Wrestling, 55.1, 65.0 );

			Fame = 3000;
			Karma = -3000;

			VirtualArmor = 40;

			PackReg( 3 );
			PackReg( 3 );
			PackReg( 3 );
			PackNecroReg( 3, 10 );
			PackItem( new Bone( Utility.Random( 5 ) ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.LowScrolls, 2 );
			AddLoot( LootPack.Potions, 2 );
		}

		public override bool BleedImmune{ get{ return true; } }
		public override int DefaultBloodHue{ get{ return -1; } }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override Poison PoisonImmune{ get{ return Poison.Regular; } }

		public GraveyardBoneMage( Serial serial ) : base( serial )
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