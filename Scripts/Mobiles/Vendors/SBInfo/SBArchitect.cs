using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBArchitect : SBInfo
	{
		private List<IBuyItemInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBArchitect()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<IBuyItemInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<IBuyItemInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( "1041280", typeof( InteriorDecorator ), 10001, 20, 0xFC1, 0 ) );
				if ( Core.AOS )
					Add( new GenericBuyInfo( "1060651", typeof( HousePlacementTool ), 627, 20, 0x14F6, 0 ));
				else
					Add( new GenericBuyInfo( "House Survey Tool", typeof( HouseSurveyTool ), 10000, 10, 0x14F6, 1072, new object[]{ 10 } ));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( InteriorDecorator ), 5000 );

				if ( Core.AOS )
					Add( typeof( HousePlacementTool ), 301 );
			}
		}
	}
}