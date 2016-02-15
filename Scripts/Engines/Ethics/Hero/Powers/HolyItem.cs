using System;
using System.Collections.Generic;
using System.Text;
using Server.Items;
using Server.Misc;
using Server.Network;
using Server.Factions;

namespace Server.Ethics.Hero
{
	public sealed class HolyItem : Power
	{
		public static readonly int GoldRequired = TestCenter.Enabled ? 100 : 3000;

		public HolyItem()
		{
			m_Definition = new PowerDefinition(
					10,
					"Holy Item",
					"Vidda K'balc",
					""
				);
		}

		public override void BeginInvoke( Player from )
		{
			from.Mobile.BeginTarget( 12, false, Targeting.TargetFlags.None, new TargetStateCallback( Power_OnTarget ), from );
			from.Mobile.SendMessage( "Which item do you wish to imbue?" );
		}

		private void Power_OnTarget( Mobile fromMobile, object obj, object state )
		{
			Player from = state as Player;

			Item item = obj as Item;

			if ( item == null || item.Deleted )
				return;

			if ( item is IEthicsItem )
			{
				EthicsItem ethicItem = EthicsItem.Find( item );

				if ( item.Parent != fromMobile )
					fromMobile.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, "You may only imbue items you are wearing." );
				else if ( ethicItem != null && ethicItem.Ethic != Ethic.Find( fromMobile ) )
					fromMobile.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, "The magic surrounding this item repels your attempts to imbue." );
				else if ( item is IFactionItem && ((IFactionItem)item).FactionItemState != null )
					fromMobile.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, "The magic surrounding this item is too chaotic to imbue." );
				else if ( item.LootType != LootType.Regular || item.BlessedFor != null )
					fromMobile.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, "The magic surrounding this item is too strong to imbue." );
				//else if ( fromMobile.Map == Map.Felucca && !fromMobile.InRange( new Point3D( 2492, 3930, 5 ), 6 ) )
				//	fromMobile.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, "You require the power of a heroic shrine to imbue this item." );
				else if ( !fromMobile.Backpack.ConsumeTotal( typeof( Gold ), GoldRequired ) )
					fromMobile.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, String.Format( "You must sacrifice {0} gold piece{1} to imbue this item.", GoldRequired, ( GoldRequired != 1 ) ? "s" : String.Empty ) );
				else if ( CheckInvoke( from ) )
				{
					if ( ethicItem != null )
						ethicItem.StartExpiration();
					else
						EthicsItem.Imbue( item, Ethic.Hero, true, Ethic.Hero.Definition.PrimaryHue );

					fromMobile.FixedEffect( 0x375A, 10, 20 );
					fromMobile.PlaySound( 0x209 );
					fromMobile.SendMessage( "The item is now blessed against evil for 24 real world hours." );

					FinishInvoke( from );
				}
			}
			else
				fromMobile.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, "That cannot be imbued!" );
		}
	}
}