using System;
using Server;
using Server.Network;
using Server.Commands;

namespace Server.Misc
{
	// This fastwalk detection is no longer required
	// As of B36 PlayerMobile implements movement packet throttling which more reliably controls movement speeds
	public class Fastwalk
	{
//		private static int  MaxSteps = 5;			// Maximum number of queued steps until fastwalk is detected
//		private static bool Enabled = true;			// Is fastwalk detection enabled?
//		private static bool UOTDOverride = false;	// Should UO:TD clients not be checked for fastwalk?
//		private static AccessLevel AccessOverride = AccessLevel.Counselor; // Anyone with this or higher access level is not checked for fastwalk

		public static void Initialize()
		{
			Mobile.FwdMaxSteps = 5;
			Mobile.FwdEnabled = false;
			Mobile.FwdUOTDOverride = false;
			Mobile.FwdAccessOverride = AccessLevel.Counselor;

			Mobile.WalkFoot = TimeSpan.FromSeconds( 0.4 );
			Mobile.RunFoot = TimeSpan.FromSeconds( 0.2 );
			Mobile.WalkMount = TimeSpan.FromSeconds( 0.25 );
			Mobile.RunMount = TimeSpan.FromSeconds( 0.125 );

			Mobile.MoveRecordDelay = TimeSpan.FromSeconds( 3.0 );

			if ( Mobile.FwdEnabled )
				EventSink.FastWalk += new FastWalkEventHandler( OnFastWalk );

			if ( TestCenter.Enabled )
			{
				CommandSystem.Register( "MaxSteps", AccessLevel.Administrator, new CommandEventHandler( MaxSteps_OnCommand ) );
				CommandSystem.Register( "WalkFoot", AccessLevel.Administrator, new CommandEventHandler( WalkFoot_OnCommand ) );
				CommandSystem.Register( "RunFoot", AccessLevel.Administrator, new CommandEventHandler( RunFoot_OnCommand ) );
				CommandSystem.Register( "WalkMount", AccessLevel.Administrator, new CommandEventHandler( WalkMount_OnCommand ) );
				CommandSystem.Register( "RunMount", AccessLevel.Administrator, new CommandEventHandler( RunMount_OnCommand ) );
				CommandSystem.Register( "MoveRecords", AccessLevel.Administrator, new CommandEventHandler( MoveRecords_OnCommand ) );
			}
		}

		public static void MaxSteps_OnCommand( CommandEventArgs e )
		{
			Mobile m = e.Mobile;
			Mobile.FwdMaxSteps = Utility.ToInt32( e.ArgString.Trim() );
		}

		public static void WalkFoot_OnCommand( CommandEventArgs e )
		{
			Mobile m = e.Mobile;
			Mobile.WalkFoot = TimeSpan.FromSeconds( Utility.ToDouble( e.ArgString.Trim() ) );
		}

		public static void RunFoot_OnCommand( CommandEventArgs e )
		{
			Mobile m = e.Mobile;
			Mobile.RunFoot = TimeSpan.FromSeconds( Utility.ToDouble( e.ArgString.Trim() ) );
		}

		public static void WalkMount_OnCommand( CommandEventArgs e )
		{
			Mobile m = e.Mobile;
			Mobile.WalkMount = TimeSpan.FromSeconds( Utility.ToDouble( e.ArgString.Trim() ) );
		}

		public static void RunMount_OnCommand( CommandEventArgs e )
		{
			Mobile m = e.Mobile;
			Mobile.RunMount = TimeSpan.FromSeconds( Utility.ToDouble( e.ArgString.Trim() ) );
		}

		public static void MoveRecords_OnCommand( CommandEventArgs e )
		{
			Mobile m = e.Mobile;
			Mobile.MoveRecordDelay = TimeSpan.FromSeconds( Utility.ToDouble( e.ArgString.Trim() ) );
		}

		public static void OnFastWalk( FastWalkEventArgs e )
		{
			Mobile m = e.NetState.Mobile;
			e.Blocked = true;//disallow this fastwalk
			if ( TestCenter.Enabled )
				e.NetState.Mobile.SendMessage( "Please slow down!! ({0}) ({1} seconds)", m.MoveRecords.Count - Mobile.FwdMaxSteps, (m.EndQueue - DateTime.Now).TotalSeconds );
			PublicOverheadMessage( e.NetState.Mobile, MessageType.Regular, 33, String.Format( "[Fastwalk]: Speed Detected!!! ({0}) ({1} seconds)", m.MoveRecords.Count - Mobile.FwdMaxSteps, (m.EndQueue - DateTime.Now).TotalSeconds ) );

			if ( m.MoveRecords.Count - Mobile.FwdMaxSteps >= 20 )
			{
				e.NetState.Mobile.SendMessage( "You have been kicked for excessive movement or network latency issues.  Please check your network connection, or third party software before logging in." );
				m.NetState.Dispose();
			}
		}

		public static void PublicOverheadMessage( Mobile m, MessageType type, int hue, string text )
		{
			if( m.Map != null )
			{
				Packet p = null;

				IPooledEnumerable eable = m.Map.GetClientsInRange( m.Location );

				foreach( NetState state in eable )
				{
					if( state.Mobile != null && state.Mobile.CanSee( m ) && state.Mobile.AccessLevel >= AccessLevel.GameMaster )
					{
						if( p == null )
						{
							p = new UnicodeMessage( m.Serial, m.Body, type, hue, 3, m.Language, m.Name, text );

							p.Acquire();
						}

						state.Send( p );
					}
				}

				Packet.Release( p );

				eable.Free();
			}
		}
	}
}