using Server;
using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Spells;
using Server.Mobiles;

namespace Server.Regions
{
	public class CustomRegion : GuardedRegion
	{
		private RegionControl m_Controller;

		public RegionControl Controller
		{
			get { return m_Controller; }
		}

		public CustomRegion(RegionControl control): base(control.RegionName, control.Map, control.RegionPriority, control.RegionArea)
		{
			Disabled = !control.IsGuarded;
			Music = control.Music;
			m_Controller = control;
		}

		public override void OnDeath( Mobile m )
		{
			if (m != null && !m.Deleted)
			{
				if (m is PlayerMobile && m_Controller.NoPlayerItemDrop)
				{
					if (m.Female)
					{
						m.FixedParticles(0x374A, 10, 30, 5013, 1153, 2, EffectLayer.Waist);
						m.Body = 403;
						m.Hidden = true;
					}
					else
					{
						m.FixedParticles(0x374A, 10, 30, 5013, 1153, 2, EffectLayer.Waist);
						m.Body = 402;
						m.Hidden = true;
					}
					m.Hidden = false;
				}
				else if (!(m is PlayerMobile) && m_Controller.NoNPCItemDrop)
				{
					if (m.Female)
					{
						m.FixedParticles(0x374A, 10, 30, 5013, 1153, 2, EffectLayer.Waist);
						m.Body = 403;
						m.Hidden = true;
					}
					else
					{
						m.FixedParticles(0x374A, 10, 30, 5013, 1153, 2, EffectLayer.Waist);
						m.Body = 402;
						m.Hidden = true;
					}
					m.Hidden = false;
				}

				// Start a 1 second timer
				// The Timer will check if they need moving, corpse deleting etc.
				//new MovePlayerTimer(m, m_Controller).Start();

				base.OnDeath( m );
			}
		}

		public void OnNPCDeath( Mobile m )
		{
			if ( m == null )
				return;

			Mobile newnpc = null;
			BaseCreature bc = m as BaseCreature;

			if ( m.Corpse != null )
			{
				if ( m_Controller.EmptyNPCCorpse )
				{
					List<Item> corpseitems = new List<Item>(m.Corpse.Items);

					foreach (Item item in corpseitems)
						if ((item.Layer != Layer.Bank) && (item.Layer != Layer.Backpack) && (item.Layer != Layer.Hair) && (item.Layer != Layer.FacialHair) && (item.Layer != Layer.Mount))
							if ((item.LootType != LootType.Blessed) && item.Movable )
								item.MoveToWorld(m.Corpse.Location, m.Corpse.Map);
				}

				if ( m_Controller.ResNPCOnDeath )
				{
					if ( bc != null && bc.IsBonded )
						m.Resurrect();
					else
					{
						try
						{
							Type type = m.GetType();
							newnpc = Activator.CreateInstance( type ) as Mobile;
							if (newnpc != null)
							{
								newnpc.Location = m.Corpse.Location;
								newnpc.Map = m.Corpse.Map;
							}
						}
						catch
						{
						}
					}
				}

				if ( m_Controller.DeleteNPCCorpse )
					m.Corpse.Delete();
			}

			if ( m_Controller.MoveNPCOnDeath )
			{
				if ( bc != null && bc.IsBonded )
					m.MoveToWorld( m_Controller.MoveNPCToLoc, m_Controller.MoveNPCToMap );
				else
					newnpc.MoveToWorld( m_Controller.MoveNPCToLoc, m_Controller.MoveNPCToMap );
			}
		}

		public void OnPlayerDeath( PlayerMobile pm )
		{
			if ( pm == null )
				return;

			// Emptys the corpse and places items on ground
			if ( pm.Corpse != null && m_Controller.EmptyPlayerCorpse )
			{
				List<Item> corpseitems = new List<Item>(pm.Corpse.Items);

				foreach ( Item item in corpseitems )
					if ( (item.Layer != Layer.Bank) && (item.Layer != Layer.Backpack) && (item.Layer != Layer.Hair) && (item.Layer != Layer.FacialHair) && (item.Layer != Layer.Mount) )
						if ( (item.LootType != LootType.Blessed) && item.Movable )
							item.MoveToWorld( pm.Corpse.Location, pm.Corpse.Map );
			}

			if ( m_Controller.ResPlayerOnDeath )
			{
				pm.Resurrect();
				pm.SendMessage("You have been resurrected!");
			}

			// Deletes the corpse
			if ( pm.Corpse != null && m_Controller.DeletePlayerCorpse )
				pm.Corpse.Delete();

			// Move Mobiles
			if ( m_Controller.MovePlayerOnDeath )
				pm.MoveToWorld( m_Controller.MovePlayerToLoc, m_Controller.MovePlayerToMap );
		}

		public override bool IsDisabled()
		{
			if (!m_Controller.IsGuarded != Disabled)
				m_Controller.IsGuarded = !Disabled;

			return Disabled;
		}

		public override bool AllowBeneficial( Mobile from, Mobile target )
		{
			if ((!m_Controller.AllowBenefitPlayer && target is PlayerMobile) || (!m_Controller.AllowBenefitNPC && target is BaseCreature))
			{
				from.SendMessage("You cannot perform beneficial acts on your target.");
				return false;
			}

			return base.AllowBeneficial( from, target );
		}

