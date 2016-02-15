using System;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Items;

namespace Server.Commands
{
	public class AddDeco
	{
		private static readonly ImageTileButtonInfo[] m_Buttons = new ImageTileButtonInfo[]
		{
			new DecoImageTileButtonInfo( new DecoParameters( 0xACA, 0xAC9, 0xACB, 0xACC, 0xACE, 0xAD0, 0xACD, 0xACF, 0xAC8 ), "Red Rug" ),
			new DecoImageTileButtonInfo( new DecoParameters( 0xAC3, 0xAC2, 0xAC4, 0xAC5, 0xAF7, 0xAF9, 0xAF6, 0xAF8, 0xABE ), "Blue Rug" ),
			new DecoImageTileButtonInfo( new DecoParameters( 0xAE4, 0xAE3, 0xAE5, 0xAE6, 0xAE8, 0xAEA, 0xAE7, 0xAE9, 0xAEB ), "Dark Red Rug" ),
			new DecoImageTileButtonInfo( new DecoParameters( 0xADC, 0xADB, 0xADD, 0xADE, 0xAE0, 0xAE2, 0xADF, 0xAE1, 0xADA ), "Green Red Rug" ),
			new DecoImageTileButtonInfo( new DecoParameters( 0xAD3, 0xAD2, 0xAD4, 0xAD5, 0xAD7, 0xAD9, 0xAD6, 0xAD8, 0xAD1 ), "Green Blue Rug" ),
			new DecoImageTileButtonInfo( new DecoParameters( 0xAEF, 0xAEE, 0xAF0, 0xAF1, 0xAF3, 0xAF5, 0xAF2, 0xAF4, 0xAEC ), "Red Blue Rug" ),
			new DecoImageTileButtonInfo( new DecoParameters( 0xB72, 0xB71, 0xB70, 0xB72, 0xB73, 0xB74, 0xB73, 0xB73, 0xB73 ), "Dark Brown Table" ),
			new DecoImageTileButtonInfo( new DecoParameters( 0xB8D, 0xB8B, 0xB8C, 0xB8A, 0xB8D, 0xB8D, 0xB8D, 0xB8D, 0xB8D ), "Light Brown Table" ),

			new DecoImageTileButtonInfo(
				new DecoParameters(
					new DecoPiece[]{ new DecoPiece( 0xB10 ), new DecoPiece( 0xAA3, 3 ) },
					new DecoPiece[]{ new DecoPiece( 0xB11 ), new DecoPiece( 0xB18, 3 ) },
					new DecoPiece[]{ new DecoPiece( 0xB12 ), new DecoPiece( 0xAA5, 3 ) },
					new DecoPiece[]{ new DecoPiece( 0xB13 ), new DecoPiece( 0xAA4, 3 ) },
					new DecoPiece[]{ new DecoPiece( 0xB17 ), new DecoPiece( 0xAA2, 3 ) },
					new DecoPiece[]{ new DecoPiece( 0xB15 ), new DecoPiece( 0xAA0, 3 ) },
					new DecoPiece[]{ new DecoPiece( 0xB16 ), new DecoPiece( 0xAA1, 3 ) },
					new DecoPiece[]{ new DecoPiece( 0xB14 ), new DecoPiece( 0xA9F, 3 ) },
					new DecoPiece[]{ new DecoPiece( 0xB0F ) } ), 0xB09, "Display Case" )
		};

		public class DecoGump : BaseImageTileButtonsGump
		{
			public DecoGump() : base( "Choose a decoration to add", m_Buttons )
			{
			}

			public override void HandleButtonResponse( NetState sender, int adjustedButton, ImageTileButtonInfo buttonInfo )
			{
				//AccessLevel check?

				DecoImageTileButtonInfo decobuttoninfo = (DecoImageTileButtonInfo)buttonInfo;
				BoundingBoxPicker.Begin( sender.Mobile, new BoundingBoxCallback( AddDeco_Callback ), decobuttoninfo.DecoParameters );
			}
		}

		public class DecoImageTileButtonInfo : ImageTileButtonInfo
		{
			private DecoParameters m_DecoParameters;
			public DecoParameters DecoParameters{ get{ return m_DecoParameters; } set{ m_DecoParameters = value; } }

			public DecoImageTileButtonInfo( DecoParameters deco, TextDefinition label ) : this( deco, ShowID( deco.Center ), label, -1 )
			{
			}

			public DecoImageTileButtonInfo( DecoParameters deco, int iconID, TextDefinition label ) : this( deco, iconID, label, -1 )
			{
			}

			public DecoImageTileButtonInfo( DecoParameters deco, int iconID, TextDefinition label, int localizedTooltip ) : base( iconID, 0, label, localizedTooltip )	//TODO: Change ShowID to use something else if there's nothign found in center
			{
				m_DecoParameters = deco;
			}
		}

