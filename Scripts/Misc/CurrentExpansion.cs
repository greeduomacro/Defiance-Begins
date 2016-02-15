using System;
using Server.Network;
using Server.Items;

namespace Server
{
	public class CurrentExpansion
	{
		private static readonly Expansion Expansion = Expansion.UOR;

		public static void Configure()
		{
			Core.Expansion = Expansion;

			bool Enabled = Core.AOS;

			Mobile.InsuranceEnabled = Enabled;
			ObjectPropertyList.Enabled = Enabled;
			//Mobile.VisibleDamageType = Enabled ? VisibleDamageType.Related : VisibleDamageType.None;
			Mobile.VisibleDamageType = VisibleDamageType.Related;
			Mobile.GuildClickMessage = !Enabled;
			Mobile.AsciiClickMessage = !Enabled;

			if ( Enabled )
			{
				AOS.DisableStatInfluences();

				if ( ObjectPropertyList.Enabled )
					PacketHandlers.SingleClickProps = true; // single click for everything is overridden to check object property list
			}

			Item.DefaultDyeType = typeof(DyeTub);
		}
	}
}