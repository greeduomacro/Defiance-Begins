using System;
using System.Collections;
using Server;
using Server.Targeting;

namespace Server.Multis
{
	public class TownHouseSetupGump : GumpPlusLight
	{
		public static Rectangle2D FixRect( Rectangle2D rect )
		{
			Point3D pointOne = Point3D.Zero;
			Point3D pointTwo = Point3D.Zero;

			if ( rect.Start.X < rect.End.X )
			{
				pointOne.X = rect.Start.X;
				pointTwo.X = rect.End.X;
			}
			else
			{
				pointOne.X = rect.End.X;
				pointTwo.X = rect.Start.X;
			}

			if ( rect.Start.Y < rect.End.Y )
			{
				pointOne.Y = rect.Start.Y;
				pointTwo.Y = rect.End.Y;
			}
			else
			{
				pointOne.Y = rect.End.Y;
				pointTwo.Y = rect.Start.Y;
			}

			return new Rectangle2D( pointOne, pointTwo );
		}

		public enum Page { Welcome, Blocks, Floors, Sign, Ban, LocSec, Items, Length, Price, Skills, Other, Other2 }
		public enum TargetType { BanLoc, SignLoc, MinZ, MaxZ, BlockOne, BlockTwo }

		private TownHouseSign m_Sign;
		private Page m_Page;
        private bool m_Quick;

		public TownHouseSetupGump( Mobile m, TownHouseSign sign ) : base( m, 50, 50 )
		{
			m.CloseGump( typeof( TownHouseSetupGump ) );

			m_Sign = sign;
		}

		protected override void BuildGump()
		{
			if ( m_Sign == null )
				return;

            int width = 300;
            int y = 0;

            if (m_Quick)
            {
                QuickPage(width, ref y);
            }
            else
            {
                switch (m_Page)
                {
                    case Page.Welcome: WelcomePage(width, ref y); break;
                    case Page.Blocks: BlocksPage(width, ref y); break;
                    case Page.Floors: FloorsPage(width, ref y); break;
                    case Page.Sign: SignPage(width, ref y); break;
                    case Page.Ban: BanPage(width, ref y); break;
                    case Page.LocSec: LocSecPage(width, ref y); break;
                    case Page.Items: ItemsPage(width, ref y); break;
                    case Page.Length: LengthPage(width, ref y); break;
                    case Page.Price: PricePage(width, ref y); break;
                    case Page.Skills: SkillsPage(width, ref y); break;
                    case Page.Other: OtherPage(width, ref y); break;
                    case Page.Other2: OtherPage2(width, ref y); break;
                }

                BuildTabs(width, ref y);
            }

            AddBackgroundZero(0, 0, width, y+=30, 0x13BE);

            if (m_Sign.PriceReady && !m_Sign.Owned)
            {
                AddBackground(width / 2 - 50, y, 100, 30, 0x13BE);
                AddHtml(width / 2 - 50 + 25, y + 5, 100, "Claim Home");
                AddButton(width / 2 - 50 + 5, y + 10, 0x837, 0x838, "Claim", new GumpCallback(Claim));
            }
        }

		private void BuildTabs(int width, ref int y)
		{
			int x = 20;

            y += 30;

            AddButton(x-5, y - 3, 0x768, "Quick", new GumpCallback(Quick));
            AddLabel(x, y - 3, m_Quick ? 0x34 : 0x47E, "Q");

            AddButton(x+=20, y, m_Page == Page.Welcome ? 0x939 : 0x93A, "Welcome Page", new GumpStateCallback(ChangePage), 0);
            AddButton(x+=20, y, m_Page == Page.Blocks ? 0x939 : 0x93A, "Blocks Page", new GumpStateCallback(ChangePage), 1);

			if ( m_Sign.BlocksReady )
				AddButton( x+=20, y, m_Page == Page.Floors ? 0x939 : 0x93A, "Floors Page", new GumpStateCallback( ChangePage ), 2 );

			if ( m_Sign.FloorsReady )
				AddButton( x+=20, y, m_Page == Page.Sign ? 0x939 : 0x93A, "Sign Page", new GumpStateCallback( ChangePage ), 3 );

			if ( m_Sign.SignReady )
				AddButton( x+=20, y, m_Page == Page.Ban ? 0x939 : 0x93A, "Ban Page", new GumpStateCallback( ChangePage ), 4 );

			if ( m_Sign.BanReady )
				AddButton( x+=20, y, m_Page == Page.LocSec ? 0x939 : 0x93A, "LocSec Page", new GumpStateCallback( ChangePage ), 5 );

			if ( m_Sign.LocSecReady )
			{
				AddButton( x+=20, y, m_Page == Page.Items ? 0x939 : 0x93A, "Items Page", new GumpStateCallback( ChangePage ), 6 );

				if ( !m_Sign.Owned )
					AddButton( x+=20, y, m_Page == Page.Length ? 0x939 : 0x93A, "Length Page", new GumpStateCallback( ChangePage ), 7 );
				else
					x+=20;

				AddButton( x+=20, y, m_Page == Page.Price ? 0x939 : 0x93A, "Price Page", new GumpStateCallback( ChangePage ), 8 );
			}

			if ( m_Sign.PriceReady )
			{
				AddButton( x+=20, y, m_Page == Page.Skills ? 0x939 : 0x93A, "Skills Page", new GumpStateCallback( ChangePage ), 9 );
                AddButton(x += 20, y, m_Page == Page.Other ? 0x939 : 0x93A, "Other Page", new GumpStateCallback(ChangePage), 10);
                AddButton(x += 20, y, m_Page == Page.Other2 ? 0x939 : 0x93A, "Other Page 2", new GumpStateCallback(ChangePage), 11);
            }
		}

