using System;
using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Factions;

namespace Server.Mobiles
{
	public class Banker : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.MerchantsGuild; } }

		[Constructable]
		public Banker() : base( "the banker" )
		{
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBBanker() );
		}

		public static int GetBalance( Mobile from )
		{
			Gold[] gold;
			BankCheck[] checks;

			return GetBalance( from, out gold, out checks );
		}

		public static int GetBalance( Mobile from, out Gold[] gold, out BankCheck[] checks )
		{
			int balance = 0;

			Container bank = from.FindBankNoCreate();

			if ( bank != null )
			{
				gold = bank.FindItemsByType<Gold>().ToArray();
				checks = bank.FindItemsByType<BankCheck>().ToArray();

				for ( int i = 0; i < gold.Length; ++i )
					balance += gold[i].Amount;

				for ( int i = 0; i < checks.Length; ++i )
					balance += checks[i].Worth;
			}
			else
			{
				gold = new Gold[0];
				checks = new BankCheck[0];
			}

			return balance;
		}

		public static bool Withdraw( Mobile from, int amount )
		{
			Gold[] gold;
			BankCheck[] checks;

			int balance = GetBalance( from, out gold, out checks );

			if ( balance >= amount )
			{
				WithdrawUpTo( from, amount, gold, checks );
				return true;
			}

			return false;
		}

		public static void WithdrawUpTo( Mobile from, int amount, Gold[] gold, BankCheck[] checks )
		{
			for ( int i = 0; amount > 0 && i < gold.Length; ++i )
			{
				int consume = Math.Min( gold[i].Amount, amount );
				gold[i].Consume( consume );
				amount -= consume;
			}

			for ( int i = 0; amount > 0 && i < checks.Length; ++i )
			{
				BankCheck check = checks[i];

				int consume = Math.Min( check.Worth, amount );
				check.ConsumeWorth( consume );
				amount -= consume;
			}
		}

		public static bool WithdrawPackAndBank( Mobile from, int amount )
		{
			int totalGold = 0;
			List<Gold> packgold;
			//BankCheck[] packchecks;
			Gold[] bankgold;
			BankCheck[] bankchecks;

			if ( from.Backpack != null )
			{
				packgold = from.Backpack.FindItemsByType<Gold>();
				//packchecks = from.Backpack.FindItemsByType<BankCheck>( false ); //They are only blessed on the top layer.

				for ( int i = 0;i < packgold.Count; i++ )
					totalGold += packgold[i].Amount;

				//for ( int i = 0;i < packchecks.Length; i++ )
				//	totalGold += packchecks[i].Worth;
			}
			else
			{
				packgold = new List<Gold>();
				//packchecks = new BankCheck[0];
			}

			if ( totalGold < amount )
				totalGold += Banker.GetBalance( from, out bankgold, out bankchecks );
			else
			{
				bankgold = new Gold[0];
				bankchecks = new BankCheck[0];
			}

			if ( totalGold >= amount )
			{
				for ( int i = 0; amount > 0 && i < packgold.Count; ++i )
				{
					int consume = Math.Min( packgold[i].Amount, amount );
					packgold[i].Consume( consume );
					amount -= consume;
				}
/*
				for ( int i = 0; amount > 0 && i < packchecks.Count; ++i )
				{
					int consume = Math.Min( packchecks[i].Worth, amount );
					packchecks[i].ConsumeWorth( consume );
					amount -= consume;
				}
*/
				Banker.WithdrawUpTo( from, amount, bankgold, bankchecks );

				return true;
			}

			return false;
		}

		public static bool Deposit( Mobile from, int amount )
		{
			BankBox box = from.FindBankNoCreate();
			if ( box == null )
				return false;

			List<Item> items = new List<Item>();

			while ( amount > 0 )
			{
				Item item;

				if ( amount < 5000 )
				{
					item = new Gold( amount );
					amount = 0;
				}
				else if ( amount <= 1000000 )
				{
					item = new BankCheck( amount );
					amount = 0;
				}
				else
				{
					item = new BankCheck( 1000000 );
					amount -= 1000000;
				}

				if ( box.TryDropItem( from, item, false ) )
					items.Add( item );
				else
				{
					item.Delete();
					foreach ( Item curItem in items )
						curItem.Delete();

					return false;
				}
			}

			return true;
		}

		public static int DepositUpTo( Mobile from, int amount )
		{
			BankBox box = from.FindBankNoCreate();
			if ( box == null )
				return 0;

			int amountLeft = amount;
			while ( amountLeft > 0 )
			{
				Item item;
				int amountGiven;

				if ( amountLeft < 5000 )
				{
					item = new Gold( amountLeft );
					amountGiven = amountLeft;
				}
				else if ( amountLeft <= 1000000 )
				{
					item = new BankCheck( amountLeft );
					amountGiven = amountLeft;
				}
				else
				{
					item = new BankCheck( 1000000 );
					amountGiven = 1000000;
				}

				if ( box.TryDropItem( from, item, false ) )
					amountLeft -= amountGiven;
				else
				{
					item.Delete();
					break;
				}
			}

			return amount - amountLeft;
		}

		public static void Deposit( Container cont, int amount )
		{
			while ( amount > 0 )
			{
				Item item;

				if ( amount < 5000 )
				{
					item = new Gold( amount );
					amount = 0;
				}
				else if ( amount <= 1000000 )
				{
					item = new BankCheck( amount );
					amount = 0;
				}
				else
				{
					item = new BankCheck( 1000000 );
					amount -= 1000000;
				}

				cont.DropItem( item );
			}
		}

		public Banker( Serial serial ) : base( serial )
		{
		}

		public override bool HandlesOnSpeech( Mobile from )
		{
			if ( from.InRange( this.Location, 12 ) )
				return true;

			return base.HandlesOnSpeech( from );
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			Mobile m = e.Mobile;

			Faction fact = Faction.Find( m );

			if ( !e.Handled && m.InRange( this.Location, 12 ) )
			{
				for ( int i = 0; i < e.Keywords.Length; ++i )
				{
					int keyword = e.Keywords[i];

					switch ( keyword )
					{
						case 0x0000: // *withdraw*
						{
							e.Handled = true;

							if ( m.Criminal )
							{
								this.Say( 500389 ); // I will not do business with a criminal!
								break;
							}
							else if ( FactionAllegiance != null && fact != null && FactionAllegiance != fact )
							{
								this.Say( "I will not do business with the enemy!" );
								break;
							}

							string[] split = e.Speech.Split( ' ' );

							if ( split.Length >= 2 )
							{
								int amount;

								Container pack = m.Backpack;

								if ( !int.TryParse( split[1], out amount ) )
									break;

								if ( (!Core.ML && amount > 5000) || (Core.ML && amount > 60000) )
								{
									this.Say( 500381 ); // Thou canst not withdraw so much at one time!
								}
								else if (pack == null || pack.Deleted || !(pack.TotalWeight < pack.MaxWeight) || !(pack.TotalItems < pack.MaxItems))
								{
									this.Say(1048147); // Your backpack can't hold anything else.
								}
								else if (amount > 0)
								{
									BankBox box = m.FindBankNoCreate();

									if (box == null || !box.ConsumeTotal(typeof(Gold), amount))
									{
										this.Say(500384); // Ah, art thou trying to fool me? Thou hast not so much gold!
									}
									else
									{
										pack.DropItem(new Gold(amount));

										this.Say(1010005); // Thou hast withdrawn gold from thy account.
									}
								}
							}

							break;
						}
						case 0x0001: // *balance*
						{
							e.Handled = true;

							if ( m.Criminal )
							{
								this.Say( 500389 ); // I will not do business with a criminal!
								break;
							}

							BankBox box = m.FindBankNoCreate();

							if ( box != null )
								this.Say( 1042759, box.TotalGold.ToString() ); // Thy current bank balance is ~1_AMOUNT~ gold.
							else
								this.Say( 1042759, "0" ); // Thy current bank balance is ~1_AMOUNT~ gold.

							break;
						}
						case 0x0002: // *bank*
						{
							e.Handled = true;

							if ( m.Criminal )
							{
								this.Say( 500378 ); // Thou art a criminal and cannot access thy bank box.
								break;
							}
							else if ( FactionAllegiance != null && fact != null && FactionAllegiance != fact )
							{
								this.Say( "Thou art an enemy and cannot access thy bank box." );
								break;
							}

							m.BankBox.Open();

							break;
						}
						case 0x0003: // *check*
						{
							e.Handled = true;

							if ( m.Criminal )
							{
								this.Say( 500389 ); // I will not do business with a criminal!
								break;
							}
							else if ( FactionAllegiance != null && fact != null && FactionAllegiance != fact )
							{
								this.Say( "I will not do business with the enemy!" );
								break;
							}

							string[] split = e.Speech.Split( ' ' );

							if ( split.Length >= 2 )
							{
								int amount;

                                if ( !int.TryParse( split[1], out amount ) )
                                    break;

								if ( amount < 5000 )
								{
									this.Say( 1010006 ); // We cannot create checks for such a paltry amount of gold!
								}
								else if ( amount > 1000000 )
								{
									this.Say( 1010007 ); // Our policies prevent us from creating checks worth that much!
								}
								else
								{
									BankCheck check = new BankCheck( amount );

									BankBox box = m.BankBox;

									if ( !box.TryDropItem( m, check, false ) )
									{
										this.Say( 500386 ); // There's not enough room in your bankbox for the check!
										check.Delete();
									}
									else if ( !box.ConsumeTotal( typeof( Gold ), amount ) )
									{
										this.Say( 500384 ); // Ah, art thou trying to fool me? Thou hast not so much gold!
										check.Delete();
									}
									else
									{
										this.Say( 1042673, AffixType.Append, amount.ToString(), "" ); // Into your bank box I have placed a check in the amount of:
									}
								}
							}

							break;
						}
					}
				}
			}

			base.OnSpeech( e );
		}

		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive )
				list.Add( new OpenBankEntry( from, this ) );

			base.AddCustomContextEntries( from, list );
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
	}
}