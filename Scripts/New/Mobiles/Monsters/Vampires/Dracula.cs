using System;
using System.Reflection;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class Dracula : BaseVampire
	{
		[Constructable]
		public Dracula() : base( AIType.AI_Melee, FightMode.Closest, 10, 3, 0.3, 0.3 )
		{
			SetSkill( SkillName.Swords, 120.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.MagicResist, 180.0 );
			Hue = 1072;
			Female = false;
			BodyValue = 0x190;
			Title = "the vampire lord";
			Name = "Count Dracula";

			AddItem( NotCorpseCont( new Doublet( 1157 ) ) );
			AddItem( NotCorpseCont( new FancyShirt( 1175 ) ) );
			AddItem( NotCorpseCont( Rehued( new LeatherGloves(), 2424 ) ) );
			AddItem( NotCorpseCont( new Boots( 1175 ) ) );
			AddItem( NotCorpseCont( new LongPants( 1157 ) ) );
			AddItem( NotCorpseCont( Renamed( Rehued( new GoldNecklace(), 2949 ), "a bloody necklace" ) ) );

			AddItem( NotCorpseCont( new BloodVampire() ) );

			PackItem( new DaemonBlood( Utility.Random( 5, 5 ) ) );

			SetResistance( ResistanceType.Physical, 100);
			SetResistance( ResistanceType.Fire, 25, 45 );
			SetResistance( ResistanceType.Cold, 25, 65 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 100 );

			Fame = 12500;
			Karma = -12500;

			SetStr( 1000, 1250 );
			SetDex( 76, 95 );
			SetInt( 36, 60 );

			SetHits( 1850 );

			SetDamage( 10, 15 );

			SetDamageType( ResistanceType.Physical, 70 );
			SetDamageType( ResistanceType.Cold, 30 );
			VirtualArmor = 50;
		}

		public override void GenerateLoot()
		{
			if ( 0.90 >= Utility.RandomDouble() )
				PackItem( new BagOfReagents( 50 ) );

			if ( 0.40 >= Utility.RandomDouble() )
				PackItem( new BagOfReagents( 50 ) );

			if ( 0.10 >= Utility.RandomDouble() )
				PackItem( new BagOfReagents( 50 ) );

			AddLoot( LootPack.UltraRich, 2 );
/*
			PackGem( 10 ,20 );
			PackGold( 3500, 5600 );
			PackPotion();
			PackScroll(3,8);
			PackMagicItems( 1, 8, 0.5, 1.0 );
			PackMagicItems( 3, 5, 0.5, 1.0 );
			PackMagicItems( 4, 4, 0.5, 1.0 );
*/
		}

		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override int VampireTimer{ get{ return 5; } }

		public override double StakeVariance( Stake stake, double stakechance )
		{
			if ( stake is HolyStake )
				return stakechance * 0.25;
			else
				return 0.0;
		}

		public Dracula( Serial serial ) : base( serial )
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

		public override void Reanimate( Corpse c )
		{
			Name = c.Name;
			Body = 317;
			BaseSoundID = 0x270;
			Hue = 1873;
			RawStr = 500;
			RawDex = 200;
			RawInt = 300;
			SetHits( 650 );

			VirtualArmor = 150;

			Fame = 12500;
			Karma = -12500;

			SetResistance( ResistanceType.Physical, 100);
			SetResistance( ResistanceType.Fire, 25, 45 );
			SetResistance( ResistanceType.Cold, 25, 65 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 100 );

			SetDamageType( ResistanceType.Physical, 70 );
			SetDamageType( ResistanceType.Cold, 30 );

			SetSkill( SkillName.Wrestling, 120.0 );
			SetSkill( SkillName.Tactics, 120.0 );
			SetSkill( SkillName.MagicResist, 160.0 );
			SetDamage( 20, 30 );
		}

		public new void ReanimateEffect( Point3D location, Map map )
		{
			Unconscience = Hidden = Blessed = CantWalk = false;
			PlaySound( 552 );
			Effects.SendLocationEffect( location, map, 0x3728, 13 );
		}

		public override void OnDeath( Container c )
		{
			if ( Body != 317 )
				base.OnDeath( c );
			else
			{
				int sound = this.GetDeathSound();

				if ( sound >= 0 )
					Effects.PlaySound( this, this.Map, sound );

				Delete();
				((VampireCorpse)c).Staked = true;
			}
		}
	}
}