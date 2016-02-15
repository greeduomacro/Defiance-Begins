using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Misc;

namespace Server.Items
{
	public abstract class Stake : Item
	{
		public abstract double StakeChance{ get; }
		public override string DefaultName{ get{ return "stake"; } }

		public Stake() : this( 1 )
		{
		}

		public Stake( int amount ) : base( 0xDE1 )
		{
			Stackable = true;
			Weight = 0.5;
			Amount = amount;
		}

		public Stake( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			list.Add( String.Format("{0} stake", Amount) );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) && from.AccessLevel < AccessLevel.GameMaster )
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else if ( Validate( from ) && from != null && from.Alive )
				from.Target = new StakeTarget( this );
		}

		public virtual bool Validate( Mobile from )
		{
			return true;
		}

		public class StakeTarget : Target
		{
			private Stake m_Stake;
			public StakeTarget(Stake stake) : base( 1, false, TargetFlags.None )
			{
				m_Stake = stake;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is VampireCorpse )
				{
					VampireCorpse c = (VampireCorpse)targeted;
					if ( c.Staked )
						return;

					double stakechance = m_Stake.StakeChance;

					if ( c.Owner is BaseVampire )
						stakechance = ((BaseVampire)c.Owner).StakeVariance( m_Stake, stakechance );

					if ( stakechance <= 0 )
						from.SendMessage( "You have no chance of killing this vampire with that!" );
					else if ( stakechance >= Utility.RandomDouble() )
					{
						//int message = 600282 + Utility.Random( 4 );

						from.PlaySound( 1170 );
						from.SendMessage( "You stake the vampire through its heart, ensuring a swift death." );

						c.Staked = true;
						c.Owner.Blessed = false;
						c.Owner.Kill();
					}
					else
						from.SendMessage( "You miss the vampire's heart." ); // completely miss

					if ( (m_Stake.Amount -= 1) <= 0 )
						m_Stake.Delete();
				}
				else
					from.SendLocalizedMessage( 1042600 );
			}
		}
	}
}