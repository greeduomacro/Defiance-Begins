using System;
using Server.Network;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a fire rabbit corpse" )]
	public class FireRabbit : BaseCreature
	{
		public override string DefaultName{ get{ return "a fire rabbit"; } }

		[Constructable]
		public FireRabbit() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 205;
			Hue = 1260;

			SetStr( 30 );
			SetDex( 2850 );
			SetInt( 1000 );

			SetHits( 2400 );
			SetStam( 750 );
			SetMana( 0 );

			SetDamage( 3 );

			SetDamageType( ResistanceType.Physical, 80 );
			SetDamageType( ResistanceType.Physical, 20 );

			SetSkill( SkillName.MagicResist, 260.0 );
			SetSkill( SkillName.Tactics, 15.0 );
			SetSkill( SkillName.Wrestling, 15.0 );

			Fame = 2500;
			Karma = 0;

			VirtualArmor = 8;

			int carrots = Utility.RandomMinMax( 6, 12 );
			PackItem( new Carrot( carrots ) );

			if ( Utility.Random( 30 ) == 0 )
				PackItem( new AnimalPheromone() );

			//PackStatue();

			PackItem( new SulfurousAsh( 10 ) );

			DelayBeginTunnel();
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.RareStatueDrop );
		}

		public class BunnyHole : Item
		{
			public BunnyHole() : base( 0x913 )
			{
				Movable = false;
				Hue = 1;
				Name = "a mysterious rabbit hole";

				Timer.DelayCall( TimeSpan.FromSeconds( 40.0 ), new TimerCallback( Delete ) );
			}

			public BunnyHole( Serial serial ) : base( serial )
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

				Delete();
			}
		}

		public virtual void DelayBeginTunnel()
		{
			Timer.DelayCall( TimeSpan.FromMinutes( 2.0 ), new TimerCallback( BeginTunnel ) );
		}

		public virtual void BeginTunnel()
		{
			if ( Deleted )
				return;

			new BunnyHole().MoveToWorld( Location, Map );

			Frozen = true;
			Say( "* The bunny begins to dig a tunnel back to its underground lair *" );
			PlaySound( 0x247 );

			Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerCallback( Delete ) );
		}

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 1; } }
		public override bool BardImmune{ get{ return !Core.AOS; } }

		public FireRabbit( Serial serial ) : base( serial )
		{
		}

		public override int GetAttackSound()
		{
			return 0xC9;
		}

		public override int GetHurtSound()
		{
			return 0xCA;
		}

		public override int GetDeathSound()
		{
			return 0xCB;
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

			DelayBeginTunnel();
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