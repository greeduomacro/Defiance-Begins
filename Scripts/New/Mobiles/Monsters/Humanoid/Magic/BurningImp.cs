using System;
using Server;
using Server.Network;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an imp corpse" )]
	public class BurningImp : BaseCreature
	{
		public override string DefaultName{ get{ return "a burning imp"; } }

		[Constructable]
		public BurningImp() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 74;
			BaseSoundID = 422;
			Hue = 1257;

			SetStr( 145, 165 );
			SetDex( 61, 80 );
			SetInt( 106, 125 );

			SetHits( 105, 170 );

			SetDamage( 13, 17 );

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Fire, 50 );
			SetDamageType( ResistanceType.Poison, 50 );

			SetResistance( ResistanceType.Physical, 25, 35 );
			SetResistance( ResistanceType.Fire, 70, 90 );
			SetResistance( ResistanceType.Cold, 0 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.EvalInt, 50.1, 60.0 );
			SetSkill( SkillName.Magery, 65.1, 95.0 );
			SetSkill( SkillName.MagicResist, 65.1, 85.0 );
			SetSkill( SkillName.Tactics, 92.1, 100.0 );
			SetSkill( SkillName.Wrestling, 90.1, 94.0 );

			Fame = 4500;
			Karma = -4500;

			VirtualArmor = 33;

			Tamable = true;
			ControlSlots = 2;
			MinTameSkill = 93.1;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager, 3 );
			AddLoot( LootPack.MedScrolls, 2 );
			AddLoot( LootPack.VeryRareStatueDrop );
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int Meat{ get{ return 2; } }
		public override int Hides{ get{ return 3; } }
		public override HideType HideType{ get{ return HideType.Spined; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Daemon; } }
		public override bool CanRummageCorpses{ get{ return true; } }

		public BurningImp( Serial serial ) : base( serial )
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