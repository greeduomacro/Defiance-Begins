using System;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Gumps;

namespace Server.Mobiles
{
	public class SpawnerGump : Gump
	{
		private Spawner m_Spawner;
		private int m_Page;
		private SpawnerEntry m_Entry;

		public SpawnerGump( Spawner spawner ) : this( spawner, 0 )
		{
		}

		public SpawnerGump( Spawner spawner, int page ) : this( spawner, null, page )
		{
		}

		public SpawnerGump( Spawner spawner, SpawnerEntry focusentry, int page ) : base( 50, 50 )
		{
			m_Spawner = spawner;
			m_Entry = focusentry;
			m_Page = page;

			AddPage( 0 );

			AddBackground( 0, 0, 343, 371 + ( m_Entry != null ? 44 : 0 ), 5054 );

			AddLabel( 95, 1, 0, "Creatures List" );

			int offset = 0;

			for ( int i = 0; i < 13; i++ )
			{
				int textindex = i * 5;
				int entryindex = ( m_Page * 13 ) + i;

				SpawnerEntry entry = null;

				if ( entryindex < spawner.Entries.Count )
					entry = m_Spawner.Entries[entryindex];

				if ( entry == null || m_Entry != entry )
					AddButton( 5, ( 22 * i ) + 21 + offset, entry != null ? 0xFBA : 0xFA5, entry != null ? 0xFBC : 0xFA7, GetButtonID( 2, (i * 2) ), GumpButtonType.Reply, 0 ); //Expand
				else
					AddButton( 5, ( 22 * i ) + 21 + offset, entry != null ? 0xFBB : 0xFA5, entry != null ? 0xFBC : 0xFA7, GetButtonID( 2, (i * 2) ), GumpButtonType.Reply, 0 ); //Unexpand

				AddButton( 38, ( 22 * i ) + 21 + offset, 0xFA2, 0xFA4, GetButtonID( 2, 1 + (i * 2) ), GumpButtonType.Reply, 0 ); //Delete

				AddImageTiled( 71, (22 * i) + 20 + offset, 161, 23, 0xA40 ); //creature text box
				AddImageTiled( 72, (22 * i) + 21 + offset, 159, 21, 0xBBC ); //creature text box

				AddImageTiled( 235, (22 * i) + 20 + offset, 35, 23, 0xA40 ); //maxcount text box
				AddImageTiled( 236, (22 * i) + 21 + offset, 33, 21, 0xBBC ); //maxcount text box

				AddImageTiled( 273, (22 * i) + 20 + offset, 35, 23, 0xA40 ); //probability text box
				AddImageTiled( 274, (22 * i) + 21 + offset, 33, 21, 0xBBC ); //probability text box

				string name = "";
				string probability = "";
				string maxcount = "";
				EntryFlags flags = EntryFlags.None;

				if ( entry != null )
				{
					name = (string)entry.SpawnedName;
					probability = entry.SpawnedProbability.ToString();
					maxcount = entry.SpawnedMaxCount.ToString();
					flags = entry.Valid;

					AddLabel( 315, (22 * i) + 20 + offset, 0, spawner.CountSpawns( entry ).ToString() );
				}

				AddTextEntry( 75, (22 * i) + 21 + offset, 156, 21, ( ( flags & EntryFlags.InvalidType ) != 0 ) ? 33 : 0, textindex, name ); //creature
				AddTextEntry( 239, (22 * i) + 21 + offset, 30, 21, 0, textindex + 1, maxcount); //max count
				AddTextEntry( 277, (22 * i) + 21 + offset, 30, 21, 0, textindex + 2, probability); //probability

				if ( entry != null && m_Entry == entry )
				{
					AddLabel( 5, (22 * i) + 42, 0x384, "Params" );
					AddImageTiled( 55, (22 * i) + 42, 253, 23, 0xA40 ); //Parameters
					AddImageTiled( 56, (22 * i) + 43, 251, 21, 0xBBC ); //Parameters

					AddLabel( 5, (22 * i) + 64, 0x384, "Props" );
					AddImageTiled( 55, (22 * i) + 64, 253, 23, 0xA40 ); //Properties
					AddImageTiled( 56, (22 * i) + 65, 251, 21, 0xBBC ); //Properties

					AddTextEntry( 59, (22 * i) + 42, 248, 21, ( ( flags & EntryFlags.InvalidParams ) != 0 ) ? 33 : 0, textindex + 3, entry.Parameters ); //parameters
					AddTextEntry( 59, (22 * i) + 62, 248, 21, ( ( flags & EntryFlags.InvalidProps ) != 0 ) ? 33 : 0, textindex + 4, entry.Properties ); //properties

					offset += 44;
				}
			}

			AddButton( 5, 347 + offset, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0 );
			AddLabel( 38, 347 + offset, 0x384, "Cancel" );

			AddButton( 5, 325 + offset, 0xFB7, 0xFB9, GetButtonID( 1, 2 ), GumpButtonType.Reply, 0 );
			AddLabel( 38, 325 + offset, 0x384, "Okay" );

			AddButton( 110, 325 + offset, 0xFB4, 0xFB6, GetButtonID( 1, 3 ), GumpButtonType.Reply, 0 );
			AddLabel( 143, 325 + offset, 0x384, "Bring to Home" );

			AddButton( 110, 347 + offset, 0xFA8, 0xFAA, GetButtonID( 1, 4 ), GumpButtonType.Reply, 0 );
			AddLabel( 143, 347 + offset, 0x384, "Total Respawn" );

			AddButton( 253, 325 + offset, 0xFB7, 0xFB9, GetButtonID( 1, 5 ), GumpButtonType.Reply, 0 );
			AddLabel( 286, 325 + offset, 0x384, "Apply" );

			if ( m_Page > 0 )
				AddButton( 276, 308 + offset, 0x15E3, 0x15E7, GetButtonID( 1, 0 ), GumpButtonType.Reply, 0 );
			else
				AddImage( 276, 308 + offset, 0x25EA );

			if ( (m_Page + 1) * 13 <= m_Spawner.Entries.Count )
				AddButton( 293, 308 + offset, 0x15E1, 0x15E5, GetButtonID( 1, 1 ), GumpButtonType.Reply, 0 );
			else
				AddImage( 293, 308 + offset, 0x25E6 );
		}

