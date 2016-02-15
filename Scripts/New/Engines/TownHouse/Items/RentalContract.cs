using System;
using System.Collections;
using Server;
using Server.Multis;
using Server.Items;

namespace Server.Multis
{
	public class RentalContract : TownHouseSign
	{
		private Mobile m_RentalMaster;
		private Mobile m_RentalClient;
		private BaseHouse m_ParentHouse;
		private bool m_Completed, m_EntireHouse;

		public BaseHouse ParentHouse{ get{ return m_ParentHouse; } }
		public Mobile RentalClient{ get{ return m_RentalClient; } set{ m_RentalClient = value; InvalidateProperties(); } }
		public Mobile RentalMaster{ get{ return m_RentalMaster; } }
		public bool Completed{ get{ return m_Completed; } set{ m_Completed = value; } }
		public bool EntireHouse{ get{ return m_EntireHouse; } set{ m_EntireHouse = value; } }

		public RentalContract() : base()
		{
			ItemID = 0x14F0;
			Movable = true;
			RentByTime = TimeSpan.FromDays( 1 );
			RecurRent = true;
			MaxZ = MinZ;
		}

		public bool HasContractedArea( Rectangle2D rect, int z )
		{
			foreach( Item item in TownHouseSign.AllSigns )
				if ( item is RentalContract && item != this && item.Map == Map && m_ParentHouse == ((RentalContract)item).ParentHouse )
					foreach( Rectangle2D rect2 in ((RentalContract)item).Blocks )
						for( int x = rect.Start.X; x < rect.End.X; ++x )
							for( int y = rect.Start.Y; y < rect.End.Y; ++y )
								if ( rect2.Contains( new Point2D( x, y ) ) )
									if ( ((RentalContract)item).MinZ <= z && ((RentalContract)item).MaxZ >= z )
										return true;

			return false;
		}

		public bool HasContractedArea( int z )
		{
			foreach( Item item in TownHouseSign.AllSigns )
				if ( item is RentalContract && item != this && item.Map == Map && m_ParentHouse == ((RentalContract)item).ParentHouse )
					if ( ((RentalContract)item).MinZ <= z && ((RentalContract)item).MaxZ >= z )
						return true;

			return false;
		}

		public void DepositTo( Mobile m )
		{
			if ( m == null )
				return;

			if ( Free )
			{
				m.SendMessage( "Since this home is free, you do not receive the deposit." );
				return;
			}

			m.BankBox.DropItem( new Gold( Price ) );
			m.SendMessage( "You have received a {0} gold deposit from your town house.", Price );
		}

		public override void ValidateOwnership()
		{
			if ( m_Completed && m_RentalMaster == null )
			{
				Delete();
				return;
			}

			if ( m_RentalClient != null && ( m_ParentHouse == null || m_ParentHouse.Deleted ) )
			{
				Delete();
				return;
			}

			if ( m_RentalClient != null && !Owned )
			{
				Delete();
				return;
			}

			if ( ParentHouse == null )
				return;

			if ( !ValidateLocSec() )
			{
				if ( DemolishTimer == null )
					BeginDemolishTimer( TimeSpan.FromHours( 48 ) );
			}
			else
				ClearDemolishTimer();
		}

		protected override void DemolishAlert()
		{
			if ( ParentHouse == null || m_RentalMaster == null || m_RentalClient == null )
				return;

			m_RentalMaster.SendMessage( "You have begun to use lockdowns reserved for {0}, and their rental unit will collapse in {1}.", m_RentalClient.Name, Math.Round( (DemolishTime-DateTime.Now).TotalHours, 2 ) );
			m_RentalClient.SendMessage( "Alert your land lord, {0}, they are using storage reserved for you.  They have violated the rental agreement, which will end in {1} if nothing is done.", m_RentalMaster.Name, Math.Round( (DemolishTime-DateTime.Now).TotalHours, 2 ) );
		}

		public void FixLocSec()
		{
			int count = 0;

			if ( (count = General.RemainingSecures( m_ParentHouse )+Secures) < Secures )
				Secures = count;

			if ( (count = General.RemainingLocks( m_ParentHouse )+Locks) < Locks )
				Locks = count;
		}

		public bool ValidateLocSec()
		{
			if ( General.RemainingSecures( m_ParentHouse )+Secures < Secures )
				return false;

			if ( General.RemainingLocks( m_ParentHouse )+Locks < Locks )
				return false;

			return true;
		}

