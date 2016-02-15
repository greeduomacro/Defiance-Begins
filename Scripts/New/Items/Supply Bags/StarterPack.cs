using System;
using Server;
using Server.Misc;
using Server.Mobiles;
using Server.Multis.Deeds;

namespace Server.Items
{
	public class StarterPack : Bag
	{
		[Constructable]
		public StarterPack() : base()
		{
			Name = "Starter Pack";
			Hue = 196;

			Item item = new MiniatureHorse();
			DropItem( item );
			item.X = 30;
			item.Y = 76;

			item = new BagOfReagents( 50 );
			DropItem( item );
			item.X = 71;
			item.Y = 55;

			Container bag = new Bag();
			bag.DropItem( new LeatherCap() );
			bag.DropItem( new LeatherChest() );
			bag.DropItem( new LeatherLegs() );
			bag.DropItem( new LeatherGloves() );
			bag.DropItem( new LeatherArms() );
			bag.DropItem( new LeatherGorget() );
			DropItem( bag );
			bag.X = 63;
			bag.Y = 75;

/*			if ( TestCenter.Enabled )
			{
	//			item = new SmallBrickHouseDeed();
	//			DropItem( item );
	//			item.X = 23;
	//			item.Y = 53;

				item = new Spellbook( ulong.MaxValue );
				item.LootType = LootType.Blessed;
				DropItem( item );
				item.X = 72;
				item.Y = 92;

				item = new Runebook();
				DropItem( item );
				item.X = 93;
				item.Y = 92;

				item = new EtherealLlama();
				item.Name = "a beta testers ethereal llama";
				DropItem( item );
				item.X = 94;
				item.Y = 34;
			}
			else
			{ */
				item = new BankCheck( 1000 );
				DropItem( item );
				item.X = 52;
				item.Y = 36;
			//}
		}

		public StarterPack( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}