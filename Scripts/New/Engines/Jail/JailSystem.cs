using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Commands;
using Server.Accounting;
using Server.Targeting;

namespace Server.Engines.Jail
{
	#region Delegates
	/// <summary>
	/// This delegate is used in the navigation of the jail system gumps
	/// </summary>
	public delegate void JailGumpCallback( Mobile user );
	/// <summary>
	/// A delegate used for jailing targets
	/// </summary>
	public delegate void JailTargetCallback( Mobile from, Mobile target );
	#endregion
	/// <summary>
	/// Defines the jail system
	/// </summary>
	public class JailSystem
	{
		#region Configuration
		/// <summary>
		/// AccessLevel needed to view jail information
		/// </summary>
		public static AccessLevel m_MinLevel = AccessLevel.Counselor;
        /// <summary>
        /// AccessLevel to jail/unjail a Player
        /// </summary>
        public static AccessLevel m_JailLevel = AccessLevel.GameMaster;
        /// <summary>
		/// AccessLevel to view a player's jail history
		/// </summary>
		public static AccessLevel m_HistoryLevel = AccessLevel.GameMaster;
		/// <summary>
		/// AccessLevel needed to administer the jail (view the full history, and purge the history)
		/// </summary>
		public static AccessLevel m_AdminLevel = AccessLevel.Administrator;
		/// <summary>
		/// Defines the minimum interval used to check up expired jailings
		/// This will be used in the PlayerLogin event.
		/// </summary>
		private static TimeSpan m_CheckInterval = TimeSpan.FromMinutes( 30 );
		/// <summary>
		/// Defines the temporary cell where the player is sent when jailed.
		/// The player will wait here until the GM fills the jail report, then
		/// they will be moved to their final cell.
		/// </summary>
		private static Point3D m_JailEnter = new Point3D( 5296, 1185, 0 );
		/// <summary>
		/// Defines the release location where the player is sent when
		/// they are about to be released. An exit in game should be provided
		/// from here.
		/// </summary>
		private static Point3D m_JailExit = new Point3D( 1420, 1539, 30 );
		/// <summary>
		/// The folder for the jail saves, relative to the base runuo folder
		/// </summary>
		private static string m_SaveFolder = "Jail";
		/// <summary>
		/// The folder for jail backups, relative to the base runuo folder
		/// </summary>
		private static string m_BackupFolder = Path.Combine("Backups", "Jail");
		/// <summary>
		/// The folder for jail purges, relative to the base runuo folder
		/// </summary>
		private static string m_PurgeFolder = Path.Combine("Jail", "Purged");
		#endregion

		public static void Initialize()
		{
			// Events
			EventSink.WorldSave += new WorldSaveEventHandler(OnWorldSave);
			EventSink.Login += new LoginEventHandler(OnPlayerLogin);

			// Commands
			CommandSystem.Register( "Jail", m_MinLevel, new CommandEventHandler( OnJailCommand ) );
			CommandSystem.Register( "JailInfo", AccessLevel.Player, new CommandEventHandler( OnJailInfoCommand ) );

			// Load and do verification
			Load();
			Verify();
		}

		#region Event Handlers

		/// <summary>
		/// On world save verify and save the jailings
		/// </summary>
		private static void OnWorldSave(WorldSaveEventArgs e)
		{
			Verify();
			Save();
		}

		/// <summary>
		/// On player login verify their jail situation and act according to their record
		/// </summary>
		private static void OnPlayerLogin(LoginEventArgs e)
		{
			// Verify if the check interval has elapsed
			if ( ( DateTime.Now - m_LastCheck ) >= m_CheckInterval )
				Verify();

			if ( IsInJail( e.Mobile ) )
			{
				if ( ShouldBeReleased( e.Mobile ) )
					SetFree( e.Mobile );
			}
			else
			{
				if ( ShouldBeJailed( e.Mobile ) )
					FinalizeJail( e.Mobile );
			}
		}

		#endregion

		#region Commands

