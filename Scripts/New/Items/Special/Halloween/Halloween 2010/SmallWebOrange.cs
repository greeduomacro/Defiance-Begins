using System;
using Server;

namespace Server.Items
{
	public class SmallWebOrange : Item
	{
		[Constructable( AccessLevel.Owner )]
		public SmallWebOrange() : base( 0x10D6 )
		{
			Hue = 43;
		}

		public SmallWebOrange( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}