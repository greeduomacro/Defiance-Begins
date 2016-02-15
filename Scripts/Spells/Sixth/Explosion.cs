using System;
using Server.Targeting;
using Server.Network;
using Server.Misc;

namespace Server.Spells.Sixth
{
	public class ExplosionSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Explosion", "Vas Ort Flam",
				230,
				9041,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot
			);

		public override SpellCircle Circle{ get{ return SpellCircle.Sixth; } }

		public ExplosionSpell( Mobile caster, Item scroll )
			: base( caster, scroll, m_Info )
		{
		}

		public override bool DelayedDamageStacking{ get{ return false; } }

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public override bool DelayedDamage{ get{ return !Core.AOS; } }

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( Caster.CanBeHarmful( m ) && CheckSequence() )
			{
				Mobile source = Caster, target = m;

				SpellHelper.Turn( source, target );

				SpellHelper.CheckReflect( (int) this.Circle, ref source, ref target );

				InternalTimer t = new InternalTimer( this, source, target, m );
				t.Start();
			}

			FinishSequence();
		}

		private class InternalTimer : Timer
		{
			private MagerySpell m_Spell;
			private Mobile m_Target;
			private Mobile m_Attacker, m_Defender;

			public InternalTimer( MagerySpell spell, Mobile attacker, Mobile defender, Mobile target )
				: base( TimeSpan.FromSeconds( Core.AOS ? 3.0 : 2.8 ) )
			{
				m_Spell = spell;
				m_Attacker = attacker;
				m_Defender = defender;
				m_Target = target;

				if ( m_Spell != null && m_Spell.DelayedDamage && !m_Spell.DelayedDamageStacking )
					m_Spell.StartDelayedDamageContext( attacker, this );

				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				if ( m_Attacker.HarmfulCheck( m_Defender ) )
				{
					double damage;

					if ( Core.AOS )
					{
						damage = m_Spell.GetNewAosDamage( 40, 1, 5, m_Target );
					}
					else
					{
						//damage = Utility.Random( 26, 9 ); // 26 - 34
						damage = Utility.Dice( 4, 3, 22 );

						if ( m_Spell.CheckResisted( m_Defender ) )
						{
							damage *= 0.70;

							m_Defender.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
						}

						damage *= m_Spell.GetDamageScalar( m_Target );
					}

					m_Defender.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
					m_Defender.PlaySound( 0x307 );

					SpellHelper.Damage( m_Spell, m_Defender, damage, 0, 100, 0, 0, 0, m_Target );

					if ( m_Spell != null )
						m_Spell.RemoveDelayedDamageContext( m_Attacker );
				}
			}
		}

		private class InternalTarget : Target
		{
			private ExplosionSpell m_Owner;

			public InternalTarget( ExplosionSpell owner )
				: base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile) o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}