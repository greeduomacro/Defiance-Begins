using System;
using Server.Items;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class RunebookDyeTub : DyeTub, IRewardItem
	{
		public override TextDefinition TargetMessage{ get{ return new TextDefinition( 1049774 ); } } // Target the runebook or runestone to dye
		public override TextDefinition FailMessage{ get{ return new TextDefinition( 1049775 ); } } // You can only dye runestones or runebooks with this tub.
		public override int LabelNumber{ get{ return 1049740; } } // Runebook Dye Tub
		public override CustomHuePicker CustomHuePicker{ get{ return CustomHuePicker.LeatherDyeTub; } }

		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get{ return m_IsRewardItem; }
			set{ m_IsRewardItem = value; InvalidateProperties(); }
		}

		[Constructable]
		public RunebookDyeTub()
		{
			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_IsRewardItem && !RewardSystem.CheckIsUsableBy( from, this, null ) )
				return;

			base.OnDoubleClick( from );
		}

		public override bool Dye( Mobile from, Item item )
		{
			if ( !item.Movable )
				from.SendLocalizedMessage( 1049776 ); // You cannot dye runes or runebooks that are locked down.
			else if ( item.Dye( from, this ) )
			{
				from.PlaySound( 0x23E );
				return true;
			}

			return false;
		}

		public RunebookDyeTub( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( Core.ML && m_IsRewardItem )
				list.Add( 1076220 ); // 4th Year Veteran Reward
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (bool) m_IsRewardItem );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_IsRewardItem = reader.ReadBool();
					break;
				}
			}
		}
	}
}