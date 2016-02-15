using System;
using System.Collections.Generic;
using Server;
using Server.Network;

namespace Server.Items
{
	public interface IFlourMill
	{
		int MaxFlour{ get; }
		int CurFlour{ get; set; }
	}

	public enum FlourMillStage
	{
		Empty,
		Filled,
		Working
	}

	public abstract class BaseFlourMillAddon : BaseAddon, IFlourMill
	{
		private int m_Flour;
		private Timer m_Timer;

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxFlour
		{
			get{ return 2; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int CurFlour
		{
			get{ return m_Flour; }
			set{ m_Flour = Math.Max( 0, Math.Min( value, MaxFlour ) ); UpdateStage(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool HasFlour
		{
			get{ return ( m_Flour > 0 ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsFull
		{
			get{ return ( m_Flour >= MaxFlour ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsWorking
		{
			get{ return ( m_Timer != null ); }
		}

		public void StartWorking( Mobile from )
		{
			if ( IsWorking )
				return;

			m_Timer = Timer.DelayCall<Mobile>( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback<Mobile>( FinishWorking_Callback ), from );
			UpdateStage();
		}

		private void FinishWorking_Callback( Mobile from )
		{
			if ( m_Timer != null )
			{
				m_Timer.Stop();
				m_Timer = null;
			}

			if ( from != null && !from.Deleted && !this.Deleted && IsFull )
			{
				SackFlour flour = new SackFlour();

				flour.ItemID = ( Utility.RandomBool() ? 4153 : 4165 );

				if ( from.PlaceInBackpack( flour ) )
				{
					m_Flour = 0;
				}
				else
				{
					flour.Delete();
					from.SendLocalizedMessage( 500998 ); // There is not enough room in your backpack!  You stop grinding.
				}
			}

			UpdateStage();
		}

		public abstract int[][] StageTable{ get; }

		private int[] FindItemTable( int itemID )
		{
			for ( int i = 0; i < StageTable.Length; ++i )
			{
				int[] itemTable = StageTable[i];

				for ( int j = 0; j < itemTable.Length; ++j )
				{
					if ( itemTable[j] == itemID )
						return itemTable;
				}
			}

			return null;
		}

		public void UpdateStage()
		{
			if ( IsWorking )
				UpdateStage( FlourMillStage.Working );
			else if ( HasFlour )
				UpdateStage( FlourMillStage.Filled );
			else
				UpdateStage( FlourMillStage.Empty );
		}

		public void UpdateStage( FlourMillStage stage )
		{
			List<AddonComponent> components = this.Components;

			for ( int i = 0; i < components.Count; ++i )
			{
				AddonComponent component = components[i] as AddonComponent;

				if ( component == null )
					continue;

				int[] itemTable = FindItemTable( component.ItemID );

				if ( itemTable != null )
					component.ItemID = itemTable[(int)stage];
			}
		}

		public override void OnComponentUsed( AddonComponent c, Mobile from )
		{
			if ( !from.InRange( GetWorldLocation(), 4 ) || !from.InLOS( this ) )
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
			else if ( !IsFull )
				from.SendLocalizedMessage( 500997 ); // You need more wheat to make a sack of flour.
			else
				StartWorking( from );
		}

		[Constructable]
		public BaseFlourMillAddon( int hue )
		{
			Hue = hue;
		}

		public BaseFlourMillAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (int) m_Flour );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_Flour = reader.ReadInt();
					break;
				}
			}

			UpdateStage();
		}
	}
}