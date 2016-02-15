using System;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Seventh
{
	public class FlameStrikeSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Flame Strike", "Kal Vas Flam",
				245,
				9042,
				Reagent.SpidersSilk,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

		public FlameStrikeSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return true; } }

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
			{
				Mobile source = Caster, target = m;

				SpellHelper.Turn( source, target );

				SpellHelper.CheckReflect( (int)this.Circle, ref source, ref target );

				double damage;

				if ( Core.AOS )
				{
					damage = GetNewAosDamage( 48, 1, 5, m );
				}
				else
				{
					//damage = Utility.Random( 29, 16 ); // 29 - 44
					damage = Utility.Dice( 3, 6, 26 );

					if ( CheckResisted( target ) )
					{
						damage *= 0.75;

						target.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
					}

					damage *= GetDamageScalar( m );
				}

				target.FixedParticles( 0x3709, 10, 30, 5052, EffectLayer.LeftFoot );
				target.PlaySound( 0x208 );

				SpellHelper.Damage( this, target, damage, 0, 100, 0, 0, 0, m );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private FlameStrikeSpell m_Owner;

			public InternalTarget( FlameStrikeSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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