		/// <summary>
		/// The main Jail command
		/// </summary>
		private static void OnJailCommand( CommandEventArgs e )
		{
			if ( e.Arguments.Length == 0 )
			{
                if (e.Mobile.AccessLevel >= m_JailLevel)
                {
                    // [Jail : target a mobile and jail it
                    e.Mobile.SendMessage("Who do you wish to jail?");
                    e.Mobile.Target = new JailTarget(new JailTargetCallback(OnJailTarget));
                }
                else
                    e.Mobile.SendMessage("You don't have access to that command");
			}
			else if ( e.Arguments.Length == 1 )
			{
				switch( e.Arguments[0].ToLower() )
				{
					case "release" : // [Jail Release: target a mobile and unjail it
					{
                        if (e.Mobile.AccessLevel >= m_JailLevel)
                        {
                            e.Mobile.SendMessage("Who do you wish to release?");
                            e.Mobile.Target = new JailTarget(new JailTargetCallback(OnUnjailTarget));
                        }
                        else
                            e.Mobile.SendMessage("You don't have access to that command");
						break;
					}
					case "view" : // [Jail View: view all the current jail roaster
					{
                        if (e.Mobile.AccessLevel >= m_JailLevel)
                            e.Mobile.SendGump(new JailListingGump(e.Mobile, m_Jailings, null));
                        else
                            e.Mobile.SendMessage("You don't have access to that command");
						break;
					}
					case "info" : // [Jail Info: view information about a specified jailed player
					{
						e.Mobile.SendMessage( "Who do you wish to receive jailing information on?" );
						e.Mobile.Target = new JailTarget( new JailTargetCallback( OnJailInfoTarget ) );
						break;
					}
					case "history": // [Jail History: View the jail history of a given mobile
					{
						if ( e.Mobile.AccessLevel >= m_HistoryLevel )
						{
							e.Mobile.SendMessage( "Requesting full jail record. Please select a player..." );
							e.Mobile.Target = new JailTarget( new JailTargetCallback( OnJailHistoryTarget ) );
						}
						else
							e.Mobile.SendMessage( "You don't have access to that command" );
						break;
					}
					case "admin": // [Jail Admin: view history and purge jail
					{
						if ( e.Mobile.AccessLevel >= m_AdminLevel )
							e.Mobile.SendGump( new JailAdminGump( e.Mobile ) );
						else
							e.Mobile.SendMessage( "You don't have access to that command" );
						break;
					}
					case "help": // [Jail Help: send help gump
					{
						e.Mobile.SendGump( new JailHelpGump( e.Mobile ) );
						break;
					}
					default:
					{
						e.Mobile.SendMessage( "Invalid command parameters. Use Jail Help to view information on the jail system." );
						break;
					}
				}
			}
			else if ( e.Arguments.Length > 1 )
			{
                if (e.Mobile.AccessLevel >= m_JailLevel)
                {
                    string lookup = e.Arguments[1];
                    for (int i = 2; i < e.Arguments.Length; i++)
                        lookup += " " + e.Arguments[i];

                    switch ( e.Arguments[0].ToLower() )
                    {
                        case "account": // [Jail Account <name>: search for accounts
						{
                            List<Account> accounts = SearchForAccounts(lookup);

                            if (accounts.Count > 0)
                                e.Mobile.SendGump(new JailSearchGump(accounts, e.Mobile, null));
                            else
                                e.Mobile.SendMessage("No matches found.");
                            break;
						}
                        case "player": // [Jail Player <name>: search for players
						{
                            List<Mobile> players = SearchForPlayers(lookup);

                            if (players.Count > 0)
                                e.Mobile.SendGump(new JailSearchGump(players, e.Mobile, null));
                            else
                                e.Mobile.SendMessage("No matches found.");
                            break;
						}
                        default:
						{
                            e.Mobile.SendMessage("Invalid command parameters. Use Jail Help to view information on the jail system.");
                            break;
						}
                    }
                }
                else
                    e.Mobile.SendMessage("You don't have access to that command");
			}
		}

		/// <summary>
		/// Callback for the Jail command target
		/// </summary>
		/// <param name="from">The GM using the command</param>
		/// <param name="target">The player being jailed</param>
		private static void OnJailTarget( Mobile from, Mobile target )
		{
			if ( InitJail( from, target ) )
				BeginJail( target ); // If the system authorizes the jailing, move the offender into the waiting cell
		}

		/// <summary>
		/// Callback for the Unjail command target
		/// </summary>
		/// <param name="from">The GM using the command</param>
		/// <param name="target">The player being unjailed</param>
		private static void OnUnjailTarget( Mobile from, Mobile target )
		{
			JailEntry jail = GetCurrentJail( target );

			if ( jail == null )
				from.SendMessage( "That player isn't currently jailed." );
			else
				Release( jail, from );
		}

		/// <summary>
		/// Views information on the jail status of the target
		/// </summary>
		/// <param name="from">The GM requesting the information</param>
		/// <param name="target">The target being searched for jail information</param>
		private static void OnJailInfoTarget( Mobile from, Mobile target )
		{
			JailEntry jail = GetCurrentJail( target );

			if ( jail == null )
				from.SendMessage( "That player isn't currently jailed." );
			else
				from.SendGump( new JailViewGump( from, jail, null ) );
		}

		/// <summary>
		/// Displays the jail history for a specified account
		/// </summary>
		private static void OnJailHistoryTarget( Mobile from, Mobile target )
		{
			Account account = target.Account as Account;

			List<JailEntry> matches = SearchHistory( account );

			if ( matches.Count == 0 )
				from.SendMessage( "That account has never been jailed" );
			else
				from.SendGump( new JailListingGump( from, matches, null ) );
		}