		public class DecoPiece
		{
			public int ItemID { get { return m_ItemID; } }
			public int ZOffset { get { return m_zOffset; } }
			public int Count { get { return m_Count; } }

			private int m_ItemID;
			private int m_zOffset;
			private int m_Count;

			public DecoPiece( int itemid ) : this( itemid, 0, 0 )
			{
			}
			public DecoPiece( int itemid, int zoffset ) : this( itemid, zoffset, 0 )
			{
			}

			public DecoPiece( int itemid, int zoffset, int count )
			{
				m_ItemID = itemid;
				m_zOffset = zoffset;
				m_Count = count;
			}

			public DecoPiece( int itemid, bool useCount, int count ) : this( itemid, 0, ( useCount ? count : 0 ) )
			{
			}

			public static implicit operator DecoPiece( int v )
			{
				return new DecoPiece( v );
			}
		}

		public class DecoParameters
		{
			public DecoPiece[] Top, Bottom, Left, Right, North, South, West, East, Center;

			public DecoParameters( DecoPiece top, DecoPiece bottom, DecoPiece left, DecoPiece right, DecoPiece north, DecoPiece south, DecoPiece west, DecoPiece east, DecoPiece center )
				: this( ToArray( top ), ToArray( bottom ), ToArray( left ), ToArray( right ), ToArray( north ), ToArray( south ), ToArray( west ), ToArray( east ), ToArray( center ) )
			{
			}

			private static DecoPiece[] ToArray( DecoPiece d )
			{
				if( d == null )
					return new DecoPiece[]{};

				DecoPiece[] pieces = new DecoPiece[1];

				pieces[0] = d;

				return pieces;
			}

			public DecoParameters( DecoPiece[] top, DecoPiece[] bottom, DecoPiece[] left, DecoPiece[] right,
				DecoPiece[] north, DecoPiece[] south, DecoPiece[] west, DecoPiece[] east, DecoPiece[] center )
			{
				Top = top;
				Bottom = bottom;
				Left = left;
				Right = right;
				North = north;
				South = south;
				West = west;
				East = east;
				Center = center;
			}
		}

		public static void Initialize()
		{
			CommandSystem.Register( "AddDeco", AccessLevel.GameMaster, new CommandEventHandler( AddDeco_OnCommand ) );
		}

		[Usage("AddDeco")]
		[Description("Allows a user to add a decoration")]
		private static void AddDeco_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.SendGump( new DecoGump() );
		}

		public static void AddDeco_Callback( Mobile from, Map map, Point3D start, Point3D end, object state )
		{
			DecoParameters decoParams = (DecoParameters)state;

			int height = end.Y - start.Y + 1;
			int width = end.X - start.X + 1;

			for( int x = 0; x < width; x++ )
			{
				int xcord = start.X + x;

				for ( int y = 0; y < height; y++ )
				{
					int ycord = start.Y + y;

					if ( xcord == start.X )
					{
						if ( ycord == start.Y )
							AddItems( decoParams.Top, new Point3D( xcord, ycord, start.Z ), map );
						else if ( ycord == end.Y )
							AddItems( decoParams.Left, new Point3D( xcord, ycord, start.Z ), map );
						else
							AddItems( decoParams.West, new Point3D( xcord, ycord, start.Z ), map );
					}
					else if ( ycord == start.Y )
					{
						if ( xcord == end.X )
							AddItems( decoParams.Right, new Point3D( xcord, ycord, start.Z ), map );
						else
							AddItems( decoParams.North, new Point3D( xcord, ycord, start.Z ), map );
					}
					else if ( xcord == end.X )
					{
						if ( ycord == end.Y )
							AddItems( decoParams.Bottom, new Point3D( xcord, ycord, start.Z ), map );
						else
							AddItems( decoParams.East, new Point3D( xcord, ycord, start.Z ), map );
					}
					else if ( ycord == end.Y )
						AddItems( decoParams.South, new Point3D( xcord, ycord, start.Z ), map );
					else
						AddItems( decoParams.Center, new Point3D( xcord, ycord, start.Z ), map );
				}
			}
		}

		public static void AddItems( DecoPiece[] Pieces, Point3D point, Map map )
		{
			for ( int i = 0; i < Pieces.Length; i++ )
			{
				DecoPiece piece = Pieces[i];

				if( piece.ItemID <= 0 )
					continue;

				Item item;

				if( piece.Count > 0 )
					item = new Static( piece.ItemID, piece.Count );
				else
					item = new Static( piece.ItemID );

				item.MoveToWorld( new Point3D( point.X, point.Y, point.Z + Pieces[i].ZOffset ), map );
			}
		}

		public static int ShowID( DecoPiece[] Pieces )
		{
			if ( Pieces.Length > 0 )
				return Pieces[0].ItemID;
			else
				return 0;
		}
	}
}