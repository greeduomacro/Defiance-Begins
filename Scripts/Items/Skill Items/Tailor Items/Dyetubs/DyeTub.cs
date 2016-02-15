using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Targeting;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Gumps;

namespace Server.Items
{
	public class DyeTub : Item, IDyeTub
	{
		private bool m_Redyable;
		private int m_DyedHue;
		private SecureLevel m_SecureLevel;

		public virtual CustomHuePicker CustomHuePicker{ get{ return null; } }

		public virtual bool IsDyable( Item item )
		{
			return item.DyeType == GetType();
		}

		public virtual bool Dye( Mobile from, Item item )
		{
			if ( item.Parent is Mobile )
				from.SendMessage( "You decided not to dye this while it is worn." ); // Can't Dye clothing that is being worn.
			else if ( item.Dye( from, this ) )
			{
				from.PlaySound( 0x23E );
				return true;
			}
			else
				TextDefinition.SendMessageTo( from, FailMessage );

			return false;
		}

		public override bool Dye( Mobile from, IDyeTub sender )
		{
			return false;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

			writer.Write( (int)m_SecureLevel );
			writer.Write( (bool) m_Redyable );
			writer.Write( (int) m_DyedHue );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
					m_SecureLevel = (SecureLevel)reader.ReadInt();
					goto case 0;
				}
				case 0:
				{
					m_Redyable = reader.ReadBool();
					m_DyedHue = reader.ReadInt();

					break;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Redyable{ get{ return m_Redyable; } set{ m_Redyable = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int DyedHue
		{
			get{ return m_DyedHue; }
			set
			{
				if ( m_Redyable )
				{
					m_DyedHue = value;
					Hue = value;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public SecureLevel Level
		{
			get
			{
				return m_SecureLevel;
			}
			set
			{
				m_SecureLevel = value;
			}
		}

		[Constructable]
		public DyeTub() : base( 0xFAB )
		{
			Weight = 10.0;
			m_Redyable = true;
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			SetSecureLevelEntry.AddTo( from, this, list );
		}

		public DyeTub( Serial serial ) : base( serial )
		{
		}

		// Select the clothing to dye.
		public virtual TextDefinition TargetMessage{ get{ return new TextDefinition( 500859 ); } }

		// You cannot dye that.
		public virtual TextDefinition FailMessage{ get{ return new TextDefinition( 1042083 ); } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( this.GetWorldLocation(), 1 ) )
			{
				TextDefinition.SendMessageTo( from, TargetMessage );
				from.Target = new InternalTarget( this );
			}
			else
				from.SendLocalizedMessage( 500446 ); // That is too far away.
		}

		private class InternalTarget : Target
		{
			private DyeTub m_Tub;

			public InternalTarget( DyeTub tub ) : base( 1, false, TargetFlags.None )
			{
				m_Tub = tub;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Item )
				{
					Item item = (Item)targeted;
					if ( !from.InRange( m_Tub.GetWorldLocation(), 1 ) || !from.InRange( item.GetWorldLocation(), 1 ) )
						from.SendLocalizedMessage( 500446 ); // That is too far away.
					else if ( !item.Deleted && item.Dyable && m_Tub.IsDyable( item ) )
						m_Tub.Dye( from, item );
					else
						TextDefinition.SendMessageTo( from, m_Tub.FailMessage );
				}
				else
					TextDefinition.SendMessageTo( from, m_Tub.FailMessage );
			}
		}
	}
}