        private void QuickPage(int width, ref int y)
        {
            m_Sign.ClearPreview();

            AddHtml(0, y += 10, width, "<CENTER>Quick Setup");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

            AddButton(5, 5, 0x768, "Quick", new GumpCallback(Quick));
            AddLabel(10, 5, m_Quick ? 0x34 : 0x47E, "Q");

            AddHtml(0, y += 25, width / 2 - 55, "<DIV ALIGN=RIGHT>Name");
            AddTextField(width / 2 - 15, y, 100, 20, 0x480, 0xBBC, "Name", m_Sign.Name);
            AddButton(width / 2 - 40, y + 3, 0x2716, "Name", new GumpCallback(Name));

            AddHtml(0, y += 25, width/2, "<CENTER>Add Area");
            AddButton(width / 4 - 50, y + 3, 0x2716, "Add Area", new GumpCallback(AddBlock));
            AddButton(width / 4 + 40, y + 3, 0x2716, "Add Area", new GumpCallback(AddBlock));

            AddHtml(width/2, y, width/2, "<CENTER>Clear All");
            AddButton(width / 4 * 3 - 50, y + 3, 0x2716, "ClearAll", new GumpCallback(ClearAll));
            AddButton(width / 4 * 3 + 40, y + 3, 0x2716, "ClearAll", new GumpCallback(ClearAll));

            AddHtml(0, y += 25, width, "<CENTER>Base Floor: " + m_Sign.MinZ.ToString());
            AddButton(width / 2 - 80, y + 3, 0x2716, "Base Floor", new GumpCallback(MinZSelect));
            AddButton(width / 2 + 70, y + 3, 0x2716, "Base Floor", new GumpCallback(MinZSelect));

            AddHtml(0, y += 17, width, "<CENTER>Top Floor: " + m_Sign.MaxZ.ToString());
            AddButton(width / 2 - 80, y + 3, 0x2716, "Top Floor", new GumpCallback(MaxZSelect));
            AddButton(width / 2 + 70, y + 3, 0x2716, "Top Floor", new GumpCallback(MaxZSelect));

            AddHtml(0, y += 25, width / 2, "<CENTER>Sign Loc");
            AddButton(width / 4 - 50, y + 3, 0x2716, "Sign Loc", new GumpCallback(SignLocSelect));
            AddButton(width / 4 + 40, y + 3, 0x2716, "Sign Loc", new GumpCallback(SignLocSelect));

            AddHtml(width/2, y, width/2, "<CENTER>Ban Loc");
            AddButton(width / 4 * 3 - 50, y + 3, 0x2716, "Ban Loc", new GumpCallback(BanLocSelect));
            AddButton(width / 4 * 3 + 40, y + 3, 0x2716, "Ban Loc", new GumpCallback(BanLocSelect));

            AddHtml(0, y += 25, width, "<CENTER>Suggest Secures");
            AddButton(width / 2 - 70, y + 3, 0x2716, "Suggest LocSec", new GumpCallback(SuggestLocSec));
            AddButton(width / 2 + 60, y + 3, 0x2716, "Suggest LocSec", new GumpCallback(SuggestLocSec));

            AddHtml(0, y += 20, width / 2 - 20, "<DIV ALIGN=RIGHT>Secures");
            AddTextField(width / 2 + 20, y, 50, 20, 0x480, 0xBBC, "Secures", m_Sign.Secures.ToString());
            AddButton(width / 2 - 5, y + 3, 0x2716, "Secures", new GumpCallback(Secures));

            AddHtml(0, y += 22, width / 2 - 20, "<DIV ALIGN=RIGHT>Lockdowns");
            AddTextField(width / 2 + 20, y, 50, 20, 0x480, 0xBBC, "Lockdowns", m_Sign.Locks.ToString());
            AddButton(width / 2 - 5, y + 3, 0x2716, "Lockdowns", new GumpCallback(Lockdowns));

            AddHtml(0, y += 25, width, "<CENTER>Give buyer items in home");
            AddButton(width / 2 - 110, y, m_Sign.KeepItems ? 0xD3 : 0xD2, "Keep Items", new GumpCallback(KeepItems));
            AddButton(width / 2 + 90, y, m_Sign.KeepItems ? 0xD3 : 0xD2, "Keep Items", new GumpCallback(KeepItems));

            if (m_Sign.KeepItems)
            {
                AddHtml(0, y += 25, width / 2 - 25, "<DIV ALIGN=RIGHT>At cost");
                AddTextField(width / 2 + 15, y, 70, 20, 0x480, 0xBBC, "ItemsPrice", m_Sign.ItemsPrice.ToString());
                AddButton(width / 2 - 10, y + 5, 0x2716, "ItemsPrice", new GumpCallback(ItemsPrice));
            }
            else
            {
                AddHtml(0, y += 25, width, "<CENTER>Don't delete items");
                AddButton(width / 2 - 110, y, m_Sign.LeaveItems ? 0xD3 : 0xD2, "LeaveItems", new GumpCallback(LeaveItems));
                AddButton(width / 2 + 90, y, m_Sign.LeaveItems ? 0xD3 : 0xD2, "LeaveItems", new GumpCallback(LeaveItems));
            }

            if (!m_Sign.Owned)
            {
                AddHtml(120, y += 25, 50, m_Sign.PriceType);
                AddButton(170, y + 8, 0x985, 0x985, "LengthUp", new GumpCallback(PriceUp));
                AddButton(170, y - 2, 0x983, 0x983, "LengthDown", new GumpCallback(PriceDown));
            }

            if (m_Sign.RentByTime != TimeSpan.Zero)
            {
                AddHtml(0, y += 25, width, "<CENTER>Recurring Rent");
                AddButton(width / 2 - 80, y, m_Sign.RecurRent ? 0xD3 : 0xD2, "RecurRent", new GumpCallback(RecurRent));
                AddButton(width / 2 + 60, y, m_Sign.RecurRent ? 0xD3 : 0xD2, "RecurRent", new GumpCallback(RecurRent));

                if (m_Sign.RecurRent)
                {
                    AddHtml(0, y += 20, width, "<CENTER>Rent To Own");
                    AddButton(width / 2 - 80, y, m_Sign.RentToOwn ? 0xD3 : 0xD2, "RentToOwn", new GumpCallback(RentToOwn));
                    AddButton(width / 2 + 60, y, m_Sign.RentToOwn ? 0xD3 : 0xD2, "RentToOwn", new GumpCallback(RentToOwn));
                }
            }

            AddHtml(0, y += 25, width, "<CENTER>Free");
            AddButton(width / 2 - 80, y, m_Sign.Free ? 0xD3 : 0xD2, "Free", new GumpCallback(Free));
            AddButton(width / 2 + 60, y, m_Sign.Free ? 0xD3 : 0xD2, "Free", new GumpCallback(Free));

            if (!m_Sign.Free)
            {
                AddHtml(0, y += 25, width / 2 - 20, "<DIV ALIGN=RIGHT>" + m_Sign.PriceType + " Price");
                AddTextField(width / 2 + 20, y, 70, 20, 0x480, 0xBBC, "Price", m_Sign.Price.ToString());
                AddButton(width / 2 - 5, y + 5, 0x2716, "Price", new GumpCallback(Price));

                AddHtml(0, y += 25, width, "<CENTER>Suggest Price");
                AddButton(width / 2 - 70, y + 3, 0x2716, "Suggest", new GumpCallback(SuggestPrice));
                AddButton(width / 2 + 50, y + 3, 0x2716, "Suggest", new GumpCallback(SuggestPrice));
            }
        }