		/// <summary>
		/// Handles the JailInfo command, stating to the player information about their jailtime
		/// </summary>
		private static void OnJailInfoCommand( CommandEventArgs e )
		{
			JailEntry jail = GetCurrentJail( e.Mobile );

			if ( jail != null )
			{
				if ( jail.AutoRelease )
				{
					TimeSpan left = jail.EndTime - DateTime.Now;

					if ( left > TimeSpan.Zero )
						e.Mobile.SendMessage( "Your sentence will expire in {0} days, {1} hours and {2} minutes.", left.Days, left.Hours, left.Minutes );
					else if ( jail.EndTime > DateTime.Now )
						Release( jail, null );
				}
				else
					e.Mobile.SendMessage( "Your sentence has no auto release date. A staff member will have to manually release you." );
				return;
			}
			else
				e.Mobile.SendMessage( "You must be jailed to use this." );
		}

		#endregion

		#region Variables and Properties

		/// <summary>
		/// Gets the currently active jailings in the system
		/// </summary>
		public static List<JailEntry> Jailings
		{
			get { return m_Jailings; }
		}

		/// <summary>
		/// Gets the base save folder
		/// </summary>
		private static string SaveFolder
		{
			get
			{
				return Path.Combine( Core.BaseDirectory, m_SaveFolder );
			}
		}

		/// <summary>
		/// Gets the folder for jail backups
		/// </summary>
		private static string BackupFolder
		{
			get
			{
				return Path.Combine( Core.BaseDirectory, m_BackupFolder );
			}
		}

		/// <summary>
		/// Gets the folder for purged history
		/// </summary>
		private static string PurgedFolder
		{
			get { return Path.Combine( Core.BaseDirectory, m_PurgeFolder ); }
		}


		/// <summary>
		/// Defines the possible locations for jail cells
		/// </summary>
		private static Point3D[] m_JailCells =
			{
				new Point3D( 5276, 1164, 0 ),
				new Point3D( 5286, 1164, 0 ),
				new Point3D( 5296, 1164, 0 ),
				new Point3D( 5306, 1164, 0 ),
				new Point3D( 5306, 1174, 0 ),
				new Point3D( 5296, 1174, 0 ),
				new Point3D( 5286, 1174, 0 ),
				new Point3D( 5276, 1164, 0 )
			};
		/// <summary>
		/// List of mobiles who have been targeted with the jail command, but
		/// whose jail report hasn't been filled yet by a GM
		/// </summary>
		private static List<Mobile> m_PendingJailings;
		/// <summary>
		/// List of JailEntry objects currently active. This is the master list of current jailings
		/// and all currently active jailings are here
		/// </summary>
		private static List<JailEntry> m_Jailings;
		/// <summary>
		/// List of JailEntry objects that have expired and should be moved into the history archive
		/// This archieve is reset with each Verify and grows until the next action is performed
		/// </summary>
		private static List<JailEntry> m_ExpiredJailings;
		/// <summary>
		/// Defines the moment of the latest jail check
		/// </summary>
		private static DateTime m_LastCheck;

		/// <summary>
		/// Initialize static variables
		/// </summary>
		static JailSystem()
		{
			m_PendingJailings = new List<Mobile>();
			m_Jailings = new List<JailEntry>();
			m_ExpiredJailings = new List<JailEntry>();
		}

		#endregion

		#region Jail Sequence

		/* 1. InitJail. Sends the JailReasonGump to the jailer and handles the mobile being jailed
		 * 2. BeginJail: moves the offender to the waiting cell (used only with [jail + target
		 * 3. CancelJail (optional). Moves the player from the jail entrance to the GM (or jail exit if logged out)
		 * 4. CommitJailing. Creates the JailEntry object and adds it to the current jailings list
		 * 5. FinalizeJailing. Moves the player into a jail cell
		 * 6. Release. Terminates the sentence.
		 * 7. SetFree. Moves a player to the jail exit. */

		/// <summary>
		/// Initializes a jailing action by sending the JailReasonGump
		/// </summary>
		/// <param name="from">The GM performing the jailing</param>
		/// <param name="offender">The Mobile being jailed</param>
		/// <returns>True if the jailing action is succesful</returns>
		public static bool InitJail( Mobile from, Mobile offender )
		{
			return InitJail( from, offender, false );
		}

		/// <summary>
		/// Initializes a jailing action by sending the JailReasonGump
		/// </summary>
		/// <param name="from">The GM performing the jailing</param>
		/// <param name="offender">The Mobile being jailed</param>
		/// <param name="forceJail">Specifies whether the jailing of the player should be forced vs an existing account jail</param>
		/// <returns>True if the jailing action is succesful</returns>
		public static bool InitJail( Mobile from, Mobile offender, bool forceJail )
		{
			if ( offender == null )
				return false;

			if ( CanBeJailed( offender ) )
			{
				// Check for existing non-full jail entries
				JailEntry jail = GetAccountJail( offender );

				if ( jail != null && !forceJail )
				{
					// Existing entry: ask for conversion
					from.SendGump( new JailAccountGump( offender, from, jail, offender.Account as Account ) );
					return false;
				}

				from.SendGump( new JailReasonGump( offender ) );
				return true;
			}
			else
			{
				JailEntry jail = GetCurrentJail( offender );

				if ( jail != null )
				{
					from.SendMessage( "That player is already jailed. Please review the current jail record." );
					from.SendGump( new JailViewGump( from, jail, null ) );
				}
				else
					from.SendMessage( "You can't jail that player because they're either already jailed or a staff member." );
				return false;
			}
		}

