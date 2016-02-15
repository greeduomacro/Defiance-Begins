using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class VampireArcanist : BaseVampire
	{
		[Constructable]
		public VampireArcanist() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.4, 0.4 )
		{
			SetSkill( SkillName.Necromancy, 35.0, 50.0 );
			SetSkill( SkillName.Meditation, 35.0, 75.0 );
			SetSkill( SkillName.Tactics, 50.0 );
			SetSkill( SkillName.MagicResist, 160.0 );

			AddItem( NotCorpseCont( new Doublet( 1194 ) ) );
			AddItem( new FancyShirt() );
			AddItem( NotCorpseCont( Rehued( new BoneHelm(), 1194 ) ) );
			AddItem( NotCorpseCont( new Robe( 1654 ) ) );
			AddItem( NotCorpseCont( Rehued( new LeatherGloves(), 2424 ) ) );
			AddItem( NotCorpseCont( new Boots( 1175 ) ) );
			AddItem( NotCorpseCont( new LongPants( 2424 ) ) );

			//if ( 0.2 >= Utility.RandomDouble() )
				AddItem( NotCorpseCont( Renamed( Rehued( new GoldNecklace(), 1194 ), "Bloody Necklace" ) ) );

			AddItem( NotCorpseCont( Renamed( Rehued( new Spellbook( 0.05 > Utility.RandomDouble() ? ulong.MaxValue : 0x90052830090CA0 ), 1194 ), "Book of Arcane Ritual" ) ) );

			SetResistance( ResistanceType.Physical, 100);
			SetResistance( ResistanceType.Fire, 50, 75 );
			SetResistance( ResistanceType.Cold, 45, 60 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 35, 50 );

			Fame = 6500;
			Karma = -6500;

			SetStr( 250, 300 );
			SetDex( 76, 95 );
			SetInt( 36, 60 );

			SetHits( 250, 350 );

			SetDamage( 10, 15 );

			SetDamageType( ResistanceType.Physical, 70 );
			SetDamageType( ResistanceType.Cold, 30 );
			VirtualArmor = 50;

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
		}

		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int VampireTimer{ get{ return 30; } }

		public override double StakeVariance( Stake stake, double stakechance )
		{
			if ( stake is SilverStake )
				return stakechance * 0.75;
			else
				return stakechance;
		}

		public VampireArcanist( Serial serial ) : base( serial )
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