		public int GetButtonID( int type, int index )
		{
			return 1 + (index * 10) + type;
		}

		public void CreateArray( RelayInfo info, Mobile from, Spawner spawner )
		{
			int ocount = spawner.Entries.Count;

			List<SpawnerEntry> rementries = new List<SpawnerEntry>();

			for ( int i = 0;i < 13; i++ )
			{
				int index = i * 5;
				int entryindex = ( m_Page * 13 ) + i;

				TextRelay cte = info.GetTextEntry( index );
				TextRelay mte = info.GetTextEntry( index + 1 );
				TextRelay poste = info.GetTextEntry( index + 2 );
				TextRelay parmte = info.GetTextEntry( index + 3 );
				TextRelay propte = info.GetTextEntry( index + 4 );

				if ( cte != null )
				{
					string str = cte.Text.Trim().ToLower();

					if ( str.Length > 0 )
					{
						Type type = SpawnerType.GetType( str );

						if ( type != null )
						{
							SpawnerEntry entry = null;

							if ( entryindex < ocount )
							{
								entry = spawner.Entries[entryindex];
								entry.SpawnedName = str;

								if ( mte != null )
									entry.SpawnedMaxCount = Utility.ToInt32( mte.Text.Trim() );

								if ( poste != null )
									entry.SpawnedProbability = Utility.ToInt32( poste.Text.Trim() );
							}
							else
							{
								int maxcount = 1;
								int probcount = 100;

								if ( mte != null )
									maxcount = Utility.ToInt32( mte.Text.Trim() );

								if ( poste != null )
									probcount = Utility.ToInt32( poste.Text.Trim() );

								entry = spawner.AddEntry( str, probcount, maxcount );
							}

							if ( parmte != null )
								entry.Parameters = parmte.Text.Trim();

							if ( propte != null )
								entry.Properties = propte.Text.Trim();
						}
						else
							from.SendMessage( "{0} is not a valid type name for entry #{1}.", str, i );
					}
					else if ( entryindex < ocount && spawner.Entries[entryindex] != null )
						rementries.Add( spawner.Entries[entryindex] );
				}
			}

			if ( rementries.Count > 0 )
				for ( int i = 0; i < rementries.Count; i++ )
					spawner.RemoveEntry( rementries[i] );

			if ( ocount == 0 && spawner.Entries.Count > 0 )
				spawner.Start();
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if ( m_Spawner.Deleted )
				return;

			Mobile from = state.Mobile;

			int val = info.ButtonID - 1;

			if ( val < 0 )
				return;

			int type = val % 10;
			int index = val / 10;

			switch ( type )
			{
				case 0: //Cancel
					return;
				case 1:
				{
					switch ( index )
					{
						case 0:
						{
							if ( m_Spawner.Entries != null && m_Page > 0 )
							{
								m_Page--;
								m_Entry = null;
							}
							break;
						}
						case 1:
						{
							if ( m_Spawner.Entries != null && (m_Page + 1) * 13 <= m_Spawner.Entries.Count )
							{
								m_Page++;
								m_Entry = null;
							}
							break;
						}
						case 2: //Okay
						{
							CreateArray(info, state.Mobile, m_Spawner);
							return;
						}
						case 3:
						{
							m_Spawner.BringToHome();
							break;
						}
						case 4: // Complete respawn
						{
							m_Spawner.Respawn();
							break;
						}
						case 5:
						{
							CreateArray(info, state.Mobile, m_Spawner);
							break;
						}
					}
					break;
				}
				case 2:
				{
					int entryindex = ( index / 2 ) + ( m_Page * 13 );
					int buttontype = index % 2;

					if ( entryindex >= 0 && entryindex < m_Spawner.Entries.Count )
					{
						SpawnerEntry entry = m_Spawner.Entries[entryindex];
						if ( buttontype == 0 ) // Spawn creature
						{
							if ( m_Entry != null && m_Entry == entry )
								m_Entry = null;
							else
								m_Entry = entry;
						}
						else // Remove creatures
							m_Spawner.RemoveSpawns( entryindex );
					}

					CreateArray( info, state.Mobile, m_Spawner );
					break;
				}
			}

			state.Mobile.SendGump( new SpawnerGump( m_Spawner, m_Entry, m_Page ) );
		}
	}
}