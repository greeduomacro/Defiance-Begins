using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Commands;

namespace Server
{
	public class Currency
	{
		public static void Initialize()
		{
			CommandSystem.Register( "Consume", AccessLevel.GameMaster, new CommandEventHandler( ConsumeGold_OnCommand ) );
		}

		[Usage( "Consume [amount]" )]
		[Description( "Consumes money from a player." )]
		private static void ConsumeGold_OnCommand(CommandEventArgs e)
		{
			Mobile from = e.Mobile;

			if ( e.Length != 1 )
			{
				from.SendMessage("Invalid Arguements");
				return;
			}

			int amount = Utility.ToInt32(e.Arguments[0]);
			if ( amount <= 0 )
				from.SendMessage( "Invalid amount specified." );
			else
			{
				from.Target = new ConsumeGoldTarget( amount );
				from.SendMessage("Who would you like to consume gold/checks from?");
			}
		}

		private class ConsumeGoldTarget : Target
		{
			private int m_Amount;

			public ConsumeGoldTarget( int amount ) : base( 15, false, TargetFlags.None )
			{
				m_Amount = amount;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Mobile )
				{
					Mobile m = (Mobile)targeted;

					int totalGold = 0;
					List<Gold> packgold;
					List<BankCheck> packchecks;
					Gold[] bankgold;
					BankCheck[] bankchecks;

					if ( from.Backpack != null )
					{
						packgold = from.Backpack.FindItemsByType<Gold>();
						packchecks = from.Backpack.FindItemsByType<BankCheck>( false ); //They are only blessed on the top layer.

						for ( int i = 0;i < packgold.Count; i++ )
							totalGold += packgold[i].Amount;

						for ( int i = 0;i < packchecks.Count; i++ )
							totalGold += packchecks[i].Worth;
					}
					else
					{
						packgold = new List<Gold>();
						packchecks = new List<BankCheck>();
					}

					if ( totalGold < m_Amount )
						totalGold += Banker.GetBalance( from, out bankgold, out bankchecks );
					else
					{
						bankgold = new Gold[0];
						bankchecks = new BankCheck[0];
					}

					if ( totalGold >= m_Amount )
					{
						int amount = m_Amount;

						for ( int i = 0; amount > 0 && i < packgold.Count; ++i )
						{
							int consume = Math.Min( packgold[i].Amount, amount );
							packgold[i].Consume( consume );
							amount -= consume;
						}

						for ( int i = 0; amount > 0 && i < packchecks.Count; ++i )
						{
							int consume = Math.Min( packchecks[i].Worth, amount );
							packchecks[i].ConsumeWorth( consume );
							amount -= consume;
						}

						Banker.WithdrawUpTo( from, amount, bankgold, bankchecks );

						from.SendMessage("Consumed {0}gp.", m_Amount );
					}
					else
						from.SendMessage("{0} lacks {1}gp.", m.Name, m_Amount - totalGold );
				}
				else
					from.SendMessage("Invalid target specified.");
			}
		}
	}
}