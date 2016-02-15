using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a skeletal dragon corpse" )]
	public class SkeletalDragon : BaseCreature
	{
		public override string DefaultName{ get{ return "a skeletal dragon"; } }

		[Constructable]
		public SkeletalDragon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 104;
			BaseSoundID = 0x488;

			SetStr( 898, 1030 );
			SetDex( 68, 200 );
			SetInt( 488, 620 );

			SetHits( 558, 599 );

			SetDamage( 29, 35 );

			SetDamageType( ResistanceType.Physical, 75 );
			SetDamageType( ResistanceType.Fire, 25 );

			SetResistance( ResistanceType.Physical, 75, 80 );
			SetResistance( ResistanceType.Fire, 40, 60 );
			SetResistance( ResistanceType.Cold, 40, 60 );
			SetResistance( ResistanceType.Poison, 70, 80 );
			SetResistance( ResistanceType.Energy, 40, 60 );

			SetSkill( SkillName.EvalInt, 80.1, 100.0 );
			SetSkill( SkillName.Magery, 80.1, 100.0 );
			SetSkill( SkillName.MagicResist, 100.3, 130.0 );
			SetSkill( SkillName.Tactics, 97.6, 100.0 );
			SetSkill( SkillName.Wrestling, 97.6, 100.0 );

			Fame = 22500;
			Karma = -22500;

			VirtualArmor = 80;
		}

		public override void GenerateLoot( bool spawning )
		{
			base.GenerateLoot( spawning );
			if ( !spawning && Utility.Random( 1000 ) == 0 )
				PackItem( new SkeletalDragonScale() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 4 );
			AddLoot( LootPack.Gems, 5 );
		}

		public override bool ReacquireOnMovement{ get{ return true; } }
		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int BreathFireDamage{ get{ return 0; } }
		public override int BreathColdDamage{ get{ return 100; } }
		public override int BreathEffectHue{ get{ return 0x480; } }
		public override double BonusPetDamageScalar{ get{ return (Core.SE)? 3.0 : 1.0; } }
		public override int DefaultBloodHue{ get{ return -1; } }

		public override bool AutoDispel{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override bool BleedImmune{ get{ return true; } }
		//public override int Meat{ get{ return 19; } } // where's it hiding these? :)
		//public override int Hides{ get{ return 20; } }
		//public override HideType HideType{ get{ return HideType.Barbed; } }

		public SkeletalDragon( Serial serial ) : base( serial )
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

		private DateTime m_NextAttack;

		public override void OnActionCombat()
		{
			Mobile combatant = Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != Map || !InRange( combatant, 12 ) || !CanBeHarmful( combatant ) || !InLOS( combatant ) )
				return;

			if ( !Paralyzed && DateTime.Now >= m_NextAttack && Utility.Random( 5 ) == 0 )
			{
				SummonUndead( combatant );
				m_NextAttack = DateTime.Now + TimeSpan.FromSeconds( 8.0 + (8.0 * Utility.RandomDouble()) );
			}
		}

		public void SummonUndead( Mobile target )
		{
			BaseCreature summon = null;

			switch ( Utility.Random( 11 ) )
			{
				default:
				case 0: summon = new Skeleton(); break;
				case 1: summon = new Zombie(); break;
				case 2: summon = new Wraith(); break;
				case 3: summon = new Spectre(); break;
				case 4: summon = new Ghoul(); break;
				case 5: summon = new Mummy(); break;
				case 6: summon = new Bogle(); break;
				case 7: summon = new BoneKnight(); break;
				case 8: summon = new SkeletalKnight(); break;
				case 9: summon = new Lich(); break;
				case 10: summon = new SkeletalMage(); break;
			}

			summon.Team = this.Team;
			summon.FightMode = FightMode.Closest;
			summon.MoveToWorld( target.Location, target.Map );
			Effects.SendLocationEffect( summon.Location, summon.Map, 0x3728, 10, 10, 0, 0 );
			summon.Combatant = target;
			summon.PlaySound( summon.GetAttackSound() );
		}
	}
}