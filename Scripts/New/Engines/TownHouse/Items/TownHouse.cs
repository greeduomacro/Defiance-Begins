using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Multis;
using Server.Targeting;

namespace Server.Multis
{
	public class TownHouse : VersionHouse
	{
		private static ArrayList s_TownHouses = new ArrayList();
		public static ArrayList AllTownHouses{ get{ return s_TownHouses; } }

		private TownHouseSign m_Sign;
		private Item m_Hanger;
        private ArrayList m_Sectors = new ArrayList();

        public TownHouseSign ForSaleSign { get { return m_Sign; } }

		public Item Hanger
		{
			get
			{
				if ( m_Hanger == null )
				{
					m_Hanger = new Static( 0xB98 );
					m_Hanger.MoveToWorld( Sign.Location, Sign.Map );
				}

				return m_Hanger;
			}
			set{ m_Hanger = value; }
		}

		public TownHouse( Mobile m, TownHouseSign sign, int locks, int secures ) : base( 0x1DD6 | 0x4000, m, locks, secures )
		{
			m_Sign = sign;

            SetSign( 0, 0, 0 );

			s_TownHouses.Add( this );
		}

		public void InitSectorDefinition()
		{
            if (m_Sign == null || m_Sign.Blocks.Count == 0)
                return;

			int minX = m_Sign.Blocks[0].Start.X;
			int minY = m_Sign.Blocks[0].Start.Y;
			int maxX = m_Sign.Blocks[0].End.X;
			int maxY = m_Sign.Blocks[0].End.Y;

			foreach( Rectangle2D rect in m_Sign.Blocks )
			{
				if ( rect.Start.X < minX )
					minX = rect.Start.X;
				if ( rect.Start.Y < minY )
					minY = rect.Start.Y;
				if ( rect.End.X > maxX )
					maxX = rect.End.X;
				if ( rect.End.Y > maxY )
					maxY = rect.End.Y;
			}

            foreach (Sector sector in m_Sectors)
                sector.OnMultiLeave(this);

            m_Sectors.Clear();
            for (int x = minX; x < maxX; ++x)
                for (int y = minY; y < maxY; ++y)
                    if(!m_Sectors.Contains(Map.GetSector(new Point2D(x, y))))
                        m_Sectors.Add(Map.GetSector(new Point2D(x, y)));

            foreach (Sector sector in m_Sectors)
                sector.OnMultiEnter(this);

            Components.Resize(maxX - minX, maxY - minY);
            Components.Add(0x520, Components.Width - 1, Components.Height - 1, -5);
        }

        public override Rectangle2D[] Area
        {
            get
            {
                if (m_Sign == null)
                    return new Rectangle2D[100];

                return m_Sign.Blocks.ToArray();
            }
        }

		public override bool IsInside( Point3D p, int height )
		{
            if (m_Sign == null)
                return false;

			if ( Map == null || Region == null )
			{
				Delete();
				return false;
			}

			Sector sector = null;

            try
            {
                if (m_Sign is RentalContract && Region.Contains(p))
                    return true;

                sector = Map.GetSector(p);

                foreach (BaseMulti m in sector.Multis)
                {
                    if (m != this
                    && m is TownHouse
                    && ((TownHouse)m).ForSaleSign is RentalContract
                    && ((TownHouse)m).IsInside(p, height))
                        return false;
                }

                return Region.Contains(p);
            }
            catch(Exception e)
            {
                Errors.Report("Error occured in IsInside().  More information on the console.");
                Console.WriteLine("Info:{0}, {1}, {2}", Map, sector, Region, sector != null ? "" + sector.Multis : "**");
                Console.WriteLine(e.Source);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }

		public override int GetNewVendorSystemMaxVendors()
		{
			return 50;
		}

		public override int GetAosMaxSecures()
		{
			return MaxSecures;
		}

		public override int GetAosMaxLockdowns()
		{
			return MaxLockDowns;
		}

		public override void OnMapChange()
		{
			base.OnMapChange();

			if ( m_Hanger != null )
				m_Hanger.Map = Map;
		}

		public override void OnLocationChange( Point3D oldLocation )
		{
			base.OnLocationChange( oldLocation );

			if ( m_Hanger != null )
				m_Hanger.Location = Sign.Location;
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			if ( e.Mobile != Owner || !IsInside( e.Mobile ) )
				return;

            if (e.Speech.ToLower() == "check house rent")
                m_Sign.CheckRentTimer();

            Timer.DelayCall<Mobile>(TimeSpan.Zero, new TimerStateCallback<Mobile>(AfterSpeech), e.Mobile);
        }

        private void AfterSpeech(Mobile m)
        {
            if (m.Target is HouseBanTarget && ForSaleSign != null && ForSaleSign.NoBanning)
            {
                m.Target.Cancel(m, TargetCancelType.Canceled);
                m.SendMessage(0x161, "You cannot ban people from this house.");
            }
        }

		public override void OnDelete()
		{
            if (m_Hanger != null)
                m_Hanger.Delete();

            foreach (Item item in Sign.GetItemsInRange(0))
                if (item != Sign)
                    item.Visible = true;

            m_Sign.ClearHouse();
            Doors.Clear();

            s_TownHouses.Remove(this);

            base.OnDelete();
		}

        public TownHouse(Serial serial)
            : base(serial)
		{
            s_TownHouses.Add(this);
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 3 );

			// Version 2

			writer.Write( m_Hanger );

			// Version 1

			writer.Write( m_Sign );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version >= 2 )
				m_Hanger = reader.ReadItem();

			m_Sign = (TownHouseSign)reader.ReadItem();

            if (version <= 2)
            {
                int count = reader.ReadInt();
                for (int i = 0; i < count; ++i)
                    reader.ReadRect2D();
            }

			if( Price == 0 )
				Price = 1;

            ItemID = 0x1DD6 | 0x4000;
        }
	}
}