		private void WelcomePage(int width, ref int y)
		{
            AddHtml(0, y += 10, width, "<CENTER>Welcome!");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

            string helptext = "";

            AddHtml(0, y += 25, width / 2 - 55, "<DIV ALIGN=RIGHT>Name");
            AddTextField(width / 2 - 15, y, 100, 20, 0x480, 0xBBC, "Name", m_Sign.Name);
            AddButton(width / 2 - 40, y + 3, 0x2716, "Name", new GumpCallback(Name));

            if (m_Sign != null && m_Sign.Map != Map.Internal && m_Sign.RootParent == null)
            {
                AddHtml(0, y += 25, width, "<CENTER>Goto");
                AddButton(width / 2 - 50, y + 3, 0x2716, "Goto", new GumpCallback(Goto));
                AddButton(width / 2 + 40, y + 3, 0x2716, "Goto", new GumpCallback(Goto));
            }

            if (m_Sign.Owned)
            {
                helptext = String.Format("  This home is owned by {0}, so be aware that changing anything " +
                "through this menu will change the home itself!  You can add more area, change the ownership " +
                "rules, almost anything!  You cannot, however, change the rental status of the home, way too many " +
                "ways for things to go ill.  If you change the restrictions and the home owner no longer meets them, " +
                "they will receive the normal 24 hour demolish warning.", m_Sign.House.Owner.Name);

                AddHtml(10, y += 25, width - 20, 180, helptext, false, false);

                y += 180;
            }
            else
            {
                helptext = String.Format("  Welcome to the TownHouse setup menu!  This menu will guide you through " +
                "each step in the setup process.  You can set up any area to be a home, and then detail everything from " +
                "lockdowns and price to whether you want to sell or rent the house.  Let's begin here with the name of " +
                "this new Town House!");

                AddHtml(10, y += 25, width - 20, 130, helptext, false, false);

                y += 130;
            }

            AddHtml(width - 60, y+=15, 60, "Next");
            AddButton(width - 30, y, 0x15E1, 0x15E5, "Next", new GumpStateCallback(ChangePage), (int)m_Page + 1);
        }

		private void BlocksPage(int width, ref int y)
		{
			if ( m_Sign == null )
				return;

			m_Sign.ShowAreaPreview(Owner);

			AddHtml( 0, y+=10, width, "<CENTER>Create the Area");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

			AddHtml( 0, y+=25, width, "<CENTER>Add Area");
            AddButton(width / 2 - 50, y + 3, 0x2716, "Add Area", new GumpCallback(AddBlock));
            AddButton(width / 2 + 40, y + 3, 0x2716, "Add Area", new GumpCallback(AddBlock));

			AddHtml( 0, y+=20, width, "<CENTER>Clear All");
            AddButton(width / 2 - 50, y + 3, 0x2716, "ClearAll", new GumpCallback(ClearAll));
            AddButton(width / 2 + 40, y + 3, 0x2716, "ClearAll", new GumpCallback(ClearAll));

			string helptext = String.Format( "   Setup begins with defining the area you wish to sell or rent.  " +
				"You can add as many boxes as you wish, and each time the preview will extend to show what " +
				"you've selected so far.  If you feel like starting over, just clear them away!  You must have " +
				"at least one block defined before continuing to the next step." );

			AddHtml( 10, y+=35, width-20, 140, helptext, false, false );

            y += 140;

            AddHtml(30, y += 15, 80, "Previous");
            AddButton(10, y, 0x15E3, 0x15E7, "Previous", new GumpStateCallback(ChangePage), (int)m_Page - 1);

            if (m_Sign.BlocksReady)
			{
				AddHtml( width-60, y, 60, "Next");
				AddButton( width-30, y, 0x15E1, 0x15E5, "Next", new GumpStateCallback( ChangePage ), (int)m_Page+1 );
			}
		}

