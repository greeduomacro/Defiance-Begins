using System;
using Server;

namespace Server.Items
{
	[TypeAlias( "Server.Items.TheBeserkersMaul" )]
	public class TheBerserkersMaul : Maul
	{
		public override int LabelNumber{ get{ return 1061108; } } // The Berserker's Maul
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable( AccessLevel.Owner )]
		public TheBerserkersMaul()
		{
			Hue = 0x21;
			Attributes.WeaponSpeed = 75;
			Attributes.WeaponDamage = 50;
		}

		public TheBerserkersMaul( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}