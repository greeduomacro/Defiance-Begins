using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class AmazonHuntress : BaseAmazon
	{
		[Constructable]
		public AmazonHuntress() : base( AIType.AI_Archer, FightMode.Closest, 5, 1, 0.45, 0.45 )
		{
			Title = "an Amazon Huntress";

			SetSkill( SkillName.Parry, 40.0, 60.0 );
			SetSkill( SkillName.Archery, 40.0, 60.0 );
			SetSkill( SkillName.Tactics, 50.0, 70.0 );
			SetSkill( SkillName.MagicResist, 100.0 );

			AddItem( MakeAmazonArmor( new FemaleLeatherChest() ) );
			AddItem( MakeAmazonArmor( new LeatherGloves() ) );
			//AddItem( Rehued( new LeatherShorts(), 1437 ) );
			AddItem( MakeAmazonArmor( new Boots() ) );

			if ( Utility.Random( 350 ) == 0 )
				AddItem( new JadeNecklace() );

			if ( 0.10 > Utility.RandomDouble() )
			{
				CompositeBow weapon = new CompositeBow();

				double random = Utility.RandomDouble();
				if ( 0.05 > random )
					weapon.DamageLevel = WeaponDamageLevel.Vanq;
				else if ( 0.10 > random )
					weapon.DamageLevel = WeaponDamageLevel.Force;

				AddItem( MakeAmazonArmor( weapon ) );
			}
			else
				AddItem( new Crossbow() );

			SetResistance( ResistanceType.Physical, 25, 40 );
			SetResistance( ResistanceType.Fire, 25, 30 );
			SetResistance( ResistanceType.Cold, 25, 30 );
			SetResistance( ResistanceType.Poison, 100, 100 );
			SetResistance( ResistanceType.Energy, 25, 40 );

			Fame = 100;
			Karma = -3000;

			SetStr( 50, 65 );
			SetDex( 50, 55 );
			SetInt( 10, 13 );

			SetHits( 180, 200 );

			SetDamage( 6, 10 );

			SetDamageType( ResistanceType.Physical, 80 );
			SetDamageType( ResistanceType.Fire, 20 );

			VirtualArmor = 30;
		}

		public override Poison PoisonImmune{ get{ return Poison.Regular; } }

		public AmazonHuntress( Serial serial ) : base( serial )
		{
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.LowScrolls, 2 );
			AddLoot( LootPack.Potions, 2 );
			AddLoot( LootPack.Gems, Utility.Random( 2, 3 ) );
			AddLoot( LootPack.Average, 2 );
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