		private void FloorsPage(int width, ref int y)
		{
            m_Sign.ShowFloorsPreview(Owner);

            AddHtml(0, y += 10, width, "<CENTER>Floors");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

            AddHtml(0, y += 25, width, "<CENTER>Base Floor: " + m_Sign.MinZ.ToString());
            AddButton(width / 2 - 80, y + 3, 0x2716, "Base Floor", new GumpCallback(MinZSelect));
            AddButton(width / 2 + 70, y + 3, 0x2716, "Base Floor", new GumpCallback(MinZSelect));

            AddHtml(0, y += 20, width, "<CENTER>Top Floor: " + m_Sign.MaxZ.ToString());
            AddButton(width / 2 - 80, y + 3, 0x2716, "Top Floor", new GumpCallback(MaxZSelect));
            AddButton(width / 2 + 70, y + 3, 0x2716, "Top Floor", new GumpCallback(MaxZSelect));

			string helptext = String.Format( "   Now you will need to target the floors you wish to sell.  " +
				"If you only want one floor, you can skip targeting the top floor.  Everything within the base " +
				"and highest floor will come with the home, and the more floors, the higher the cost later on.");

			AddHtml( 10, y+=35, width-20, 110, helptext, false, false);

            y += 110;

			AddHtml( 30, y+=15, 80, "Previous");
			AddButton( 10, y, 0x15E3, 0x15E7, "Previous", new GumpStateCallback( ChangePage ), (int)m_Page-1 );

			if ( m_Sign.FloorsReady )
			{
				AddHtml( width-60, y, 60, "Next");
				AddButton( width-30, y, 0x15E1, 0x15E5, "Next", new GumpStateCallback( ChangePage ), (int)m_Page+1 );
			}
		}

		private void SignPage(int width, ref int y)
		{
			if ( m_Sign == null )
				return;

			m_Sign.ShowSignPreview();

			AddHtml( 0, y+=10, width, "<CENTER>Sign Location");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

			AddHtml( 0, y+=25, width, "<CENTER>Set Location");
            AddButton(width / 2 - 60, y + 3, 0x2716, "Sign Loc", new GumpCallback(SignLocSelect));
            AddButton(width / 2 + 50, y + 3, 0x2716, "Sign Loc", new GumpCallback(SignLocSelect));

			string helptext = String.Format( "   With this sign, the owner will have the same home owning rights " +
				"as custom or classic homes.  If they use the sign to demolish the home, it will automatically " +
				"return to sale or rent.  The sign players will use to purchase the home will appear in the same " +
				"spot, slightly below the normal house sign." );

			AddHtml( 10, y+=35, width-20, 130, helptext, false, false);

            y += 130;

			AddHtml( 30, y+=15, 80, "Previous");
			AddButton( 10, y, 0x15E3, 0x15E7, "Previous", new GumpStateCallback( ChangePage ), (int)m_Page-1 );

			if ( m_Sign.SignReady )
			{
				AddHtml( width-60, y, 60, "Next");
				AddButton( width-30, y, 0x15E1, 0x15E5, "Next", new GumpStateCallback( ChangePage ), (int)m_Page+1 );
			}
		}

		private void BanPage(int width, ref int y)
		{
			if ( m_Sign == null )
				return;

			m_Sign.ShowBanPreview();

			AddHtml( 0, y+=10, width, "<CENTER>Ban Location");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

			AddHtml( 0, y+=25, width, "<CENTER>Set Location");
            AddButton(width / 2 - 60, y + 3, 0x2716, "Ban Loc", new GumpCallback(BanLocSelect));
            AddButton(width / 2 + 50, y + 3, 0x2716, "Ban Loc", new GumpCallback(BanLocSelect));

			string helptext = String.Format( "   The ban location determines where players are sent when ejected or " +
				"banned from a home.  If you never set this, they would appear at the south west corner of the outside " +
                "of the home.");

			AddHtml( 10, y+=35, width-20, 100, helptext, false, false );

            y += 100;

			AddHtml( 30, y+=15, 80, "Previous");
			AddButton( 10, y, 0x15E3, 0x15E7, "Previous", new GumpStateCallback( ChangePage ), (int)m_Page-1 );

			if ( m_Sign.BanReady )
			{
				AddHtml( width-60, y, 60, "Next");
				AddButton( width-30, y, 0x15E1, 0x15E5, "Next", new GumpStateCallback( ChangePage ), (int)m_Page+1 );
			}
		}

		private void LocSecPage(int width, ref int y)
		{
			AddHtml( 0, y+=10, width, "<CENTER>Lockdowns and Secures");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

            AddHtml(0, y+=25, width, "<CENTER>Suggest");
            AddButton(width / 2 - 50, y + 3, 0x2716, "Suggest LocSec", new GumpCallback(SuggestLocSec));
            AddButton(width / 2 + 40, y + 3, 0x2716, "Suggest LocSec", new GumpCallback(SuggestLocSec));

            AddHtml(0, y += 25, width / 2 - 20, "<DIV ALIGN=RIGHT>Secures");
			AddTextField( width/2+20, y, 50, 20, 0x480, 0xBBC, "Secures", m_Sign.Secures.ToString() );
            AddButton(width / 2 - 5, y + 3, 0x2716, "Secures", new GumpCallback(Secures));

			AddHtml( 0, y+=25, width/2-20, "<DIV ALIGN=RIGHT>Lockdowns");
			AddTextField( width/2+20, y, 50, 20, 0x480, 0xBBC, "Lockdowns", m_Sign.Locks.ToString() );
            AddButton(width / 2 - 5, y + 3, 0x2716, "Lockdowns", new GumpCallback(Lockdowns));

			string helptext = String.Format( "   With this step you'll set the amount of storage for the home, or let " +
				"the system do so for you using the Suggest button.  In general, players get half the number of lockdowns " +
				"as secure storage." );

			AddHtml( 10, y+=35, width-20, 90, helptext, false, false );

            y += 90;

			AddHtml( 30, y+=15, 80, "Previous");
			AddButton( 10, y, 0x15E3, 0x15E7, "Previous", new GumpStateCallback( ChangePage ), (int)m_Page-1 );

			if ( m_Sign.LocSecReady )
			{
				AddHtml( width-60, y, 60, "Next");
				AddButton( width-30, y, 0x15E1, 0x15E5, "Next", new GumpStateCallback( ChangePage ), (int)m_Page+1 );
			}
		}