		/// <summary>
		/// Initializes a jailing action by sending the JailReasonGump
		/// </summary>
		/// <param name="from">The GM performing the jailing</param>
		/// <param name="offender">The account being jailed</param>
		/// <returns>True if the jailing can proceed</returns>
		public static bool InitJail( Mobile from, Account offender )
		{
			if ( CanBeJailed( offender ) )
			{
				from.SendGump( new JailReasonGump( offender ) );
				return true;
			}
			else
			{
				JailEntry jail = GetCurrentJail( offender );

				if ( jail != null )
				{
					from.SendMessage( "That account has already been jailed. Please review the existing jail record." );
					from.SendGump( new JailViewGump( from, jail, null ) );
				}
				else
				{
					from.SendMessage( "You can't jail that account because it's either already jailed or a staff account." );
				}

				return false;
			}
		}

		/// <summary>
		/// Moves a mobile into the jail waiting cell
		/// </summary>
		/// <param name="offender">The mobile about to be jailed</param>
		private static void BeginJail( Mobile offender )
		{
			if ( offender == null )
				return;

			offender.Location = m_JailEnter;

			if ( offender.NetState != null )
				offender.Map = Map.Felucca;
			else
			{
				offender.LogoutLocation = m_JailEnter;
				offender.LogoutMap = Map.Felucca;
			}

			m_PendingJailings.Add( offender );
		}

		/// <summary>
		/// A jailing action is cancelled before it's committed
		/// </summary>
		/// <param name="m">The mobile being jailed</param>
		/// <param name="from">The GM performing the jailing</param>
		public static void CancelJail ( Mobile m, Mobile from )
		{
			if ( m == null )
				return;

			if ( m_PendingJailings.Contains( m ) ) // This will prevent moving to the exit players jailed offline who aren't put in the pending list
			{
				if ( m.NetState != null )
				{
					m.Location = from.Location;
					m.Map = from.Map;
					m.SendMessage( "Your sentence has been revoked..." );
				}
				else
				{
					m.Location = m_JailExit;
					m.LogoutMap = Map.Felucca;
					m.Map = Map.Internal;
				}

				m_PendingJailings.Remove( m );

				from.SendMessage( "You have canceled the jailing of {0} and brought him to your location", m.Name );
			}
			else
			{
				from.SendMessage( "You have canceled the jailing of {0}.", m.Name );
			}
		}

		/// <summary>
		/// Moves a mobile out of their jail cell into the jail exit area
		/// This function only repositons the mobile, assuming the mobile
		/// is logged in and currently in jail
		/// </summary>
		/// <param name="m">The mobile that's being released</param>
		public static void SetFree( Mobile m )
		{
			if ( m == null )
				return;

			m.Location = m_JailExit;

			if ( m.NetState != null )
			{
				m.SendMessage( "Your sentence is now over. Learn from your mistakes." );
				m.Map = Map.Felucca;
			}
			else
				m.LogoutMap = Map.Felucca;
		}

		/// <summary>
		/// Finalizes the jailing sequence and sends a mobile into a random jail cell
		/// This function only performs the physical move of the player
		/// </summary>
		/// <param name="m">The mobile that should be imprisoned</param>
		public static void FinalizeJail( Mobile m )
		{
			if ( m == null ) // Offline account jail
				return;

			m.Location = m_JailCells[ Utility.Random( m_JailCells.Length ) ];

			if ( m.NetState != null )
			{
				m.Map = Map.Felucca;
				m.SendMessage( "May your stay here allow you to think over your actions..." );
			}
			else
				m.LogoutMap = Map.Felucca;

			if ( m_PendingJailings.Contains( m ) )
				m_PendingJailings.Remove( m );
		}

