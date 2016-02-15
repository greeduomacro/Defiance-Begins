using System;
using Server;
using Server.Network;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an ice fiend corpse" )]
	public class IceFiend : BaseCreature
	{
		public override string DefaultName{ get{ return "an ice fiend"; } }

		[Constructable]
		public IceFiend () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 43;
			BaseSoundID = 357;

			SetStr( 376, 405 );
			SetDex( 176, 195 );
			SetInt( 201, 225 );

			SetHits( 276, 343 );

			SetDamage( 8, 19 );

			SetSkill( SkillName.EvalInt, 80.1, 90.0 );
			SetSkill( SkillName.Magery, 80.1, 90.0 );
			SetSkill( SkillName.MagicResist, 75.1, 85.0 );
			SetSkill( SkillName.Tactics, 80.1, 90.0 );
			SetSkill( SkillName.Wrestling, 80.1, 100.0 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 60, 70 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			Fame = 18000;
			Karma = -18000;

			VirtualArmor = 60;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Average );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override int TreasureMapLevel{ get{ return 4; } }
		public override int Meat{ get{ return 1; } }

		public IceFiend( Serial serial ) : base( serial )
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
						AOS.Damage( m, this, Utility.RandomMinMax( 5, 10 ), 0, 0, 100, 0, 0 );
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