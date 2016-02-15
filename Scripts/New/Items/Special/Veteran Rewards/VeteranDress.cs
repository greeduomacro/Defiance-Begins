using System;
using Server;
using Server.Mobiles;
using Server.Accounting;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class VeteranDress : PlainDress, IVeteranClothing, IRewardItem
	{
		public bool IsRewardItem{ get{ return true; } set{} }
		
		private int m_LabelNumber;
		public override int LabelNumber
		{
			get
			{
				if ( m_LabelNumber > 0 )
					return m_LabelNumber;

				return base.LabelNumber;
			}
		}

		private Account m_Account;
		public Account Account{ get{ return m_Account; } }

		private byte m_Level;

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
		public int Number
		{
			get{ return m_LabelNumber; }
			set{ m_LabelNumber = value; InvalidateProperties(); }
		}

		public VeteranDress() : this( String.Empty )
		{
		}

		public VeteranDress( string un ) : base( 0 )
		{
			SetAccount( Accounts.GetAccount( un ) as Account );
			Weight = 1.0;
			LootType = LootType.Blessed;
		}

		private void SetAccount( Account acct )
		{
			m_Account = acct;
			SetAttributes();

			InvalidateProperties();
		}

		private void SetAttributes()
		{
			SetAttributes( true );
		}

		private void SetAttributes( bool message )
		{
			if ( Parent is PlayerMobile )
				RemoveSkillGainMod( (PlayerMobile)Parent, message );

			if ( m_Account != null )
			{
				m_Level = (byte)RewardSystem.GetRewardLevel( m_Account );
				RewardLabelHue labelhue = RewardSystem.LabelHues[m_Level];
				m_LabelNumber = labelhue.DressLabel;
				Hue = labelhue.Hue;

				if ( m_Level > 0 && Parent is PlayerMobile )
					AddSkillGainMod( (PlayerMobile)Parent, message );
			}
			else
			{
				m_Level = 0;
				m_LabelNumber = 0;
				Hue = 0;
			}
		}

		private void AddSkillGainMod( PlayerMobile pm, bool message )
		{
			if ( m_Level > 0 )
			{
				pm.AddSkillGainMod( "Veteran SkillGain Mod", (SkillName)(-1), 0.05 * m_Level, TimeSpan.Zero );

				if ( message )
				{
					string msg = String.Empty;
					switch ( m_Level )
					{
						case 10: default: msg = "quite drastically"; break;
						case 8: case 9: msg = "drastically"; break;
						case 6: case 7: msg = "quite a bit"; break;
						case 4: case 5: msg = "somewhat"; break;
						case 2: case 3: msg = "meagerly"; break;
						case 1: msg = "a little bit"; break;
					}

					pm.SendMessage( String.Format( "Your ability to learn new skills has increased {0}.", msg ) );
				}
			}
		}

		private void RemoveSkillGainMod( PlayerMobile pm, bool message )
		{
			pm.RemoveSkillGainMod( "Veteran SkillGain Mod" );
			if ( message )
				pm.SendMessage( "Your ability to learn new skills has normalized." );
		}

		public override void OnAdded( object parent )
		{
			if ( parent is PlayerMobile )
				AddSkillGainMod( (PlayerMobile)parent, true );
		}

		public override void OnRemoved( object parent )
		{
			if ( parent is PlayerMobile )
				RemoveSkillGainMod( (PlayerMobile)parent, true );
		}

		public override bool Nontransferable{ get{ return true; } }

		public override bool CanEquip( Mobile m )
		{
			return base.CanEquip( m ) && (m.AccessLevel > AccessLevel.GameMaster || (m_Account != null && m.Account == m_Account));
		}

		public VeteranDress( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			SetAttributes( false );

			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			if ( m_Account != null )
			{
				writer.Write( true );
				writer.Write( m_Account.Username );
			}
			else
				writer.Write( false );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			if ( reader.ReadBool() )
				SetAccount( Accounts.GetAccount( reader.ReadString() ) as Account );

			SetAttributes( false );
		}
	}
}