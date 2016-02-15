using System;

namespace Server.Items
{
	[Flipable( 0xEC6, 0xEC7 )]
	public class Dressform : Item
	{
		[Constructable]
		public Dressform() : this( 0xEC6 )
		{
			Weight = 10;
		}

		public Dressform( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}