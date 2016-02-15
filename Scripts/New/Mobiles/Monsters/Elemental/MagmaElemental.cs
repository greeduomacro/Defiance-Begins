using System;
using Server.Network;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a magma elemental corpse" )]
	public class MagmaElemental : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 45.0; } }
		public override string DefaultName{ get{ return "a magma elemental"; } }

		[Constructable]
		public MagmaElemental() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 199;
			BaseSoundID = 268;
			Hue = 1360;

			SetStr( 300, 355 );
			SetDex( 70, 95 );
			SetInt( 71, 92 );

			SetHits( 300, 600 );

			SetDamage( 20, 30 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 30, 35 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 10, 20 );
			SetResistance( ResistanceType.Poison, 15, 25 );
			SetResistance( ResistanceType.Energy, 15, 25 );

			SetSkill( SkillName.MagicResist, 80.1, 95.0 );
			SetSkill( SkillName.Tactics, 100.1, 110.0 );
			SetSkill( SkillName.Wrestling, 80.1, 100.0 );

			Fame = 13500;
			Karma = -13500;

			VirtualArmor = 70;

			PackItem( new SulfurousAsh( 5 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Gems );
		}

		public override bool Unprovokable{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 4; } }
		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override int DefaultBloodHue{ get{ return 1260; } }

		public MagmaElemental( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);

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