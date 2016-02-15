using System;
using System.Collections.Generic;
using System.Text;
using Server.Items;
using Server.Misc;
using Server.Network;
using Server.Factions;

namespace Server.Ethics.Hero
{
	public sealed class HolyBlade : Power
	{
		public static readonly int SapphiresRequired = TestCenter.Enabled ? 1 : 15;

		public HolyBlade()
		{
			m_Definition = new PowerDefinition(
					10,
					"Holy Runic",
					"Erstok Reyam",
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

			if ( item is BaseArmor || item is BaseWeapon )
			{
				EthicsItem ethicItem = EthicsItem.Find( item );

				if ( item.Parent != fromMobile )
					fromMobile.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, "You may only imbue items you are wearing." );
				else if ( ethicItem == null )
					fromMobile.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, "This item has not been imbued with a holy blessing." );
				else if ( ethicItem.Ethic != Ethic.Find( fromMobile ) )
					fromMobile.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, "The magic surrounding this item repels your attempts to imbue." );
				else if ( !fromMobile.Backpack.ConsumeTotal( typeof( Sapphire ), SapphiresRequired ) )
					fromMobile.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, String.Format( "You must sacrifice {0} sapphire{1} to imbue this item.", SapphiresRequired, ( SapphiresRequired != 1 ) ? "s" : String.Empty ) );
				else if ( CheckInvoke( from ) )
				{
					ethicItem.MakeRunic();

					fromMobile.FixedEffect( 0x375A, 10, 20, 1153, 0 );
					fromMobile.PlaySound( 0x209 );

					FinishInvoke( from );
				}
			}
			else
				fromMobile.LocalOverheadMessage( MessageType.Regular, 0x3B2, 500352 ); // This is neither weapon nor armor.
		}
	}
}