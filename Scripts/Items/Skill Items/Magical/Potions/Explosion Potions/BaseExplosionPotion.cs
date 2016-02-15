using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Spells;

namespace Server.Items
{
	public abstract class BaseExplosionPotion : BasePotion
	{
		//public abstract int MinDamage { get; }
		//public abstract int MaxDamage { get; }
		public abstract int Damage{ get; }
		public abstract double Delay { get; }

		public override bool RequireFreeHand{ get{ return true; } }

		private static bool LeveledExplosion = false; // Should explosion potions explode other nearby potions?
		private static bool InstantExplosion = false; // Should explosion potions explode on impact?
		private static bool RelativeLocation = false; // Is the explosion target location relative for mobiles?
		private const int   ExplosionRange   = 2;     // How long is the blast radius?

		public BaseExplosionPotion( PotionEffect effect ) : base( 0xF0D, effect )
		{
		}

		public BaseExplosionPotion( Serial serial ) : base( serial )
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

		public virtual IEntity FindParent( Mobile from )
		{
			Mobile m = this.HeldBy;

			if ( m != null )
				return m;

			IEntity obj = this.RootParentEntity;

			if ( obj != null )
				return obj;

			if ( Map == Map.Internal )
				return from;

			return this;
		}

		private Timer m_Timer;

//		private List<Mobile> m_Users;

		public override void Drink( Mobile from )
		{
			if ( Core.AOS && (from.Paralyzed || from.Frozen || (from.Spell != null && from.Spell.IsCasting)) )
				from.SendLocalizedMessage( 1062725 ); // You can not use that potion while paralyzed.
			else
			{
				ThrowTarget targ = from.Target as ThrowTarget;
				this.Stackable = false; // Scavenged explosion potions won't stack with those ones in backpack, and still will explode.

				if ( targ != null && targ.Potion == this ) //Already have a targeter from this potion
					return;

				from.RevealingAction();

//				if ( m_Users == null )
//					m_Users = new List<Mobile>();

				if ( m_Timer == null ) //Is this already ticking?
				{
					if ( from.BeginAction( typeof( BaseExplosionPotion ) ) ) //Can we throw another potion?
					{
//						if ( !m_Users.Contains( from ) )
//							m_Users.Add( from );

						from.Target = new ThrowTarget( this );

						from.SendLocalizedMessage( 500236 ); // You should throw it now!

						int count = Utility.Random( 3, 2 );

						if( Core.ML )
							m_Timer = Timer.DelayCall<ExplodeCount>( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.25 ), 5, new TimerStateCallback<ExplodeCount>( Detonate_OnTick ), new ExplodeCount( from, count ) ); // 3.6 seconds explosion delay
						else
							m_Timer = Timer.DelayCall<ExplodeCount>( TimeSpan.FromSeconds( 0.75 ), TimeSpan.FromSeconds( 1.0 ), 4, new TimerStateCallback<ExplodeCount>( Detonate_OnTick ), new ExplodeCount( from, count ) ); // 2.6 seconds explosion delay

						Timer.DelayCall<Mobile>( TimeSpan.FromSeconds( Delay ), new TimerStateCallback<Mobile>( ReleaseExplosionLock ), from );
					}
					else
						from.LocalOverheadMessage( MessageType.Regular, 0x22, false, "You must wait a moment before using another explosion potion." );
				}
				else
					from.Target = new ThrowTarget( this );
			}
		}

		private static void ReleaseExplosionLock( Mobile from )
		{
			from.EndAction( typeof( BaseExplosionPotion ) );
		}

		private class ExplodeCount
		{
			public Mobile From;
			public int Count;

			public ExplodeCount( Mobile from, int count )
			{
				From = from;
				Count = count;
			}
		}

		private void Detonate_OnTick( ExplodeCount counter )
		{
			if ( Deleted )
				return;

			Mobile from = counter.From;
			int timer = counter.Count--;

//			from.SendMessage( "Click!" );

			IEntity parent = FindParent( from );

			if ( timer == 0 )
			{
				Explode( from, true, parent.Location, parent.Map );
				m_Timer = null;
			}
			else
			{
				if ( parent is Item )
					((Item)parent).PublicOverheadMessage( MessageType.Regular, 0x22, false, timer.ToString() );
				else if ( parent is Mobile )
					((Mobile)parent).PublicOverheadMessage( MessageType.Regular, 0x22, false, timer.ToString() );
			}
		}

