using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Engines.Harvest;
using Server.Engines.CannedEvil;

namespace Server.Mobiles
{
	public class GrimReaper : BaseChampion
	{
		private Mobile m_Target;

		public override Mobile ConstantFocus{ get{ return m_Target; } }
		public override bool NoHouseRestrictions{ get{ return true; } }

		public override string DefaultName{ get{ return "Grim Reaper"; } }

		public override ChampionSkullType SkullType{ get{ return ChampionSkullType.None; } }

		public GrimReaper() : base( AIType.AI_Melee, FightMode.Closest, 12, 1, 0.1, 0.15 )
		{
			Body = 400;
			Hue = 1;

			SetStr( 1000 );
			SetDex( 175 );
			SetInt( 650 );
			SetMana( 300 );

			SetDamage( 20, 25 );
			SetHits( 5599, 6599 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Cold, 50 );

			SetSkill( SkillName.MagicResist, 125.0 ); 
			SetSkill( SkillName.Tactics, 125.0 );
			SetSkill( SkillName.Swords, 125.0 ); // not displayed in animal lore but tests clearly show this is influenced
			SetSkill( SkillName.DetectHidden, 100.0 );
			SetSkill( SkillName.Anatomy, 125.0 );

			SetResistance( ResistanceType.Physical, 100 );
			SetResistance( ResistanceType.Cold, 100 );
			SetResistance( ResistanceType.Fire, 60, 75 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 60, 75 );

			Fame = 30000;
			Karma = -30000;

			VirtualArmor = 75;

			Item shroud = new DeathShroud();

			shroud.Hue = 0x455;

			shroud.Movable = false;

			AddItem( shroud );

			GrimReaperScythe weapon = new GrimReaperScythe();

			weapon.Movable = false;

			AddItem( weapon );
		}

		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool BleedImmune{ get{ return true; } }
		public override bool BardImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int DefaultBloodHue{ get{ return -2; } }
		public override int BloodHueTemplate{ get{ return Utility.RandomBlackHue(); } }
		public override double WeaponAbilityChance{ get{ return 0.5; } }

		public override Type[] UniqueList{ get{ return new Type[]{ typeof( Scythe ) }; } }
		public override Type[] SharedList{ get{ return new Type[0]; } }
		public override Type[] DecorativeList{ get{ return new Type[0]; } }
		public override MonsterStatuetteType[] StatueTypes{ get{ return new MonsterStatuetteType[0]; } }

		public override bool NoGoodies{ get{ return true; } }

		public override Item GetArtifact()
		{
			return CreateArtifact( UniqueList );
		}

		public override WeaponAbility GetWeaponAbility()
		{
			WeaponAbility a = null;

			switch ( Utility.Random( 5 ) )
			{
				default:
				case 0: a = WeaponAbility.BleedAttack; break;
				case 1: a = WeaponAbility.WhirlwindAttack; break;
				case 2: a = WeaponAbility.ParalyzingBlow; break;
				case 3: a = WeaponAbility.CrushingBlow; break;
				case 4: a = WeaponAbility.MortalStrike; break;
			}

			return a;
		}

		private bool CanPath()
		{
			IPoint3D p = m_Target as IPoint3D;

			if ( p == null )
				return false;

			Point3D newP = new Point3D( p );

			if ( InRange( newP, 1 ) )
				return true;

			MovementPath path = new MovementPath( this, newP );

			return path.Success;
		}

		public override void OnThink()
		{
			//Do we have a valid target?
			if ( m_Target != null )
			{
				//Lets not go after dead players, logged out players, blessed mobs, or staff
				if ( m_Target.Alive && m_Target.Map != null && m_Target.Map != Map.Internal && !m_Target.Blessed && m_Target.AccessLevel == AccessLevel.Player )
				{
					if ( Map != m_Target.Map || !InRange( m_Target, 15 ) || !CanPath() )
					{
						Map fromMap = Map;
						Point3D from = Location;

						Map toMap = m_Target.Map;
						Point3D to = m_Target.Location;

						if ( (Hits / 3.0) <= HitsMax && ( Map != HomeMap || !InRange( Home, 18 ) ) ) //Return home, less than 33% health, and not near the altar!
						{
							//Reset to the altar!
							toMap = HomeMap;
							to = Home;
							m_Target = null;
							Combatant = null;
							FocusMob = null;
						}

						if ( toMap != null )
						{
							for ( int i = 0; i < 10; ++i )
							{
								Point3D loc = new Point3D( to.X - 4 + Utility.Random( 9 ), to.Y - 4 + Utility.Random( 9 ), to.Z );

								if ( toMap.CanSpawnMobile( loc ) )
								{
									to = loc;
									break;
								}
								else
								{
									loc.Z = toMap.GetAverageZ( loc.X, loc.Y );

									if ( toMap.CanSpawnMobile( loc ) )
									{
										to = loc;
										break;
									}
								}
							}
						}

						MoveToWorld( to, toMap );

						ProcessDelta();
					}
					
					if ( m_Target != null )
					{
						if ( m_Target.Hidden && InRange( m_Target, 4 ) && DateTime.Now >= this.NextSkillTime && UseSkill( SkillName.DetectHidden ) )
						{
							Target targ = this.Target;

							if ( targ != null )
								targ.Invoke( this, this );
						}

						Combatant = m_Target;
						FocusMob = m_Target;

						if ( AIObject != null )
							AIObject.Action = ActionType.Combat;
					}
				}
				else
					m_Target = null;
			}

			base.OnThink();
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			if ( c is Corpse ) //Sanity check?
			{
				c.ItemID = 0xECA;
				c.Hue = 1899;
			}
		}

		public GrimReaper( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}

namespace Server.Items
{
	public class GrimReaperScythe : Scythe
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int AosStrengthReq{ get{ return 45; } }
		public override int AosMinDamage{ get{ return 15; } }
		public override int AosMaxDamage{ get{ return 18; } }
		public override int AosSpeed{ get{ return 32; } }
		public override float MlSpeed{ get{ return 3.50f; } }

		public override int OldStrengthReq{ get{ return 150; } }
		public override int OldMinDamage{ get{ return 14; } }
		public override int OldMaxDamage{ get{ return 28; } }
		public override int DiceDamage { get { return Utility.Dice( 7, 3, 8 ); } }
		public override int OldSpeed{ get{ return 45; } }

		public override HarvestSystem HarvestSystem{ get{ return null; } }

		public GrimReaperScythe() : base()
		{
		}

		public GrimReaperScythe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}