using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Commands
{
   public class BlowPouch
   {
		public static void Initialize()
		{
			CommandSystem.Register( "BlowPouch", AccessLevel.Player, new CommandEventHandler( BlowPouch_OnCommand ) );
		}

		[Usage( "BlowPouch" )]
		[Description( "Blows a trapped pouch if any available." )]
		public static void BlowPouch_OnCommand( CommandEventArgs e )
		{
			bool nopouch = true;
			Mobile m = e.Mobile;

			List<Pouch> pouches = m.Backpack.FindItemsByType<Pouch>();

			for ( int i = 0; nopouch && i < pouches.Count; ++i )
			{
				Pouch pouch = pouches[i];

				if ( pouch.TrapType == TrapType.MagicTrap )
				{
					pouch.ExecuteTrap( m );
					nopouch = false;
				}
			}

			if ( nopouch )
				m.SendMessage( "You have no trapped pouchs." );
		}
	}
}