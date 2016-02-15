using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Multis
{
	public enum Intu{ Neither, No, Yes }

	[Flipable( 0xC0B, 0xC0C )]
	public class TownHouseSign : Item
	{
		private static List<TownHouseSign> s_TownHouseSigns = new List<TownHouseSign>();
		public static List<TownHouseSign> AllSigns{ get{ return s_TownHouseSigns; } }

		private Point3D m_BanLoc, m_SignLoc;
		private int m_Locks, m_Secures, m_Price, m_MinZ, m_MaxZ, m_MinTotalSkill, m_MaxTotalSkill, m_ItemsPrice, m_RTOPayments;
		private bool m_YoungOnly, m_RecurRent, m_Relock, m_KeepItems, m_LeaveItems, m_RentToOwn, m_Free, m_ForcePrivate, m_ForcePublic, m_NoTrade, m_NoBanning;
		private string m_Skill;
		private double m_SkillReq;
		private List<Rectangle2D> m_Blocks;
		private List<DecoreItemInfo> m_DecoreItemInfos;
		private ArrayList m_PreviewItems;
		private TownHouse m_House;
		private Timer m_DemolishTimer, m_RentTimer, m_PreviewTimer;
		private DateTime m_DemolishTime, m_RentTime;
		private TimeSpan m_RentByTime, m_OriginalRentTime;
		private Intu m_Murderers;

		public Point3D BanLoc
		{
			get{ return m_BanLoc; }
			set
			{
				m_BanLoc = value;
				InvalidateProperties();
				if ( Owned )
					m_House.Region.GoLocation = value;
			}
		}

		public Point3D SignLoc
		{
			get{ return m_SignLoc; }
			set
			{
				m_SignLoc = value;
				InvalidateProperties();

				if ( Owned )
				{
					m_House.Sign.Location = value;
					m_House.Hanger.Location = value;
				}
			}
		}

		public int Locks
		{
			get{ return m_Locks; }
			set
			{
				m_Locks = value;
				InvalidateProperties();
				if ( Owned )
					m_House.MaxLockDowns = value;
			}
		}

		public int Secures
		{
			get{ return m_Secures; }
			set
			{
				m_Secures = value;
				InvalidateProperties();
				if ( Owned )
					m_House.MaxSecures = value;
			}
		}

		public int Price
		{
			get{ return m_Price; }
			set
			{
				m_Price = value;
				InvalidateProperties();
			}
		}

		public int MinZ
		{
			get{ return m_MinZ; }
			set
			{
				if ( value > m_MaxZ )
					m_MaxZ = value+1;

				m_MinZ = value;
                if (Owned)
                    RUOVersion.UpdateRegion(this);
            }
		}

		public int MaxZ
		{
			get{ return m_MaxZ; }
			set
			{
				if ( value < m_MinZ )
					value = m_MinZ;

				m_MaxZ = value;
                if (Owned)
                    RUOVersion.UpdateRegion(this);
            }
		}

		public int MinTotalSkill
		{
			get{ return m_MinTotalSkill; }
			set
			{
				if ( value > m_MaxTotalSkill )
					value = m_MaxTotalSkill;

				m_MinTotalSkill = value;
				ValidateOwnership();
				InvalidateProperties();
			}
		}

		public int MaxTotalSkill
		{
			get{ return m_MaxTotalSkill; }
			set
			{
				if ( value < m_MinTotalSkill )
					value = m_MinTotalSkill;

				m_MaxTotalSkill = value;
				ValidateOwnership();
				InvalidateProperties();
			}
		}

		public bool YoungOnly
		{
			get{ return m_YoungOnly; }
			set
			{
				m_YoungOnly = value;

				if ( m_YoungOnly )
					m_Murderers = Intu.Neither;

				ValidateOwnership();
				InvalidateProperties();
			}
		}

		public TimeSpan RentByTime
		{
			get{ return m_RentByTime; }
			set
			{
				m_RentByTime = value;
				m_OriginalRentTime = value;

				if ( value == TimeSpan.Zero )
                    ClearRentTimer();
				else
				{
					ClearRentTimer();
					BeginRentTimer( value );
				}

				InvalidateProperties();
			}
		}

		public bool RecurRent
		{
			get{ return m_RecurRent; }
			set
			{
				m_RecurRent = value;

				if ( !value )
					m_RentToOwn = value;

				InvalidateProperties();
			}
		}

		public bool KeepItems
		{
			get{ return m_KeepItems; }
			set
			{
				m_LeaveItems = false;
				m_KeepItems = value;
				InvalidateProperties();
			}
		}

		public bool Free
		{
			get{ return m_Free; }
			set
			{
				m_Free = value;
				m_Price = 1;
				InvalidateProperties();
			}
		}

		public Intu Murderers
		{
			get{ return m_Murderers; }
			set
			{
				m_Murderers = value;

				ValidateOwnership();
				InvalidateProperties();
			}
		}

        public bool ForcePrivate
        {
            get { return m_ForcePrivate; }
            set
            {
                m_ForcePrivate = value;

                if (value)
                {
                    m_ForcePublic = false;

                    if (m_House != null)
                        m_House.Public = false;
                }
            }
        }

        public bool ForcePublic
        {
            get { return m_ForcePublic; }
            set
            {
                m_ForcePublic = value;

                if (value)
                {
                    m_ForcePrivate = false;

                    if (m_House != null)
                        m_House.Public = true;
                }
            }
        }

        public bool NoBanning
        {
            get { return m_NoBanning; }
            set
            {
                m_NoBanning = value;

                if (value && m_House != null)
                    m_House.Bans.Clear();
            }
        }

        public List<Rectangle2D> Blocks { get { return m_Blocks; } set { m_Blocks = value; } }
        public string Skill { get { return m_Skill; } set { m_Skill = value; ValidateOwnership(); InvalidateProperties(); } }
        public double SkillReq { get { return m_SkillReq; } set { m_SkillReq = value; ValidateOwnership(); InvalidateProperties(); } }
		public bool LeaveItems{ get{ return m_LeaveItems; } set{ m_LeaveItems = value; InvalidateProperties(); } }
		public bool RentToOwn{ get{ return m_RentToOwn; } set{ m_RentToOwn = value; InvalidateProperties(); } }
        public bool Relock { get { return m_Relock; } set { m_Relock = value; } }
        public bool NoTrade { get { return m_NoTrade; } set { m_NoTrade = value; } }
        public int ItemsPrice { get { return m_ItemsPrice; } set { m_ItemsPrice = value; InvalidateProperties(); } }
		public TownHouse House{ get{ return m_House; } set{ m_House = value; } }
		public Timer DemolishTimer{ get{ return m_DemolishTimer; } }
		public DateTime DemolishTime{ get{ return m_DemolishTime; } }

		public bool Owned{ get{ return m_House != null && !m_House.Deleted; } }
		public int Floors{ get{ return (m_MaxZ-m_MinZ)/20+1; } }

		public bool BlocksReady{ get{ return Blocks.Count != 0; } }
		public bool FloorsReady{ get{ return ( BlocksReady && MinZ != short.MinValue ); } }
		public bool SignReady{ get{ return ( FloorsReady && SignLoc != Point3D.Zero ); } }
		public bool BanReady{ get{ return ( SignReady && BanLoc != Point3D.Zero ); } }
		public bool LocSecReady{ get{ return ( BanReady && Locks != 0 && Secures != 0 ); } }
		public bool ItemsReady{ get{ return LocSecReady; } }
		public bool LengthReady{ get{ return ItemsReady; } }
		public bool PriceReady{ get{ return ( LengthReady && Price != 0 ); } }

		public string PriceType
		{
			get
			{
				if ( m_RentByTime == TimeSpan.Zero )
					return "Sale";
				if ( m_RentByTime == TimeSpan.FromDays( 1 ) )
					return "Daily";
				if ( m_RentByTime == TimeSpan.FromDays( 7 ) )
					return "Weekly";
				if ( m_RentByTime == TimeSpan.FromDays( 30 ) )
					return "Monthly";

				return "Sale";
			}
		}

		public string PriceTypeShort
		{
			get
			{
				if ( m_RentByTime == TimeSpan.Zero )
					return "Sale";
				if ( m_RentByTime == TimeSpan.FromDays( 1 ) )
					return "Day";
				if ( m_RentByTime == TimeSpan.FromDays( 7 ) )
					return "Week";
				if ( m_RentByTime == TimeSpan.FromDays( 30 ) )
					return "Month";

				return "Sale";
			}
		}

		[Constructable]
		public TownHouseSign() : base( 0xC0B )
		{
			Name = "This building is for sale or rent!";
			Movable = false;

			m_BanLoc = Point3D.Zero;
			m_SignLoc = Point3D.Zero;
			m_Skill = "";
			m_Blocks = new List<Rectangle2D>();
			m_DecoreItemInfos = new List<DecoreItemInfo>();
			m_PreviewItems = new ArrayList();
			m_DemolishTime = DateTime.Now;
			m_RentTime = DateTime.Now;
			m_RentByTime = TimeSpan.Zero;
			m_RecurRent = true;

			m_MinZ = short.MinValue;
			m_MaxZ = short.MaxValue;

			s_TownHouseSigns.Add( this );
		}

		private void SearchForHouse()
		{
			foreach( TownHouse house in TownHouse.AllTownHouses )
				if (house.ForSaleSign == this )
					m_House = house;
		}

		public void UpdateBlocks()
		{
			if ( !Owned )
				return;

            if (m_Blocks.Count == 0)
				UnconvertDoors();

            RUOVersion.UpdateRegion(this);
            ConvertItems(false);
			m_House.InitSectorDefinition();
		}

		public void ShowAreaPreview( Mobile m )
		{
			ClearPreview();

			Point2D point = Point2D.Zero;
			List<Point2D> blocks = new List<Point2D>();

			foreach( Rectangle2D rect in m_Blocks )
				for( int x = rect.Start.X; x < rect.End.X; ++x )
					for( int y = rect.Start.Y; y < rect.End.Y; ++y )
					{
						point = new Point2D( x, y );
						if ( !blocks.Contains( point ) )
							blocks.Add( point );
					}

            if (blocks.Count > 500)
            {
                m.SendMessage("Due to size of the area, skipping the preview.");
                return;
            }

			Item item = null;
            int avgz = 0;
			foreach( Point2D p in blocks )
			{
                avgz = Map.GetAverageZ(p.X, p.Y);

				item = new Item( 0x1766 );
				item.Name = "Area Preview";
				item.Movable = false;
				item.Location = new Point3D( p.X, p.Y, (avgz <= m.Z ? m.Z+2 : avgz+2 ) );
				item.Map = Map;

				m_PreviewItems.Add( item );
			}

			m_PreviewTimer = Timer.DelayCall( TimeSpan.FromSeconds( 100 ), new TimerCallback( ClearPreview ) );
		}

		public void ShowSignPreview()
		{
			ClearPreview();

			Item sign = new Item( 0xBD2 );
			sign.Name = "Sign Preview";
			sign.Movable = false;
			sign.Location = SignLoc;
			sign.Map = Map;

			m_PreviewItems.Add( sign );

			sign = new Item( 0xB98 );
			sign.Name = "Sign Preview";
			sign.Movable = false;
			sign.Location = SignLoc;
			sign.Map = Map;

			m_PreviewItems.Add( sign );

			m_PreviewTimer = Timer.DelayCall( TimeSpan.FromSeconds( 100 ), new TimerCallback( ClearPreview ) );
		}

		public void ShowBanPreview()
		{
			ClearPreview();

			Item ban = new Item( 0x17EE );
			ban.Name = "Ban Loc Preview";
			ban.Movable = false;
			ban.Location = BanLoc;
			ban.Map = Map;

			m_PreviewItems.Add( ban );

			m_PreviewTimer = Timer.DelayCall( TimeSpan.FromSeconds( 100 ), new TimerCallback( ClearPreview ) );
		}

        public void ShowFloorsPreview(Mobile m)
        {
            ClearPreview();

            Item item = new Item(0x7BD);
            item.Name = "Bottom Floor Preview";
            item.Movable = false;
            item.Location = m.Location;
            item.Z = m_MinZ;
            item.Map = Map;

            m_PreviewItems.Add(item);

            item = new Item(0x7BD);
            item.Name = "Top Floor Preview";
            item.Movable = false;
            item.Location = m.Location;
            item.Z = m_MaxZ;
            item.Map = Map;

            m_PreviewItems.Add(item);

            m_PreviewTimer = Timer.DelayCall(TimeSpan.FromSeconds(100), new TimerCallback(ClearPreview));
        }

        public void ClearPreview()
		{
			foreach( Item item in new ArrayList( m_PreviewItems ) )
			{
				m_PreviewItems.Remove( item );
				item.Delete();
			}

			if ( m_PreviewTimer != null )
				m_PreviewTimer.Stop();

			m_PreviewTimer = null;
		}

		public void Purchase( Mobile m )
		{
			Purchase( m, false );
		}

		public void Purchase( Mobile m, bool sellitems )
		{
            try
            {
                if (Owned)
                {
                    m.SendMessage("Someone already owns this house!");
                    return;
                }

                if (!PriceReady)
                {
                    m.SendMessage("The setup for this house is not yet complete.");
                    return;
                }

                int price = m_Price + (sellitems ? m_ItemsPrice : 0);

                if (m_Free)
                    price = 0;

                if (m.AccessLevel == AccessLevel.Player && !Server.Mobiles.Banker.Withdraw(m, price))
                {
                    m.SendMessage("You cannot afford this house.");
                    return;
                }

                if (m.AccessLevel == AccessLevel.Player)
                    m.SendLocalizedMessage(1060398, price.ToString()); // ~1_AMOUNT~ gold has been withdrawn from your bank box.

                Visible = false;
				Rectangle2D startrec = m_Blocks[0];

                int minX = startrec.Start.X;
                int minY = startrec.Start.Y;
                int maxX = startrec.End.X;
                int maxY = startrec.End.Y;

                foreach (Rectangle2D rect in m_Blocks)
                {
                    if (rect.Start.X < minX)
                        minX = rect.Start.X;
                    if (rect.Start.Y < minY)
                        minY = rect.Start.Y;
                    if (rect.End.X > maxX)
                        maxX = rect.End.X;
                    if (rect.End.Y > maxY)
                        maxY = rect.End.Y;
                }

                m_House = new TownHouse(m, this, m_Locks, m_Secures);

                m_House.Components.Resize( maxX-minX, maxY-minY );
                m_House.Components.Add( 0x520, m_House.Components.Width-1, m_House.Components.Height-1, -5 );

				m_House.MoveToWorld( new Point3D(minX, minY, Map.GetAverageZ(minX, minY)), Map );

                m_House.Region.GoLocation = m_BanLoc;
                m_House.Sign.Location = m_SignLoc;
                m_House.Hanger = new Static(0xB98);
                m_House.Hanger.MoveToWorld( m_SignLoc, Map );

                if (m_ForcePublic)
                    m_House.Public = true;

                m_House.Price = (RentByTime == TimeSpan.FromDays(0) ? m_Price : 1);

                RUOVersion.UpdateRegion(this);

                if (m_House.Price == 0)
                    m_House.Price = 1;

                if (m_RentByTime != TimeSpan.Zero)
                    BeginRentTimer(m_RentByTime);

                m_RTOPayments = 1;

                HideOtherSigns();

                m_DecoreItemInfos = new List<DecoreItemInfo>();

                ConvertItems(sellitems);

				uint keyval = m_House.CreateKeys( m );
				foreach ( GenericHouseDoor door in m_House.Doors )
					door.KeyValue = keyval;
            }
            catch(Exception e)
            {
                Errors.Report(String.Format("An error occurred during home purchasing.  More information available on the console."));
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
        }

		private void HideOtherSigns()
		{
			foreach( Item item in m_House.Sign.GetItemsInRange( 0 ) )
				if ( !(item is HouseSign) )
					if ( item.ItemID == 0xB95
					|| item.ItemID == 0xB96
					|| item.ItemID == 0xC43
					|| item.ItemID == 0xC44
					|| ( item.ItemID > 0xBA3 && item.ItemID < 0xC0E ) )
						item.Visible = false;
		}

		public virtual void ConvertItems( bool keep )
		{
			if ( m_House == null )
				return;

            List<Item> items = new List<Item>();
            foreach(Rectangle2D rect in m_Blocks)
                foreach (Item item in Map.GetItemsInBounds(rect))
                    if (m_House.Region.Contains(item.Location) && item.RootParent == null && !items.Contains(item))
                        items.Add(item);

			Item[] itemar = items.ToArray();

            for ( int i = itemar.Length-1; i >= 0; i-- )
            {
				Item item = itemar[i];

                if (item is HouseSign
                || item is BaseMulti
                || item is BaseAddon
                || item is AddonComponent
                || item == m_House.Hanger
                || !item.Visible
                || item.IsLockedDown
                || item.IsSecure
                || item.Movable
                || m_PreviewItems.Contains(item))
                    continue;

                if (item is BaseDoor)
                    ConvertDoor((BaseDoor)item);
                else if (!m_LeaveItems)
                {
                    m_DecoreItemInfos.Add(new DecoreItemInfo(item.GetType().ToString(), item.Name, item.ItemID, item.Hue, item.Location, item.Map));

                    if (!m_KeepItems || !keep)
                        item.Delete();
                    else
                    {
                        item.Movable = true;
                        m_House.LockDown(m_House.Owner, item, false);
                    }
                }
            }
        }

		protected void ConvertDoor( BaseDoor door )
		{
			if ( !Owned )
				return;

			if ( door is ISecurable )
			{
				door.Locked = false;
				m_House.Doors.Add( door );
                return;
			}

			door.Open = false;

			GenericHouseDoor newdoor = new GenericHouseDoor( (DoorFacing)0, door.ClosedID, door.OpenedSound, door.ClosedSound );
			newdoor.Offset = door.Offset;
			newdoor.ClosedID = door.ClosedID;
			newdoor.OpenedID = door.OpenedID;
			newdoor.Location = door.Location;
			newdoor.Map = door.Map;

			door.Delete();

			foreach( Item inneritem in newdoor.GetItemsInRange( 1 ) )
				if ( inneritem is BaseDoor && inneritem != newdoor && inneritem.Z == newdoor.Z )
				{
					((BaseDoor)inneritem).Link = newdoor;
					newdoor.Link = (BaseDoor)inneritem;
				}

            m_House.Doors.Add(newdoor);
        }

		public virtual void UnconvertDoors()
		{
			if ( m_House == null )
				return;

			BaseDoor newdoor = null;

            foreach (BaseDoor door in new ArrayList(m_House.Doors))
			{
                door.Open = false;

				Type type = typeof( StrongWoodDoor );

				for ( int i = 0; i < AddDoorGump.m_Types.Length; i++ )
				{
					DoorInfo di = AddDoorGump.m_Types[i];
					if ( di.BaseID == door.ClosedID || di.BaseID + 2 == door.ClosedID || di.BaseID + 8 == door.ClosedID || di.BaseID + 10 == door.ClosedID )
						type = di.Type;
				}

				newdoor = Activator.CreateInstance( type, new object[]{ DoorFacing.WestCW } ) as BaseDoor;
				//newdoor = new StrongWoodDoor( (DoorFacing)0 );
				newdoor.ItemID = door.ItemID;
				newdoor.ClosedID = door.ClosedID;
				newdoor.OpenedID = door.OpenedID;
				newdoor.OpenedSound = door.OpenedSound;
				newdoor.ClosedSound = door.ClosedSound;
				newdoor.Offset = door.Offset;
				newdoor.Location = door.Location;
				newdoor.Map = door.Map;

				door.Delete();

				if ( m_Relock )
					newdoor.Locked = true;

				foreach( Item inneritem in newdoor.GetItemsInRange( 1 ) )
					if ( inneritem is BaseDoor && inneritem != newdoor && inneritem.Z == newdoor.Z )
					{
						( (BaseDoor)inneritem ).Link = newdoor;
						newdoor.Link = (BaseDoor)inneritem;
					}

				m_House.Doors.Remove( door );
			}
		}

		public void RecreateItems()
		{
			Item item = null;
			foreach( DecoreItemInfo info in m_DecoreItemInfos )
			{
				item = null;

				if ( info.TypeString.ToLower().IndexOf( "static" ) != -1 )
					item = new Static( info.ItemID );
				else
				{
					try{
					item = Activator.CreateInstance( ScriptCompiler.FindTypeByFullName( info.TypeString ) ) as Item;
					}catch{ continue; }
				}

				if ( item == null )
					continue;

				item.ItemID = info.ItemID;
				item.Name = info.Name;
				item.Hue = info.Hue;
				item.Location = info.Location;
				item.Map = info.Map;
				item.Movable = false;
			}
		}

		public virtual void ClearHouse()
		{
			UnconvertDoors();
			ClearDemolishTimer();
			ClearRentTimer();
			PackUpItems();
			RecreateItems();
			m_House = null;
			Visible = true;

			if ( m_RentToOwn )
				m_RentByTime = m_OriginalRentTime;
		}

		public virtual void ValidateOwnership()
		{
			if ( !Owned )
				return;

			if ( m_House.Owner == null )
			{
				m_House.Delete();
				return;
			}

			if ( m_House.Owner.AccessLevel != AccessLevel.Player )
				return;

			if ( !CanBuyHouse( m_House.Owner ) && m_DemolishTimer == null )
				BeginDemolishTimer();
			else
				ClearDemolishTimer();
		}

		public int CalcVolume()
		{
			int floors = 1;
			if ( m_MaxZ - m_MinZ < 100 )
				floors = 1 + Math.Abs( (m_MaxZ - m_MinZ)/20 );

			Point3D point = Point3D.Zero;
			List<Point3D> blocks = new List<Point3D>();

			foreach( Rectangle2D rect in m_Blocks )
				for( int x = rect.Start.X; x < rect.End.X; ++x )
					for( int y = rect.Start.Y; y < rect.End.Y; ++y )
						for( int z = 0; z < floors; z++ )
						{
							point = new Point3D( x, y, z );
							if ( !blocks.Contains( point ) )
								blocks.Add( point );
						}
			return blocks.Count;
		}

        private void StartTimers()
        {
            if (m_DemolishTime > DateTime.Now)
                BeginDemolishTimer(m_DemolishTime - DateTime.Now);
            else if (m_RentByTime != TimeSpan.Zero)
                BeginRentTimer(m_RentByTime);
        }

		#region Demolish

		public void ClearDemolishTimer()
		{
			if ( m_DemolishTimer == null )
				return;

			m_DemolishTimer.Stop();
			m_DemolishTimer = null;
			m_DemolishTime = DateTime.Now;

			if ( !m_House.Deleted && Owned )
				m_House.Owner.SendMessage( "Demolition canceled." );
		}

		public void CheckDemolishTimer()
		{
			if ( m_DemolishTimer == null || !Owned )
				return;

			DemolishAlert();
		}

		protected void BeginDemolishTimer()
		{
			BeginDemolishTimer( TimeSpan.FromHours( 24 ) );
		}

		protected void BeginDemolishTimer( TimeSpan time )
		{
			if ( !Owned )
				return;

			m_DemolishTime = DateTime.Now + time;
			m_DemolishTimer = Timer.DelayCall( time, new TimerCallback( PackUpHouse ) );

			DemolishAlert();
		}

		protected virtual void DemolishAlert()
		{
			m_House.Owner.SendMessage( "You no longer meet the requirements for your town house, which will be demolished automatically in {0}:{1}:{2}.", (m_DemolishTime-DateTime.Now).Hours, (m_DemolishTime-DateTime.Now).Minutes, (m_DemolishTime-DateTime.Now).Seconds );
		}

		protected void PackUpHouse()
		{
			if ( !Owned || m_House.Deleted )
				return;

			PackUpItems();

			m_House.Owner.BankBox.DropItem( new BankCheck( m_House.Price ) );

            try
            {
                m_House.Delete();
            }
            catch
            {
                Errors.Report("The infamous SVN bug has occured.");
            }

		}

		protected void PackUpItems()
		{
			if ( m_House == null )
				return;

			Container bag = new Bag();
			bag.Name = "Town House Belongings";

			foreach( Item item in new ArrayList( m_House.LockDowns ) )
			{
				item.IsLockedDown = false;
				item.Movable = true;
				m_House.LockDowns.Remove( item );
				bag.DropItem( item );
			}

			foreach( SecureInfo info in new ArrayList( m_House.Secures ) )
			{
				info.Item.IsLockedDown = false;
				info.Item.IsSecure = false;
				info.Item.Movable = true;
				info.Item.SetLastMoved();
				m_House.Secures.Remove( info );
				bag.DropItem( info.Item );
			}

            foreach (Rectangle2D rect in m_Blocks)
            {
                List<Item> l = new List<Item>();
                foreach (Item item in Map.GetItemsInBounds(rect))
                    l.Add(item);

                foreach (Item item in l)
                {
                    if (item is HouseSign
                    || item is BaseDoor
                    || item is BaseMulti
                    || item is BaseAddon
                    || item is AddonComponent
                    || !item.Visible
                    || item.IsLockedDown
                    || item.IsSecure
                    || !item.Movable
                    || item.Map != m_House.Map
                    || !m_House.Region.Contains(item.Location))
                        continue;

                    bag.DropItem(item);
                }
            }

			if ( bag.Items.Count == 0 )
			{
				bag.Delete();
				return;
			}

			m_House.Owner.BankBox.DropItem( bag );
		}

		#endregion

		#region Rent

		public void ClearRentTimer()
		{
			if ( m_RentTimer != null )
			{
				m_RentTimer.Stop();
				m_RentTimer = null;
			}

			m_RentTime = DateTime.Now;
		}

		private void BeginRentTimer()
		{
			BeginRentTimer( TimeSpan.FromDays( 1 ) );
		}

		private void BeginRentTimer( TimeSpan time )
		{
			if ( !Owned )
				return;

			m_RentTimer = Timer.DelayCall( time, new TimerCallback( RentDue ) );
			m_RentTime = DateTime.Now + time;
		}

		public void CheckRentTimer()
		{
			if ( m_RentTimer == null || !Owned )
				return;

			m_House.Owner.SendMessage( "This rent cycle ends in {0} days, {1}:{2}:{3}.", (m_RentTime-DateTime.Now).Days, (m_RentTime-DateTime.Now).Hours, (m_RentTime-DateTime.Now).Minutes, (m_RentTime-DateTime.Now).Seconds );
		}

		private void RentDue()
		{
			if ( !Owned || m_House.Owner == null )
				return;

			if ( !m_RecurRent )
			{
				m_House.Owner.SendMessage( "Your town house rental contract has expired, and the bank has once again taken possession." );
				PackUpHouse();
				return;
			}

			if ( !m_Free && m_House.Owner.AccessLevel == AccessLevel.Player && !Server.Mobiles.Banker.Withdraw( m_House.Owner, m_Price ) )
			{
				m_House.Owner.SendMessage( "Since you can not afford the rent, the bank has reclaimed your town house." );
				PackUpHouse();
				return;
			}

			if ( !m_Free )
				m_House.Owner.SendMessage( "The bank has withdrawn {0} gold rent for your town house.", m_Price );

			OnRentPaid();

			if ( m_RentToOwn )
			{
				m_RTOPayments++;

				bool complete = false;

				if ( m_RentByTime == TimeSpan.FromDays( 1 ) && m_RTOPayments >= 60 )
				{
					complete = true;
					m_House.Price = m_Price*60;
				}

				if ( m_RentByTime == TimeSpan.FromDays( 7 ) && m_RTOPayments >= 9 )
				{
					complete = true;
					m_House.Price = m_Price*9;
				}

				if ( m_RentByTime == TimeSpan.FromDays( 30 ) && m_RTOPayments >= 2 )
				{
					complete = true;
					m_House.Price = m_Price*2;
				}

				if ( complete )
				{
					m_House.Owner.SendMessage( "You now own your rental home." );
					m_RentByTime = TimeSpan.FromDays( 0 );
					return;
				}
			}

			BeginRentTimer( m_RentByTime );
		}

		protected virtual void OnRentPaid()
		{
		}

		public void NextPriceType()
		{
			if ( m_RentByTime == TimeSpan.Zero )
				RentByTime = TimeSpan.FromDays( 1 );
			else if ( m_RentByTime == TimeSpan.FromDays( 1 ) )
				RentByTime = TimeSpan.FromDays( 7 );
			else if ( m_RentByTime == TimeSpan.FromDays( 7 ) )
				RentByTime = TimeSpan.FromDays( 30 );
			else
				RentByTime = TimeSpan.Zero;
		}

		public void PrevPriceType()
		{
			if ( m_RentByTime == TimeSpan.Zero )
				RentByTime = TimeSpan.FromDays( 30 );
			else if ( m_RentByTime == TimeSpan.FromDays( 30 ) )
				RentByTime = TimeSpan.FromDays( 7 );
			else if ( m_RentByTime == TimeSpan.FromDays( 7 ) )
				RentByTime = TimeSpan.FromDays( 1 );
			else
				RentByTime = TimeSpan.Zero;
		}

		#endregion

		public bool CanBuyHouse( Mobile m )
		{
			if ( m_Skill != "" )
			{
				try
				{
					SkillName index = (SkillName)Enum.Parse( typeof( SkillName ), m_Skill, true );
					if ( m.Skills[index].Value < m_SkillReq )
						return false;
				}
				catch
				{
					return false;
				}
			}

			if ( m_MinTotalSkill != 0 && m.SkillsTotal/10 < m_MinTotalSkill )
				return false;

			if ( m_MaxTotalSkill != 0 && m.SkillsTotal/10 > m_MaxTotalSkill )
				return false;

			if ( m_YoungOnly && m.Player && !((PlayerMobile)m).Young )
				return false;

			if ( m_Murderers == Intu.Yes && m.Kills < 5 )
				return false;

			if ( m_Murderers == Intu.No && m.Kills >= Mobile.MurderCount )
				return false;

			return true;
		}

		public override void OnDoubleClick( Mobile m )
		{
			if ( m.AccessLevel != AccessLevel.Player )
				new TownHouseSetupGump( m, this );
			else if ( !Visible )
				return;
			else if ( CanBuyHouse( m ) && !BaseHouse.HasAccountHouse( m ) )
				new TownHouseConfirmGump( m, this );
			else
				m.SendMessage( "You cannot purchase this house." );
		}

		public override void Delete()
		{
			if ( m_House == null || m_House.Deleted )
				base.Delete();
			else
				PublicOverheadMessage( Server.Network.MessageType.Regular, 0x0, true, "You cannot delete this while the home is owned." );

			if ( this.Deleted )
				s_TownHouseSigns.Remove( this );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Free )
				list.Add( 1060658, "Price\tFree" );
			else if ( m_RentByTime == TimeSpan.Zero )
				list.Add( 1060658, "Price\t{0}{1}", m_Price, m_KeepItems ? " (+" + m_ItemsPrice + " for the items)" : "" );
			else if ( m_RecurRent )
				list.Add( 1060658, "{0}\t{1}\r{2}", PriceType + (m_RentToOwn ? " Rent-to-Own" : " Recurring"), m_Price, m_KeepItems ? " (+" + m_ItemsPrice + " for the items)" : "" );
			else
				list.Add( 1060658, "One {0}\t{1}{2}", PriceTypeShort, m_Price, m_KeepItems ? " (+" + m_ItemsPrice + " for the items)" : "" );

			list.Add( 1060659, "Lockdowns\t{0}", m_Locks );
			list.Add( 1060660, "Secures\t{0}", m_Secures );

			if ( m_SkillReq != 0.0 )
				list.Add( 1060661, "Requires\t{0}", m_SkillReq + " in " + m_Skill );
			if ( m_MinTotalSkill != 0 )
				list.Add( 1060662, "Requires more than\t{0} total skills", m_MinTotalSkill );
			if ( m_MaxTotalSkill != 0 )
				list.Add( 1060663, "Requires less than\t{0} total skills", m_MaxTotalSkill );

			if ( m_YoungOnly )
				list.Add( 1063483, "Must be\tYoung" );
			else if ( m_Murderers == Intu.Yes )
				list.Add( 1063483, "Must be\ta murderer" );
			else if ( m_Murderers == Intu.No )
				list.Add( 1063483, "Must be\tinnocent" );
		}

		public TownHouseSign( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 13 );

            // Version 13

            writer.Write(m_ForcePrivate);
            writer.Write(m_ForcePublic);
            writer.Write(m_NoTrade);

            // Version 12

			writer.Write( m_Free );

			// Version 11

			writer.Write( (int)m_Murderers );

			// Version 10

			writer.Write( m_LeaveItems );

			// Version 9
			writer.Write( m_RentToOwn );
			writer.Write( m_OriginalRentTime );
			writer.Write( m_RTOPayments );

			// Version 7
			writer.WriteItemList( m_PreviewItems, true );

			// Version 6
			writer.Write( m_ItemsPrice );
			writer.Write( m_KeepItems );

			// Version 5
			writer.Write( m_DecoreItemInfos.Count );
			foreach( DecoreItemInfo info in m_DecoreItemInfos )
				info.Save( writer );

			writer.Write( m_Relock );

			// Version 4
			writer.Write( m_RecurRent );
			writer.Write( m_RentByTime );
			writer.Write( m_RentTime );
			writer.Write( m_DemolishTime );
			writer.Write( m_YoungOnly );
			writer.Write( m_MinTotalSkill );
			writer.Write( m_MaxTotalSkill );

			// Version 3
			writer.Write( m_MinZ );
			writer.Write( m_MaxZ );

			// Version 2
			writer.Write( m_House );

			// Version 1
			writer.Write( m_Price );
			writer.Write( m_Locks );
			writer.Write( m_Secures );
			writer.Write( m_BanLoc );
			writer.Write( m_SignLoc );
			writer.Write( m_Skill );
			writer.Write( m_SkillReq );
			writer.Write( m_Blocks.Count );
			foreach( Rectangle2D rect in m_Blocks )
				writer.Write( rect );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (version >= 13)
            {
                m_ForcePrivate = reader.ReadBool();
                m_ForcePublic = reader.ReadBool();
                m_NoTrade = reader.ReadBool();
            }

            if (version >= 12)
				m_Free = reader.ReadBool();

			if ( version >= 11 )
				m_Murderers = (Intu)reader.ReadInt();

			if ( version >= 10 )
				m_LeaveItems = reader.ReadBool();

			if ( version >= 9 )
			{
				m_RentToOwn = reader.ReadBool();
				m_OriginalRentTime = reader.ReadTimeSpan();
				m_RTOPayments = reader.ReadInt();
			}

			m_PreviewItems = new ArrayList();
			if ( version >= 7 )
				m_PreviewItems = reader.ReadItemList();

			if ( version >= 6 )
			{
				m_ItemsPrice = reader.ReadInt();
				m_KeepItems = reader.ReadBool();
			}

			m_DecoreItemInfos = new List<DecoreItemInfo>();
			if ( version >= 5 )
			{
				int decorecount = reader.ReadInt();
				DecoreItemInfo info;
				for( int i = 0; i < decorecount; ++i )
				{
					info = new DecoreItemInfo();
					info.Load( reader );
					m_DecoreItemInfos.Add( info );
				}

				m_Relock = reader.ReadBool();
			}

			if ( version >= 4 )
			{
				m_RecurRent = reader.ReadBool();
				m_RentByTime = reader.ReadTimeSpan();
				m_RentTime = reader.ReadDateTime();
				m_DemolishTime = reader.ReadDateTime();
				m_YoungOnly = reader.ReadBool();
				m_MinTotalSkill = reader.ReadInt();
				m_MaxTotalSkill = reader.ReadInt();
			}

			if ( version >= 3 )
			{
				m_MinZ = reader.ReadInt();
				m_MaxZ = reader.ReadInt();
			}

			if ( version >= 2 )
				m_House = (TownHouse)reader.ReadItem();

			m_Price = reader.ReadInt();
			m_Locks = reader.ReadInt();
			m_Secures = reader.ReadInt();
			m_BanLoc = reader.ReadPoint3D();
			m_SignLoc = reader.ReadPoint3D();
			m_Skill = reader.ReadString();
			m_SkillReq = reader.ReadDouble();

			m_Blocks = new List<Rectangle2D>();
			int count = reader.ReadInt();
			for ( int i = 0; i < count; ++i )
				m_Blocks.Add( reader.ReadRect2D() );

			if ( m_RentTime > DateTime.Now )
				BeginRentTimer( m_RentTime-DateTime.Now );

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(StartTimers));

			ClearPreview();

			s_TownHouseSigns.Add( this );
		}
	}
}