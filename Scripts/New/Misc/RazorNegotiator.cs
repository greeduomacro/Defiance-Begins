using System;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Gumps;

namespace Server.Misc
{
	public class RazorFeatureControl
	{
		public const bool Enabled = false; // Is the "Feature Enforced" turned on?
		public const bool KickOnFailure = true; // When true, this will cause anyone who does not negotiate (include those not running Razor at all) to be disconnected from the server.
		public static readonly TimeSpan HandshakeTimeout = TimeSpan.FromSeconds( 30.0 ); // How long to wait for a handshake response before showing warning and disconnecting
		public static readonly TimeSpan DisconnectDelay = TimeSpan.FromSeconds( 60.0 ); // How long to show warning message before they are disconnected
		public const string WarningMessage = "The server was unable to negotiate features with Razor on your system.<BR>You must be running the <A HREF=\"http://razor.runuo.com/download.php\">latest version of Razor</A> to play on this server.<BR>Once you have Razor installed and running, go to the <B>Options</B> tab and check the box in the lower right-hand corner marked <B>Negotiate features with server</B>.  Once you have this box checked, you may log in and play normally.<BR>You will be disconnected shortly.";

		public static void Configure()
		{
			// TODO: Add your server's feature allowances here
			// For example, the following line will disallow all looping macros on your server
			DisallowFeature( RazorFeatures.FilterLight );
			DisallowFeature( RazorFeatures.AutoPotionEquip );
		}

		[Flags]
		public enum RazorFeatures : ulong
		{
			None = 0,

			FilterWeather	= 1 << 0, // Weather Filter
			FilterLight		= 1 << 1, // Light Filter
			SmartTarget		= 1 << 2, // Smart Last Target
			RangedTarget	= 1 << 3, // Range Check Last Target
			AutoOpenDoors	= 1 << 4, // Automatically Open Doors
			DequipOnCast	= 1 << 5, // Unequip Weapon on spell cast
			AutoPotionEquip	= 1 << 6, // Un/Re-equip weapon on potion use
			PoisonedChecks	= 1 << 7, // Block heal If poisoned/Macro IIf Poisoned condition/Heal or Cure self
			LoopedMacros	= 1 << 8, // Disallow Looping macros, For loops, and macros that call other macros
			UseOnceAgent	= 1 << 9, // The use once agent
			RestockAgent	= 1 << 10,// The restock agent
			SellAgent		= 1 << 11,// The sell agent
			BuyAgent		= 1 << 12,// The buy agent
			PotionHotkeys	= 1 << 13,// All potion hotkeys
			RandomTargets	= 1 << 14,// All random target hotkeys (Not target next, last target, target self)
			ClosestTargets	= 1 << 15,// All closest target hotkeys
			OverheadHealth	= 1 << 16,// Health and Mana/Stam messages shown over player's heads

			All = 0xFFFFFFFFFFFFFFFF  // Every feature possible
		}

		private static RazorFeatures m_DisallowedFeatures = RazorFeatures.None;

		public static void DisallowFeature( RazorFeatures feature )
		{
			SetDisallowed( feature, true );
		}

		public static void AllowFeature( RazorFeatures feature )
		{
			SetDisallowed( feature, false );
		}

		public static void SetDisallowed( RazorFeatures feature, bool value )
		{
			if ( value )
				m_DisallowedFeatures |= feature;
			else
				m_DisallowedFeatures &= ~feature;
		}

		public static RazorFeatures DisallowedFeatures { get { return m_DisallowedFeatures; } }
	}

	public class RazorFeatureEnforcer
	{
		private static Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();
		private static TimerStateCallback<Mobile> OnHandshakeTimeout_Callback = new TimerStateCallback<Mobile>( OnHandshakeTimeout );
		private static TimerStateCallback<Mobile> OnForceDisconnect_Callback = new TimerStateCallback<Mobile>( OnForceDisconnect );

		public static void Initialize()
		{
			Console.Write( "Scripts: Razor feature negotiations..." );
			if ( RazorFeatureControl.Enabled )
			{
				EventSink.Login += new LoginEventHandler( EventSink_Login );

				ProtocolExtensions.Register( 0xFF, true, new OnPacketReceive( OnHandshakeResponse ) );
				Console.WriteLine( "enabled." );
			}
			else
				Console.WriteLine( "disabled." );
		}

		private static void EventSink_Login( LoginEventArgs e )
		{
			Mobile m = e.Mobile;
			NegotiateFeatures( m );
		}

		private static void NegotiateFeatures( Mobile m )
		{
			if ( m != null && m.NetState != null && m.NetState.Running )
			{
				Timer t;

				if ( m.AccessLevel > AccessLevel.Counselor )
					m.Send( new BeginRazorHandshakeGM() );
				else
					m.Send( new BeginRazorHandshake() );

				m_Table.TryGetValue( m, out t );

				if ( t != null && t.Running )
					t.Stop();

				m_Table[m] = t = Timer.DelayCall<Mobile>( RazorFeatureControl.HandshakeTimeout, OnHandshakeTimeout_Callback, m );
				t.Start();
			}
		}

		private static void OnHandshakeResponse( NetState state, PacketReader pvSrc )
		{
			pvSrc.Trace( state );

			if ( state == null || state.Mobile == null || !state.Running )
				return;

			Mobile m = state.Mobile;
			Timer t;

			m_Table.TryGetValue( m, out t );

			if ( t != null )
				t.Stop();

			m_Table.Remove( m );
		}

		private static void OnHandshakeTimeout( Mobile m )
		{
			if ( m != null )
			{
				m_Table.Remove( m );

				if ( !RazorFeatureControl.KickOnFailure )
					Console.WriteLine( "Player '{0}' failed to negotiate Razor features.", m );
				else if ( m.NetState != null && m.NetState.Running )
				{
					m.SendGump( new Gumps.WarningGump( 1060635, 30720, RazorFeatureControl.WarningMessage, 0xFFC000, 420, 250, null, null ) );

					if ( m.AccessLevel <= AccessLevel.Player )
					{
						Timer t = m_Table[m] = Timer.DelayCall<Mobile>( RazorFeatureControl.DisconnectDelay, OnForceDisconnect_Callback, m );
						t.Start();
					}
				}
			}
		}

		private static void OnForceDisconnect( Mobile m )
		{
			if ( m.NetState != null && m.NetState.Running )
				m.NetState.Dispose();
			m_Table.Remove( m );

			Console.WriteLine( "Player {0} kicked (Failed Razor handshake)", m );
		}

		private sealed class BeginRazorHandshakeGM : ProtocolExtension
		{
			public BeginRazorHandshakeGM() : base( 0xFE, 8 )
			{
				m_Stream.Write( (uint)((ulong)RazorFeatureControl.RazorFeatures.None >> 32) );
				m_Stream.Write( (uint)((ulong)RazorFeatureControl.RazorFeatures.None & 0xFFFFFFFF) );
			}
		}

		private sealed class BeginRazorHandshake : ProtocolExtension
		{
			public BeginRazorHandshake() : base( 0xFE, 8 )
			{
				m_Stream.Write( (uint)((ulong)RazorFeatureControl.DisallowedFeatures >> 32) );
				m_Stream.Write( (uint)((ulong)RazorFeatureControl.DisallowedFeatures & 0xFFFFFFFF) );
			}
		}
	}
}