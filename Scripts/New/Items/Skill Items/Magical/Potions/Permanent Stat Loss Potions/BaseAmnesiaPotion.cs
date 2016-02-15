using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public abstract class BaseAmnesiaPotion : BasePotion
	{
		public abstract int Loss { get; }

		public BaseAmnesiaPotion( PotionEffect effect ) : base( 0xE24, effect )
		{
			Hue = 1282;
		}

		public BaseAmnesiaPotion( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public void DoStatLoss( Mobile from )
		{
			from.RawInt -= Loss;
		}

		public override void Drink( Mobile from )
		{
			if ( from.RawInt > 10 )
			{
				if ( from.BeginAction( typeof( BaseAmnesiaPotion ) ) )
				{
					DoStatLoss( from );

					BasePotion.PlayDrinkEffect( from );

					this.Consume();

					new DelayTimer( from ).Start();
				}
				else
					from.LocalOverheadMessage( MessageType.Regular, 0x22, true, "You must wait for your body to adjust to the potion." );
			}
			else
				from.SendMessage( "You decide against drinking this potion, as you are already fairly forgetful." );
		}

		private class DelayTimer : Timer
		{
			private Mobile m_Mobile;

			public DelayTimer( Mobile m ) : base( TimeSpan.FromSeconds( 3.0 ) )
			{
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				m_Mobile.EndAction( typeof( BaseAmnesiaPotion ) );
			}
		}
	}
}