		private void ItemsPage(int width, ref int y)
		{
			AddHtml( 0, y+=10, width, "<CENTER>Decoration Items");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

			AddHtml( 0, y+=25, width, "<CENTER>Give buyer items in home");
            AddButton(width / 2 - 110, y, m_Sign.KeepItems ? 0xD3 : 0xD2, "Keep Items", new GumpCallback(KeepItems));
            AddButton(width / 2 + 90, y, m_Sign.KeepItems ? 0xD3 : 0xD2, "Keep Items", new GumpCallback(KeepItems));

			if ( m_Sign.KeepItems )
			{
				AddHtml( 0, y+=25, width/2-25, "<DIV ALIGN=RIGHT>At cost");
				AddTextField( width/2+15, y, 70, 20, 0x480, 0xBBC, "ItemsPrice", m_Sign.ItemsPrice.ToString());
                AddButton(width/2-10, y + 5, 0x2716, "ItemsPrice", new GumpCallback(ItemsPrice));
			}
			else
			{
				AddHtml( 0, y+=25, width, "<CENTER>Don't delete items");
                AddButton(width / 2 - 110, y, m_Sign.LeaveItems ? 0xD3 : 0xD2, "LeaveItems", new GumpCallback(LeaveItems));
                AddButton(width / 2 + 90, y, m_Sign.LeaveItems ? 0xD3 : 0xD2, "LeaveItems", new GumpCallback(LeaveItems));
            }

			string helptext = String.Format( "   By default, the system will delete all items non-static items already " +
				"in the home at the time of purchase.  These items are commonly referred to as Decoration Items. " +
				"They do not include home addons, like forges and the like.  They do include containers.  You can " +
				"allow players to keep these items by saying so here, and you may also charge them to do so!");

			AddHtml( 10, y+=35, width-20, 160, helptext, false, false );

            y+=160;

			AddHtml( 30, y+=15, 80, "Previous");
			AddButton( 10, y, 0x15E3, 0x15E7, "Previous", new GumpStateCallback( ChangePage ), (int)m_Page-1 );

			if ( m_Sign.ItemsReady )
			{
				AddHtml( width-60, y, 60, "Next");
				AddButton( width-30, y, 0x15E1, 0x15E5, "Next", new GumpStateCallback( ChangePage ), (int)m_Page + ( m_Sign.Owned ? 2: 1 ) );
			}
		}

		private void LengthPage(int width, ref int y)
		{
			AddHtml( 0, y+=10, width, "<CENTER>Buy or Rent" );
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

			AddHtml( 120, y+=25, 50, m_Sign.PriceType);
			AddButton( 170, y+8, 0x985, 0x985, "LengthUp", new GumpCallback( PriceUp ) );
			AddButton( 170, y-2, 0x983, 0x983, "LengthDown", new GumpCallback( PriceDown ) );

			if ( m_Sign.RentByTime != TimeSpan.Zero )
			{
				AddHtml( 0, y+=25, width, "<CENTER>Recurring Rent");
                AddButton(width / 2 - 80, y, m_Sign.RecurRent ? 0xD3 : 0xD2, "RecurRent", new GumpCallback(RecurRent));
                AddButton(width / 2 + 60, y, m_Sign.RecurRent ? 0xD3 : 0xD2, "RecurRent", new GumpCallback(RecurRent));

				if ( m_Sign.RecurRent )
				{
					AddHtml( 0, y+=20, width, "<CENTER>Rent To Own");
                    AddButton(width / 2 - 80, y, m_Sign.RentToOwn ? 0xD3 : 0xD2, "RentToOwn", new GumpCallback(RentToOwn));
                    AddButton(width / 2 + 60, y, m_Sign.RentToOwn ? 0xD3 : 0xD2, "RentToOwn", new GumpCallback(RentToOwn));
                }
			}

			string helptext = String.Format( "   Getting closer to completing the setup!  Now you get to specify whether " +
				"this is a purchase or rental property.  Simply use the arrows until you have the setting you desire.  For " +
				"rental property, you can also make the purchase non-recuring, meaning after the time is up the player " +
				"gets the boot!  With recurring, if they have the money available they can continue to rent.  You can " +
				"also enable Rent To Own, allowing players to own the property after making two months worth of payments." );

			AddHtml( 10, y+=35, width-20, 160, helptext, false, true );

            y += 160;

			AddHtml( 30, y+=15, 80, "Previous");
			AddButton( 10, y, 0x15E3, 0x15E7, "Previous", new GumpStateCallback( ChangePage ), (int)m_Page-1 );

			if ( m_Sign.LengthReady )
			{
				AddHtml( width-60, y, 60, "Next");
				AddButton( width-30, y, 0x15E1, 0x15E5, "Next", new GumpStateCallback( ChangePage ), (int)m_Page+1 );
			}
		}

