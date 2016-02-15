using System;
using Server;
using Server.Items;
using Server.Factions;

namespace Server.Mobiles
{
	[CorpseName( "a daemon corpse" )]
	public class ShadowDaemon : BaseCreature
	{
		[Constructable]
		public ShadowDaemon() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "daemon" );
			Body = 9;
			BaseSoundID = 357;
			Hue = 16385;

			SetStr( 1254, 1381 );
			SetDex( 93, 135 );
			SetInt( 745, 810 );

			SetHits( 694, 875 );

			SetDamage( 12, 20 );

			SetDamageType( ResistanceType.Physical, 70 );
			SetDamageType( ResistanceType.Cold, 30 );

			SetResistance( ResistanceType.Physical, 60, 75 );
			SetResistance( ResistanceType.Fire, 40, 60 );
			SetResistance( ResistanceType.Cold, 60, 75 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 60, 75 );

			SetSkill( SkillName.EvalInt, 90.1, 100.0 );
			SetSkill( SkillName.Magery, 75.1, 95.0 );
			SetSkill( SkillName.MagicResist, 95.1, 110.0 );
			SetSkill( SkillName.Tactics, 95.1, 100.0 );
			SetSkill( SkillName.Wrestling, 90.1, 100.0 );

			Fame = 18750;
			Karma = -18750;

			VirtualArmor = 58;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Rich, 2 );
			AddLoot( LootPack.Average, 2 );
			AddLoot( LootPack.HighScrolls );
			AddLoot( LootPack.Gems, 5 );

			AddLoot( LootPack.UncommonStatueDrop );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int TreasureMapLevel{ get{ return 4; } }
		public override int Meat{ get{ return 1; } }
		public override int DefaultBloodHue{ get{ return -1; } }

		public ShadowDaemon( Serial serial ) : base( serial )
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