using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a ghostly corpse" )]
	public class HalloweenGhoul : BaseCreature
	{
		public override string DefaultName{ get{ return "a ghoul"; } }

		[Constructable]
		public HalloweenGhoul() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 153;
			BaseSoundID = 0x482;
			Hue = Utility.RandomList( 43, 1899 );

			SetStr( 76, 125 );
			SetDex( 76, 95 );
			SetInt( 36, 60 );

			SetHits( 76, 100 );
			SetMana( 0 );

			SetDamage( 8, 10 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 25, 30 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 5, 10 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.MagicResist, 45.1, 60.0 );
			SetSkill( SkillName.Tactics, 55.1, 70.0 );
			SetSkill( SkillName.Wrestling, 55.1, 65.0 );

			Fame = 2500;
			Karma = -2500;

			VirtualArmor = 29;

			PackItem( Loot.RandomWeapon() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager, 2 );
		}

		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int DefaultBloodHue{ get{ return -1; } }

		public HalloweenGhoul( Serial serial ) : base( serial )
		{
		}

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
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