		/// <summary>
		/// Finalizes a jailing action by adding a jail record and moving the mobile into a jail cell
		/// </summary>
		/// <param name="m">The player being jailed</param>
		/// <param name="account">The account of the player being jailed</param>
		/// <param name="from">The GM performing the jailing</param>
		/// <param name="reason">The reason for the jailing</param>
		/// <param name="autorelease">States if the player should be auto released after the sentence is over</param>
		/// <param name="duration">The length of the sentence</param>
		/// <param name="fulljail">States if the full account should be jailed</param>
		/// <param name="comment">An additional comment about the jailing</param>
		public static void CommitJailing( Mobile m, Account account, Mobile from, string reason, bool autorelease, TimeSpan duration, bool fulljail, string comment )
		{
			JailEntry jail = new JailEntry( m, account, from, duration, reason, comment, autorelease, fulljail );

			// Verify if the new entry is valid
			JailEntry existing = null;

			if ( ! IsValid( jail ) )
			{
				if ( jail.Mobile != null )
					existing = GetCurrentJail( jail.Mobile );
				else
					existing = GetCurrentJail( jail.Account );
			}

			if ( existing != null )
			{
				// This jailing wont' be committed, if there's a pending player, release them
				CancelJail( m, from );

				from.SendMessage( 0x40, "Your new jail report is in conflict with an existing entry. Please review and eventually update the existing jailing." );
				from.SendGump( new JailViewGump( from, existing, null ) );
				return;
			}

			// The jailing is valid so go on and add it

			m_Jailings.Add( jail );
			FinalizeJail( m );

			// Send jailing review gump to the offender
			if ( m != null && m.NetState != null )
				m.SendGump( new PlayerJailGump( jail, from ) );
			else if ( jail.FullJail ) // Handle full account jailing/player relogging
			{
				Mobile mob = GetOnlineMobile( jail.Account );

				if ( mob != null )
				{
					FinalizeJail( mob );
					mob.SendGump( new PlayerJailGump( jail, from ) );
				}
			}
		}

		/// <summary>
		/// Forces a release action on a JailEntry. This also adds a comment specifying the moment of the release.
		/// </summary>
		/// <param name="jail">The jail entry to consider expired</param>
		/// <param name="from">The staff member releasing the jailing</param>
		public static void Release( JailEntry jail, Mobile from )
		{
			if ( from != null )
				jail.AddComment( from, "Released at {0}", DateTime.Now.ToShortTimeString() );

			jail.Release();

			if ( m_Jailings.Contains( jail ) )
			{
				m_Jailings.Remove( jail );
				m_ExpiredJailings.Add( jail );
			}
		}

		#endregion

		#region File Managment

		/// <summary>
		/// Performs a save and a backup
		/// </summary>
		private static void Save()
		{
			try
			{
				Backup();
			}
			catch ( Exception err )
			{
				Console.WriteLine( err.ToString() );
			}

			if ( !Directory.Exists( SaveFolder ) )
			{
				Directory.CreateDirectory( SaveFolder );
			}

			string jailFile = Path.Combine( SaveFolder, "Jail.xml" );
			string jailHistoryFile = Path.Combine( SaveFolder, "JailHistory.xml" );

			SaveJailFile( jailFile, m_Jailings );

			if ( m_ExpiredJailings.Count > 0 )
			{
				List<JailEntry> history = GetHistory();
				history.AddRange( m_ExpiredJailings );

				SaveJailFile( jailHistoryFile, history );

				m_ExpiredJailings.Clear();
			}
		}

		/// <summary>
		/// Perforums a backup of the current jail save
		/// </summary>
		private static void Backup()
		{
			/*
			 * Backup will use the same scheme as RunUO backups do.
			 * Three backup levels in the following folders:
			 *
			 * ./Backups/Jail/Most Recent (Timestamp)
			 * ./Backups/Jail/Second Backup (Timestamp)
			 * ./Backups/Jail/Third Backup (Timestamp)
			 */

			string[] folders = new string[] { "Third Backup", "Second Backup", "Most Recent" };

			if ( ! Directory.Exists( BackupFolder ) )
				Directory.CreateDirectory( BackupFolder );

			string[] existing = Directory.GetDirectories( BackupFolder );

			for ( int i = 0; i < folders.Length; i++ )
			{
				DirectoryInfo dir = Match( existing, folders[ i ] );

				if ( dir == null )
					continue;

				if ( i > 0 )
				{
					string timeStamp = FindTimeStamp( dir.Name	 );

					if ( timeStamp != null )
					{
						try
						{
							dir.MoveTo( FormatDirectory(	folders[ i - 1 ], timeStamp ) );
						}
						catch {}
					}
				}
				else
				{
					try
					{
						dir.Delete( true );
					}
					catch {}
				}
			}

			string jailFile = Path.Combine( SaveFolder, "Jail.xml" );
			string historyFile = Path.Combine( SaveFolder, "JailHistory.xml" );
			string targetFolder = FormatDirectory( folders[ folders.Length - 1 ], GetTimeStamp() );

			if ( ! File.Exists( jailFile ) && ! File.Exists( historyFile ) )
				return;

			if ( ! Directory.Exists( targetFolder ) )
				Directory.CreateDirectory( targetFolder );

			if ( File.Exists( jailFile ) )
			{
				File.Copy( jailFile, Path.Combine( targetFolder, "Jail.xml" ), true );
			}

			if ( File.Exists( historyFile ) )
			{
				File.Copy( historyFile, Path.Combine( targetFolder, "JailHistory.xml" ), true );
			}
		}

