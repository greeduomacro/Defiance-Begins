using System;
using Server;
using Server.Items;

namespace Server.Multis
{
	public class RentalContractCopy : Item
	{
		private RentalContract m_Contract;

		public RentalContractCopy( RentalContract contract )
		{
			Name = "rental contract copy";
			ItemID = 0x14F0;
			m_Contract = contract;
		}

		public override void OnDoubleClick( Mobile m )
		{
			if ( m_Contract == null || m_Contract.Deleted )
			{
				Delete();
				return;
			}

			m_Contract.OnDoubleClick( m );
		}

		public RentalContractCopy( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}