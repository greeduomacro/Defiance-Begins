using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Accounting;
using Server.Regions;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;

namespace Server.Engines.VeteranRewards
{
	public class RewardSystem
	{
		private static RewardCategory[] m_Categories;
		private static RewardList[] m_Lists;
		private static RewardLabelHue[] m_LabelHues;

		public static RewardCategory[] Categories
		{
			get
			{
				if ( m_Categories == null )
					SetupRewardTables();

				return m_Categories;
			}
		}

		public static RewardList[] Lists
		{
			get
			{
				if ( m_Lists == null )
					SetupRewardTables();

				return m_Lists;
			}
		}

		public static RewardLabelHue[] LabelHues
		{
			get
			{
				if ( m_LabelHues == null )
					SetupRewardTables();

				return m_LabelHues;
			}
		}

		public static bool Enabled = true; // change to true to enable vet rewards
		public static bool SkillCapRewards = false; // assuming vet rewards are enabled, should total skill cap bonuses be awarded? (720 skills total at 4th level)
		public static TimeSpan RewardInterval = TimeSpan.FromDays( 180 );
		public static TimeSpan RewardGameTimeInterval = TimeSpan.FromHours( 100 );

		public static bool HasAccess( Mobile mob, RewardCategory category )
		{
			List<RewardEntry> entries = category.Entries;

			for ( int j = 0; j < entries.Count; ++j )
			{
				//RewardEntry entry = entries[j];
				if ( RewardSystem.HasAccess( mob, entries[j] ) )
					return true;
			}
			return false;
		}

		public static bool HasAccess( Mobile mob, RewardEntry entry )
		{
			if ( Core.Expansion < entry.RequiredExpansion )
			{
				return false;
			}

			TimeSpan tsAge, tsGameTime;
			return HasAccess( mob, entry.List, out tsAge, out tsGameTime );
		}

		public static bool HasAccess( Mobile mob, RewardList list, out TimeSpan tsAge, out TimeSpan tsGameTime )
		{
			if ( list == null )
			{
				tsAge = TimeSpan.Zero;
				tsGameTime = TimeSpan.Zero;
				return false;
			}

			Account acct = mob.Account as Account;

			if ( acct == null )
			{
				tsAge = TimeSpan.Zero;
				tsGameTime = TimeSpan.Zero;
				return false;
			}

			TimeSpan totalTime = (DateTime.Now - acct.Created);

			tsAge = ( list.Age - totalTime );
			tsGameTime = (list.GameTime - acct.TotalGameTime );

			if ( tsAge <= TimeSpan.Zero && tsGameTime <= TimeSpan.Zero )
				return true;

			return false;
		}

		public static int GetRewardLevel( Mobile mob )
		{
			Account acct = mob.Account as Account;

			if ( acct == null )
				return 0;

			return GetRewardLevel( acct );
		}

		public static int GetRewardLevel( Account acct )
		{
			TimeSpan totalTime = (DateTime.Now - acct.Created);

			int level = (int) Math.Min( totalTime.TotalDays / RewardInterval.TotalDays,
				acct.TotalGameTime.TotalHours / RewardGameTimeInterval.TotalHours );

			if ( level < 0 )
				level = 0;

			return level;
		}

		public static bool HasHalfLevel( Mobile mob )
		{
			Account acct = mob.Account as Account;

			if ( acct == null )
				return false;

			return HasHalfLevel( acct );
		}

		public static bool HasHalfLevel( Account acct )
		{
			TimeSpan totalTime = (DateTime.Now - acct.Created);

			double level = (totalTime.TotalDays / RewardInterval.TotalDays);

			return level >= 0.5;
		}

		public static bool ConsumeRewardPoint( Mobile mob )
		{
			int cur, max;

			ComputeRewardInfo( mob, out cur, out max );

			if ( cur >= max )
				return false;

			Account acct = mob.Account as Account;

			if ( acct == null )
				return false;

			if ( mob.AccessLevel == AccessLevel.Player )
				acct.SetTag( "numRewardsChosen", (cur + 1).ToString() );

			return true;
		}

