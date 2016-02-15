using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Accounting;
using Server.Commands;
using Server.Mobiles;

namespace Server.Misc
{
	/**
	 * This file requires to be saved in a Unicode
	 * compatible format.
	 *
	 * Warning: if you change String.Format methods,
	 * please note that the following character
	 * is suggested before any left-to-right text
	 * in order to prevent undesired formatting
	 * resulting from mixing LR and RL text: ?
	 *
	 * Use this one if you need to force RL: ?
	 *
	 * If you do not see the above chars, please
	 * enable showing of unicode control chars
	 **/

	public class LanguageStatistics
	{
		struct InternationalCode
		{
			string m_Code;
			string m_Language;
			string m_Country;
			string m_Language_LocalName;
			string m_Country_LocalName;
			bool m_HasLocalInfo;

			public string Code{ get{ return m_Code; } }
			public string Language{ get{ return m_Language; } }
			public string Country{ get{ return m_Country; } }
			public string Language_LocalName{ get{ return m_Language_LocalName; } }
			public string Country_LocalName{ get{ return m_Country_LocalName; } }

			public InternationalCode( string code, string language, string country ) : this( code, language, country, null, null )
			{
				m_HasLocalInfo = false;
			}

			public InternationalCode( string code, string language, string country, string language_localname, string country_localname )
			{
				m_Code = code;
				m_Language = language;
				m_Country = country;
				m_Language_LocalName = language_localname;
				m_Country_LocalName = country_localname;
				m_HasLocalInfo = true;
			}

			public string GetName()
			{
				string s;

				if ( m_HasLocalInfo )
				{
					s = String.Format( "{0}? - {1}", DefaultLocalNames ? m_Language_LocalName : m_Language, DefaultLocalNames ? m_Country_LocalName : m_Country );

					if ( ShowAlternatives )
						s += String.Format( "? ?{0}? - {1}??", DefaultLocalNames ? m_Language : m_Language_LocalName, DefaultLocalNames ? m_Country : m_Country_LocalName );
				}
				else
				{
					s = String.Format( "{0}? - {1}", m_Language, m_Country );
				}

				return s;
			}
		}