		public override void ConvertItems( bool keep )
		{
			if ( House == null || m_ParentHouse == null || m_RentalMaster == null )
				return;

			foreach( BaseDoor door in new ArrayList( m_ParentHouse.Doors ) )
				if ( door.Map == House.Map && House.Region.Contains( door.Location ) )
					ConvertDoor( door );

			foreach( SecureInfo info in new ArrayList( m_ParentHouse.Secures ) )
				if ( info.Item.Map == House.Map && House.Region.Contains( info.Item.Location ) )
					m_ParentHouse.Release( m_RentalMaster, info.Item );

			foreach( Item item in new ArrayList( m_ParentHouse.LockDowns ) )
				if ( item.Map == House.Map && House.Region.Contains( item.Location ) )
					m_ParentHouse.Release( m_RentalMaster, item );
		}

		public override void UnconvertDoors( )
		{
			if ( House == null || m_ParentHouse == null )
				return;

			foreach( BaseDoor door in new ArrayList( House.Doors ) )
				House.Doors.Remove( door );
		}

		protected override void OnRentPaid()
		{
			if ( m_RentalMaster == null || m_RentalClient == null )
				return;

			if ( Free )
				return;

			m_RentalMaster.BankBox.DropItem( new Gold( Price ) );
			m_RentalMaster.SendMessage( "The bank has transfered your rent from {0}.", m_RentalClient.Name );
		}

		public override void ClearHouse()
		{
			if ( !Deleted )
				Delete();

			base.ClearHouse();
		}

		public override void OnDoubleClick( Mobile m )
		{
			ValidateOwnership();

			if ( Deleted )
				return;

			if ( m_RentalMaster == null )
				m_RentalMaster = m;

			BaseHouse house = BaseHouse.FindHouseAt( m );

			if ( m_ParentHouse == null )
				m_ParentHouse = house;

			if ( house == null || ( house != m_ParentHouse && house != House ) )
			{
				m.SendMessage( "You must be in the home to view this contract." );
				return;
			}

			if ( m == m_RentalMaster
			 && !m_Completed
			 && house is TownHouse
			 && ((TownHouse)house).ForSaleSign.PriceType != "Sale" )
			{
				m_ParentHouse = null;
				m.SendMessage( "You can only rent property you own." );
				return;
			}

			if ( m == m_RentalMaster && !m_Completed && General.EntireHouseContracted( m_ParentHouse ) )
			{
				m.SendMessage( "This entire house already has a rental contract." );
				return;
			}

			if ( m_Completed )
				new ContractConfirmGump( m, this );
			else if ( m == m_RentalMaster )
				new ContractSetupGump( m, this );
			else
				m.SendMessage( "This rental contract has not yet been completed." );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			if ( m_RentalClient != null )
				list.Add( "a house rental contract with " + m_RentalClient.Name );
			else if ( m_Completed )
				list.Add( "a completed house rental contract" );
			else
				list.Add( "an uncompleted house rental contract" );
		}

		public override void Delete()
		{
			if ( m_ParentHouse == null )
			{
				base.Delete();
				return;
			}

			if ( !Owned && !m_ParentHouse.IsFriend( m_RentalClient ) )
			{
				if ( m_RentalClient != null && m_RentalMaster != null )
				{
					m_RentalMaster.SendMessage( "{0} has ended your rental agreement.  Because you revoked their access, their last payment will be refunded.", m_RentalMaster.Name );
					m_RentalClient.SendMessage( "You have ended your rental agreement with {0}.  Because your access was revoked, your last payment is refunded.", m_RentalClient.Name );
				}

				DepositTo( m_RentalClient );
			}
			else if ( Owned )
			{
				if ( m_RentalClient != null && m_RentalMaster != null )
				{
					m_RentalClient.SendMessage( "{0} has ended your rental agreement.  Since they broke the contract, your are refunded the last payment.", m_RentalMaster.Name );
					m_RentalMaster.SendMessage( "You have ended your rental agreement with {0}.  They will be refunded their last payment.", m_RentalClient.Name );
				}

				DepositTo( m_RentalClient );

				PackUpHouse();
			}
			else
			{
				if ( m_RentalClient != null && m_RentalMaster != null )
				{
					m_RentalMaster.SendMessage( "{0} has ended your rental agreement.", m_RentalClient.Name );
					m_RentalClient.SendMessage( "You have ended your rental agreement with {0}.", m_RentalMaster.Name );
				}

				DepositTo( m_RentalMaster );
			}

			ClearRentTimer();
			base.Delete();
		}

		public RentalContract( Serial serial ) : base( serial )
		{
			RecurRent = true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			// Version 1

			writer.Write( m_EntireHouse );

			writer.Write( m_RentalMaster );
			writer.Write( m_RentalClient );
			writer.Write( m_ParentHouse );
			writer.Write( m_Completed );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version >= 1 )
				m_EntireHouse = reader.ReadBool();

			m_RentalMaster = reader.ReadMobile();
			m_RentalClient = reader.ReadMobile();
			m_ParentHouse = reader.ReadItem() as BaseHouse;
			m_Completed = reader.ReadBool();
		}
	}
}