using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Multis;
using Server.Multis.Deeds;
using Server.Network;
using Server.Mobiles;

namespace Server.Gumps
{
	public class HouseDemolishGump : Gump
	{
		private Mobile m_Mobile;
		private BaseHouse m_House;

		public HouseDemolishGump( Mobile mobile, BaseHouse house ) : base( 110, 100 )
		{
			m_Mobile = mobile;
			m_House = house;

			mobile.CloseGump( typeof( HouseDemolishGump ) );

			Closable = false;

			AddPage( 0 );

			AddBackground( 0, 0, 420, 280, 5054 );

			AddImageTiled( 10, 10, 400, 20, 2624 );
			AddAlphaRegion( 10, 10, 400, 20 );

			AddHtmlLocalized( 10, 10, 400, 20, 1060635, 30720, false, false ); // <CENTER>WARNING</CENTER>

			AddImageTiled( 10, 40, 400, 200, 2624 );
			AddAlphaRegion( 10, 40, 400, 200 );

			AddHtmlLocalized( 10, 40, 400, 200, 1061795, 32512, false, true ); /* You are about to demolish your house.
																				* You will be refunded the house's value directly to your bank box.
																				* All items in the house will remain behind and can be freely picked up by anyone.
																				* Once the house is demolished, anyone can attempt to place a new house on the vacant land.
																				* This action will not un-condemn any other houses on your account, nor will it end your 7-day waiting period (if it applies to you).
																				* Are you sure you wish to continue?
																				*/

			AddImageTiled( 10, 250, 400, 20, 2624 );
			AddAlphaRegion( 10, 250, 400, 20 );

			AddButton( 10, 250, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 40, 250, 170, 20, 1011036, 32767, false, false ); // OKAY

			AddButton( 210, 250, 4005, 4007, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 240, 250, 170, 20, 1011012, 32767, false, false ); // CANCEL
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if ( info.ButtonID == 1 && !m_House.Deleted )
			{
				if ( m_House.IsOwner( m_Mobile ) )
				{
					if ( m_House.MovingCrate != null || m_House.InternalizedVendors.Count > 0 )
					{
						return;
					}
					else if( !Guilds.Guild.NewGuildSystem && m_House.FindGuildstone() != null )
					{
						m_Mobile.SendLocalizedMessage( 501389 ); // You cannot redeed a house with a guildstone inside.
						return;
					}
					/*else if ( m_House.PlayerVendors.Count > 0 )
					{
						m_Mobile.SendLocalizedMessage( 503236 ); // You need to collect your vendor's belongings before moving.
						return;
					}*/
					else if ( m_House.HasRentedVendors && m_House.VendorInventories.Count > 0 )
					{
						m_Mobile.SendLocalizedMessage( 1062679 ); // You cannot do that that while you still have contract vendors or unclaimed contract vendor inventory in your house.
						return;
					}
					else if ( m_House.HasRentedVendors )
					{
						m_Mobile.SendLocalizedMessage( 1062680 ); // You cannot do that that while you still have contract vendors in your house.
						return;
					}
					else if ( m_House.VendorInventories.Count > 0 )
					{
						m_Mobile.SendLocalizedMessage( 1062681 ); // You cannot do that that while you still have unclaimed contract vendor inventory in your house.
						return;
					}

					if ( m_Mobile.AccessLevel >= AccessLevel.GameMaster )
					{
						m_Mobile.SendMessage( "You do not get a refund for your house as you are not a player" );
						m_House.RemoveKeys(m_Mobile);
						m_House.Delete();
					}
					else
					{
						Item toGive = null;
						BankCheck check = null;
						BankBox box = m_Mobile.BankBox;

						if ( m_House.IsAosRules )
						{
							if ( m_House.Price > 0 )
								check = new BankCheck( m_House.Price );

							toGive = m_House.GetDeed();
						}
						else
						{
							toGive = m_House.GetDeed();

							if ( toGive == null && m_House.Price > 0 )
								check = new BankCheck( m_House.Price );
						}

						if ( check != null )
						{
							if ( box.TryDropItem( m_Mobile, check, false ) )
							{
								m_Mobile.SendLocalizedMessage( 1060397, check.Worth.ToString() ); // ~1_AMOUNT~ gold has been deposited into your bank box.
							}
							else
							{
								check.Delete();
								if ( toGive != null )
									toGive.Delete();
								m_Mobile.SendLocalizedMessage( 500390 ); // Your bank box is full.
								return;
							}
						}

						if ( toGive != null )
						{
							if ( box.TryDropItem( m_Mobile, toGive, false ) )
							{
/*								if ( pm.HasDonated )
								{
									m_House.KillVendors( true );
									HouseDecayingCrate crate = m_House.MoveAllDecayingToCrate();
									if ( crate.Items.Count == 0 )
										crate.Delete();
									else
										m_Mobile.BankBox.DropItem( crate );
								} */

								m_Mobile.CloseGump( typeof( ConfirmCommitGump ) );

								m_House.RemoveKeys( m_Mobile );
								m_House.Delete();
							}
							else
							{
								if ( check != null )
									check.Delete();
								toGive.Delete();
								m_Mobile.SendLocalizedMessage( 500390 ); // Your bank box is full.
								return;
							}
						}
						else if ( TestCenter.Enabled )
							m_Mobile.SendMessage( "This house cannot be refunded during beta.  Please contact a Gamemaster for replacement options." );
						else
							m_Mobile.SendMessage( "Your house cannot be refunded." );
					}
				}
				else
					m_Mobile.SendLocalizedMessage( 501320 ); // Only the house owner may do this.
			}
		}
	}
}