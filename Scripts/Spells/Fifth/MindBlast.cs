using System;
using Server.Targeting;
using Server.Network;
using Server.Misc;

namespace Server.Spells.Fifth
{
	public class MindBlastSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Mind Blast", "Por Corp Wis",
				218,
				Core.AOS ? 9002 : 9032,
				Reagent.BlackPearl,
				Reagent.MandrakeRoot,
				Reagent.Nightshade,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public MindBlastSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
			if ( Core.AOS )
				m_Info.LeftHandEffect = m_Info.RightHandEffect = 9002;
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		private class DelayInfo
		{
			public Mobile OrigTarget;
			public Mobile Target;
			public Mobile Defender;
			public int Damage;

			public DelayInfo( Mobile target, Mobile defender, int damage, Mobile origTarget )
			{
				Target = target;
				Defender = defender;
				Damage = damage;
				OrigTarget = origTarget;
			}
		}

		private void AosDelay_Callback( DelayInfo info )
		{
			Mobile target = info.Target;
			Mobile defender = info.Defender;
			int damage = info.Damage;

			if ( Caster.HarmfulCheck( defender ) )
			{
				SpellHelper.Damage( this, target, Utility.Random( damage, 5 ), 0, 0, 100, 0, 0, info.OrigTarget );

				target.FixedParticles( 0x374A, 10, 15, 5038, 1181, 2, EffectLayer.Head );
				target.PlaySound( 0x213 );
			}
		}

		public override bool DelayedDamage{ get{ return !Core.AOS; } }

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( Core.AOS )
			{
				if ( Caster.CanBeHarmful( m ) && CheckSequence() )
				{
					Mobile from = Caster, target = m;

					SpellHelper.Turn( from, target );

					SpellHelper.CheckReflect( (int)this.Circle, ref from, ref target );

					int damage = (int)((Caster.Skills[SkillName.Magery].Value + Caster.Int) / 5);

					if ( damage > 60 )
						damage = 60;

					Timer.DelayCall<DelayInfo>( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback<DelayInfo>( AosDelay_Callback ), new DelayInfo( target, from, damage, m ) );
				}
			}
			else if ( CheckHSequence( m ) )
			{
				Mobile source = Caster, target = m;

				SpellHelper.Turn( source, target );

				SpellHelper.CheckReflect( (int)this.Circle, ref source, ref target );

				// Algorithm: (highestStat - lowestStat) / 2 [- 50% if resisted]

				int highestStat = target.Str, lowestStat = target.Str;

				if ( target.Dex > highestStat )
					highestStat = target.Dex;

				if ( target.Dex < lowestStat )
					lowestStat = target.Dex;

				if ( target.Int > highestStat )
					highestStat = target.Int;

				if ( target.Int < lowestStat )
					lowestStat = target.Int;

				if ( highestStat > 90 )
					highestStat = 90;

				if ( lowestStat > 90 )
					lowestStat = 90;

				double damage = GetDamageScalar(m)*(highestStat - lowestStat) / 2; //less damage

				if ( damage > 35 )
					damage = 35;

				if ( CheckResisted( target ) )
				{
					damage /= 2;
					target.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
				}

				source.FixedParticles( 0x374A, 10, 15, 2038, EffectLayer.Head );

				target.FixedParticles( 0x374A, 10, 15, 5038, EffectLayer.Head );
				target.PlaySound( 0x213 );

				SpellHelper.Damage( this, TimeSpan.FromSeconds( 1.2 ), target, damage, m );
			}

			FinishSequence();
		}

		public override double GetSlayerDamageScalar( Mobile target )
		{
			return 1.0; //This spell isn't affected by slayer spellbooks
		}

		private class InternalTarget : Target
		{
			private MindBlastSpell m_Owner;

			public InternalTarget( MindBlastSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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