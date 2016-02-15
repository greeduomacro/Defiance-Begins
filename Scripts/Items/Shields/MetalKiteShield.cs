using System;
using Server;

namespace Server.Items
{
	public class MetalKiteShield : BaseShield
	{
		public override int BasePhysicalResistance{ get{ return 0; } }
		public override int BaseFireResistance{ get{ return 0; } }
		public override int BaseColdResistance{ get{ return 0; } }
		public override int BasePoisonResistance{ get{ return 0; } }
		public override int BaseEnergyResistance{ get{ return 1; } }

		public override int InitMinHits{ get{ return 45; } }
		public override int InitMaxHits{ get{ return 60; } }

		public override int AosStrReq{ get{ return 45; } }

		public override int ArmorBase{ get{ return 16; } }

		[Constructable]
		public MetalKiteShield() : base( 0x1B74 )
		{
			Weight = 7.0;
			Dyable = true;
		}

		public MetalKiteShield( Serial serial ) : base(serial)
		{
		}

		public override bool DisplayDyable{ get{ return false; } }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( Weight == 5.0 )
				Weight = 7.0;
		}
	}
}