		/// <summary>
		/// Creates the name of a directory used for backups
		/// </summary>
		/// <param name="name">The name of the directory</param>
		/// <param name="timeStamp">The timestamp</param>
		/// <returns></returns>
		private static string FormatDirectory( string name, string timeStamp )
		{
			return Path.Combine( BackupFolder, string.Format( "{0} ({1})", name, timeStamp ) );
		}

		/// <summary>
		/// Verifies a number of directory names to find one whose name starts with match
		/// </summary>
		/// <param name="paths">Array of folders</param>
		/// <param name="match">The match that's being looked for at the beginning of the folder names</param>
		/// <returns>A DirectoryInfo object with the match, or null if none is found</returns>
		private static DirectoryInfo Match( string[] paths, string match )
		{
			for ( int i = 0; i < paths.Length; i++ )
			{
				DirectoryInfo info = new DirectoryInfo( paths[ i ] );

				if ( info.Name.StartsWith( match ) )
					return info;
			}

			return null;
		}

		/// <summary>
		/// Finds the time stamp on a specified folder
		/// </summary>
		/// <param name="input">The folder containing the timestamp</param>
		/// <returns>The timestamp string, null if not found</returns>
		private static string FindTimeStamp( string input )
		{
			int start = input.IndexOf( '(' );

			if ( start >= 0 )
			{
				int end = input.IndexOf( ')', ++start );

				if ( end >= start )
					return input.Substring( start, end-start );
			}

			return null;
		}

		/// <summary>
		/// Loads the current jail entries
		/// </summary>
		private static void Load()
		{
			m_Jailings.Clear();
			m_ExpiredJailings.Clear();

			string jailFile = Path.Combine( SaveFolder, "Jail.xml" );

			if ( File.Exists( jailFile ) )
			{
				m_Jailings.AddRange( LoadJailFile( jailFile ) );
			}
		}

		/// <summary>
		/// Saves the history file to the default save location
		/// </summary>
		/// <param name="history"></param>
		private static void SaveHistory( List<JailEntry> history )
		{
			if ( ! Directory.Exists( SaveFolder ) )
				Directory.CreateDirectory( SaveFolder );

			string historyFile = Path.Combine( SaveFolder, "JailHistory.xml" );

			// Save history
			SaveJailFile( historyFile, history );
		}

		/// <summary>
		/// Saves a jail file
		/// </summary>
		/// <param name="filename">The target filename</param>
		/// <param name="jailings">A list of JailEntry objects</param>
		private static void SaveJailFile( string filename, List<JailEntry> jailings )
		{
			// Create the dom
			XmlDocument dom = new XmlDocument();

			XmlNode xDeclaration = dom.CreateXmlDeclaration( "1.0", null, null );
			dom.AppendChild( xDeclaration );

			XmlNode xItems = dom.CreateElement( "Jailings" );

			foreach( JailEntry jail in jailings )
			{
				XmlNode xJail = jail.GetXmlNode( dom );
				xItems.AppendChild( xJail );
			}

			dom.AppendChild( xItems );

			try
			{
				dom.Save( filename );
			}
			catch{}
		}

		/// <summary>
		/// Loads a file containing the jail entries
		/// </summary>
		/// <param name="filename">The filename to load</param>
		/// <returns></returns>
		private static List<JailEntry> LoadJailFile( string filename )
		{
			List<JailEntry> jailings = new List<JailEntry>();

			XmlDocument dom = new XmlDocument();

			if ( System.IO.File.Exists( filename ) )
			{
				try
				{
					dom.Load( filename );
				}
				catch{}

				if ( dom == null || dom.ChildNodes.Count < 2 )
					return jailings;

				XmlNode xItems = dom.ChildNodes[ 1 ];

				foreach( XmlNode xJail in xItems.ChildNodes )
				{
					JailEntry jail = null;

					try // If modified manually, some entries might be broken
					{
						jail = JailEntry.Load( xJail );
					}
					finally
					{
						if ( jail != null )
							jailings.Add( jail );
					}
				}
			}

			return jailings;
		}

		#endregion

		#region Checks

		/// <summary>
		/// Verifies if a mobile should be jailed.
		/// This function is called on player login
		/// This function assumes the mobile is not in jail.
		/// </summary>
		/// <param name="m">The mobile being examined</param>
		/// <returns>True if the mobile should be jailed, false otherwise</returns>
		private static bool ShouldBeJailed( Mobile m )
		{
			Account account = m.Account as Account;

			foreach( JailEntry jail in m_Jailings )
				if ( jail.Mobile == m || ( jail.Account == account && jail.FullJail ) )
					return true;

			return false;
		}

		/// <summary>
		/// Verifies if a mobile should be released from jail.
		/// This function assumes that the mobile already is in jail.
		/// </summary>
		/// <param name="m">The mobile to examine</param>
		/// <returns>True if the mobile should be released</returns>
		private static bool ShouldBeReleased( Mobile m )
		{
			if ( m.AccessLevel > AccessLevel.Player )
				return false;

			Account account = m.Account as Account;

			foreach( JailEntry jail in m_Jailings )
				if ( jail.Mobile == m || ( jail.Account == account && jail.FullJail ) )
					return false;

			return true;
		}