		private static InternationalCode[] InternationalCodes =
			{
				new InternationalCode( "ARA", "Arabic", "Saudi Arabia", "???????", "????????" ),
				new InternationalCode( "ARI", "Arabic", "Iraq", "???????", "??????" ),
				new InternationalCode( "ARE", "Arabic", "Egypt", "???????", "???" ),
				new InternationalCode( "ARL", "Arabic", "Libya", "???????", "?????" ),
				new InternationalCode( "ARG", "Arabic", "Algeria", "???????", "???????" ),
				new InternationalCode( "ARM", "Arabic", "Morocco", "???????", "??????" ),
				new InternationalCode( "ART", "Arabic", "Tunisia", "???????", "????" ),
				new InternationalCode( "ARO", "Arabic", "Oman", "???????", "????" ),
				new InternationalCode( "ARY", "Arabic", "Yemen", "???????", "?????" ),
				new InternationalCode( "ARS", "Arabic", "Syria", "???????", "?????" ),
				new InternationalCode( "ARJ", "Arabic", "Jordan", "???????", "??????" ),
				new InternationalCode( "ARB", "Arabic", "Lebanon", "???????", "?????" ),
				new InternationalCode( "ARK", "Arabic", "Kuwait", "???????", "??????" ),
				new InternationalCode( "ARU", "Arabic", "U.A.E.", "???????", "????????" ),
				new InternationalCode( "ARH", "Arabic", "Bahrain", "???????", "???????" ),
				new InternationalCode( "ARQ", "Arabic", "Qatar", "???????", "???" ),
				new InternationalCode( "BGR", "Bulgarian", "Bulgaria", "?????????", "????????" ),
				new InternationalCode( "CAT", "Catalan", "Spain", "Catal�", "Espanya" ),
				new InternationalCode( "CHT", "Chinese", "Taiwan", "??", "??" ),
				new InternationalCode( "CHS", "Chinese", "PRC", "??", "??" ),
				new InternationalCode( "ZHH", "Chinese", "Hong Kong", "??", "??" ),
				new InternationalCode( "ZHI", "Chinese", "Singapore", "??", "???" ),
				new InternationalCode( "ZHM", "Chinese", "Macau", "??", "??" ),
				new InternationalCode( "CSY", "Czech", "Czech Republic", "Ce�tina", "Cesk� republika" ),
				new InternationalCode( "DAN", "Danish", "Denmark", "Dansk", "Danmark" ),
				new InternationalCode( "DEU", "German", "Germany", "Deutsch", "Deutschland" ),
				new InternationalCode( "DES", "German", "Switzerland", "Deutsch", "der Schweiz" ),
				new InternationalCode( "DEA", "German", "Austria", "Deutsch", "�sterreich" ),
				new InternationalCode( "DEL", "German", "Luxembourg", "Deutsch", "Luxembourg" ),
				new InternationalCode( "DEC", "German", "Liechtenstein", "Deutsch", "Liechtenstein" ),
				new InternationalCode( "ELL", "Greek", "Greece", "????????", "????da" ),
				new InternationalCode( "ENU", "English", "United States" ),
				new InternationalCode( "ENG", "English", "United Kingdom" ),
				new InternationalCode( "ENA", "English", "Australia" ),
				new InternationalCode( "ENC", "English", "Canada" ),
				new InternationalCode( "ENZ", "English", "New Zealand" ),
				new InternationalCode( "ENI", "English", "Ireland" ),
				new InternationalCode( "ENS", "English", "South Africa" ),
				new InternationalCode( "ENJ", "English", "Jamaica" ),
				new InternationalCode( "ENB", "English", "Caribbean" ),
				new InternationalCode( "ENL", "English", "Belize" ),
				new InternationalCode( "ENT", "English", "Trinidad" ),
				new InternationalCode( "ENW", "English", "Zimbabwe" ),
				new InternationalCode( "ENP", "English", "Philippines" ),
				new InternationalCode( "ESP", "Spanish", "Spain (Traditional Sort)", "Espa�ol", "Espa�a (tipo tradicional)" ),
				new InternationalCode( "ESM", "Spanish", "Mexico", "Espa�ol", "M�xico" ),
				new InternationalCode( "ESN", "Spanish", "Spain (International Sort)", "Espa�ol", "Espa�a (tipo internacional)" ),
				new InternationalCode( "ESG", "Spanish", "Guatemala", "Espa�ol", "Guatemala" ),
				new InternationalCode( "ESC", "Spanish", "Costa Rica", "Espa�ol", "Costa Rica" ),
				new InternationalCode( "ESA", "Spanish", "Panama", "Espa�ol", "Panama" ),
				new InternationalCode( "ESD", "Spanish", "Dominican Republic", "Espa�ol", "Republica Dominicana" ),
				new InternationalCode( "ESV", "Spanish", "Venezuela", "Espa�ol", "Venezuela" ),
				new InternationalCode( "ESO", "Spanish", "Colombia", "Espa�ol", "Colombia" ),
				new InternationalCode( "ESR", "Spanish", "Peru", "Espa�ol", "Peru" ),
				new InternationalCode( "ESS", "Spanish", "Argentina", "Espa�ol", "Argentina" ),
				new InternationalCode( "ESF", "Spanish", "Ecuador", "Espa�ol", "Ecuador" ),
				new InternationalCode( "ESL", "Spanish", "Chile", "Espa�ol", "Chile" ),
				new InternationalCode( "ESY", "Spanish", "Uruguay", "Espa�ol", "Uruguay" ),
				new InternationalCode( "ESZ", "Spanish", "Paraguay", "Espa�ol", "Paraguay" ),
				new InternationalCode( "ESB", "Spanish", "Bolivia", "Espa�ol", "Bolivia" ),
				new InternationalCode( "ESE", "Spanish", "El Salvador", "Espa�ol", "El Salvador" ),
				new InternationalCode( "ESH", "Spanish", "Honduras", "Espa�ol", "Honduras" ),
				new InternationalCode( "ESI", "Spanish", "Nicaragua", "Espa�ol", "Nicaragua" ),
				new InternationalCode( "ESU", "Spanish", "Puerto Rico", "Espa�ol", "Puerto Rico" ),
				new InternationalCode( "FIN", "Finnish", "Finland", "Suomi", "Suomi" ),
				new InternationalCode( "FRA", "French", "France", "Fran�ais", "France" ),
				new InternationalCode( "FRB", "French", "Belgium", "Fran�ais", "Belgique" ),
				new InternationalCode( "FRC", "French", "Canada", "Fran�ais", "Canada" ),
				new InternationalCode( "FRS", "French", "Switzerland", "Fran�ais", "Suisse" ),
				new InternationalCode( "FRL", "French", "Luxembourg", "Fran�ais", "Luxembourg" ),
				new InternationalCode( "FRM", "French", "Monaco", "Fran�ais", "Monaco" ),
				new InternationalCode( "HEB", "Hebrew", "Israel", "????????", "??????" ),
				new InternationalCode( "HUN", "Hungarian", "Hungary", "Magyar", "Magyarorsz�g" ),
				new InternationalCode( "ISL", "Icelandic", "Iceland", "�slenska", "�sland" ),
				new InternationalCode( "ITA", "Italian", "Italy", "Italiano", "Italia" ),
				new InternationalCode( "ITS", "Italian", "Switzerland", "Italiano", "Svizzera" ),
				new InternationalCode( "JPN", "Japanese", "Japan", "???", "??" ),
				new InternationalCode( "KOR", "Korean (Extended Wansung)", "Korea", "???", "??" ),
				new InternationalCode( "NLD", "Dutch", "Netherlands", "Nederlands", "Nederland" ),
				new InternationalCode( "NLB", "Dutch", "Belgium", "Nederlands", "Belgi�" ),
				new InternationalCode( "NOR", "Norwegian", "Norway (Bokm�l)", "Norsk", "Norge (Bokm�l)" ),
				new InternationalCode( "NON", "Norwegian", "Norway (Nynorsk)", "Norsk", "Norge (Nynorsk)" ),
				new InternationalCode( "PLK", "Polish", "Poland", "Polski", "Polska" ),
				new InternationalCode( "PTB", "Portuguese", "Brazil", "Portugu�s", "Brasil" ),
				new InternationalCode( "PTG", "Portuguese", "Portugal", "Portugu�s", "Brasil" ),
				new InternationalCode( "ROM", "Romanian", "Romania", "Limba Rom�na", "Rom�nia" ),
				new InternationalCode( "RUS", "Russian", "Russia", "???????", "??????" ),
				new InternationalCode( "HRV", "Croatian", "Croatia", "Hrvatski", "Hrvatska" ),
				new InternationalCode( "SRL", "Serbian", "Serbia (Latin)", "Srpski", "Srbija i Crna Gora" ),
				new InternationalCode( "SRB", "Serbian", "Serbia (Cyrillic)", "??????", "?????? ? ???? ????" ),
				new InternationalCode( "SKY", "Slovak", "Slovakia", "Slovencina", "Slovensko" ),
				new InternationalCode( "SQI", "Albanian", "Albania", "Shqip", "Shqip�ria" ),
				new InternationalCode( "SVE", "Swedish", "Sweden", "Svenska", "Sverige" ),
				new InternationalCode( "SVF", "Swedish", "Finland", "Svenska", "Finland" ),
				new InternationalCode( "THA", "Thai", "Thailand", "???????", "?????????" ),
				new InternationalCode( "TRK", "Turkish", "Turkey", "T�rk�e", "T�rkiye" ),
				new InternationalCode( "URP", "Urdu", "Pakistan", "????", "???????" ),
				new InternationalCode( "IND", "Indonesian", "Indonesia", "Bahasa Indonesia", "Indonesia" ),
				new InternationalCode( "UKR", "Ukrainian", "Ukraine", "??????????", "???????" ),
				new InternationalCode( "BEL", "Belarusian", "Belarus", "?????????", "????????" ),
				new InternationalCode( "SLV", "Slovene", "Slovenia", "Sloven�cina", "Slovenija" ),
				new InternationalCode( "ETI", "Estonian", "Estonia", "Eesti", "Eesti" ),
				new InternationalCode( "LVI", "Latvian", "Latvia", "Latvie�u", "Latvija" ),
				new InternationalCode( "LTH", "Lithuanian", "Lithuania", "Lietuviu", "Lietuva" ),
				new InternationalCode( "LTC", "Classic Lithuanian", "Lithuania", "Lietuvi�kai", "Lietuva" ),
				new InternationalCode( "FAR", "Farsi", "Iran", "?????", "?????" ),
				new InternationalCode( "VIT", "Vietnamese", "Viet Nam", "ti�ng Vi�?t", "Vi?t Nam" ),
				new InternationalCode( "HYE", "Armenian", "Armenia", "???????", "????????" ),
				new InternationalCode( "AZE", "Azeri", "Azerbaijan (Latin)", "Az?rbaycanca", "Az?rbaycan" ),
				new InternationalCode( "AZE", "Azeri", "Azerbaijan (Cyrillic)", "????????????", "??????????" ),
				new InternationalCode( "EUQ", "Basque", "Spain", "Euskera", "Espainia" ),
				new InternationalCode( "MKI", "Macedonian", "Macedonia", "??????????", "??????????" ),
				new InternationalCode( "AFK", "Afrikaans", "South Africa", "Afrikaans", "Republiek van Suid-Afrika" ),
				new InternationalCode( "KAT", "Georgian", "Georgia", "???????", "??????????" ),
				new InternationalCode( "FOS", "Faeroese", "Faeroe Islands", "F�royska", "F�roya" ),
				new InternationalCode( "HIN", "Hindi", "India", "??????", "????" ),
				new InternationalCode( "MSL", "Malay", "Malaysia", "Bahasa melayu", "Malaysia" ),
				new InternationalCode( "MSB", "Malay", "Brunei Darussalam", "Bahasa melayu", "Negara Brunei Darussalam" ),
				new InternationalCode( "KAZ", "Kazak", "Kazakstan", "?????", "?????????" ),
				new InternationalCode( "SWK", "Swahili", "Kenya", "Kiswahili", "Kenya" ),
				new InternationalCode( "UZB", "Uzbek", "Uzbekistan (Latin)", "O'zbek", "O'zbekiston" ),
				new InternationalCode( "UZB", "Uzbek", "Uzbekistan (Cyrillic)", "?????", "??????????" ),
				new InternationalCode( "TAT", "Tatar", "Tatarstan", "???????", "?????????" ),
				new InternationalCode( "BEN", "Bengali", "India", "?????", "????" ),
				new InternationalCode( "PAN", "Punjabi", "India", "??????", "????" ),
				new InternationalCode( "GUJ", "Gujarati", "India", "???????", "????" ),
				new InternationalCode( "ORI", "Oriya", "India", "????", "????" ),
				new InternationalCode( "TAM", "Tamil", "India", "?????", "???????" ),
				new InternationalCode( "TEL", "Telugu", "India", "??????", "????" ),
				new InternationalCode( "KAN", "Kannada", "India", "?????", "????" ),
				new InternationalCode( "MAL", "Malayalam", "India", "??????", "????" ),
				new InternationalCode( "ASM", "Assamese", "India", "??????", "???" ), // missing correct country name
				new InternationalCode( "MAR", "Marathi", "India", "?????", "????" ),
				new InternationalCode( "SAN", "Sanskrit", "India", "???????", "??????" ),
				new InternationalCode( "KOK", "Konkani", "India", "??????", "????" )
			};