		public static void ComputeRewardInfo( Mobile mob, out int cur, out int max )
		{
			int level;

			ComputeRewardInfo( mob, out cur, out max, out level );
		}

		public static void ComputeRewardInfo( Mobile mob, out int cur, out int max, out int level )
		{
			Account acct = mob.Account as Account;

			if ( acct == null )
			{
				cur = max = level = 0;
				return;
			}

			level = GetRewardLevel( acct );

			if ( level == 0 )
			{
				cur = max = 0;
				return;
			}

			string tag = acct.GetTag( "numRewardsChosen" );

			if ( String.IsNullOrEmpty( tag ) )
				cur = 0;
			else
				cur = Utility.ToInt32( tag );

			if ( level >= 6 )
				max = 9 + ((level - 6) * 2);
			else
				max = 2 + level;
		}

		public static bool CheckIsUsableBy( Mobile from, Item item, object[] args )
		{
			if ( from.AccessLevel >= AccessLevel.GameMaster )
				return true;

			if ( m_Lists == null )
				SetupRewardTables();

			bool isRelaxedRules = ( item is DyeTub || item is MonsterStatuette );

			Type type = item.GetType();

			for ( int i = 0; i < m_Lists.Length; ++i )
			{
				RewardList list = m_Lists[i];
				RewardEntry[] entries = list.Entries;
				TimeSpan tsAge, tsGameTime;

				for ( int j = 0; j < entries.Length; ++j )
				{
					if ( entries[j].ItemType == type )
					{
						if ( args == null && entries[j].Args.Length == 0 )
						{
							if ( (!isRelaxedRules || i > 0) && !HasAccess( from, list, out tsAge, out tsGameTime ) )
							{
								if ( tsAge > TimeSpan.Zero )
									from.SendLocalizedMessage( 1008126, true, Math.Ceiling(tsAge.TotalDays / 30.0).ToString()); // Your account is not old enough to use this item. Months until you can use this item :
								else
									from.SendMessage( "Your account is not active enough to take advantage of this veteran reward." );
								return false;
							}
							return true;
						}

						if ( args.Length == entries[j].Args.Length )
						{
							bool match = true;

							for ( int k = 0; match && k < args.Length; ++k )
								match = ( args[k].Equals( entries[j].Args[k] ) );

							if ( match )
							{
								if ((!isRelaxedRules || i > 0) && !HasAccess(from, list, out tsAge, out tsGameTime) )
								{
									if ( tsAge > TimeSpan.Zero )
										from.SendLocalizedMessage( 1008126, true, Math.Ceiling( tsAge.TotalDays / 30.0 ).ToString()); // Your account is not old enough to use this item. Months until you can use this item :
									else
										from.SendMessage( "Your account is not active enough to take advantage of this veteran reward." );
									return false;
								}
								return true;
							}
						}
					}
				}
			}

			// no entry?
			return true;
		}

		public static int GetRewardYearLabel( Item item, object[] args )
		{
			int level =  GetRewardYear( item, args );

			return 1076216 + ( ( level < 10 ) ? level : ( level < 12 ) ? (( level - 9 ) + 4240 ) : (( level - 11 ) + 37585 ) );
		}

		public static int GetRewardYear( Item item, object[] args )
		{
			if ( m_Lists == null )
				SetupRewardTables();

			Type type = item.GetType();

			for ( int i = 0; i < m_Lists.Length; ++i )
			{
				RewardList list = m_Lists[i];
				RewardEntry[] entries = list.Entries;

				for ( int j = 0; j < entries.Length; ++j )
				{
					if ( entries[j].ItemType == type )
					{
						if ( args == null && entries[j].Args.Length == 0 )
							return i + 1;

						if ( args.Length == entries[j].Args.Length )
						{
							bool match = true;

							for ( int k = 0; match && k < args.Length; ++k )
								match = ( args[k].Equals( entries[j].Args[k] ) );

							if ( match )
								return i + 1;
						}
					}
				}
			}

			// no entry?
			return 0;
		}

