using System;
using System.Collections.Generic;
using System.Text;
using Server.Mobiles;

namespace Server.Ethics
{
	[PropertyObject]
	public class Player
	{
		public static Player Find( Mobile mob )
		{
			return Find( mob, false );
		}

		public static Player Find( Mobile mob, bool inherit )
		{
			PlayerMobile pm = mob as PlayerMobile;

			if ( pm == null )
			{
				if ( inherit && mob is BaseCreature )
				{
					BaseCreature bc = mob as BaseCreature;

					if ( bc != null && bc.Controlled )
						pm = bc.ControlMaster as PlayerMobile;
					else if ( bc != null && bc.Summoned )
						pm = bc.SummonMaster as PlayerMobile;
				}

				if ( pm == null )
					return null;
			}

			Player pl = pm.EthicPlayer;

			if ( pl != null && !pl.Ethic.IsEligible( pl.Mobile ) )
				pm.EthicPlayer = pl = null;

			return pl;
		}

		public static readonly int MaxPower = 250;

		private Ethic m_Ethic;
		private Mobile m_Mobile;

		private int m_Power; //Lifeforce
		private int m_Sphere; //Sphere Points
		private int m_Rank; //Sphere Rank
		private int m_History; //Sphere Point History

		private Mobile m_Steed;
		private Mobile m_Familiar;

		private DateTime m_FallenExpire;

		//private DateTime m_Shield;

		public Ethic Ethic { get { return m_Ethic; } }
		public Mobile Mobile { get { return m_Mobile; } }

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Lead )]
		public int Power { get { return m_Power; } set { m_Power = value; } }

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Owner )]
		public int Sphere { get { return m_Sphere; } set { m_Sphere = value;  AdjustRank(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Rank { get { return m_Rank; } } //Rank is adjusted automatically by Sphere points.

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Owner )]
		public int History { get { return m_History; } set { m_History = value; } }

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Lead )]
		public Mobile Steed { get { return m_Steed; } set { m_Steed = value; } }

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Lead )]
		public Mobile Familiar { get { return m_Familiar; } set { m_Familiar = value; } }

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Lead )]
		public DateTime FallenExpire { get { return m_FallenExpire; } set { m_FallenExpire = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool HasFallen { get { return m_FallenExpire > DateTime.Now || m_Mobile.Kills >= Mobile.MurderCount; } }

/*
		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsShielded
		{
			get
			{
				if ( m_Shield == DateTime.MinValue )
					return false;

				if ( DateTime.Now < ( m_Shield + TimeSpan.FromHours( 1.0 ) ) )
					return true;

				FinishShield();
				return false;
			}
		}

		public void BeginShield()
		{
			m_Shield = DateTime.Now;
		}

		public void FinishShield()
		{
			m_Shield = DateTime.MinValue;
		}
*/
		public Player( Ethic ethic, Mobile mobile )
		{
			m_Ethic = ethic;
			m_Mobile = mobile;

			m_Power = 10;
		}

		public void CheckAttach()
		{
			if ( m_Ethic.IsEligible( m_Mobile ) )
				Attach();
		}

		public void Attach()
		{
			if ( m_Mobile is PlayerMobile )
				( m_Mobile as PlayerMobile ).EthicPlayer = this;

			m_Ethic.Players.Add( this );
		}

		public void Detach()
		{
			if ( m_Mobile is PlayerMobile )
				( m_Mobile as PlayerMobile ).EthicPlayer = null;

			m_Ethic.Players.Remove( this );
		}

		public void AdjustRank()
		{
			int rank = m_Rank;

			RankDefinition[] defs = m_Ethic.Definition.Ranks;

			for ( int i = 0; i < defs.Length; i++ )
			{
				if ( i+1 == defs.Length || ( m_Sphere >= defs[i].Points && m_Sphere < defs[i+1].Points ) )
				{
					m_Rank = i;
					break;
				}
			}

			if ( m_Rank < rank )
			{
				m_Mobile.SendMessage( "You have lost a sphere of power." );
				m_Mobile.SendMessage( "You now have {0} sphere{1} of power.", m_Rank, m_Rank != 1 ? "s" : String.Empty );
			}
			else if ( m_Rank > rank )
			{
				m_Mobile.SendMessage( "You have gained a sphere of power." );
				m_Mobile.SendMessage( "You now have {0} sphere{1} of power.", m_Rank, m_Rank != 1 ? "s" : String.Empty );
			}
		}

		public string Title()
		{
			//Theoretically the rank should NEVER go beyond the length of the array.
			return m_Ethic.Definition.Ranks[m_Rank].Title;
		}

		public override string ToString()
		{
			return "...";
		}

		public Player( Ethic ethic, GenericReader reader )
		{
			m_Ethic = ethic;

			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 3:
				{
					m_FallenExpire = reader.ReadDeltaTime();

					goto case 2;
				}
				case 2:
				{
					Sphere = reader.ReadInt(); //We want the rank adjusted from here.

					goto case 1;
				}
				case 1:
				{
					m_Mobile = reader.ReadMobile();

					m_Power = reader.ReadEncodedInt();

					if ( version < 2 )
					{
						/*m_History =*/ reader.ReadEncodedInt();
						m_History = 0;
					}

					m_Steed = reader.ReadMobile();
					m_Familiar = reader.ReadMobile();

					break;
				}
				case 0:
				{
					m_Mobile = reader.ReadMobile();

					m_Power = reader.ReadEncodedInt();

					if ( version < 2 )
					{
						/*m_History =*/ reader.ReadEncodedInt();
						m_History = 0;
					}

					m_Steed = reader.ReadMobile();
					m_Familiar = reader.ReadMobile();

					/*m_Shield =*/ reader.ReadDeltaTime();

					break;
				}
			}
		}

		public void Serialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( 3 ); // version

			writer.WriteDeltaTime( m_FallenExpire );

			writer.Write( m_Sphere );

			writer.Write( m_Mobile );

			writer.WriteEncodedInt( m_Power );

			writer.Write( m_Steed );
			writer.Write( m_Familiar );

			//writer.WriteDeltaTime( m_Shield );
		}
	}
}