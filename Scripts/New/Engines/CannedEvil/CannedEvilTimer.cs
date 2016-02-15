using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Engines.CannedEvil
{
	public class CannedEvilTimer : Timer
	{
		public static void Initialize()
		{
			Instance = new CannedEvilTimer();
			Instance.Start();
		}

		private static List<DungeonChampionSpawn> DungeonSpawns = new List<DungeonChampionSpawn>();
		private static List<LLChampionSpawn> LLSpawns = new List<LLChampionSpawn>();
		private static List<GraveyardChampionSpawn> GraveyardSpawns = new List<GraveyardChampionSpawn>();
		private static DateTime SliceTime;

		public static CannedEvilTimer Instance;

		public static void AddSpawn( DungeonChampionSpawn spawn )
		{
			DungeonSpawns.Add( spawn );
			if ( Instance != null )
				Instance.DungeonSpawnOnSlice( false );
		}

		public static void AddSpawn( LLChampionSpawn spawn )
		{
			LLSpawns.Add( spawn );
			if ( Instance != null )
				Instance.LLSpawnOnSlice( false );
		}

		public static void AddSpawn( GraveyardChampionSpawn spawn )
		{
			GraveyardSpawns.Add( spawn );
			if ( Instance != null )
				Instance.GraveyardSpawnOnSlice( false );
		}

		public static void RemoveSpawn( DungeonChampionSpawn spawn )
		{
			DungeonSpawns.Remove( spawn );
			if ( Instance != null )
				Instance.DungeonSpawnOnSlice( false );
		}

		public static void RemoveSpawn( LLChampionSpawn spawn )
		{
			LLSpawns.Remove( spawn );
			if ( Instance != null )
				Instance.LLSpawnOnSlice( false );
		}

		public static void RemoveSpawn( GraveyardChampionSpawn spawn )
		{
			GraveyardSpawns.Remove( spawn );
			if ( Instance != null )
				Instance.GraveyardSpawnOnSlice( false );
		}

		public CannedEvilTimer() : base( TimeSpan.Zero, TimeSpan.FromMinutes( 1.0 ) )
		{
			Priority = TimerPriority.OneMinute;
			SliceTime = DateTime.Now;
		}

		public void DungeonSpawnOnSlice( bool rotate )
		{
			if ( DungeonSpawns.Count > 0 ) //Activate one dungeon
			{
				List<DungeonChampionSpawn> valid = new List<DungeonChampionSpawn>();

				foreach ( DungeonChampionSpawn spawn in DungeonSpawns )
				{
					if ( spawn.ActivatedByValor || ( !spawn.Active || ( rotate && spawn.Kills == 0 && spawn.Level == 0 ) ) )
					{
						spawn.Active = false;
						spawn.ReadyToActivate = false;
						valid.Add( spawn );
					}
				}

				if ( valid.Count > 0 && valid.Count > (DungeonSpawns.Count - 2) )
					valid[Utility.Random( valid.Count )].ReadyToActivate = true;
			}
		}

		public void LLSpawnOnSlice( bool rotate )
		{
			if ( LLSpawns.Count > 0 ) //Activate one lost lands total
			{
				bool inactive = true;
				foreach ( LLChampionSpawn spawn in LLSpawns )
				{
					if ( spawn.Active && !spawn.ActivatedByValor && ( !rotate || spawn.Kills > 0 || spawn.Level > 0 ) )
						inactive = false;
					else
					{
						spawn.Active = false;
						spawn.ReadyToActivate = false;
					}
				}

				if ( inactive )
					LLSpawns[Utility.Random( LLSpawns.Count )].ReadyToActivate = true;
			}
		}

		public void GraveyardSpawnOnSlice( bool rotate )
		{
			if ( GraveyardSpawns.Count > 0 ) //Activate one dungeon
			{
				List<GraveyardChampionSpawn> valid = new List<GraveyardChampionSpawn>();
				bool halloween = BaseHalloweenGiftGiver.IsHalloween();

				foreach ( GraveyardChampionSpawn spawn in GraveyardSpawns )
				{
					if ( !spawn.Active || ( rotate && spawn.Kills == 0 && spawn.Level == 0 ) )
					{
						spawn.Active = false;
						spawn.ReadyToActivate = false;
						if ( halloween )
							valid.Add( spawn );
					}
				}

				if ( valid.Count > 0 && valid.Count > (GraveyardSpawns.Count - 2) )
					valid[Utility.Random( valid.Count )].ReadyToActivate = true;
			}
		}

		protected override void OnTick()
		{
			if ( AutoRestart.Restarting )
				return;

			if ( DateTime.Now >= SliceTime )
			{
				DungeonSpawnOnSlice( true );
				LLSpawnOnSlice( true );
				GraveyardSpawnOnSlice( true );
				SliceTime = DateTime.Now.Date + TimeSpan.FromDays( 1.0 );
			}
			else
				LLSpawnOnSlice( false );
		}
	}
}