		private void Reposition_OnTick( IEntity[] states )
		{
			if ( Deleted )
				return;

			if ( InstantExplosion )
				Explode( (Mobile)states[0], true, states[1].Location, states[1].Map );
			else
				MoveToWorld( states[1].Location, states[1].Map );
		}

		private class ThrowTarget : Target
		{
			private BaseExplosionPotion m_Potion;

			public BaseExplosionPotion Potion
			{
				get{ return m_Potion; }
			}

			public ThrowTarget( BaseExplosionPotion potion ) : base( 12, true, TargetFlags.None )
			{
				m_Potion = potion;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Potion.Deleted || m_Potion.Map == Map.Internal )
					return;

				IPoint3D p = targeted as IPoint3D;

				if ( p == null )
					return;

				Map map = from.Map;

				if ( map == null )
					return;

				SpellHelper.GetSurfaceTop( ref p );

				from.RevealingAction();

				IEntity to = new Entity( Serial.Zero, new Point3D( p ), map );

				Effects.SendMovingEffect( from, ( RelativeLocation && p is Mobile ) ? (Mobile)p : to, m_Potion.ItemID, 7, 0, false, false, m_Potion.Hue, 0 );

				if ( m_Potion.Amount > 1 )
					Mobile.LiftItemDupe( m_Potion, 1 );

				m_Potion.Internalize();
				Timer.DelayCall<IEntity[]>( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback<IEntity[]>( m_Potion.Reposition_OnTick ), new IEntity[]{ from, to } );
			}
		}

		public void Explode( Mobile from, bool direct, Point3D loc, Map map )
		{
			if ( Deleted )
				return;

			Consume();

			ThrowTarget targ = from.Target as ThrowTarget;

			if ( targ != null && targ.Potion == this ) //Blew yourself up without throwing it, cancel target now
				Target.Cancel( from );

			if ( map == null )
				return;

			Effects.PlaySound( loc, map, 0x307 );
			Effects.SendLocationEffect( loc, map, 0x36BD, 9, 10, 0, 0 );

			int alchemyBonus = 0;

			if ( direct )
				alchemyBonus = (int)(from.Skills.Alchemy.Value / (Core.AOS ? 5 : 10));

			IPooledEnumerable eable = LeveledExplosion ? map.GetObjectsInRange( loc, ExplosionRange ) : map.GetMobilesInRange( loc, ExplosionRange );
			ArrayList toExplode = new ArrayList();

			int toDamage = 0;

			foreach ( IEntity o in eable )
			{
				if ( o is Mobile && (from == null || (SpellHelper.ValidIndirectTarget( from, (Mobile)o ) && from.CanBeHarmful( (Mobile)o, false ))))
				{
					toExplode.Add( o );
					++toDamage;
				}
				else if ( o is BaseExplosionPotion && o != this )
				{
					toExplode.Add( o );
				}
			}

			eable.Free();

//			int min = Scale( from, MinDamage );
//			int max = Scale( from, MaxDamage );

			for ( int i = 0; i < toExplode.Count; ++i )
			{
				object o = toExplode[i];

				if ( o is Mobile )
				{
					Mobile m = (Mobile)o;

					if ( from != null )
					{
						if ( from != null )
							from.DoHarmful( m );

						int damage = Scale( from, Damage );

						damage += alchemyBonus;

						if ( /*!Core.AOS &&*/ damage > 40 )
							damage = 40;

						if ( /*Core.AOS &&*/ toDamage > 2 )
							damage /= toDamage - 1;

						if ( from is PlayerMobile )
							damage /= 2;

						AOS.Damage( m, from, damage, 0, 100, 0, 0, 0 );
					}
				}
				else if ( o is BaseExplosionPotion )
				{
					BaseExplosionPotion pot = (BaseExplosionPotion)o;

					pot.Explode( from, false, pot.GetWorldLocation(), pot.Map );
				}
			}
		}
	}
}