		private static string GetFormattedInfo( string code )
		{
			if ( code == null || code.Length != 3 )
				return String.Format( "Unknown code {0}", code );

			for ( int i = 0; i < InternationalCodes.Length; i++ )
				if ( code == InternationalCodes[i].Code )
					return String.Format( "{0}", InternationalCodes[i].GetName() );

			return String.Format( "Unknown code {0}", code );
		}

		private static bool DefaultLocalNames = false;
		private static bool ShowAlternatives = true;
		private static bool CountAccounts = true; // will consider only first character's valid language

		public static void Initialize()
		{
			CommandSystem.Register( "LanguageStatistics", AccessLevel.Administrator, new CommandEventHandler( LanguageStatistics_OnCommand ) );
		}

		[Usage( "LanguageStatistics" )]
		[Description( "Generate a file containing the list of languages for each PlayerMobile." )]
		public static void LanguageStatistics_OnCommand( CommandEventArgs e )
		{
			Dictionary<string, InternationalCodeCounter> ht = new Dictionary<string, InternationalCodeCounter>();

			using ( StreamWriter writer = new StreamWriter( "languages.txt" ) )
			{
				if ( CountAccounts )
				{
					// count accounts
					foreach ( Account acc in Accounts.GetAccounts() )
					{
						for ( int i = 0; i < acc.Length; i++ )
						{
							Mobile mob = acc[i];

							if ( mob == null )
								continue;

							string lang = mob.Language;

							if ( lang != null )
							{
								lang = lang.ToUpper();

								if ( !ht.ContainsKey( lang ) )
									ht[lang] = new InternationalCodeCounter( lang );
								else
									ht[lang].Increase();

								break;
							}
						}
					}
				}
				else
				{
					// count playermobiles
					foreach( Mobile mob in World.Mobiles.Values )
					{
						if ( mob.Player )
						{
							string lang = mob.Language;

							if ( lang != null )
							{
								lang = lang.ToUpper();

								if ( !ht.ContainsKey( lang ) )
									ht[lang] = new InternationalCodeCounter( lang );
								else
									ht[lang].Increase();
							}
						}
					}
				}

				writer.WriteLine( String.Format( "Language statistics. Numbers show how many {0} use the specified language.", CountAccounts ? "accounts" : "playermobile" ) );
				writer.WriteLine( "====================================================================================================" );
				writer.WriteLine();

				// sort the list
				List<InternationalCodeCounter> list = new List<InternationalCodeCounter>( ht.Values );
				list.Sort( InternationalCodeComparer.Instance );

				foreach ( InternationalCodeCounter c in list )
					writer.WriteLine( String.Format( "{0}? : {1}", GetFormattedInfo( c.Code ), c.Count ) );

				e.Mobile.SendMessage( "Languages list generated." );
			}
		}

		private class InternationalCodeCounter
		{
			private string m_Code;
			private int m_Count;

			public string Code{ get{ return m_Code; } }
			public int Count{ get{ return m_Count; } }

			public InternationalCodeCounter( string code )
			{
				m_Code = code;
				m_Count = 1;
			}

			public void Increase()
			{
				m_Count++;
			}
		}

		private class InternationalCodeComparer : IComparer<InternationalCodeCounter>
		{
			public static readonly InternationalCodeComparer Instance = new InternationalCodeComparer();

			public InternationalCodeComparer()
			{
			}

			public int Compare( InternationalCodeCounter x, InternationalCodeCounter y )
			{
				string a = null, b = null;
				int ca = 0, cb = 0;

				a = x.Code;
				ca = x.Count;
				b = y.Code;
				cb = y.Count;


				if ( ca > cb )
					return -1;

				if ( ca < cb )
					return 1;

				if ( a == null && b == null )
					return 0;

				if ( a == null )
					return 1;

				if ( b == null )
					return -1;

				return a.CompareTo( b );
			}
		}
	}
}