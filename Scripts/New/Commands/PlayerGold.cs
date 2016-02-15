using System;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Commands;
using Server.Multis;

namespace Server.Misc
{
	public class PlayerGold
	{
		public static void Initialize()
		{
			CommandSystem.Register( "PlayerGold", AccessLevel.Developer, new CommandEventHandler( PlayerGold_OnCommand ) );
		}

		private static void PlayerGold_OnCommand( CommandEventArgs args )
		{
			long goldcount = GoldOnPlayers( args.Mobile );

			args.Mobile.SendMessage( "There is {0} gold owned by players.", goldcount );
		}

		public static long GoldOnPlayers( Mobile m )
		{
			long gold = 0;

			foreach( Mobile mob in World.Mobiles.Values )
			{
				if ( mob is PlayerMobile && mob.AccessLevel == AccessLevel.Player )
				{
					PlayerMobile pm = (PlayerMobile)mob;

					gold += SearchForGold( pm.BankBox );
					gold += SearchForGold( pm.Backpack );
					List<BaseHouse> houses = BaseHouse.GetHouses( pm );

					foreach ( BaseHouse house in houses )
					{
						List<Item> houseitems = house.GetItems();
						foreach ( Item item in houseitems )
						{
							if ( item is Container )
								gold += SearchForGold( (Container)item );
							else if ( item is Gold )
								gold += ((Gold)item).Amount;
							else if ( item is BankCheck )
								gold += ((BankCheck)item).Worth;
						}
					}
				}
			}

			return gold;
		}

		public static long SearchForGold( Container c )
		{
			long gold = 0;

			foreach( Item item in c.Items )
				if ( item is Container )
					gold += SearchForGold( (Container)item );
				else if ( item is Gold )
					gold += ((Gold)item).Amount;
				else if ( item is BankCheck )
					gold += ((BankCheck)item).Worth;

			return gold;
		}
	}
}