		/// <summary>
		/// Verifies if a mobile is in jail by checking their location
		/// </summary>
		/// <param name="m">The mobile to check</param>
		/// <returns>True if the mobile is located inside the jail</returns>
		public static bool IsInJail( Mobile m )
		{
			return m.X >= 5272 && m.Y >= 1160 && m.X <= 5310 && m.Y <= 1178;
		}

		/// <summary>
		/// Verifies if a mobile can be jailed
		/// </summary>
		/// <param name="m">The mobile being jailed</param>
		/// <returns>True if the mobile isn't yet jailed</returns>
		public static bool CanBeJailed( Mobile m )
		{
			if ( m.AccessLevel > AccessLevel.Player )
				return false;

			if ( m_PendingJailings.Contains( m ) )
				return false;

			Account acct = m.Account as Account;

			foreach( JailEntry jail in m_Jailings )
				if ( jail.Mobile == m || ( jail.Account == acct && jail.FullJail ) )
					return false;

			return true;
		}

		/// <summary>
		/// Verifies if an account can be jailed
		/// </summary>
		/// <param name="acc">The account to verify</param>
		/// <returns>True if the account can be jailed</returns>
		public static bool CanBeJailed( Account acc )
		{
			if ( acc.AccessLevel > AccessLevel.Player )
				return false;

			foreach( JailEntry jail in m_Jailings )
				if ( jail.FullJail && jail.Account == acc )
					return false;

			return true;
		}

		/// <summary>
		/// Verifies if a given jail entry is active or if it's a historical one
		/// </summary>
		/// <param name="jail">The JailEntry to verify</param>
		/// <returns>True if the jail entry is currently active, false if it's historical</returns>
		public static bool IsActive( JailEntry jail )
		{
			return m_Jailings.Contains( jail );
		}

		/// <summary>
		/// Verifies if a new jail entry has conflicts with other jail entries
		/// </summary>
		/// <param name="jail">The potential new JailEntry</param>
		/// <returns>True if the new jail entry can be added to the jail</returns>
		private static bool IsValid( JailEntry jail )
		{
			foreach( JailEntry cmp in m_Jailings )
			{
				if ( jail.Account == cmp.Account )
				{
					if ( cmp.FullJail && jail.FullJail )
						return false; // New jail is full account, and a previous full jail already exists
					else if ( cmp.Mobile == jail.Mobile && cmp.FullJail == jail.FullJail )
						return false; // Jailing the same mobile without full jail
				}
			}

			return true;
		}

		#endregion

		#region History

		/// <summary>
		/// Gets all the records stored in the history
		/// </summary>
		/// <returns>A list of JailEnty objects stored in the history</returns>
		public static List<JailEntry> GetHistory()
		{
			List<JailEntry> history = new List<JailEntry>();
			string file = Path.Combine( SaveFolder, "JailHistory.xml" );

			if ( File.Exists( file ) )
				history.AddRange( LoadJailFile( file ) );

			return history;
		}

		/// <summary>
		/// Gets all the jailing reports, including history and current/expired jailings
		/// </summary>
		/// <returns>An ArrayList of JailEntry objects</returns>
		public static List<JailEntry> GetFullHistory()
		{
			List<JailEntry> history = GetHistory();

			history.AddRange( m_Jailings );
			history.AddRange( m_ExpiredJailings );

			return history;
		}

		/// <summary>
		/// Purges the jailing history
		/// </summary>
		/// <param name="m">The user invoking the history purge</param>
		/// <param name="filter">The purge filter</param>
		public static void PurgeHistory( Mobile m, JailPurge filter )
		{
			List<JailEntry> history = GetHistory();
			List<JailEntry> purged = new List<JailEntry>();

			foreach( JailEntry jail in history )
				if ( filter.PurgeCheck( jail ) )
					purged.Add( jail );

			if ( purged.Count == 0 )
				m.SendMessage( "The specified purge found no matches in the history." );
			else
				foreach( JailEntry jail in purged )
					history.Remove( jail );

			SaveHistory( history );

			// Save the purged file
			if ( !Directory.Exists( PurgedFolder ) )
				Directory.CreateDirectory( PurgedFolder );

			string path = Path.Combine( PurgedFolder, string.Format( "{0} Purged by {1}.xml", GetTimeStamp(), m.Name ) );

			Console.WriteLine( "{0} purged {1} jail entries", m.Name, purged.Count );
			m.SendMessage( "You have just purged {0} jail entries", purged.Count );

			SaveJailFile( path, purged );
		}

		#endregion

		#region Searching

