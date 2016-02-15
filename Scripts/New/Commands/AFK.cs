using System;
using System.Collections.Generic;
using Server;

namespace Server.Commands
{
	public class AFK : Timer
	{
		private static Dictionary<Mobile, AFK> m_AFK = new Dictionary<Mobile, AFK>();
		private Mobile m_Mobile;
		private Point3D m_Location;
		private DateTime m_DateTime;
		private string m_Message;

		public static void Initialize()
		{
			CommandSystem.Register( "afk", AccessLevel.Player, new CommandEventHandler( AFK_OnCommand ) );
			EventSink.Logout += new LogoutEventHandler( OnLogout );
			EventSink.Speech += new SpeechEventHandler( OnSpeech );
			EventSink.PlayerDeath += new PlayerDeathEventHandler( OnDeath );
		}

		public static void OnDeath( PlayerDeathEventArgs e )
		{
			WakeUp( e.Mobile );
		}

		public static void OnLogout( LogoutEventArgs e )
		{
			WakeUp( e.Mobile );
		}

		public static void OnSpeech( SpeechEventArgs e )
		{
			WakeUp( e.Mobile );
		}

		public static void AFK_OnCommand( CommandEventArgs e )
		{
			Mobile m = e.Mobile;

			AFK afk;
			m_AFK.TryGetValue( m, out afk );
			if ( afk != null )
				afk.WakeUp();
			else
			{
				AFK timer = new AFK( m, e.ArgString.Trim() );
				m_AFK.Add( m, timer );
				timer.Start();
			}
		}

		public static void WakeUp( Mobile m )
		{
			AFK afk;
			m_AFK.TryGetValue( m, out afk );
			if ( afk != null )
				afk.WakeUp();
		}

		public void WakeUp()
		{
			m_AFK.Remove( m_Mobile );
			m_Mobile.PlaySound( m_Mobile.Female ? 814 : 1088 );
			m_Mobile.Say( "huh?" );
			m_Mobile.SendMessage( "Welcome back {0}.", m_Mobile.RawName );

			this.Stop();
		}

		public AFK( Mobile afker, string message ) : base( TimeSpan.FromSeconds( 60.0 ), TimeSpan.FromSeconds( 60.0 ) )
		{
			if ( String.IsNullOrEmpty( message ) )
				message = "I'm out of my head.  Come back later.";

			m_Message = message;
			m_Mobile = afker;
			m_DateTime = DateTime.Now;
			m_Location = m_Mobile.Location;
		}
		protected override void OnTick()
		{
			if ( m_Mobile.Location != m_Location )
				WakeUp();
			else
			{
				m_Mobile.Say("zZz");
				TimeSpan ts = DateTime.Now - m_DateTime;
				m_Mobile.Emote("*{0} (for {1}:{2}:{3}:{4})*",m_Message,ts.Days,ts.Hours,ts.Minutes,ts.Seconds );
				m_Mobile.PlaySound( m_Mobile.Female ? 819 : 1093 );
			}
		}
	}
}