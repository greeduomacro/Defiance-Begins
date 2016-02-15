using System;
using System.Collections.Generic;
using System.Text;
using Server.Mobiles;

namespace Server.Ethics.Hero
{
	public sealed class HolySteedPower : Power
	{
		public HolySteedPower()
		{
			m_Definition = new PowerDefinition(
					30,
					"Holy Steed",
					"Trebuchs Yeliab",
					""
				);
		}

		public override void BeginInvoke( Player from )
		{
			if ( from.Steed != null && from.Steed.Deleted )
				from.Steed = null;

			if ( from.Steed != null )
				from.Mobile.LocalOverheadMessage( Server.Network.MessageType.Regular, 0x3B2, false, "You already have a holy steed." );
			else if ( ( from.Mobile.Followers + 1 ) > from.Mobile.FollowersMax )
				from.Mobile.SendLocalizedMessage( 1049645 ); // You have too many followers to summon that creature.
			else
			{
				Mobiles.HolySteed steed = new Mobiles.HolySteed();

				if ( BaseCreature.Summon( steed, from.Mobile, from.Mobile.Location, 0x217, TimeSpan.FromHours( 3.0 ) ) )
				{
					from.Steed = steed;

					FinishInvoke( from );
				}
			}
		}
	}
}