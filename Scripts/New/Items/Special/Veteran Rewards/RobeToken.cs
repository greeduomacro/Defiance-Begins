using System;
using Server;

namespace Server.Items
{
	public class RobeToken : PromotionalToken
	{
		public RobeToken() : base()
		{
		}

		public RobeToken( Serial serial ) : base( serial )
		{
		}

		public override Item CreateItemFor( Mobile from )
		{
			if ( from != null && from.Account != null )
				return new VeteranRobe( from.Account.ToString() );
			else
				return null;
		}

		public override TextDefinition ItemName{ get{ return new TextDefinition( "Veteran Robe" ); } }
		public override TextDefinition ItemReceiveMessage { get{ return new TextDefinition( "A veteran robe has been created in your bank box." ); } }
		public override TextDefinition ItemGumpName { get{ return new TextDefinition( @"<center>Veteran Robe</center>" ); } }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}