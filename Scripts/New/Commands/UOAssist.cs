using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Commands
{
	public class UOAssist
	{
		public static void Initialize()
		{
			CommandSystem.Register( "Band", AccessLevel.Player, new CommandEventHandler( Band_OnCommand ) );
			CommandSystem.Register( "BandSelf", AccessLevel.Player, new CommandEventHandler( BandSelf_OnCommand ) );
		}

		[Usage( "Band" )]
		[Description( "Uses a bandage if any available." )]
		public static void Band_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			Container backpack = from.Backpack;

			if ( backpack != null )
			{
				Bandage bandage = (Bandage)backpack.FindItemByType( typeof(Bandage) );
				if ( bandage != null )
				{
					from.SendLocalizedMessage( 500948 ); // Who will you use the bandages on?
					from.Target = new InternalTarget( bandage );
				}
			}
		}

		private class InternalTarget : Target
		{
			private Bandage m_Bandage;

			public InternalTarget( Bandage bandage ) : base( Bandage.Range, false, TargetFlags.Beneficial )
			{
				m_Bandage = bandage;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Bandage.Deleted )
					return;

				if ( targeted is Mobile )
				{
					if ( from.InRange( m_Bandage.GetWorldLocation(), Bandage.Range ) )
					{
						if ( BandageContext.BeginHeal( from, (Mobile)targeted ) != null )
							m_Bandage.Consume();
					}
					else
						from.SendLocalizedMessage( 500295 ); // You are too far away to do that.
				}
				else if ( targeted is PlagueBeastInnard )
				{
					if ( ((PlagueBeastInnard) targeted).OnBandage( from ) )
						m_Bandage.Consume();
				}
				else
				{
					from.SendLocalizedMessage( 500970 ); // Bandages can not be used on that.
				}
			}

			protected override void OnNonlocalTarget( Mobile from, object targeted )
			{
				if ( targeted is PlagueBeastInnard )
				{
					if ( ((PlagueBeastInnard) targeted).OnBandage( from ) )
						m_Bandage.Consume();
				}
				else
					base.OnNonlocalTarget( from, targeted );
			}
		}

		[Usage( "BandSelf" )]
		[Description( "Uses a bandage on yourself if any available." )]
		public static void BandSelf_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;

			if( from != null)
			{
				Container backpack = from.Backpack;

				if( backpack != null )
				{
					Bandage bandage = (Bandage) backpack.FindItemByType( typeof( Bandage ) );

					if ( bandage != null )
					{
						Targeting.Target.Cancel( from );

						from.RevealingAction();

						if ( BandageContext.BeginHeal( from, from ) != null )
							bandage.Consume();
					}
				}
			}
		}
	}
}