using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Regions;

namespace Server.Engines.CannedEvil
{
	public class GraveyardChampionSpawn : ChampionSpawn
	{
		private static Tuple<Type,int>[] m_DropList = new Tuple<Type,int>[]
		{
			new Tuple<Type,int>( typeof( TwilightLantern ), 10 ), new Tuple<Type,int>( typeof( HalloweenGhoulStatuette ), 10 ),
			new Tuple<Type,int>( typeof( HalloweenCatStatue), 1 ), new Tuple<Type,int>( typeof( PumpkinScarecrow ), 1 ),
			new Tuple<Type,int>( typeof( RuinedTapestry ), 1 )
		};

		public static Tuple<Type,int>[] DropList{ get{ return m_DropList; } }

		private static int m_DropTotal = 0;
		public static int DropTotal{ get{ return m_DropTotal; } }

		public static void Initialize()
		{
			for ( int i = 0; i < m_DropList.Length; i++ )
				m_DropTotal += m_DropList[i].Item2;
		}

		[Constructable]
		public GraveyardChampionSpawn() : base()
		{
			CannedEvilTimer.AddSpawn( this );
			MaxLevel = 18; //Maximum max level
			Type = ChampionSpawnType.Graveyard;
		}

		public GraveyardChampionSpawn( Serial serial ) : base( serial )
		{
			CannedEvilTimer.AddSpawn( this );
		}

		public override bool ProximitySpawn{ get{ return true; } }
		public override bool AlwaysActive{ get{ return false; } }
		public override bool CanAdvanceByValor{ get{ return false; } }
		public override bool CanActivateByValor{ get{ return false; } }
		public override bool HasStarRoomGate{ get{ return false; } }
		public override TimeSpan DecayTime{ get{ return TimeSpan.FromMinutes( 7.0 ); } }
		public override ChampionSpawnRegion GetRegion()
		{
			return new GraveyardChampionSpawnRegion(this);
		}

		public override int MaxSpawn{ get{ return 32 + (GetSubLevel() * 16); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public override int MaxKills
		{
			get
			{
				if ( Level >= Level5 )
					return 32;
				else if ( Level == Level4 )
					return 64;
				else if ( Level >= Level3 )
					return 96;
				else if ( Level >= Level2 )
					return 128;
				else if ( Level >= Level1 )
					return 192;
				else
					return 256;
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int)0 ); //version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();
			CannedEvilTimer.RemoveSpawn( this );
		}
	}

	public class GraveyardChampionSpawnRegion : ChampionSpawnRegion
	{
		public GraveyardChampionSpawnRegion( ChampionSpawn spawn ) : base( spawn )
		{
		}

		public override void AlterLightLevel( Mobile m, ref int global, ref int personal )
		{
			if ( Parent != null )
				Parent.AlterLightLevel( m, ref global, ref personal );
		}

		public override void OnDeath( Mobile m )
		{
			if ( m is BaseCreature )
			{
				BaseCreature bc = m as BaseCreature;

				if ( bc.ChampSpawn == Spawn )
				{
					if ( 0.05 > Utility.RandomDouble() )
					{
						int rc = Utility.RandomMinMax( 1, GraveyardChampionSpawn.DropTotal );
						int count = GraveyardChampionSpawn.DropTotal;
						Type itemType = null;

						for ( int i = 0; itemType == null && i < GraveyardChampionSpawn.DropList.Length; i++ )
						{
							Tuple<Type, int> tuple = GraveyardChampionSpawn.DropList[i];
							if ( rc <= tuple.Item2 )
								itemType = tuple.Item1;
						}

						//Assume its never null, because it shouldn't unless something has a rc of 0.
						Item item = Activator.CreateInstance( itemType ) as Item;
						if ( bc.Corpse != null )
							bc.Corpse.AddItem( item );
						else
							item.MoveToWorld( bc.Location, bc.Map );
					}
				}
			}
		}
	}
}