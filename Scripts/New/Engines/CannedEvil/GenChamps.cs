using System;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;

namespace Server.Engines.CannedEvil
{
	public class ChampionGenerator
	{
		public static void Initialize()
		{
			CommandSystem.Register( "GenChamps", AccessLevel.Owner, new CommandEventHandler( ChampionGenerator.ChampGen_OnCommand ) );
		}

		private static readonly ChampionEntry[] LLLocations = new ChampionEntry[]
		{
			new ChampionEntry( typeof( LLChampionSpawn ), new Point3D( 5511, 2360, 42 ), Map.Felucca ),
			new ChampionEntry( typeof( LLChampionSpawn ), new Point3D( 6038, 2401, 47 ), Map.Felucca ),
			new ChampionEntry( typeof( LLChampionSpawn ), new Point3D( 5549, 2640, 16 ), Map.Felucca ),
			new ChampionEntry( typeof( LLChampionSpawn ), new Point3D( 5636, 2916, 37 ), Map.Felucca ),
			new ChampionEntry( typeof( LLChampionSpawn ), new Point3D( 6035, 2943, 50 ), Map.Felucca ),
			new ChampionEntry( typeof( LLChampionSpawn ), new Point3D( 5265, 3171, 105 ), Map.Felucca ),
			new ChampionEntry( typeof( LLChampionSpawn ), new Point3D( 5282, 3368, 50 ), Map.Felucca ),
			new ChampionEntry( typeof( LLChampionSpawn ), new Point3D( 5207, 3637, 20 ), Map.Felucca ),
			new ChampionEntry( typeof( LLChampionSpawn ), new Point3D( 5954, 3475, 25 ), Map.Felucca ),
			new ChampionEntry( typeof( LLChampionSpawn ), new Point3D( 5982, 3882, 20 ), Map.Felucca ),
			new ChampionEntry( typeof( LLChampionSpawn ), new Point3D( 5724, 3991, 41 ), Map.Felucca ),

			//Lord Oaks
			new ChampionEntry( typeof( ChampionSpawn ), ChampionSpawnType.ForestLord, new Point3D( 5559, 3757, 21 ), Map.Felucca ),
		};

		private static readonly ChampionEntry[] DungeonLocations = new ChampionEntry[]
			{
				new ChampionEntry( typeof( DungeonChampionSpawn ), ChampionSpawnType.UnholyTerror, new Point3D( 5179, 709, 20 ), Map.Felucca ),
				new ChampionEntry( typeof( DungeonChampionSpawn ), ChampionSpawnType.VerminHorde, new Point3D( 5556, 825, 65 ), Map.Felucca ),
				new ChampionEntry( typeof( DungeonChampionSpawn ), ChampionSpawnType.ColdBlood, new Point3D( 5259, 837, 64 ), Map.Felucca ),
				new ChampionEntry( typeof( DungeonChampionSpawn ), ChampionSpawnType.Abyss, new Point3D( 5815, 1352, 5 ), Map.Felucca ),
				new ChampionEntry( typeof( DungeonChampionSpawn ), ChampionSpawnType.Arachnid, new Point3D( 5190, 1607, 20 ), Map.Felucca ),
			};

		[Usage( "GenChamps" )]
		[Description( "Generates champions for Felucca Dungeons & Lost Lands." )]
		private static void ChampGen_OnCommand( CommandEventArgs e )
		{
			/*
			//We take the assumption that we are spawning managed champions
			for ( int i = CannedEvilTimer.DungeonSpawns.Count-1;i >= 0; i-- )
				CannedEvilTimer.DungeonSpawns[i].Delete();

			for ( int i = CannedEvilTimer.LLSpawns.Count-1;i >= 0; i-- )
				CannedEvilTimer.LLSpawns[i].Delete();
			*/

			//We assume that all champion spawns are generated here.
			List<ChampionSpawn> spawns = new List<ChampionSpawn>();
			foreach ( Item item in World.Items.Values )
			{
				ChampionSpawn spawn = item as ChampionSpawn;
				if ( spawn != null )
					spawns.Add( spawn );
			}

			for ( int i = spawns.Count-1;i >= 0; i-- )
				spawns[i].Delete();

			Process( DungeonLocations );
			Process( LLLocations );
			//ProcessIlshenar();
			//ProcessTokuno();
		}

		private static void Process( ChampionEntry[] entries )
		{
			for ( int i = 0;i < entries.Length; i++ )
			{
				ChampionEntry entry = entries[i];

				try
				{
					ChampionSpawn spawn = Activator.CreateInstance( entry.m_ChampType ) as ChampionSpawn;
					spawn.RandomizeType = entry.m_RandomizeType;
					spawn.Type = entry.m_Type;
					spawn.MoveToWorld( entry.m_SignLocation, entry.m_Map );
					if ( spawn.AlwaysActive )
						spawn.ReadyToActivate = true;
				}
				catch
				{
					Console.WriteLine( "World: Failed to generate champion spawn {0} at {1} ({2}).", entry.m_ChampType.FullName, entry.m_SignLocation, entry.m_Map );
				}
			}
		}

		private class ChampionEntry
		{
			public bool m_RandomizeType;
			public ChampionSpawnType m_Type;
			public Point3D m_SignLocation;
			public Type m_ChampType;
			public Map m_Map;

			public ChampionEntry( Type champtype, ChampionSpawnType type, Point3D signloc, Map map ) : this( champtype, type, signloc, map, false )
			{
			}

			public ChampionEntry( Type champtype, Point3D signloc, Map map ) : this( champtype, ChampionSpawnType.Abyss, signloc, map, true )
			{
			}

			public ChampionEntry( Type champtype, ChampionSpawnType type, Point3D signloc, Map map, bool randomizetype )
			{
				m_ChampType = champtype;
				m_RandomizeType = randomizetype;
				m_Type = type;
				m_SignLocation = signloc;
				m_Map = map;
			}
		}
	}
}