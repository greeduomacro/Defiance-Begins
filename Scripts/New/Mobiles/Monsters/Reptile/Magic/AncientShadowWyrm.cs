using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a dragon corpse" )]
	public class AncientShadowWyrm : BaseCreature
	{
		public override string DefaultName{ get{ return "an ancient shadow wyrm"; } }

		[Constructable]
		public AncientShadowWyrm () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.1, 0.15 )
		{
			Body = 46;
			BaseSoundID = 362;
			Hue = 0x4001;

			SetStr( 1296, 1385 );
			SetDex( 186, 275 );
			SetInt( 786, 875 );

			SetHits( 858, 911 );

			SetDamage( 26, 37 );

			SetDamageType( ResistanceType.Physical, 75 );
			SetDamageType( ResistanceType.Poison, 25 );

			SetResistance( ResistanceType.Physical, 65, 75 );
			SetResistance( ResistanceType.Fire, 80, 90 );
			SetResistance( ResistanceType.Cold, 70, 80 );
			SetResistance( ResistanceType.Poison, 60, 70 );
			SetResistance( ResistanceType.Energy, 60, 70 );

			SetSkill( SkillName.EvalInt, 95.1, 115.0 );
			SetSkill( SkillName.Magery, 95.1, 115.0 );
			SetSkill( SkillName.Meditation, 69.5, 86.0 );
			SetSkill( SkillName.MagicResist, 115.5, 165.0 );
			SetSkill( SkillName.Tactics, 110.6, 125.0 );
			SetSkill( SkillName.Wrestling, 110.6, 125.0 );

			Fame = 22500;
			Karma = -22500;

			VirtualArmor = 85;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich );
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Gems, 5 );
			AddLoot( LootPack.HighScrolls );
			AddLoot( LootPack.MedScrolls, 3 );
		}

		public override void GenerateLoot( bool spawning )
		{
			base.GenerateLoot( spawning );

			if ( !spawning )
			{
				Bag bag = PackBagofRegs( (0.25 > Utility.RandomDouble()) ? 75 : Utility.RandomMinMax( 35, 50 ) );

				if ( 0.50 > Utility.RandomDouble() )
				{
					Spellbook book = new Spellbook( (0.15 > Utility.RandomDouble()) ? ulong.MaxValue : 0 );
					if ( 0.15 > Utility.RandomDouble() )
						book.LootType = LootType.Blessed;
					if ( 0.02 > Utility.RandomDouble() )
						book.Dyable = true;

					bag.DropItem( book );
				}
			}
		}

		public override int GetIdleSound()
		{
			return 0x2D3;
		}

		public override int GetHurtSound()
		{
			return 0x2D1;
		}

		public override bool Unprovokable{ get{ return true; } }
		public override bool ReacquireOnMovement{ get{ return true; } }
		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override bool AutoDispel{ get{ return true; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override int Hides{ get{ return 35; } }
		public override int Meat{ get{ return 15; } }
		public override int Scales{ get{ return 18; } }
		public override ScaleType ScaleType{ get{ return (ScaleType)Utility.Random( 4 ); } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override Poison HitPoison{ get{ return Poison.Deadly; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int DefaultBloodHue{ get{ return -1; } }

		public AncientShadowWyrm( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}