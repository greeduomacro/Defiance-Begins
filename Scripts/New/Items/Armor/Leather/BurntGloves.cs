using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Items
{
	public class BurntGloves : LeatherGloves
	{
		public override int BasePhysicalResistance{ get{ return 7; } }
		public override int BaseFireResistance{ get{ return 15; } }
		//public override int BaseColdResistance{ get{ return 3; } }
		//public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 8; } }

		public override int InitMinHits{ get{ return 10; } }
		public override int InitMaxHits{ get{ return 25; } }

		public override int AosStrReq{ get{ return 12; } }

		public override int ArtifactRarity{ get{ return 5; } }
		public override string DefaultName{ get{ return "burnt leather gloves"; } }
		public override int LabelNumber{ get{ return 0; } }

		[Constructable]
		public BurntGloves() : base()
		{
			Weight = 0.75;
			Hue = 0.30 > Utility.RandomDouble() ? 1255 : Utility.Random( 1355, 6 );
			Attributes.CastRecovery = 2;
			Attributes.EnhancePotions = -4;
			Attributes.LowerManaCost = 2;
			Attributes.LowerRegCost = 7;
		}

		public BurntGloves( Serial serial ) : base( serial )
		{
		}

		public override void OnRemoved( object parent )
		{
			Mobile from = parent as Mobile;

			if ( from != null && from.FindItemOnLayer(Layer.TwoHanded) is InfernoStaff )
			{
				AOS.Damage( from, from, Utility.Random( 15, 5 ), true, 25, 75, 0, 0, 0, 0, 0, true, false );
				from.PlaySound( 84 );
				from.SendMessage( "Your fingers burn as they grip the staff." );
			}

			base.OnRemoved( parent );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}