		public static void SetupRewardTables()
		{
			RewardCategory monsterStatues = new RewardCategory( 1049750 );
			RewardCategory cloaksAndRobes = new RewardCategory( 1049752 );
			RewardCategory etherealSteeds = new RewardCategory( 1049751 );
			RewardCategory specialDyeTubs = new RewardCategory( 1049753 );
			RewardCategory houseAddOns    = new RewardCategory( 1049754 );
			RewardCategory miscellaneous  = new RewardCategory( 1078596 );

			m_Categories = new RewardCategory[]
				{
					monsterStatues,
					//cloaksAndRobes,
					etherealSteeds,
					specialDyeTubs,
					houseAddOns,
					miscellaneous
				};

			int Bronze = CraftResources.GetHue( CraftResource.Bronze );
			int Copper = CraftResources.GetHue( CraftResource.Copper );
			int Golden = CraftResources.GetHue( CraftResource.Gold );
			int Agapite = CraftResources.GetHue( CraftResource.Agapite );
			int Verite = CraftResources.GetHue( CraftResource.Verite );
			int Valorite = CraftResources.GetHue( CraftResource.Valorite );
			int IceGreen = 0x47F;
			int IceBlue = 0x482;
			int DarkGray = 0x902;
			int Fire = 0x489;
			int IceWhite = 0x47E;
			int JetBlack = 0x001;
			int Pink = 0x490;
			int Crimson = 0x485;
			int ForestGreen = 0x483;
			int RoyalBlue = 0x484;

			m_LabelHues = new RewardLabelHue[]
				{
					new RewardLabelHue( 1041286, 1041287, 1080366, Bronze ),
					new RewardLabelHue( 1041288, 1041289, 1080367, Copper ),
					new RewardLabelHue( 1041292, 1041293, 1080368, Golden ),
					new RewardLabelHue( 1041290, 1041291, 1080369, Agapite ),
					new RewardLabelHue( 1041294, 1041295, 1080370, Verite ),
					new RewardLabelHue( 1041296, 1041297, 1080371, Valorite ),
					new RewardLabelHue( 1049759, 1049758, 1080372, IceGreen ),
					new RewardLabelHue( 1049761, 1049760, 1080373, IceBlue ),
					new RewardLabelHue( 1049757, 1049756, 1080374, DarkGray ),
					new RewardLabelHue( 1049767, 1049766, 1080375, Fire ),
					new RewardLabelHue( 1049765, 1049764, 1080376, IceWhite ),
					new RewardLabelHue( 1049763, 1049762, 1080377, JetBlack ),
					new RewardLabelHue( 1080382, 1080380, 1080378, Pink ),
					new RewardLabelHue( 1080383, 1080381, 1080379, Crimson ),
					new RewardLabelHue( 1113902, 1113903, 1113904, ForestGreen ),
					new RewardLabelHue( 1113910, 1113911, 1113912, RoyalBlue )
				};

			m_Lists = new RewardList[]
				{
					new RewardList( 1, new RewardEntry[]
					{
						//new RewardEntry( specialDyeTubs, 1006008, typeof( RewardBlackDyeTub ) ),
						new RewardEntry( specialDyeTubs, 1006013, typeof( FurnitureDyeTub ) ),
						//new RewardEntry( specialDyeTubs, 1006047, typeof( SpecialDyeTub ) ),
						//new RewardEntry(    houseAddOns, 1006049, typeof( StoneFaceAddonDeed ) ),
						//new RewardEntry(    houseAddOns, "Lilac Bush Deed", typeof( LilacBushAddonDeed ) ),
						//new RewardEntry(    houseAddOns, 1062913, typeof( RoseOfTrinsic ) ),
						//new RewardEntry( cloaksAndRobes, 1006009, typeof( RewardCloak ), Bronze, 1041286 ),
						//new RewardEntry( cloaksAndRobes, 1006010, typeof( RewardRobe ), Bronze, 1041287 ),
						//new RewardEntry( cloaksAndRobes, 1080366, typeof( RewardDress ), /*Expansion.ML,*/ Bronze, 1080366 ),

						//new RewardEntry( cloaksAndRobes, 1006011, typeof( RewardCloak ), Copper, 1041288 ),
						//new RewardEntry( cloaksAndRobes, 1006012, typeof( RewardRobe ), Copper, 1041289 ),
						//new RewardEntry( cloaksAndRobes, 1080367, typeof( RewardDress ), /*Expansion.ML,*/ Copper, 1080367 ),

						new RewardEntry( monsterStatues, 1006024, typeof( MonsterStatuette ), MonsterStatuetteType.Crocodile ),
						new RewardEntry( monsterStatues, 1006025, typeof( MonsterStatuette ), MonsterStatuetteType.Daemon ),
						new RewardEntry( monsterStatues, 1006026, typeof( MonsterStatuette ), MonsterStatuetteType.Dragon ),
						new RewardEntry( monsterStatues, 1006027, typeof( MonsterStatuette ), MonsterStatuetteType.EarthElemental ),
						new RewardEntry( monsterStatues, 1006028, typeof( MonsterStatuette ), MonsterStatuetteType.Ettin ),
						new RewardEntry( monsterStatues, 1006029, typeof( MonsterStatuette ), MonsterStatuetteType.Gargoyle ),
						new RewardEntry( monsterStatues, 1006030, typeof( MonsterStatuette ), MonsterStatuetteType.Gorilla ),
						new RewardEntry( monsterStatues, 1006031, typeof( MonsterStatuette ), MonsterStatuetteType.Lich ),
						new RewardEntry( monsterStatues, 1006032, typeof( MonsterStatuette ), MonsterStatuetteType.Lizardman ),
						new RewardEntry( monsterStatues, 1006033, typeof( MonsterStatuette ), MonsterStatuetteType.Ogre ),
						new RewardEntry( monsterStatues, 1006034, typeof( MonsterStatuette ), MonsterStatuetteType.Orc ),
						new RewardEntry( monsterStatues, 1006035, typeof( MonsterStatuette ), MonsterStatuetteType.Ratman ),
						new RewardEntry( monsterStatues, 1006036, typeof( MonsterStatuette ), MonsterStatuetteType.Skeleton ),
						new RewardEntry( monsterStatues, 1006037, typeof( MonsterStatuette ), MonsterStatuetteType.Troll ),
						//new RewardEntry( houseAddOns,    1062692, typeof( ContestMiniHouseDeed ), /*Expansion.AOS,*/ MiniHouseType.MalasMountainPass ),
						//new RewardEntry( houseAddOns,    1072216, typeof( ContestMiniHouseDeed ), /*Expansion.SE,*/ MiniHouseType.ChurchAtNight ),
						//new RewardEntry( miscellaneous,  1076155, typeof( RedSoulstone )/*, Expansion.ML*/ )
					} ),
					new RewardList( 2, new RewardEntry[]
					{
						new RewardEntry( specialDyeTubs, 1049740, typeof( RunebookDyeTub ) ),
						new RewardEntry( specialDyeTubs, 1006052, typeof( LeatherDyeTub ) ),
						//new RewardEntry( cloaksAndRobes, 1006014, typeof( RewardCloak ), Agapite, 1041290 ),
						//new RewardEntry( cloaksAndRobes, 1006015, typeof( RewardRobe ), Agapite, 1041291 ),
						//new RewardEntry( cloaksAndRobes, 1080369, typeof( RewardDress ), /*Expansion.ML,*/ Agapite, 1080369 ),
						//new RewardEntry( cloaksAndRobes, 1006016, typeof( RewardCloak ), Golden, 1041292 ),
						//new RewardEntry( cloaksAndRobes, 1006017, typeof( RewardRobe ), Golden, 1041293 ),
						//new RewardEntry( cloaksAndRobes, 1080368, typeof( RewardDress ), /*Expansion.ML,*/ Golden, 1080368 ),
						//new RewardEntry( houseAddOns,    1006048, typeof( BannerDeed ) ),
						new RewardEntry( houseAddOns, 	 1006049, typeof( FlamingHeadDeed ) ),
//						new RewardEntry( houseAddOns, 	 1080409, typeof( MinotaurStatueDeed ), Expansion.ML ),

						//new RewardEntry(    houseAddOns, "Pillar of Fire Deed", typeof( FirePillarAddonDeed ) ),
						//new RewardEntry(    houseAddOns, "Shrine of Wisdom Deed", typeof( WisdomShrineAddonDeed ) ),
//						new RewardEntry(    houseAddOns, "Crystal Formation Deed", typeof( CrystalFormationAddonDeed ) ),
					} ),
					new RewardList( 3, new RewardEntry[]
					{
						//new RewardEntry( cloaksAndRobes, 1006020, typeof( RewardCloak ), Verite, 1041294 ),
						//new RewardEntry( cloaksAndRobes, 1006021, typeof( RewardRobe ), Verite, 1041295 ),
						//new RewardEntry( cloaksAndRobes, 1080370, typeof( RewardDress ), /*Expansion.ML,*/ Verite, 1080370 ),
						//new RewardEntry( cloaksAndRobes, 1006022, typeof( RewardCloak ), Valorite, 1041296 ),
						//new RewardEntry( cloaksAndRobes, 1006023, typeof( RewardRobe ), Valorite, 1041297 ),
						//new RewardEntry( cloaksAndRobes, 1080371, typeof( RewardDress ), /*Expansion.ML,*/ Valorite, 1080371 ),
						new RewardEntry( monsterStatues, 1006038, typeof( MonsterStatuette ), MonsterStatuetteType.Cow ),
						new RewardEntry( monsterStatues, 1006039, typeof( MonsterStatuette ), MonsterStatuetteType.Zombie ),
						new RewardEntry( monsterStatues, 1006040, typeof( MonsterStatuette ), MonsterStatuetteType.Llama ),
						new RewardEntry( etherealSteeds, 1006019, typeof( EtherealHorse ) ),
						//new RewardEntry( etherealSteeds, 1006050, typeof( EtherealOstard ) ),
						//new RewardEntry( etherealSteeds, 1049747, typeof( EtherealRidgeback ) ),
						//new RewardEntry( etherealSteeds, "Ethereal Frenzied Ostard Statuette", typeof( EtherealFrenziedOstard ) ),
						//new RewardEntry( etherealSteeds, "Ethereal Forest Ostard Statuette", typeof( EtherealForestOstard ) ),
						new RewardEntry( etherealSteeds, 1006051, typeof( EtherealLlama ) ),

						new RewardEntry( houseAddOns,	 1080407, typeof( PottedCactusDeed )/*, Expansion.ML*/ )

					} ),
					new RewardList( 4, new RewardEntry[]
					{
						//new RewardEntry( specialDyeTubs, 1049740, typeof( RunebookDyeTub ) ),
						//new RewardEntry( cloaksAndRobes, 1049725, typeof( RewardCloak ), DarkGray, 1049757 ),
						//new RewardEntry( cloaksAndRobes, 1049726, typeof( RewardRobe ), DarkGray, 1049756 ),
						//new RewardEntry( cloaksAndRobes, 1080374, typeof( RewardDress ), /*Expansion.ML,*/ DarkGray, 1080374 ),
						//new RewardEntry( cloaksAndRobes, 1049727, typeof( RewardCloak ), IceGreen, 1049759 ),
						//new RewardEntry( cloaksAndRobes, 1049728, typeof( RewardRobe ), IceGreen, 1049758 ),
						//new RewardEntry( cloaksAndRobes, 1080372, typeof( RewardDress ), /*Expansion.ML,*/ IceGreen, 1080372 ),
						//new RewardEntry( cloaksAndRobes, 1049729, typeof( RewardCloak ), IceBlue, 1049761 ),
						//new RewardEntry( cloaksAndRobes, 1049730, typeof( RewardRobe ), IceBlue, 1049760 ),
						//new RewardEntry( cloaksAndRobes, 1080373, typeof( RewardDress ), /*Expansion.ML,*/ IceBlue, 1080373 ),

						new RewardEntry( monsterStatues, 1049742, typeof( MonsterStatuette ), MonsterStatuetteType.OphidianWarrior_Old ),
						//new RewardEntry( monsterStatues, "Ophidian Mage", typeof( MonsterStatuette ), MonsterStatuetteType.OphidianMage_Old ),
						//new RewardEntry( monsterStatues, "Ophidian Warrior", typeof( MonsterStatuette ), MonsterStatuetteType.OphidianWarrior_Old ),
						new RewardEntry( monsterStatues, 1049743, typeof( MonsterStatuette ), MonsterStatuetteType.Reaper ),
						new RewardEntry( monsterStatues, 1049744, typeof( MonsterStatuette ), MonsterStatuetteType.Mongbat ),

						//new RewardEntry( etherealSteeds, 1049746, typeof( EtherealKirin ) ),
						//new RewardEntry( etherealSteeds, 1049745, typeof( EtherealUnicorn ) ),
						//new RewardEntry( etherealSteeds, 1049747, typeof( EtherealRidgeback ) ),

						new RewardEntry( houseAddOns,    1049737, typeof( DecorativeShieldDeed ) ),
						//new RewardEntry( houseAddOns, 	 1049738, typeof( HangingSkeletonDeed ) )
						new RewardEntry( miscellaneous,  1080523, typeof( CommodityDeedBox )/*, Expansion.ML*/ )
					} ),
					new RewardList( 5, new RewardEntry[]
					{
						new RewardEntry( specialDyeTubs, 1049741, typeof( StatuetteDyeTub ) ),
						//new RewardEntry( cloaksAndRobes, 1049731, typeof( RewardCloak ), JetBlack, 1049763 ),
						//new RewardEntry( cloaksAndRobes, 1049732, typeof( RewardRobe ), JetBlack, 1049762 ),
						//new RewardEntry( cloaksAndRobes, 1080377, typeof( RewardDress ), /*Expansion.ML,*/ JetBlack, 1080377 ),
						//new RewardEntry( cloaksAndRobes, 1049733, typeof( RewardCloak ), IceWhite, 1049765 ),
						//new RewardEntry( cloaksAndRobes, 1049734, typeof( RewardRobe ), IceWhite, 1049764 ),
						//new RewardEntry( cloaksAndRobes, 1080376, typeof( RewardDress ), /*Expansion.ML,*/ IceWhite, 1080376 ),
						//new RewardEntry( cloaksAndRobes, 1049735, typeof( RewardCloak ), Fire, 1049767 ),
						//new RewardEntry( cloaksAndRobes, 1049736, typeof( RewardRobe ), Fire, 1049766 ),
						//new RewardEntry( cloaksAndRobes, 1080375, typeof( RewardDress ), /*Expansion.ML,*/ Fire, 1080375 ),
						new RewardEntry( monsterStatues, 1049768, typeof( MonsterStatuette ), MonsterStatuetteType.Gazer ),
						new RewardEntry( monsterStatues, 1049769, typeof( MonsterStatuette ), MonsterStatuetteType.FireElemental ),
						new RewardEntry( monsterStatues, 1049770, typeof( MonsterStatuette ), MonsterStatuetteType.Wolf ),
						//new RewardEntry( etherealSteeds, 1049749, typeof( EtherealSwampDragon ) ),
						//new RewardEntry( etherealSteeds, 1049748, typeof( EtherealBeetle ) ),
						new RewardEntry( houseAddOns,    1049739, typeof( StoneAnkhDeed ) ),
						new RewardEntry( etherealSteeds, 1006050, typeof( EtherealOstard ) )
						//new RewardEntry( houseAddOns,    1080384, typeof( BloodyPentagramDeed ) )
					} ),
					new RewardList( 6, new RewardEntry[]
					{
						//new RewardEntry( houseAddOns,	1076188, typeof( CharacterStatueMaker ), /*Expansion.ML,*/ StatueType.Jade ),
						//new RewardEntry( houseAddOns,	1076189, typeof( CharacterStatueMaker ), /*Expansion.ML,*/ StatueType.Marble ),
						//new RewardEntry( houseAddOns,	1076190, typeof( CharacterStatueMaker ), /*Expansion.ML,*/ StatueType.Bronze ),
						new RewardEntry( houseAddOns,	1080527, typeof( RewardBrazierDeed )/*, Expansion.ML*/ ),
						new RewardEntry( houseAddOns,    1006048, typeof( BannerDeed ) ),
						new RewardEntry( houseAddOns, 	 1049738, typeof( HangingSkeletonDeed ) )
						
					} ),
					new RewardList( 7, new RewardEntry[]
					{
						new RewardEntry( houseAddOns,	1076157, typeof( CannonDeed )/*, Expansion.ML*/ ),
						new RewardEntry( houseAddOns,	1080550, typeof( TreeStumpDeed )/*, Expansion.ML*/ ),
						//new RewardEntry( miscellaneous,  1080523, typeof( CommodityDeedBox )/*, Expansion.ML*/ ),
						//new RewardEntry( houseAddOns, "Amethyst Tree Deed", typeof( AmethystTreeAddonDeed ) ),
						new RewardEntry( miscellaneous,  1080548, typeof( DecoOreMiningCartDeed )/*, Expansion.ML*/ )
					} ),
					new RewardList( 8, new RewardEntry[]
					{
						new RewardEntry( houseAddOns,		1080549, typeof( WallBannerDeed )/*, Expansion.ML*/ ),
						new RewardEntry( etherealSteeds, "Ethereal Frenzied Ostard Statuette", typeof( EtherealFrenziedOstard ) ),
						new RewardEntry( etherealSteeds, "Ethereal Forest Ostard Statuette", typeof( EtherealForestOstard ) )
						//new RewardEntry( miscellaneous,	1076158, typeof( WeaponEngravingTool )/*, Expansion.ML*/ )
					} ),
					new RewardList( 9, new RewardEntry[]
					{
						#region Veteran Rewards
						new RewardEntry( monsterStatues,	1080520, typeof( MonsterStatuette ), /*Expansion.ML,*/ MonsterStatuetteType.Harrower ),
						new RewardEntry( monsterStatues,	1080521, typeof( MonsterStatuette ), /*Expansion.ML,*/ MonsterStatuetteType.Efreet ),
						//new RewardEntry( etherealSteeds,	1076159, typeof( RidablePolarBear )/*, Expansion.ML*/ )
						//new RewardEntry( houseAddOns,		1080549, typeof( WallBannerDeed )/*, Expansion.ML*/ )
						#endregion
					} ),
					new RewardList( 10, new RewardEntry[]
					{
						//new RewardEntry( monsterStatues,	1080520, typeof( MonsterStatuette ), /*Expansion.ML,*/ MonsterStatuetteType.Harrower ),
						//new RewardEntry( monsterStatues,	1080521, typeof( MonsterStatuette ), /*Expansion.ML,*/ MonsterStatuetteType.Efreet ),

						//new RewardEntry( cloaksAndRobes,	1080382, typeof( RewardCloak ), /*Expansion.ML,*/ Pink, 1080382 ),
						//new RewardEntry( cloaksAndRobes,	1080380, typeof( RewardRobe ), /*Expansion.ML,*/ Pink, 1080380 ),
						//new RewardEntry( cloaksAndRobes,	1080378, typeof( RewardDress ), /*Expansion.ML,*/ Pink, 1080378 ),
						//new RewardEntry( cloaksAndRobes,	1080383, typeof( RewardCloak ), /*Expansion.ML,*/ Crimson, 1080383 ),
						//new RewardEntry( cloaksAndRobes,	1080381, typeof( RewardRobe ), /*Expansion.ML,*/ Crimson, 1080381 ),
						//new RewardEntry( cloaksAndRobes,	1080379, typeof( RewardDress ), /*Expansion.ML,*/ Crimson, 1080379 ),

						//new RewardEntry( etherealSteeds,	1080386, typeof( EtherealCuSidhe )/*, Expansion.ML*/ ),

						//new RewardEntry( houseAddOns,		1080548, typeof( MiningCartDeed )/*, Expansion.ML*/ ),
						//new RewardEntry( houseAddOns,		1080397, typeof( AnkhOfSacrificeDeed )/*, Expansion.ML*/ )
					} )
				};
		}

