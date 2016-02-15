using System;
using System.Reflection;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class VampireLord : BaseVampire
	{
		[Constructable]
		public VampireLord() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.4 )
		{
			SetSkill( SkillName.Swords, 35.0, 50.0 );
			SetSkill( SkillName.Macing, 35.0, 75.0 );
			SetSkill( SkillName.Fencing, 69.0, 100.0 );
			SetSkill( SkillName.Tactics, 50.0 );
			SetSkill( SkillName.MagicResist, 120.0 );

			AddItem( NotCorpseCont( new Doublet( 1194 ) ) );
			AddItem( new FancyShirt( 37 ) );
			AddItem( NotCorpseCont( Rehued( new LeatherGloves(), 2424 ) ) );
			AddItem( NotCorpseCont( new Boots( 1175 ) ) );
			AddItem( NotCorpseCont( new LongPants( 2424 ) ) );

			//if (0.2 >= Utility.RandomDouble())
				AddItem( NotCorpseCont( Renamed( Rehued( new GoldNecklace(), 2406 ), "Broodu Family Stone" ) ) );

			BaseWeapon weapon = new Kryss();
			weapon.Hue = 2406;
			weapon.Name = "an ancient ritual blade";
			weapon.Slayer = SlayerName.Silver;
			AddItem( NotCorpseCont( weapon ) );

			SetResistance( ResistanceType.Physical, 65 );
			SetResistance( ResistanceType.Fire, 25, 45 );
			SetResistance( ResistanceType.Cold, 25, 65 );
			SetResistance( ResistanceType.Poison, 65 );
			SetResistance( ResistanceType.Energy, 65 );

			Fame = 7000;
			Karma = -9000;

			SetStr( 430, 475 );
			SetDex( 76, 95 );
			SetInt( 36, 60 );

			SetHits( 500, 600 );

			SetDamage( 20, 25 );


			SetDamageType( ResistanceType.Cold, 100 );
			VirtualArmor = 70;
		}

		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int VampireTimer{ get{ return 15; } }

		public override double StakeVariance( Stake stake, double stakechance )
		{
			return stakechance * 0.50;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 2 );
		}

		public VampireLord( Serial serial ) : base( serial )
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

	public class ShadowVampireLord : BaseVampire
	{
		[Constructable]
		public ShadowVampireLord() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.4 )
		{
			Hue += 16384;
			HairHue += 16384;

			SetSkill( SkillName.Swords, 35.0, 50.0 );
			SetSkill( SkillName.Macing, 35.0, 75.0 );
			SetSkill( SkillName.Fencing, 69.0, 100.0 );
			SetSkill( SkillName.Tactics, 50.0 );
			SetSkill( SkillName.MagicResist, 120.0 );

			AddItem( NotCorpseCont( new Doublet( 1194+16384 ) ) );
			AddItem( new FancyShirt( 37+16384 ) );
			AddItem( NotCorpseCont( Rehued( new LeatherGloves(), 2424+16384 ) ) );
			AddItem( NotCorpseCont( new Boots( 1175+16384 ) ) );
			AddItem( NotCorpseCont( new LongPants( 2424+16384 ) ) );

			//if (0.2 >= Utility.RandomDouble())
				AddItem( NotCorpseCont( Renamed( Rehued( new GoldNecklace(), 2406 ), "Broodu Family Stone" ) ) );

			BaseWeapon weapon = new Kryss();
			weapon.Hue = 2406;
			weapon.Name = "an ancient ritual blade";
			weapon.Slayer = SlayerName.Silver;
			AddItem( NotCorpseCont( weapon ) );

			SetResistance( ResistanceType.Physical, 65 );
			SetResistance( ResistanceType.Fire, 25, 45 );
			SetResistance( ResistanceType.Cold, 25, 65 );
			SetResistance( ResistanceType.Poison, 65 );
			SetResistance( ResistanceType.Energy, 65 );

			Fame = 7000;
			Karma = -9000;

			SetStr( 430, 475 );
			SetDex( 76, 95 );
			SetInt( 36, 60 );

			SetHits( 500, 600 );

			SetDamage( 20, 25 );


		    SetDamageType( ResistanceType.Cold, 100 );
			VirtualArmor = 70;

		}

		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int VampireTimer{ get{ return 15; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 2 );
		}

		public override void Reanimate( Corpse c )
		{
			Hue += 16384;
			HairHue += 16384;
			base.Reanimate( c );
		}

		public override double StakeVariance( Stake stake, double stakechance )
		{
			if ( stake is WoodenStake )
				return 0.0;
			else
				return stakechance * 0.45;
		}

		public ShadowVampireLord( Serial serial ) : base( serial )
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

		public override bool OnBeforeDeath()
		{
			Hue -= 16384;
			HairHue -= 16384;
			List<Item> equipitems = new List<Item>( Items );

			foreach ( Item item in equipitems )
				if ((item.Layer != Layer.Bank) && (item.Layer != Layer.Backpack) && (item.Layer != Layer.Mount) )
					item.Hue %= 16384;

			return base.OnBeforeDeath();
		}
	}
}