		public override bool AllowHarmful( Mobile from, Mobile target )
		{
			if ((!m_Controller.AllowHarmPlayer && target is PlayerMobile) || (!m_Controller.AllowHarmNPC && target is BaseCreature))
			{
				from.SendMessage( "You cannot perform harmful acts on your target." );
				return false;
			}

			return base.AllowHarmful( from, target );
		}

		public override bool AllowHousing( Mobile from, Point3D p )
		{
			return m_Controller.AllowHousing;
		}

		public override bool AllowSpawn()
		{
			return m_Controller.AllowSpawn;
		}

		public override bool CanUseStuckMenu( Mobile m )
		{
			if ( ! m_Controller.CanUseStuckMenu )
				m.SendMessage( "You cannot use the stuck menu here." );
			return m_Controller.CanUseStuckMenu;
		}

		public override bool OnDamage( Mobile m, ref int damage )
		{
			if ( !m_Controller.CanBeDamaged )
				m.SendMessage( "You cannot be damaged here." );

			return m_Controller.CanBeDamaged;
		}

		public override bool OnResurrect( Mobile m )
		{
			if ( ! m_Controller.CanRessurect && m.AccessLevel == AccessLevel.Player)
				m.SendMessage( "You cannot ressurect here." );
			return m_Controller.CanRessurect;
		}

		public override bool OnBeginSpellCast( Mobile from, ISpell s )
		{
			if ( from.AccessLevel == AccessLevel.Player )
			{
				bool restricted = m_Controller.IsRestrictedSpell( s );
				if ( restricted )
				{
					from.SendMessage( "You cannot cast that spell here." );
					return false;
				}

				//if ( s is EtherealSpell && !CanMountEthereal ) Grr, EthereealSpell is private :<
				if ( !m_Controller.CanMountEthereal && s is EtherealMount.EtherealSpell ) //Hafta check with a name compare of the string to see if ethy
				{
					from.SendMessage( "You cannot mount ethereals here." );
					return false;
				}
			}

			//Console.WriteLine( m_Controller.GetRegistryNumber( s ) );

			//return base.OnBeginSpellCast( from, s );
			return true;	//Let users customize spells, not rely on weather it's guarded or not.
		}

		public override bool OnDecay( Item item )
		{
			return m_Controller.ItemDecay;
		}

		public override bool OnHeal( Mobile m, ref int heal )
		{
			if ( !m_Controller.CanHeal )
			{
				m.SendMessage( "You cannot be healed here." );
			}

			return m_Controller.CanHeal;
		}

		public override bool OnSkillUse( Mobile m, int skill )
		{
			bool restricted = m_Controller.IsRestrictedSkill( skill );
			if ( restricted && m.AccessLevel == AccessLevel.Player )
			{
				m.SendMessage( "You cannot use that skill here." );
				return false;
			}

			return base.OnSkillUse( m, skill );
		}

		public override void OnExit( Mobile m )
		{
			if ( m_Controller.ShowExitMessage )
				m.SendMessage("You have left {0}", this.Name );

			base.OnExit( m );

		}

		public override void OnEnter( Mobile m )
		{
			if ( m_Controller.ShowEnterMessage )
				m.SendMessage("You have entered {0}", this.Name );

			base.OnEnter( m );
		}

		public override bool OnMoveInto( Mobile m, Direction d, Point3D newLocation, Point3D oldLocation )
		{
			if( m_Controller.CannotEnter && ! this.Contains( oldLocation ) && m.AccessLevel < AccessLevel.GameMaster )
			{
				m.SendMessage( "You cannot enter this area." );
				return false;
			}

			return true;
		}

		public override TimeSpan GetLogoutDelay( Mobile m )
		{
			if( m.AccessLevel == AccessLevel.Player )
				return m_Controller.PlayerLogoutDelay;

			return base.GetLogoutDelay( m );
		}

		public override bool OnDoubleClick( Mobile m, object o )
		{
			if( o is BasePotion && !m_Controller.CanUsePotions )
			{
				m.SendMessage( "You cannot drink potions here." );
				return false;
			}

			if( o is Corpse )
			{
				Corpse c = (Corpse)o;

				bool canLoot;

				if( c.Owner == m )
					canLoot = !m_Controller.CannotLootOwnCorpse;
				else if ( c.Owner is PlayerMobile )
					canLoot = m_Controller.CanLootPlayerCorpse;
				else
					canLoot = m_Controller.CanLootNPCCorpse;

				if( !canLoot )
				{
					if ( m.AccessLevel >= AccessLevel.GameMaster )
					{
						m.SendMessage( "This is unlootable but you are able to open that with your godly powers." );
						return true;
					}
					else
						m.SendMessage( "You cannot loot that corpse here." );
				}

				return canLoot;
			}

			return base.OnDoubleClick( m, o );
		}

		public override void AlterLightLevel( Mobile m, ref int global, ref int personal )
		{
			if ( m_Controller.LightLevel >= 0 )
				global = m_Controller.LightLevel;
			else
				base.AlterLightLevel( m, ref global, ref personal );
		}

		public bool CannotStun
		{
			get { return m_Controller.IsRestrictedSkill((int) SkillName.Anatomy); }
		}

		public bool CannotDisarm
		{
			get { return m_Controller.IsRestrictedSkill((int) SkillName.ArmsLore); }
		}

		public bool NoPoisonSkillEffects
		{
			get { return m_Controller.IsRestrictedSkill((int)SkillName.Poisoning); }
		}
	}
}