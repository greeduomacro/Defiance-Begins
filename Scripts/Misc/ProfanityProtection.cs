using System;
using Server;
using Server.Network;

namespace Server.Misc
{
	public enum ProfanityAction
	{
		None,			// no action taken
		Disallow,		// speech is not displayed
		Criminal,		// makes the player criminal, not killable by guards
		CriminalAction,	// makes the player criminal, can be killed by guards
		Disconnect,		// player is kicked
		Other			// some other implementation
	}

	public class ProfanityProtection
	{
		private static bool Enabled = false;
		private static ProfanityAction Action = ProfanityAction.Disallow; // change here what to do when profanity is detected

		public static void Initialize()
		{
			if ( Enabled )
				EventSink.Speech += new SpeechEventHandler( EventSink_Speech );
		}

		private static bool OnProfanityDetected( Mobile from, string speech )
		{
			switch ( Action )
			{
				case ProfanityAction.None: return true;
				case ProfanityAction.Disallow: return false;
				case ProfanityAction.Criminal: from.Criminal = true; return true;
				case ProfanityAction.CriminalAction: from.CriminalAction( false ); return true;
				case ProfanityAction.Disconnect:
				{
					NetState ns = from.NetState;

					if ( ns != null )
						ns.Dispose();

					return false;
				}
				default:
				case ProfanityAction.Other: // TODO: Provide custom implementation if this is chosen
				{
					return true;
				}
			}
		}

		private static void EventSink_Speech( SpeechEventArgs e )
		{
			Mobile from = e.Mobile;

			if ( from.AccessLevel > AccessLevel.Player )
				return;

			if ( NameVerification.Validate( e.Speech, 0, int.MaxValue, true, true, false, int.MaxValue, m_Exceptions, m_DisallowedWords, m_StartDisallowed, m_DisallowedAnywhere ) != NameResultMessage.Allowed )
				e.Blocked = !OnProfanityDetected( from, e.Speech );
		}

		public static char[]	Exceptions{	get{ return m_Exceptions; } }
		public static string[]	StartDisallowed{ get{ return m_StartDisallowed; } }
		public static string[]	DisallowedWords{ get{ return m_DisallowedWords; } }
		public static string[]	DisallowedAnywhere{ get{ return m_DisallowedAnywhere; } }

		private static char[] m_Exceptions = new char[]
			{
				' ', '-', '.', '\'', '"', ',', '_', '+', '=', '~', '`', '!', '^', '*', '\\', '/', ';', ':', '<', '>', '[', ']', '{', '}', '?', '|', '(', ')', '%', '$', '&', '#', '@'
			};

		private static string[] m_StartDisallowed = new string[]{};

		private static string[] m_DisallowedWords = new string[]
			{
				"wop",
				"tit",
				"spic",
				"cum",
				"ass",
				"clit",
				"klit",
				"dick",
				"anal"
			};

		private static string[] m_DisallowedAnywhere = new string[]
			{
				"wop",
				"kyke",
				"kike",
				"tit",
				"spic",
				"lezbo",
				"lesbo",
				"felatio",
				"dyke",
				"dildo",
				"chinc",
				"chink",
				"cunnilingus",
				"cock",
				"clitoris",
				"penis",
				"nigga",
				"nigger",
				"kunt",
				"jiz",
				"jism",
				"jackoff",
				"jack off",
				"goddamn",
				"god damn",
				"fag",
				"blowjob",
				"blow job",
				"handjob",
				"hand job",
				"rimjob",
				"rim job",
				"bitch",
				"asshole",
				"ass hole",
				"pussy",
				"cunt",
				"twat",
				"shit",
				"fuck",
				"kurwa",
				"vittu"
			};
	}
}