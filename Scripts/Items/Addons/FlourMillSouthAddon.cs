using System;
using Server;

namespace Server.Items
{
	public class FlourMillSouthAddon : BaseFlourMillAddon
	{
		public override BaseAddonDeed Deed{ get{ return new FlourMillSouthDeed(); } }

		public override int[][] StageTable{ get{ return m_StageTable; } }

		private static int[][] m_StageTable = new int[][]
			{
				new int[]{ 0x192C, 0x192D, 0x1931 },
				new int[]{ 0x192E, 0x192F, 0x1932 },
				new int[]{ 0x1930, 0x1930, 0x1934 }
			};

		[Constructable]
		public FlourMillSouthAddon() : this( 0 )
		{
		}

		[Constructable]
		public FlourMillSouthAddon( int hue ) : base( hue )
		{
			AddComponent( new AddonComponent( 0x192C ), 0,-1, 0 );
			AddComponent( new AddonComponent( 0x192E ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0x1930 ), 0, 1, 0 );
		}

		public FlourMillSouthAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class FlourMillSouthDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new FlourMillSouthAddon( Hue ); } }
		public override int LabelNumber{ get{ return 1044348; } } // flour mill (south)

		[Constructable]
		public FlourMillSouthDeed()
		{
		}

		public FlourMillSouthDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}