using System;
using Server.Targeting;
using Server.Network;
using Server.Misc;

namespace Server.Spells.Fourth
{
	public class LightningSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Lightning", "Por Ort Grav",
				239,
				9021,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

		public LightningSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				Mobile source = Caster, target = m;

				SpellHelper.CheckReflect( (int)this.Circle, ref source, ref target );

				double damage;

				if ( Core.AOS )
				{
					damage = GetNewAosDamage( 23, 1, 4, m );
				}
				else
				{
					//damage = Utility.Random( 10, 7 ); // 10 - 16 damage
					damage = Utility.Dice( 2, 4, 8 );

					if ( CheckResisted( target ) )
					{
						damage *= 0.65;

						target.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
					}

					damage *= GetDamageScalar( m );
				}

				Timer.DelayCall<DamageInfo>( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback<DamageInfo>( FinishDamage ), new DamageInfo( target, damage, m ) );
			}

			FinishSequence();
		}

		private class DamageInfo
		{
			public Mobile OrigTarget;
			public Mobile Target;
			public double Damage;

			public DamageInfo( Mobile target, double damage, Mobile origTarget )
			{
				Target = target;
				Damage = damage;
				OrigTarget = origTarget;
			}
		}

		private void FinishDamage( DamageInfo info )
		{
			Mobile m = info.Target;
			double damage = info.Damage;

			m.BoltEffect( 0 );

			SpellHelper.Damage( this, m, damage, 0, 0, 0, 0, 100, info.OrigTarget );
		}

		private class InternalTarget : Target
		{
			private LightningSpell m_Owner;

			public InternalTarget( LightningSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}