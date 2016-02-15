using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "a lava serpent corpse" )]
	[TypeAlias( "Server.Mobiles.Lavaserpant" )]
	public class LavaSerpent : BaseCreature
	{
		public override string DefaultName{ get{ return "a lava serpent"; } }

		[Constructable]
		public LavaSerpent() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 90;
			BaseSoundID = 219;

			SetStr( 386, 415 );
			SetDex( 56, 80 );
			SetInt( 66, 85 );

			SetHits( 232, 249 );
			SetMana( 0 );

			SetDamage( 10, 22 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 80 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 70, 80 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.MagicResist, 25.3, 70.0 );
			SetSkill( SkillName.Tactics, 65.1, 70.0 );
			SetSkill( SkillName.Wrestling, 60.1, 80.0 );

			Fame = 4500;
			Karma = -4500;

			VirtualArmor = 40;

			PackItem( new SulfurousAsh( 3 ) );

			switch ( Utility.Random( 10 ) )
			{
				case 0: PackItem( new LeftArm() ); break;
				case 1: PackItem( new RightArm() ); break;
				case 2: PackItem( new Torso() ); break;
				case 3: PackItem( new Bone() ); break;
				case 4: PackItem( new RibCage() ); break;
				case 5: PackItem( new RibCage() ); break;
				case 6: PackItem( new BonePile() ); break;
				case 7: PackItem( new BonePile() ); break;
				case 8: PackItem( new BonePile() ); break;
				case 9: PackItem( new BonePile() ); break;
			}
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
		}

		public override bool DeathAdderCharmable{ get{ return true; } }

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int Meat{ get{ return 4; } }
		public override int Hides{ get{ return 15; } }
		public override HideType HideType{ get{ return HideType.Spined; } }

		public LavaSerpent(Serial serial) : base(serial)
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