using System;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Network
{
	public class BandagePacket
	{
		public static void Initialize()
		{
			PacketHandlers.RegisterExtended( 0x2C, true, new OnPacketReceive( BandageRequest ) );
		}

		public static void BandageRequest( NetState state, PacketReader pvSrc )
		{
			Mobile from = state.Mobile;

			if ( from.AccessLevel >= AccessLevel.Counselor || DateTime.Now >= from.NextActionTime )
			{
				Serial use = pvSrc.ReadInt32();
				Serial targ = pvSrc.ReadInt32();

				Bandage bandage = World.FindItem( use ) as Bandage;

				if ( bandage != null )
				{
					if ( from.InRange( bandage.GetWorldLocation(), 2 ) )
					{
						from.RevealingAction();

						Mobile to = World.FindMobile( targ );

						if ( to != null )
						{
							if ( BandageContext.BeginHeal( from, to ) != null )
								bandage.Consume();
						}
						else
							from.SendLocalizedMessage( 500970 ); // Bandages can not be used on that.
					}
					else
						from.SendLocalizedMessage( 500295 ); // You are too far away to do that.

					from.NextActionTime = DateTime.Now + TimeSpan.FromSeconds( 0.5 );
				}
			}
			else
				from.SendActionMessage();
		}
	}
}