		public static void Initialize()
		{
			CommandSystem.Register("WipeRewards", AccessLevel.Owner, new CommandEventHandler(WipeRewards_OnCommand));
			if ( Enabled )
				EventSink.Login += new LoginEventHandler( EventSink_Login );
		}

		[Usage("WipeRewards")]
		[Description("Wipe all veteran rewards and reset accounts.")]
		private static void WipeRewards_OnCommand(CommandEventArgs e)
		{
			int itemsRemoved = 0;
			List<Item> deleteList = new List<Item>();

			//Wipe all reward items
			e.Mobile.SendMessage("Wiping rewards...");
			foreach ( Item item in World.Items.Values )
			{
				IRewardItem reward = item as IRewardItem;
				if ( reward != null && reward.IsRewardItem )
					deleteList.Add( item );
			}

			itemsRemoved = deleteList.Count;

			foreach ( Item item in deleteList )
				item.Delete();

			//Reset accounts
			e.Mobile.SendMessage("Resetting accounts...");
			foreach ( Account a in Accounts.GetAccounts() )
				a.RemoveTag("numRewardsChosen");

			e.Mobile.SendMessage("Reward item wipe done. {0} reward items wiped.", itemsRemoved);
		}

		private static void EventSink_Login( LoginEventArgs e )
		{
			Mobile m = e.Mobile;

			if ( m.Alive && m.AccessLevel == AccessLevel.Player && m.Region.IsPartOf( typeof( Server.Regions.Jail ) ) )
			{
				//if (!e.Mobile.Alive || e.Mobile.Region is GameRegion )
				//	return;

				int cur, max, level;

				ComputeRewardInfo( m, out cur, out max, out level );
		/*
				if ( e.Mobile.SkillsCap == 7000 || e.Mobile.SkillsCap == 7050 || e.Mobile.SkillsCap == 7100 || e.Mobile.SkillsCap == 7150 || e.Mobile.SkillsCap == 7200 )
				{
					if ( level > 4 )
						level = 4;
					else if ( level < 0 )
						level = 0;

					if ( SkillCapRewards )
						e.Mobile.SkillsCap = 7000 + (level * 50);
					else
						e.Mobile.SkillsCap = 7000;
				}

				if ( Core.ML && e.Mobile is PlayerMobile && !((PlayerMobile)e.Mobile).HasStatReward && HasHalfLevel( e.Mobile ) )
				{
					((PlayerMobile)e.Mobile).HasStatReward = true;
					e.Mobile.StatCap += 5;
				}
		*/
				if ( level > 4 )
					level = 4;
				else if ( level < 0 )
					level = 0;

				if ( cur < max )
					m.SendGump( new RewardNoticeGump( m ) );
			}
		}
	}

	public class RewardLabelHue
	{
		private int m_CloakLabel;
		private int m_RobeLabel;
		private int m_DressLabel;
		private int m_Hue;
		
		public int CloakLabel{ get{ return m_CloakLabel; } }
		public int RobeLabel{ get{ return m_RobeLabel; } }
		public int DressLabel{ get{ return m_DressLabel; } }
		public int Hue{ get{ return m_Hue; } }

		public RewardLabelHue( int cloak, int robe, int dress, int hue )
		{
			m_CloakLabel = cloak;
			m_RobeLabel = robe;
			m_DressLabel = dress;
			m_Hue = hue;
		}
	}

	public interface IRewardItem
	{
		bool IsRewardItem{ get; set; }
	}

	public interface IVeteranClothing
	{
	}
}