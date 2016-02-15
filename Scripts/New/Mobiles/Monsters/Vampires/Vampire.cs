using System;
using System.Reflection;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class Vampire : BaseVampire
	{
		[Constructable]
		public Vampire() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.4 )
		{
			SetSkill( SkillName.Swords, 35.0, 50.0 );
			SetSkill( SkillName.Macing, 35.0, 75.0 );
			SetSkill( SkillName.Fencing, 35.0, 75.0 );
			SetSkill( SkillName.Tactics, 50.0 );
			SetSkill( SkillName.MagicResist, 120.0 );

			AddItem( NotCorpseCont( new Doublet( 1194 ) ) );
			AddItem( new FancyShirt() );
			AddItem( NotCorpseCont( Rehued( new LeatherGloves(), 2424 ) ) );
			AddItem( NotCorpseCont( new Boots( 1175 ) ) );
			AddItem( NotCorpseCont( new LongPants( 2424 ) ) );

			//if (0.2 >= Utility.RandomDouble())
				AddItem( NotCorpseCont( Renamed( Rehued( new GoldNecklace(), 1194 ), "a bloody necklace" ) ) );

			BaseWeapon weapon = new Dagger();
			weapon.Hue = 2949;
			weapon.Name = "a dagger of the undead";
			weapon.Slayer = SlayerName.Silver;
			//weapon.WeaponSlayerAttributes.Silver = 25;
			AddItem( weapon );

			PackItem( new DaemonBlood(Utility.RandomMinMax(1,3)) );

			SetResistance( ResistanceType.Physical, 100);
			SetResistance( ResistanceType.Fire, 25, 45 );
			SetResistance( ResistanceType.Cold, 25, 65 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 100 );

			Fame = 5000;
			Karma = -5000;

			SetStr( 250, 300 );
			SetDex( 76, 95 );
			SetInt( 36, 60 );

			SetHits( 250, 300 );

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

		public Vampire( Serial serial ) : base( serial )
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

	public class ShadowVampire : BaseVampire
	{
		[Constructable]
		public ShadowVampire() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.4 )
		{
			Hue += 16384;
			HairHue += 16384;

			SetSkill( SkillName.Swords, 35.0, 50.0 );
			SetSkill( SkillName.Macing, 35.0, 75.0 );
			SetSkill( SkillName.Fencing, 35.0, 75.0 );
			SetSkill( SkillName.Tactics, 50.0 );
			SetSkill( SkillName.MagicResist, 120.0 );

			AddItem( NotCorpseCont( new Doublet( 1194+16384 ) ) );
			AddItem( NotCorpseCont( new FancyShirt( 1072+16384 ) ) );
			AddItem( NotCorpseCont( Rehued( new LeatherGloves(), 2424+16384 ) ) );
			AddItem( NotCorpseCont( new Boots( 1175+16384 ) ) );
			AddItem( NotCorpseCont( new LongPants( 2424+16384 ) ) );

			//if (0.2 >= Utility.RandomDouble())
				AddItem( NotCorpseCont( Renamed( Rehued( new GoldNecklace(), 1194 ), "Bloody Necklace" ) ) );

			BaseWeapon weapon = new Dagger();
			weapon.Hue = 2949+16384;
			weapon.Name = "a dagger of the undead";
			weapon.Slayer = SlayerName.Silver;
			//weapon.WeaponSlayerAttributes.Silver = 25;
			AddItem( weapon );

			PackItem( new DaemonBlood(Utility.RandomMinMax(1,3)) );

			SetResistance( ResistanceType.Physical, 100);
			SetResistance( ResistanceType.Fire, 25, 45 );
			SetResistance( ResistanceType.Cold, 25, 65 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 100 );

			Fame = 6000;
			Karma = -6000;

			SetStr( 250, 300 );
			SetDex( 76, 95 );
			SetInt( 36, 60 );

			SetHits( 250, 300 );

			SetDamage( 10, 15 );

			SetDamageType( ResistanceType.Physical, 70 );
			SetDamageType( ResistanceType.Cold, 30 );
			VirtualArmor = 50;
		}

		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int VampireTimer{ get{ return 30; } }

		public override double StakeVariance( Stake stake, double stakechance )
		{
			if ( stake is WoodenStake )
				return 0.0;
			else
				return stakechance * 0.75;
		}

		public ShadowVampire( Serial serial ) : base( serial )
		{
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
		}

		public override void Reanimate( Corpse c )
		{
			Hue += 16384;
			HairHue += 16384;
			base.Reanimate( c );
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
				if ((item.Layer != Layer.Bank) && (item.Layer != Layer.Backpack) && (item.Layer != Layer.Mount))
					item.Hue %= 16384;

			return base.OnBeforeDeath();
		}
	}
}