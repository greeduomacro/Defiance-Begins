using System;
using Server;

namespace Server.Items
{
	public class FlourMillEastAddon : BaseFlourMillAddon
	{
		public override BaseAddonDeed Deed{ get{ return new FlourMillEastDeed(); } }

		public override int[][] StageTable{ get{ return m_StageTable; } }

		private static int[][] m_StageTable = new int[][]
			{
				new int[]{ 0x1920, 0x1921, 0x1925 },
				new int[]{ 0x1922, 0x1923, 0x1926 },
				new int[]{ 0x1924, 0x1924, 0x1928 }
			};

		[Constructable]
		public FlourMillEastAddon() : this( 0 )
		{
		}

		[Constructable]
		public FlourMillEastAddon( int hue ) : base( hue )
		{
			AddComponent( new AddonComponent( 0x1920 ),-1, 0, 0 );
			AddComponent( new AddonComponent( 0x1922 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0x1924 ), 1, 0, 0 );
		}

		public FlourMillEastAddon( Serial serial ) : base( serial )
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

			UpdateStage();
		}
	}

	public class FlourMillEastDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new FlourMillEastAddon( Hue ); } }
		public override int LabelNumber{ get{ return 1044347; } } // flour mill (east)

		[Constructable]
		public FlourMillEastDeed()
		{
		}

		public FlourMillEastDeed( Serial serial ) : base( serial )
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