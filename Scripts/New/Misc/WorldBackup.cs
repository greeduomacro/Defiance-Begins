using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Commands;

namespace Server.Misc
{
	public static class WorldBackup
	{
		private static readonly TimeSpan m_MaxBackup = TimeSpan.FromDays( 7.0 ); //No more than a week.

		private static bool m_BackingUp = false;
		private static bool m_BackupSuccess = false;


		public static bool BackingUp
		{
			get{ return m_BackingUp; }
			set{ m_BackingUp = value; }
		}

		public static bool BackupSuccess
		{
			get{ return m_BackupSuccess; }
		}

		public static void ThreadedBackup()
		{
			if ( AutoRestart.Restarting || AutoRestart.ServerWars || m_BackingUp )
				return;

			//Console.WriteLine( "World: Backup initiated." );

			m_BackupSuccess = false;

			m_BackingUp = true; //Just in case the new thread is delayed somehow

			Thread thread = new Thread( new ThreadStart( BackupByDate ) );
			thread.Name = "Server.Misc.AutoSave.Backup";
			thread.Priority = ThreadPriority.Normal;
			thread.Start();
		}

		private static string[] m_Months = new string[]
			{
				"January", "February", "March", "April", "May",
				"June", "July", "August", "September", "October",
				"November", "December"
			};

		public static void BackupByDate()
		{
			m_BackingUp = true;

			try
			{
				DateTime timestamp = DateTime.Now;

				string root = Path.Combine( Core.BaseDirectory, "Backups/Automatic" );

				if ( !Directory.Exists( root ) )
					Directory.CreateDirectory( root );

				//Lets clean up old saves, more than 1 week ago.
				DateTime maxback = timestamp - m_MaxBackup;

				string[] subdirs = Directory.GetDirectories( root );
				List<string> todel = new List<string>();
				if ( subdirs.Length > 3 ) //More than three prior saves found
				{
					for ( int i = 0; i < subdirs.Length; i++ )
					{
						DirectoryInfo subdir = new DirectoryInfo( subdirs[i] );
						string[] tsplit = subdir.Name.Split( '-' );
						DateTime timeframe = new DateTime( Utility.ToInt32(tsplit[0]),
							Utility.ToInt32(tsplit[1]), Utility.ToInt32(tsplit[2]),
							Utility.ToInt32(tsplit[3]), Utility.ToInt32(tsplit[4]), Utility.ToInt32(tsplit[5]) );
						if ( timeframe < maxback ) //Is it earlier than the maximum backup date?
							todel.Add( subdirs[i] );
					}

					if ( todel.Count > 0 && ( subdirs.Length - todel.Count >= 3 ) ) //At least three saves left
					{
						for ( int i = 0; i < todel.Count; i++ )
							Directory.Delete( todel[i], true );
						Console.WriteLine( "Backups: Removed {0} save{1} from before {2:MMMM dd, yyyy HH':'mm}", todel.Count, todel.Count != 1 ? "s" : "", maxback );
					}
				}

				string folder = Path.Combine( root, GetTimeStamp( timestamp ) );

				if ( Directory.Exists( folder ) ) //Split it into milliseconds if there is already a save for this time frame
					folder = String.Format( "{0}-{1:D3}", folder, timestamp.Millisecond );

				string saves = Path.Combine( Core.BaseDirectory, "Saves" );

				if ( Directory.Exists( saves ) )
					Directory.Move( saves, folder ); //We will make an exception here, or during the actual new save.

				m_BackupSuccess = true;
				//Console.WriteLine( "World: Backup completed successfully." );
			}
			catch ( Exception e )
			{
				Console.Write( "WARNING: Automatic backup " );
				Utility.PushColor( ConsoleColor.Red );
				Console.Write( "FAILED" );
				Utility.PopColor();
				Console.WriteLine( ": {0}\nWARNING: Previous save is still in the Saves folder.", e );

				m_BackupSuccess = false;
			}

			m_BackingUp = false;
		}

		private static string GetTimeStamp( DateTime now )
		{
			return String.Format( "{0}-{1:D2}-{2:D2}-{3:D2}-{4:D2}-{5:D2}",
					now.Year,
					now.Month,
					now.Day,
					now.Hour,
					now.Minute,
					now.Second
				);
		}
	}
}