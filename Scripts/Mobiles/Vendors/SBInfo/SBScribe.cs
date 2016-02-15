using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBScribe: SBInfo
	{
		private List<IBuyItemInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBScribe()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<IBuyItemInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<IBuyItemInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( ScribesPen ), 8,  20, 0xFBF, 0 ) );
				Add( new GenericBuyInfo( typeof( BlankScroll ), 5, 80, 0x0E34, 0 ) );
				Add( new GenericBuyInfo( typeof( ScribesPen ), 8,  20, 0xFC0, 0 ) );
				Add( new GenericBuyInfo( typeof( BrownBook ), 15, 10, 0xFEF, 0 ) );
				Add( new GenericBuyInfo( typeof( TanBook ), 15, 10, 0xFF0, 0 ) );
				Add( new GenericBuyInfo( typeof( BlueBook ), 15, 10, 0xFF2, 0 ) );
				//Add( new GenericBuyInfo( "1041267", typeof( Runebook ), 3500, 10, 0xEFA, 0x461 ) );

				Type[] types = Loot.RegularScrollTypes;

				for ( int i = 0;i < types.Length; ++i )
				{
					int itemID = 0x1F2E + i;

					if ( i == 6 )
						itemID = 0x1F2D;
					else if ( i > 6 )
						--itemID;

					int price = 0;
					if ( (i/8) > 4 )
						price = 50 + ((i/8)*25);
					else
						price = 25 + ((i/8)*10);

					Add( new GenericBuyInfo( types[i], price, 20 - ((i/8)*2), itemID, 0 ) );
				}
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( ScribesPen ), 4 );
				Add( typeof( BrownBook ), 7 );
				Add( typeof( TanBook ), 7 );
				Add( typeof( BlueBook ), 7 );
				Add( typeof( BlankScroll ), 3 );
			}
		}
	}
}