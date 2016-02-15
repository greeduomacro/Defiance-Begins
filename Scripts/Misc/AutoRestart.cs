using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Server;
using Server.Network;
using Server.Items;
using Server.Regions;
using Server.Commands;
using Server.Factions;

namespace Server.Misc
{
	public class AutoRestart : Timer
	{
		public static bool Enabled = false; // is the script enabled?

		private static TimeSpan DefaultRestartTime = TimeSpan.FromHours( 2.0 ); // time of day at which to restart
		private static TimeSpan RestartDelay = TimeSpan.FromMinutes( 30.0 ); // how long the server should remain active before restart (period of 'server wars')

		public static int[] WarningIntervals = new int[]{ 172800, 86400, 43200, 21600, 10800, 3600, 900, 600, 300, 180, 60, 10, 5, 4, 3, 2, 1 }; //Warning intervals in seconds BEFORE any given restart time

		private static int m_WarningCount = 0;
		private static bool m_Restarting;
		private static bool m_ServerWars;
		private static DateTime m_RestartTime;

		public static bool Restarting
		{
			get{ return m_Restarting; }
		}

		public static bool ServerWars
		{
			get{ return m_ServerWars; }
		}

		public static DateTime RestartTime
		{
			get{ return m_RestartTime; }
			set{ m_RestartTime = value; }
		}

		public static int WarningCount
		{
			get{ return m_WarningCount; }
			set{ m_WarningCount = value; }
		}

		public static TimeSpan TimeTillRestart
		{
			get{ return m_RestartTime + RestartDelay - DateTime.Now; }
		}

		public static void Initialize()
		{
			CommandSystem.Register( "Restart", AccessLevel.Administrator, new CommandEventHandler( Restart_OnCommand ) );
			CommandSystem.Register( "RestartWithWar", AccessLevel.Administrator, new CommandEventHandler( RestartWithWar_OnCommand ) );
			new AutoRestart().Start();
			EventSink.Login += new LoginEventHandler( OnLogin );
		}

		private static void OnLogin( LoginEventArgs e )
		{
			Mobile from = e.Mobile;
			if ( m_ServerWars )
			{
				from.SendMessage( 38, 0, "---- SERVER WARS ----" );
				from.SendMessage( 38, 0, String.Format( "RESTARTING in {0}", FormatTimeSpan( TimeTillRestart ) ) );
			}
		}

		public static void RestartWithWar_OnCommand( CommandEventArgs e )
		{
			if ( m_Restarting || m_ServerWars )
			{
				e.Mobile.SendMessage( "The server is already restarting." );
			}
			else
			{
				e.Mobile.SendMessage( "You have initiated a server shutdown." );
				Enabled = true;
				m_RestartTime = DateTime.Now;
				m_WarningCount = WarningIntervals.Length;
				RestartDelay = TimeSpan.FromMinutes( 30.0 );
			}
		}

		public static void Restart_OnCommand( CommandEventArgs e )
		{
			if ( m_Restarting || m_ServerWars )
			{
				e.Mobile.SendMessage( "The server is already restarting." );
			}
			else
			{
				e.Mobile.SendMessage( "You have initiated a server shutdown." );
				Enabled = true;
				m_RestartTime = DateTime.Now;
				m_WarningCount = WarningIntervals.Length-1;
				RestartDelay = TimeSpan.Zero;
			}
		}

		public AutoRestart() : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
		{
			Priority = TimerPriority.FiveSeconds;

			m_RestartTime = DateTime.Now.Date + DefaultRestartTime;

			if ( m_RestartTime < DateTime.Now )
				m_RestartTime += TimeSpan.FromDays( 1.0 );
		}

		private void Warning_Callback()
		{
			if ( RestartDelay > TimeSpan.Zero )
				World.Broadcast( 38, 0, true, String.Format( "--- SERVER RESTARTING IN {0} ---", FormatTimeSpan( TimeSpan.FromSeconds( WarningIntervals[m_WarningCount] ) ) ) );
			else
				World.Broadcast( 38, 0, true, "--- SERVER RESTARTING ---" );

			m_WarningCount++;
		}

		private void Restart_Callback()
		{
			if ( File.Exists( "PublishPusher.exe" ) )
			{
				Process.Start( "PublishPusher.exe" );
				Core.Kill();
			}
			else
				Core.Kill( true );
		}