		private void PricePage(int width, ref int y)
		{
			AddHtml( 0, y+=10, width, "<CENTER>Price");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

			AddHtml( 0, y+=25, width, "<CENTER>Free");
            AddButton(width / 2 - 80, y, m_Sign.Free ? 0xD3 : 0xD2, "Free", new GumpCallback(Free));
            AddButton(width / 2 + 60, y, m_Sign.Free ? 0xD3 : 0xD2, "Free", new GumpCallback(Free));

			if ( !m_Sign.Free )
			{
				AddHtml( 0, y+=25, width/2-20, "<DIV ALIGN=RIGHT>" + m_Sign.PriceType + " Price");
				AddTextField( width/2+20, y, 70, 20, 0x480, 0xBBC, "Price", m_Sign.Price.ToString());
                AddButton(width / 2 - 5, y + 5, 0x2716, "Price", new GumpCallback(Price));

				AddHtml( 0, y+=20, width, "<CENTER>Suggest");
                AddButton(width / 2 - 50, y + 3, 0x2716, "Suggest", new GumpCallback(SuggestPrice));
                AddButton(width / 2 + 40, y + 3, 0x2716, "Suggest", new GumpCallback(SuggestPrice));
			}

			string helptext = String.Format( "   Now you get to set the price for the home.  Remember, if this is a " +
				"rental home, the system will charge them this amount for every period!  Luckily the Suggestion " +
				"takes this into account.  If you don't feel like guessing, let the system suggest a price for you.  " +
				"You can also give the home away with the Free option." );

			AddHtml( 10, y+=35, width-20, 130, helptext, false, false );

            y += 130;

			AddHtml( 30, y+=15, 80, "Previous");
			AddButton( 10, y, 0x15E3, 0x15E7, "Previous", new GumpStateCallback( ChangePage ), (int)m_Page - ( m_Sign.Owned ? 2 : 1 ) );

			if ( m_Sign.PriceReady )
			{
				AddHtml( width-60, y, 60, "Next");
				AddButton( width-30, y, 0x15E1, 0x15E5, "Next", new GumpStateCallback( ChangePage ), (int)m_Page+1 );
			}
		}

		private void SkillsPage(int width, ref int y)
		{
			AddHtml( 0, y+=10, width, "<CENTER>Skill Restictions");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

			AddHtml( 0, y+=25, width/2-20, "<DIV ALIGN=RIGHT>Skill");
			AddTextField( width/2+20, y, 100, 20, 0x480, 0xBBC, "Skill", m_Sign.Skill.ToString());
            AddButton(width / 2 - 5, y + 5, 0x2716, "Skill", new GumpCallback(Skill));

            AddHtml(0, y+=25, width / 2 - 20, "<DIV ALIGN=RIGHT>Amount");
            AddTextField(width / 2 + 20, y, 50, 20, 0x480, 0xBBC, "SkillReq", m_Sign.SkillReq.ToString());
            AddButton(width / 2 - 5, y + 5, 0x2716, "Skill", new GumpCallback(Skill));

            AddHtml(0, y += 25, width/2-20, "<DIV ALIGN=RIGHT>Min Total");
            AddTextField(width / 2 + 20, y, 60, 20, 0x480, 0xBBC, "MinTotalSkill", m_Sign.MinTotalSkill.ToString());
            AddButton(width / 2 - 5, y + 5, 0x2716, "Skill", new GumpCallback(Skill));

			AddHtml( 0, y+=25, width/2-20, "<DIV ALIGN=RIGHT>Max Total");
            AddTextField(width / 2 + 20, y, 60, 20, 0x480, 0xBBC, "MaxTotalSkill", m_Sign.MaxTotalSkill.ToString());
            AddButton(width / 2 - 5, y + 5, 0x2716, "Skill", new GumpCallback(Skill));

			string helptext = String.Format( "   These settings are all optional.  If you want to restrict who can own " +
				"this home by their skills, here's the place.  You can specify by the skill name and value, or by " +
				"player's total skills." );

			AddHtml( 10, y+=35, width-20, 90, helptext, false, false );

            y += 90;

			AddHtml( 30, y+=15, 80, "Previous");
			AddButton( 10, y, 0x15E3, 0x15E7, "Previous", new GumpStateCallback( ChangePage ), (int)m_Page-1 );

			if ( m_Sign.PriceReady )
			{
				AddHtml( width-60, y, 60, "Next");
				AddButton( width-30, y, 0x15E1, 0x15E5, "Next", new GumpStateCallback( ChangePage ), (int)m_Page+1 );
			}
		}

        private void OtherPage(int width, ref int y)
        {
            AddHtml(0, y += 10, width, "<CENTER>Other Options");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

            AddHtml(0, y += 25, width, "<CENTER>Young");
            AddButton(width / 2 - 80, y, m_Sign.YoungOnly ? 0xD3 : 0xD2, "Young Only", new GumpCallback(Young));
            AddButton(width / 2 + 60, y, m_Sign.YoungOnly ? 0xD3 : 0xD2, "Young Only", new GumpCallback(Young));

            if (!m_Sign.YoungOnly)
            {
                AddHtml(0, y += 25, width, "<CENTER>Innocents");
                AddButton(width / 2 - 80, y, m_Sign.Murderers == Intu.No ? 0xD3 : 0xD2, "No Murderers", new GumpStateCallback(Murderers), Intu.No);
                AddButton(width / 2 + 60, y, m_Sign.Murderers == Intu.No ? 0xD3 : 0xD2, "No Murderers", new GumpStateCallback(Murderers), Intu.No);
                AddHtml(0, y += 20, width, "<CENTER>Murderers");
                AddButton(width / 2 - 80, y, m_Sign.Murderers == Intu.Yes ? 0xD3 : 0xD2, "Yes Murderers", new GumpStateCallback(Murderers), Intu.Yes);
                AddButton(width / 2 + 60, y, m_Sign.Murderers == Intu.Yes ? 0xD3 : 0xD2, "Yes Murderers", new GumpStateCallback(Murderers), Intu.Yes);
                AddHtml(0, y += 20, width, "<CENTER>All");
                AddButton(width / 2 - 80, y, m_Sign.Murderers == Intu.Neither ? 0xD3 : 0xD2, "Neither Murderers", new GumpStateCallback(Murderers), Intu.Neither);
                AddButton(width / 2 + 60, y, m_Sign.Murderers == Intu.Neither ? 0xD3 : 0xD2, "Neither Murderers", new GumpStateCallback(Murderers), Intu.Neither);
            }

            AddHtml(0, y += 25, width, "<CENTER>Relock doors on demolish");
            AddButton(width / 2 - 110, y, m_Sign.Relock ? 0xD3 : 0xD2, "Relock", new GumpCallback(Relock));
            AddButton(width / 2 + 90, y, m_Sign.Relock ? 0xD3 : 0xD2, "Relock", new GumpCallback(Relock));

            string helptext = String.Format("   These options are also optional.  With the young setting, you can restrict " +
                "who can buy the home to young players only.  Similarly, you can specify whether murderers or innocents are " +
                " allowed to own the home.  You can also specify whether the doors within the " +
                "home are locked when the owner demolishes their property.");

            AddHtml(10, y += 35, width - 20, 180, helptext, false, false);

            y += 180;

            AddHtml(30, y += 15, 80, "Previous");
            AddButton(10, y, 0x15E3, 0x15E7, "Previous", new GumpStateCallback(ChangePage), (int)m_Page - 1);

            AddHtml(width - 60, y, 60, "Next");
            AddButton(width - 30, y, 0x15E1, 0x15E5, "Next", new GumpStateCallback(ChangePage), (int)m_Page + 1);
        }

