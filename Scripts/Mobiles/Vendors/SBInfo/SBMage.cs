using System;
using System.Collections.Generic;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	public class SBMage : SBInfo
	{
		private List<IBuyItemInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBMage()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<IBuyItemInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<IBuyItemInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( Spellbook ), 125, 10, 0xEFA, 0 ) );

				if ( Core.AOS )
					Add( new GenericBuyInfo( typeof( NecromancerSpellbook ), 115, 10, 0x2253, 0 ) );

				Add( new GenericBuyInfo( typeof( ScribesPen ), 8, 10, 0xFBF, 0 ) );

				Add( new GenericBuyInfo( typeof( BlankScroll ), 5, 20, 0x0E34, 0 ) );

				Add( new GenericBuyInfo( "1041072", typeof( MagicWizardsHat ), 11, 10, 0x1718, Utility.RandomDyedHue() ) );

				Add( new GenericBuyInfo( typeof( RecallRune ), 15, 10, 0x1F14, 0 ) );

				Add( new GenericBuyInfo( typeof( RefreshPotion ), 15, 10, 0xF0B, 0 ) );
				Add( new GenericBuyInfo( typeof( AgilityPotion ), 15, 10, 0xF08, 0 ) );
				Add( new GenericBuyInfo( typeof( NightSightPotion ), 15, 10, 0xF06, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserHealPotion ), 15, 10, 0xF0C, 0 ) );
				Add( new GenericBuyInfo( typeof( StrengthPotion ), 15, 10, 0xF09, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserPoisonPotion ), 15, 10, 0xF0A, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserCurePotion ), 15, 10, 0xF07, 0 ) );
				Add( new GenericBuyInfo( typeof( LesserExplosionPotion ), 21, 10, 0xF0D, 0 ) );

				Add( new GenericBuyInfo( typeof( BlackPearl ), 5, 20, 0xF7A, 0 ) );
				Add( new GenericBuyInfo( typeof( Bloodmoss ), 5, 20, 0xF7B, 0 ) );
				Add( new GenericBuyInfo( typeof( Garlic ), 3, 20, 0xF84, 0 ) );
				Add( new GenericBuyInfo( typeof( Ginseng ), 3, 20, 0xF85, 0 ) );
				Add( new GenericBuyInfo( typeof( MandrakeRoot ), 3, 20, 0xF86, 0 ) );
				Add( new GenericBuyInfo( typeof( Nightshade ), 3, 20, 0xF88, 0 ) );
				Add( new GenericBuyInfo( typeof( SpidersSilk ), 3, 20, 0xF8D, 0 ) );
				Add( new GenericBuyInfo( typeof( SulfurousAsh ), 3, 20, 0xF8C, 0 ) );

				/*if ( Core.AOS )
				//{
					Add( new GenericBuyInfo( typeof( BatWing ), 3, 20, 0xF78, 0 ) );
					Add( new GenericBuyInfo( typeof( DaemonBlood ), 6, 20, 0xF7D, 0 ) );
					Add( new GenericBuyInfo( typeof( PigIron ), 5, 20, 0xF8A, 0 ) );
					Add( new GenericBuyInfo( typeof( NoxCrystal ), 6, 20, 0xF8E, 0 ) );
					Add( new GenericBuyInfo( typeof( GraveDust ), 3, 20, 0xF8F, 0 ) );
				//}*/
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( WizardsHat ), 15 );

				if ( !TestCenter.Enabled )
				{
					Add( typeof( BlackPearl ), 3 );
					Add( typeof( Bloodmoss ), 3 );
					Add( typeof( MandrakeRoot ), 2 );
					Add( typeof( Garlic ), 2 );
					Add( typeof( Ginseng ), 2 );
					Add( typeof( Nightshade ), 2 );
					Add( typeof( SpidersSilk ), 2 );
					Add( typeof( SulfurousAsh ), 2 );

					if ( Core.AOS )
					{
						Add( typeof( BatWing ), 1 );
						Add( typeof( DaemonBlood ), 2 );
						Add( typeof( PigIron ), 2 );
						Add( typeof( NoxCrystal ), 2 );
						Add( typeof( GraveDust ), 1 );
					}
				}

				Add( typeof( RecallRune ), 13 );
				Add( typeof( Spellbook ), 15 );

				Type[] types = Loot.RegularScrollTypes;

				for ( int i = 0; i < types.Length; ++i )
					Add( types[i], ((i / 8) + 2) * 2 );


				if ( Core.SE )
				{
					Add( typeof( ExorcismScroll ), 3 );
					Add( typeof( AnimateDeadScroll ), 8 );
					Add( typeof( BloodOathScroll ), 8 );
					Add( typeof( CorpseSkinScroll ), 8 );
					Add( typeof( CurseWeaponScroll ), 8 );
					Add( typeof( EvilOmenScroll ), 8 );
					Add( typeof( PainSpikeScroll ), 8 );
					Add( typeof( SummonFamiliarScroll ), 8 );
					Add( typeof( HorrificBeastScroll ), 8 );
					Add( typeof( MindRotScroll ), 10 );
					Add( typeof( PoisonStrikeScroll ), 10 );
					Add( typeof( WraithFormScroll ), 15 );
					Add( typeof( LichFormScroll ), 16 );
					Add( typeof( StrangleScroll ), 16 );
					Add( typeof( WitherScroll ), 16 );
					Add( typeof( VampiricEmbraceScroll ), 20 );
					Add( typeof( VengefulSpiritScroll ), 20 );
				}
			}
		}
	}
}