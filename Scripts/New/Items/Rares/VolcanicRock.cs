using System;
using Server;

namespace Server.Items
{
	public class VolcanicRock : Item
	{
		public override string DefaultName{ get{ return "a volcanic rock"; } }

		[Constructable]
		public VolcanicRock() : base( Utility.Random( 0x1369, 5 ) )
		{
			Hue = Utility.RandomBool() ? Utility.Random( 0x2401, 12 ) : Utility.Random( 1355, 5 );
		}

		public VolcanicRock( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}