		/// <summary>
		/// Performs a search for accounts (case insensitive)
		/// </summary>
		/// <param name="account">The account name to search for (accepts partial matches)</param>
		/// <returns>An ArrayList including the accounts found</returns>
		public static List<Account> SearchForAccounts( string account )
		{
			List<Account> matches = new List<Account>();

			account = account.ToLower();

			foreach( Account cmp in Accounts.GetAccounts() )
			{
				if ( cmp.Username.ToLower().IndexOf( account ) > -1 && cmp.AccessLevel == AccessLevel.Player )
				{
                    if ( cmp.Username.Length <= account.Length * 2)
                        matches.Add( cmp );
				}
			}

			return matches;
		}

		/// <summary>
		/// Performs a search for players
		/// </summary>
		/// <param name="name">The name to match</param>
		/// <returns>An ArrayList of matching players</returns>
		public static List<Mobile> SearchForPlayers( string name )
		{
			name = name.ToLower();
			List<Mobile> matches = new List<Mobile>();

			foreach( Mobile m in World.Mobiles.Values )
			{
				PlayerMobile pm = m as PlayerMobile;

				if ( pm != null && pm.Name.ToLower().IndexOf( name ) > -1 && pm.AccessLevel == AccessLevel.Player )
                    if (pm.Name.Length <= name.Length * 2)
                        matches.Add(pm);
			}

			return matches;
		}

		/// <summary>
		/// Searches the jail history for references of an account
		/// </summary>
		/// <param name="account">The account to search for</param>
		/// <returns>An ArrayList of JailEntry objects</returns>
		public static List<JailEntry> SearchHistory( Account account )
		{
			List<JailEntry> matches = new List<JailEntry>();
			List<JailEntry> history = GetHistory();

			history.AddRange( m_ExpiredJailings );
			history.AddRange( m_Jailings );

			foreach( JailEntry jail in history )
				if ( jail.Account != null && jail.Account == account )
					matches.Add( jail );

			return matches;
		}

		#endregion

		#region Misc Methods

		/// <summary>
		/// Gets a timestamp for the current moment
		/// </summary>
		/// <returns>A string with the current timestamp</returns>
		private static string GetTimeStamp()
		{
			DateTime now = DateTime.Now;

			return String.Format( "{0}-{1}-{2} {3}-{4:D2}-{5:D2}",
				now.Day,
				now.Month,
				now.Year,
				now.Hour,
				now.Minute,
				now.Second
				);
		}

		/// <summary>
		/// Performs verification of the current jail entries, removes expired entries and frees any
		/// mobiles if needed.
		/// </summary>
		private static void Verify()
		{
			m_LastCheck = DateTime.Now;

			List<JailEntry> expired = new List<JailEntry>();

			foreach( JailEntry jail in m_Jailings )
				if ( jail.Expired && jail.AutoRelease )
					expired.Add( jail );

			foreach( JailEntry jail in expired )
			{
				// Remove the entry from the list and release any online players
				if ( m_Jailings.Contains ( jail ) )
				{
					m_Jailings.Remove( jail );
					jail.Release();
				}
			}

			m_ExpiredJailings.AddRange( expired );
		}

		/// <summary>
		/// Gets the current jail record for a given mobile
		/// </summary>
		/// <param name="m">The mobile being examined</param>
		/// <returns>A JailEntry if there is one, null otherwise</returns>
		private static JailEntry GetCurrentJail( Mobile m )
		{
			foreach( JailEntry jail in m_Jailings )
				if ( jail.Mobile == m )
					return jail;
				else if ( jail.Account == ( m.Account as Account ) && jail.FullJail )
					return jail;

			return null;
		}

		/// <summary>
		/// Gets the current jail record for a given account
		/// </summary>
		/// <param name="acc">The account being examined</param>
		/// <returns>A JailEntry if one is matching, null otherwise</returns>
		private static JailEntry GetCurrentJail( Account acc )
		{
			foreach( JailEntry jail in m_Jailings )
				if ( jail.Account == acc && jail.FullJail )
					return jail;

			return null;
		}

		/// <summary>
		/// Gets a jail entry for a mobile. This function assumes that the mobile has no full jail entries.
		/// </summary>
		/// <param name="m">The mobile being jailed</param>
		/// <returns>A JailEntry if another character on the account is already jailed, null otherwise</returns>
		private static JailEntry GetAccountJail( Mobile m )
		{
			Account account = m.Account as Account;

			foreach( JailEntry jail in m_Jailings )
				if ( jail.Account == account && ! jail.FullJail )
					return jail;

			return null;
		}

		/// <summary>
		/// Gets the online mobile for a given account.
		/// This method will not return any staff mobiles.
		/// </summary>
		/// <param name="account">The account object</param>
		/// <returns>A mobile if one is online, null otherwise</returns>
		public static Mobile GetOnlineMobile( Account account )
		{
			if ( account == null )
				return null;

			for ( int i = 0; i < account.Length; i++ )
			{
				Mobile m = account[i];

				if ( m != null && m.NetState != null && m.AccessLevel == AccessLevel.Player )
					return m;
			}

			return null;
		}

		#endregion
	}
}