using System;
using System.Text;
using System.Reflection;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Commands;

namespace Server.Items
{
	public class VendStone : Item
	{
		private string m_ItemType, m_ItemName, m_Parameters;
		private int m_Price, m_Amount, m_TextHue;

		[CommandProperty( AccessLevel.GameMaster )]
		public string ItemName
		{
			get{ return m_ItemName; }
			set{ m_ItemName = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string ItemType
		{
			get{ return m_ItemType; }
			set{ m_ItemType = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string Parameters
		{
			get{ return m_Parameters; }
			set{ m_Parameters = value; CreateParameters(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Price
		{
			get{ return m_Price; }
			set{ m_Price = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int TextHue
		{
			get{ return m_TextHue; }
			set{ m_TextHue = value; }
		}

		private string[] m_ParamList;

		public string FullName{ get{ return String.Format( m_ItemName, m_Amount ); } }

		public override int LabelNumber{ get{ return 0; } }
		public override string DefaultName{ get{ return "Vending Stone"; } }

		[Constructable]
		public VendStone() : this( 0x2D1, "BagOfReagents", "Bag of 60 Reagents", 2500 )
		{
			Parameters = "60";
		}

		[Constructable]
		public VendStone( int hue, string itype, string tname, int price ) : this( hue, itype, tname, price, String.Empty )
		{
		}

		[Constructable]
		public VendStone( int hue, string itype, string tname, int price, string parameters ) : base( 0xED4 )
		{
			Movable = false;
			m_ItemType = itype;
			m_ItemName = tname;
			m_Price = price;
			Hue = hue;
			m_TextHue = 1346;
			Parameters = parameters;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_ItemType != null )
				list.Add( 1060661, "Item\t{0}", m_ItemName );
			else
				list.Add( 1060661, "Item\tNothing");

			if ( m_Price > 0 )
				list.Add( 1060659, "Price\t{0}", m_Price );
			else
				list.Add( 1060659, "Price\tFree");
		}

		public override void OnSingleClick( Mobile from )
		{
			bool free = m_Price <= 0;
			LabelTo( from, m_TextHue, "{0} for {1}{2}", FullName, free ? "free" : m_Price.ToString(), free ? String.Empty : "gp" );
		}

		private void CreateParameters()
		{
			if ( String.IsNullOrEmpty( m_Parameters ) )
				m_ParamList = new string[0];
			else
				m_ParamList = m_Parameters.Trim().Split( ' ' );
		}

		public override void OnDoubleClick( Mobile from )
		{
			Type type = SpawnerType.GetType( m_ItemType );

			if ( type != null && typeof( Item ).IsAssignableFrom( type ) )
			{
				if ( m_Price < 0 )
					m_Price = 0;

				if ( from.AccessLevel < AccessLevel.GameMaster && m_Price > 0 )
				{
					int totalGold = 0;

					if ( from.Backpack != null )
						totalGold += from.Backpack.GetAmount( typeof( Gold ) );

					totalGold += Banker.GetBalance( from );

					if ( totalGold < m_Price )
					{
						from.SendMessage( 1153, "You lack the funds to purchase this." );
						return;
					}
				}

				object o = null;

				try
				{
					ConstructorInfo[] ctors = type.GetConstructors();

					for ( int i = 0; i < ctors.Length; ++i )
					{
						ConstructorInfo ctor = ctors[i];

						if ( Add.IsConstructable( ctor, AccessLevel.GameMaster ) )
						{
							ParameterInfo[] paramList = ctor.GetParameters();

							if ( m_ParamList.Length == paramList.Length )
							{
								object[] paramValues = Add.ParseValues( paramList, m_ParamList );

								if ( paramValues != null )
								{
									o = ctor.Invoke( paramValues );
									break;
								}
							}
						}
					}
				}
				catch
				{
					Console.WriteLine( "VendStone: Invalid constructor or parameters for {0}: {1} {2}", Serial, m_ItemType, m_Parameters );
				}

				Item item = o as Item;

				if ( item != null && from.Backpack != null )
				{
					if ( from.AddToBackpack( item ) )
					{
						from.SendMessage( "You place the {0} into your backpack.", FullName );
						from.PlaySound( from.Backpack.GetDroppedSound( item ) );

						int leftPrice = m_Price;

						if ( from.Backpack != null )
							leftPrice -= from.Backpack.ConsumeUpTo( typeof( Gold ), leftPrice );

						if ( leftPrice > 0 )
							Banker.Withdraw( from, leftPrice );
					}
					else
					{
						from.SendMessage( "You do not have room for this item in your backpack." );
						int sound = item.GetDropSound();
						from.PlaySound( sound == -1 ? 0x42 : sound );
					}
				}
			}
			else
				from.SendMessage( "The magic from this stone has faded." );
		}

		public VendStone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 1 ); // version

			writer.Write(m_Parameters);
			writer.Write(m_TextHue);

			writer.Write(m_ItemName);
			writer.Write(m_ItemType);
			writer.Write(m_Price);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 1:
				{
					Parameters = reader.ReadString();
					m_TextHue = reader.ReadInt();
					goto case 0;
				}
				case 0:
				{
					m_ItemName = reader.ReadString();
					m_ItemType = reader.ReadString();
					m_Price = reader.ReadInt();

					if ( version < 1 )
					{
						int amount = reader.ReadInt();

						Parameters = amount.ToString();
						if ( amount > 0 )
							m_ItemName = String.Format( m_ItemName, amount );
					}

					break;
				}
			}
		}
	}
}