		public static string FormatTimeSpan( TimeSpan span )
		{
			return String.Format( "{0}{1}{2}{3}", span.Days > 0 ? String.Format( "{0} days ", span.Days ) : "", span.Hours > 0 ? String.Format( "{0:0#} hours ", span.Hours ) : "", span.Minutes > 0 ? String.Format( "{0:0#} minutes " , span.Minutes ) : "", span.Seconds > 0 ? String.Format( "{0:0#} seconds", span.Seconds ) : "" );
		}

		protected override void OnTick()
		{
			if ( m_Restarting || !Enabled )
				return;

//			if ( DateTime.Now < m_RestartTime )
//				return;

			if ( m_WarningCount < WarningIntervals.Length && DateTime.Now >= ( m_RestartTime - TimeSpan.FromSeconds( WarningIntervals[m_WarningCount] ) ) )
				Warning_Callback();
			else if ( DateTime.Now >= m_RestartTime )
			{
				AutoSave.SavesEnabled = false;

				if ( !WorldBackup.BackingUp )
					WorldBackup.ThreadedBackup();

				FinalRestart();
			}
		}

		public void FinalRestart()
		{
			if ( WorldBackup.BackingUp )
				Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerCallback( FinalRestart ) );
			else
			{
				AutoSave.Save(); //ONE LAST SAVE! :P

				m_Restarting = true;

				Timer.DelayCall( RestartDelay, new TimerCallback( Restart_Callback ) );

				if ( RestartDelay > TimeSpan.Zero )
				{
					World.Broadcast( 38, 0, true, "---- SERVER WARS ----" );
					World.Broadcast( 38, 0, true, String.Format( "RESTARTING IN {0}", FormatTimeSpan( RestartDelay ) ) );
					PrepareServerWar();
				}
			}
		}

		public static readonly Point3D[] m_RegStoneLocs = new Point3D[]
			{
				new Point3D( 1419, 1621, 20 ),
				new Point3D( 1424, 1687, 20 ),
				new Point3D( 2704, 2166, 0 ),
				new Point3D( 1171, 2599, 1 ),
				new Point3D( 973, 764, 0 ),
				new Point3D( 3751, 2231, 20 ),
				new Point3D( 1376, 1494, 10 ),
				new Point3D( 1375, 1743, 0 )
			};

		public static void PrepareServerWar()
		{
			m_ServerWars = true;
			FactionReset();

			Map[] maps = Map.Maps;

			for( int i = 0; i < maps.Length; i++ )
				if ( maps[i] != null )
					foreach( Region region in maps[i].Regions.Values )
						if ( region != null && region is GuardedRegion )
							((GuardedRegion)region).Disabled = true;

			for ( int i = 0;i < m_RegStoneLocs.Length; i++ )
				new VendStone( 1153, "BagOfReagents", "Bag of {0} Reagents (SERVER WARS)", 0, "100" ).MoveToWorld( m_RegStoneLocs[i], Map.Felucca );

			foreach ( NetState ns in NetState.Instances )
			{
				if ( ns.Mobile != null && ns.Mobile.AccessLevel == AccessLevel.Player )
				{
					ns.Mobile.BankBox.DropItem( new BankCheck( 100000 ) );
					ns.Mobile.BankBox.DropItem( new BagOfReagents( 10000 ) );
					Spellbook book = new Spellbook( ulong.MaxValue );
					book.LootType = LootType.Blessed;
					ns.Mobile.BankBox.DropItem( book );

					Server.Skills skills = ns.Mobile.Skills;

					for ( int i = 0; i < skills.Length; ++i )
						skills[i].BaseFixedPoint = 1000;
				}
			}
		}

		private static void FactionReset()
		{
			List<Faction> factions = Faction.Factions;

			for ( int i = 0; i < factions.Count; ++i )
			{
				Faction f = factions[i];

				List<PlayerState> memberlist = new List<PlayerState>( f.Members );

				for ( int j = 0; j < memberlist.Count; ++j )
					f.RemoveMember( memberlist[j].Mobile );

				List<FactionItem> itemlist = new List<FactionItem>( f.State.FactionItems );

				for ( int j = 0; j < itemlist.Count; ++j )
				{
					FactionItem fi = itemlist[j];

					if ( fi.Expiration == DateTime.MinValue )
						fi.Item.Delete();
					else
						fi.Detach();
				}

				List<BaseFactionTrap> traplist = new List<BaseFactionTrap>( f.Traps );

				for ( int j = 0; j < traplist.Count; ++j )
					traplist[i].Delete();
			}
		}
	}
}