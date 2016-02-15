using System;
using Server.Items;

namespace Server.ContextMenus
{
	public class OpenBankEntry : ContextMenuEntry
	{
		private Mobile m_Banker;
		private bool m_Criminal;

		public OpenBankEntry( Mobile from, Mobile banker ) : this( from, banker, true )
		{
		}

		public OpenBankEntry( Mobile from, Mobile banker, bool criminal ) : base( 6105, 12 )
		{
			m_Banker = banker;
			m_Criminal = criminal;
		}

		public override void OnClick()
		{
			if ( !Owner.From.CheckAlive() )
				return;

			if ( Owner.From.Criminal && m_Criminal )
			{
				m_Banker.Say( 500378 ); // Thou art a criminal and cannot access thy bank box.
			}
			else
			{
				this.Owner.From.BankBox.Open();
			}
		}
	}
}