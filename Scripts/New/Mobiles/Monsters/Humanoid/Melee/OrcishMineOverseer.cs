using System;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an orcish corpse" )]
	public class OrcishMineOverseer : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }
		public override string DefaultName{ get{ return "an orcish mine overseer"; } }

		[Constructable]
		public OrcishMineOverseer() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 138;
			BaseSoundID = 0x45A;

			SetStr( 325, 371 );
			SetDex( 91, 115 );
			SetInt( 81, 105 );

			SetHits( 245, 318 );

			SetDamage( 6, 16 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 25, 35 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 85.1, 105.0 );
			SetSkill( SkillName.Swords, 75.1, 105.0 );
			SetSkill( SkillName.Tactics, 98.1, 108.0 );
			SetSkill( SkillName.Wrestling, 85.1, 105.0 );

			Fame = 5000;
			Karma = -5000;

			switch ( Utility.Random( 5 ) )
			{
				case 0: PackItem( new Lockpick() );  break;
				case 1: PackItem( new MortarPestle() ); break;
				case 2: PackItem( new Bottle() ); break;
				case 3: PackItem( new RawRibs() ); break;
				case 4: PackItem( new Shovel() ); break;
			}

			PackItem( new RingmailChest() );

			if ( 0.3 > Utility.RandomDouble() )
			{
				PackItem( Loot.RandomPossibleReagent() );
				PackItem( Loot.RandomPossibleReagent() );
				PackItem( Loot.RandomPossibleReagent() );
			}

			if ( 0.2 > Utility.RandomDouble() )
				PackItem( new BolaBall() );

			if ( 0.75 > Utility.RandomDouble() )
				PackItem( new IronOre( 10 ) );
			else
				PackItem( new IronIngot( 5 ) );
		}

		public override void GenerateLoot()
		{
			if ( 0.75 > Utility.RandomDouble() )
			{
				AddLoot( LootPack.Meager );
				AddLoot( LootPack.Average, 2 );
			}
			else
			{
				AddLoot( LootPack.Average, 2 );
				AddLoot( LootPack.Rich );
			}

			// TODO: evil orc helm
			if ( 0.05 > Utility.RandomDouble() )
				PackItem( new EvilOrcHelm() );

			int random = Utility.Random( 250 );

			if ( random == 0 )
				PackItem( new OrcishPickaxe() );
			else if ( random < 10 )
				PackItem( new SturdyPickaxe() );
			else
				PackItem( new Shovel() );

			AddLoot( LootPack.RareStatueDrop );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 3; } }
		public override int Meat{ get{ return 2; } }
		public override int DefaultBloodHue{ get{ return -2; } }
		public override int BloodHueTemplate{ get{ return Utility.RandomBlackHue(); } }

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.SavagesAndOrcs; }
		}

		public override bool IsEnemy( Mobile m )
		{
			if ( m.Player && m.FindItemOnLayer( Layer.Helm ) is OrcishKinMask )
				return false;

			return base.IsEnemy( m );
		}

		public override void AggressiveAction( Mobile aggressor, bool criminal )
		{
			base.AggressiveAction( aggressor, criminal );

			Item item = aggressor.FindItemOnLayer( Layer.Helm );

			if ( item is OrcishKinMask )
			{
				AOS.Damage( aggressor, 50, 0, 100, 0, 0, 0 );
				item.Delete();
				aggressor.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
				aggressor.PlaySound( 0x307 );
			}
		}

		public OrcishMineOverseer( Serial serial ) : base( serial )
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