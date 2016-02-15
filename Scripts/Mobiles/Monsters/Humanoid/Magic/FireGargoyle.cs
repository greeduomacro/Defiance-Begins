using System;
using Server;
using Server.Network;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a charred corpse" )]
	public class FireGargoyle : BaseCreature
	{
		[Constructable]
		public FireGargoyle() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "fire gargoyle" );
			Body = 130;
			BaseSoundID = 0x174;

			SetStr( 351, 400 );
			SetDex( 126, 145 );
			SetInt( 226, 250 );

			SetHits( 211, 240 );

			SetDamage( 7, 14 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 80 );

			SetResistance( ResistanceType.Physical, 30, 35 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.Anatomy, 75.1, 85.0 );
			SetSkill( SkillName.EvalInt, 90.1, 105.0 );
			SetSkill( SkillName.Magery, 90.1, 105.0 );
			SetSkill( SkillName.Meditation, 90.1, 105.0 );
			SetSkill( SkillName.MagicResist, 90.1, 105.0 );
			SetSkill( SkillName.Tactics, 80.1, 100.0 );
			SetSkill( SkillName.Wrestling, 40.1, 80.0 );

			Fame = 3500;
			Karma = -3500;

			VirtualArmor = 32;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Gems );
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int TreasureMapLevel{ get{ return 1; } }
		public override int Meat{ get{ return 1; } }

		public FireGargoyle( Serial serial ) : base( serial )
		{
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