        private void OtherPage2(int width, ref int y)
        {
            AddHtml(0, y += 10, width, "<CENTER>Other Options 2");
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

            AddHtml(0, y += 25, width, "<CENTER>Force Public");
            AddButton(width / 2 - 110, y, m_Sign.ForcePublic ? 0xD3 : 0xD2, "Public", new GumpCallback(ForcePublic));
            AddButton(width / 2 + 90, y, m_Sign.ForcePublic ? 0xD3 : 0xD2, "Public", new GumpCallback(ForcePublic));

            AddHtml(0, y += 25, width, "<CENTER>Force Private");
            AddButton(width / 2 - 110, y, m_Sign.ForcePrivate ? 0xD3 : 0xD2, "Private", new GumpCallback(ForcePrivate));
            AddButton(width / 2 + 90, y, m_Sign.ForcePrivate ? 0xD3 : 0xD2, "Private", new GumpCallback(ForcePrivate));

            AddHtml(0, y += 25, width, "<CENTER>No Trading");
            AddButton(width / 2 - 110, y, m_Sign.NoTrade ? 0xD3 : 0xD2, "NoTrade", new GumpCallback(NoTrade));
            AddButton(width / 2 + 90, y, m_Sign.NoTrade ? 0xD3 : 0xD2, "NoTrade", new GumpCallback(NoTrade));

            AddHtml(0, y += 25, width, "<CENTER>No Banning");
            AddButton(width / 2 - 110, y, m_Sign.NoBanning ? 0xD3 : 0xD2, "NoBan", new GumpCallback(NoBan));
            AddButton(width / 2 + 90, y, m_Sign.NoBanning ? 0xD3 : 0xD2, "NoBan", new GumpCallback(NoBan));

            string helptext = String.Format("   Another page of optional options!  Sometimes houses have features you don't want players using.  " +
                "So here you can force homes to be private or public.  You can also prevent trading of the home.  Lastly, you can remove their ability to ban players.");

            AddHtml(10, y += 35, width - 20, 180, helptext, false, false);

            y += 180;

            AddHtml(30, y += 15, 80, "Previous");
            AddButton(10, y, 0x15E3, 0x15E7, "Previous", new GumpStateCallback(ChangePage), (int)m_Page - 1);
        }

        private bool SkillNameExists(string text)
		{
			try
			{
				SkillName index = (SkillName)Enum.Parse( typeof( SkillName ), text, true );
				return true;
			}
			catch
			{
				Owner.SendMessage( "You provided an invalid skill name." );
				return false;
			}
		}

		private void ChangePage( object obj )
		{
			if ( m_Sign == null )
				return;

			if ( !(obj is int) )
				return;

			m_Page = (Page)(int)obj;

			m_Sign.ClearPreview();

			NewGump();
		}

        private void Name()
        {
            m_Sign.Name = GetTextField("Name");
            Owner.SendMessage("Name set!");
            NewGump();
        }

        private void Goto()
        {
            Owner.Location = m_Sign.Location;
            Owner.Z += 5;
            Owner.Map = m_Sign.Map;

            NewGump();
        }

        private void Quick()
        {
            m_Quick = !m_Quick;
            NewGump();
        }

		private void BanLocSelect()
		{
			Owner.SendMessage( "Target the ban location." );
			Owner.Target = new InternalTarget( this, m_Sign, TargetType.BanLoc );
		}

		private void SignLocSelect()
		{
			Owner.SendMessage( "Target the location for the home sign." );
			Owner.Target = new InternalTarget( this, m_Sign, TargetType.SignLoc );
		}

		private void MinZSelect()
		{
			Owner.SendMessage( "Target the base floor." );
			Owner.Target = new InternalTarget( this, m_Sign, TargetType.MinZ );
		}

		private void MaxZSelect()
		{
			Owner.SendMessage( "Target the highest floor." );
			Owner.Target = new InternalTarget( this, m_Sign, TargetType.MaxZ );
		}

		private void Young()
		{
			m_Sign.YoungOnly = !m_Sign.YoungOnly;
			NewGump();
		}

		private void Murderers( object obj )
		{
			if ( !(obj is Intu) )
				return;

			m_Sign.Murderers = (Intu)obj;

			NewGump();
		}

        private void Relock()
        {
            m_Sign.Relock = !m_Sign.Relock;
            NewGump();
        }

        private void ForcePrivate()
        {
            m_Sign.ForcePrivate = !m_Sign.ForcePrivate;
            NewGump();
        }

        private void ForcePublic()
        {
            m_Sign.ForcePublic = !m_Sign.ForcePublic;
            NewGump();
        }

        private void NoTrade()
        {
            m_Sign.NoTrade = !m_Sign.NoTrade;
            NewGump();
        }

        private void NoBan()
        {
            m_Sign.NoBanning = !m_Sign.NoBanning;
            NewGump();
        }

        private void KeepItems()
		{
			m_Sign.KeepItems = !m_Sign.KeepItems;
			NewGump();
		}

		private void LeaveItems()
		{
			m_Sign.LeaveItems = !m_Sign.LeaveItems;
			NewGump();
		}

