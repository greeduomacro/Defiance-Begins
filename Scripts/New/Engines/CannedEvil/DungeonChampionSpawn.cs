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
	public class DungeonChampionSpawn : ChampionSpawn
	{
		[Constructable]
		public DungeonChampionSpawn() : base()
		{
			CannedEvilTimer.AddSpawn( this );
		}

		public DungeonChampionSpawn( Serial serial ) : base( serial )
		{
			CannedEvilTimer.AddSpawn( this );
		}

		public override bool ProximitySpawn{ get{ return true; } }
		public override bool AlwaysActive{ get{ return false; } }

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
}