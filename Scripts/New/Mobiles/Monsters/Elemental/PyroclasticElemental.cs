using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Engines.Quests.Collector;

namespace Server.Mobiles
{
	[CorpseName( "a pyroclastic elemental corpse" )]
	public class PyroclasticElemental : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 120.5; } }
		public override double DispelFocus{ get{ return 55.0; } }
		public override string DefaultName{ get{ return "a pyroclastic elemental"; } }

		[Constructable]
		public PyroclasticElemental() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 13;
			Hue = Utility.Random( 1255, 4 );
			BaseSoundID = 655;

			SetStr( 226, 255 );
			SetDex( 166, 185 );
			SetInt( 201, 225 );

			SetHits( 226, 293 );

			SetDamage( 13, 19 );

			SetDamageType( ResistanceType.Fire, 100 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 60, 85 );
			SetResistance( ResistanceType.Cold, 0 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 25, 35 );

			SetSkill( SkillName.EvalInt, 80.1, 95.0 );
			SetSkill( SkillName.Magery, 60.1, 75.0 );
			SetSkill( SkillName.MagicResist, 80.1, 95.0 );
			SetSkill( SkillName.Tactics, 80.1, 90.0 );
			SetSkill( SkillName.Wrestling, 80.1, 90.0 );

			Fame = 7500;
			Karma = -7500;

			VirtualArmor = 43;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );
			AddLoot( LootPack.Meager, 2 );
			AddLoot( LootPack.LowScrolls, 2 );
			AddLoot( LootPack.MedScrolls );
		}

		public override void GenerateLoot( bool spawning )
		{
			if ( !spawning )
			{
				double rand = Utility.RandomDouble();
				if ( 0.025 > Utility.RandomDouble() )
					PackItem( new VolcanicRock() );
				else if ( 0.125 > Utility.RandomDouble() )
					PackItem( new ObsidianStatue() );

				if ( 0.05 > Utility.RandomDouble() )
					PackItem( new ObsidianStatue() );
			}
		}

		public override bool BleedImmune{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 3; } }
		public override int DefaultBloodHue{ get{ return -1; } }

		public override void AlterMeleeDamageFrom( Mobile from, ref int damage )
		{
			if ( from.FindItemOnLayer( Layer.TwoHanded ) is BaseRanged )
				damage = 0; // no melee damage from arrows
		}

		public PyroclasticElemental( Serial serial ) : base( serial )
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

		public override bool HasAura{ get{ return true; } }
		private DateTime m_NextAura;

		public override void OnThink()
		{
			base.OnThink();

			if ( Alive && !Controlled && DateTime.Now >= m_NextAura )
			{
				IPooledEnumerable eable = GetMobilesInRange( 2 );

				Packet p = Packet.Acquire( new MessageLocalizedAffix( Serial.MinusOne, -1, MessageType.Label, 0x3B2, 3, 1072073, "", AffixType.Prepend | AffixType.System, Name, "" ) );

				foreach ( Mobile m in eable )
				{
					BaseCreature bc = m as BaseCreature;

					if ( m != this && ( m.Player || ( bc != null && bc.Controlled ) ) && CanBeHarmful( m ) && m.AccessLevel == AccessLevel.Player )
					{
						DoHarmful( m );
						m.Hidden = false;
						m.Send( p );
						AOS.Damage( m, this, Utility.RandomMinMax( 5, 10 ), 0, 100, 0, 0, 0 );
						Combatant = m;
					}
				}

				Packet.Release( p );

				eable.Free();

				m_NextAura = DateTime.Now + TimeSpan.FromSeconds( 5.0 + ( Utility.RandomDouble() * 5.0 ) );
			}
		}
	}
}