        private void ItemsPrice()
        {
            m_Sign.ItemsPrice = GetTextFieldInt("ItemsPrice");
            Owner.SendMessage("Item Price set!");
            NewGump();
        }

		private void RecurRent()
		{
			m_Sign.RecurRent = !m_Sign.RecurRent;
			NewGump();
		}

		private void RentToOwn()
		{
			m_Sign.RentToOwn = !m_Sign.RentToOwn;
			NewGump();
		}

        private void Skill()
        {
            if (GetTextField("Skill") != "" && SkillNameExists(GetTextField("Skill")))
                m_Sign.Skill = GetTextField("Skill");
            else
                m_Sign.Skill = "";

            m_Sign.SkillReq = GetTextFieldInt("SkillReq");
            m_Sign.MinTotalSkill = GetTextFieldInt("MinTotalSkill");
            m_Sign.MaxTotalSkill = GetTextFieldInt("MaxTotalSkill");

            Owner.SendMessage("Skill info set!");

            NewGump();
        }

		private void Claim()
		{
			new TownHouseConfirmGump( Owner, m_Sign );
            OnClose();
		}

		private void SuggestLocSec()
		{
			int price = m_Sign.CalcVolume()*General.SuggestionFactor;
			m_Sign.Secures = price/75;
			m_Sign.Locks = m_Sign.Secures/2;

			NewGump();
		}

        private void Secures()
        {
            m_Sign.Secures = GetTextFieldInt("Secures");
            Owner.SendMessage("Secures set!");
            NewGump();
        }

        private void Lockdowns()
        {
            m_Sign.Locks = GetTextFieldInt("Lockdowns");
            Owner.SendMessage("Lockdowns set!");
            NewGump();
        }

        private void SuggestPrice()
		{
			m_Sign.Price = m_Sign.CalcVolume()*General.SuggestionFactor;

			if ( m_Sign.RentByTime == TimeSpan.FromDays( 1 ) )
				m_Sign.Price /= 60;
			if ( m_Sign.RentByTime == TimeSpan.FromDays( 7 ) )
				m_Sign.Price = (int)((double)m_Sign.Price/8.57);
			if ( m_Sign.RentByTime == TimeSpan.FromDays( 30 ) )
				m_Sign.Price /= 2;

			NewGump();
		}

        private void Price()
        {
            m_Sign.Price = GetTextFieldInt("Price");
            Owner.SendMessage("Price set!");
            NewGump();
        }

		private void Free()
		{
			m_Sign.Free = !m_Sign.Free;
			NewGump();
		}

		private void AddBlock()
		{
			if ( m_Sign == null )
				return;

			Owner.SendMessage( "Target the north western corner." );
			Owner.Target = new InternalTarget( this, m_Sign, TargetType.BlockOne );
		}

		private void ClearAll()
		{
			if ( m_Sign == null )
				return;

			m_Sign.Blocks.Clear();
			m_Sign.ClearPreview();
			m_Sign.UpdateBlocks();

			NewGump();
		}

		private void PriceUp()
		{
			m_Sign.NextPriceType();
			NewGump();
		}

		private void PriceDown()
		{
			m_Sign.PrevPriceType();
			NewGump();
		}

        protected override void OnClose()
        {
            m_Sign.ClearPreview();
        }


		private class InternalTarget : Target
		{
			private TownHouseSetupGump m_Gump;
			private TownHouseSign m_Sign;
			private TargetType  m_Type;
			private Point3D m_BoundOne;

			public InternalTarget( TownHouseSetupGump gump, TownHouseSign sign, TargetType type ) : this( gump, sign, type, Point3D.Zero ){}

			public InternalTarget( TownHouseSetupGump gump, TownHouseSign sign, TargetType type, Point3D point ) : base( 20, true, TargetFlags.None )
			{
				m_Gump = gump;
				m_Sign = sign;
				m_Type = type;
				m_BoundOne = point;
			}

			protected override void OnTarget( Mobile m, object o )
			{
				IPoint3D point = (IPoint3D)o;

				switch( m_Type )
				{
					case TargetType.BanLoc:
						m_Sign.BanLoc = new Point3D( point.X, point.Y, point.Z );
						m_Gump.NewGump();
						break;

					case TargetType.SignLoc:
						m_Sign.SignLoc = new Point3D( point.X, point.Y, point.Z );
                        m_Sign.MoveToWorld(m_Sign.SignLoc, m_Sign.Map);
                        m_Sign.Z -= 5;
						m_Sign.ShowSignPreview();
						m_Gump.NewGump();
						break;

					case TargetType.MinZ:
						m_Sign.MinZ = point.Z;

						if ( m_Sign.MaxZ < m_Sign.MinZ+19 )
							m_Sign.MaxZ = point.Z+19;

						if ( m_Sign.MaxZ == short.MaxValue )
							m_Sign.MaxZ = point.Z+19;

						m_Gump.NewGump();
						break;

					case TargetType.MaxZ:
						m_Sign.MaxZ = point.Z+19;

						if ( m_Sign.MinZ > m_Sign.MaxZ )
							m_Sign.MinZ = point.Z;

						m_Gump.NewGump();
						break;

					case TargetType.BlockOne:
						m.SendMessage( "Now target the south eastern corner." );
						m.Target = new InternalTarget( m_Gump, m_Sign, TargetType.BlockTwo, new Point3D( point.X, point.Y, point.Z ) );
						break;

					case TargetType.BlockTwo:
						m_Sign.Blocks.Add( FixRect( new Rectangle2D( m_BoundOne, new Point3D( point.X+1, point.Y+1, point.Z ) ) ) );
                        m_Sign.UpdateBlocks();
                        m_Sign.ShowAreaPreview(m);
						m_Gump.NewGump();
						break;
				}
			}

			protected override void OnTargetCancel( Mobile m, TargetCancelType cancelType )
			{
				m_Gump.NewGump();
			}
		}
	}
}