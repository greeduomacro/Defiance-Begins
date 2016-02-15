using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Multis;
using Server.Mobiles;
using Server.Ethics;

namespace Server.Misc
{
	public class Cleanup
	{
		public static void Initialize()
		{
			Timer.DelayCall( TimeSpan.FromSeconds( 2.5 ), new TimerCallback( Run ) );
		}

		public static void Run()
		{
			List<Item> items = new List<Item>();
			List<Item> validItems = new List<Item>();
			List<Mobile> hairCleanup = new List<Mobile>();
			List<Mobile> mobiles = new List<Mobile>();
			List<Mobile> validMobiles = new List<Mobile>();

			List<Mobile> orphans = new List<Mobile>();
			int stabled = 0;

			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m is PlayerMobile )
				{
					PlayerMobile pm = (PlayerMobile)m;
					if ( pm.Account == null )
						orphans.Add( m );
					else if ( !Ethic.Enabled && pm.EthicPlayer != null )
						pm.EthicPlayer.Detach();
				}
				else
				{
					BaseCreature pet = m as BaseCreature;
					if ( pet != null && pet.Controlled && !pet.Summoned && pet.ControlMaster != null && !pet.Blessed && pet.ControlMaster.Player && !pet.IsStabled && pet.Map != Map.Internal )
					{
						if ( pet.IsDeadPet )
							pet.Resurrect();

						PlayerMobile master = pet.ControlMaster as PlayerMobile;

						if ( master != null )
						{
							master.StablePet( pet, true, true );
							stabled++;
						}
					}
					else //Invalid Internalized Mobiles
					{
						if ( pet != null && (pet.IsStabled || (pet is BaseMount && ((BaseMount)pet).Rider != null)) )
							continue;

						if ( m.Spawner != null || m.Blessed )
							continue;

						if ( m is PlayerMobile || m.Player || m is PlayerVendor || m is PlayerBarkeeper )
							continue;

						if ( m.Map != null && m.Map != Map.Internal )
							continue;

						if ( m.Location != Point3D.Zero )
							continue;

						mobiles.Add( m );
					}
				}
			}

			int boxes = 0;
			int emptyboxes = 0;
			int spawners = 0;
			int parents = 0;
			int emptyspawners = 0;

			foreach ( Item item in World.Items.Values )
			{
				if ( item.Map == null )
				{
					items.Add( item );
					continue;
				}
				else if ( item is Spawner )
				{
					if ( item.Map == Map.Internal && item.Parent == null )
					{
						spawners++;
						continue;
					}
					else if ( item.Parent == null && (((Spawner)item).Entries == null || ((Spawner)item).Entries.Count == 0) )
					{
						items.Add( item );
						//Console.WriteLine( "Cleanup: Detected invalid spawner {0} at ({1},{2},{3}) [{4}]", item.Serial, item.X, item.Y, item.Z, item.Map );
						emptyspawners++;
					}
				}
				else if ( item is CommodityDeed )
				{
					CommodityDeed deed = (CommodityDeed)item;

					if ( deed.CommodityItem != null )
						validItems.Add( deed.CommodityItem );

					continue;
				}
				else if ( item is BaseHouse )
				{
					BaseHouse house = (BaseHouse)item;

					List<IEntity> entities = house.GetHouseEntities();

					foreach ( RelocatedEntity relEntity in house.RelocatedEntities )
						if ( relEntity.Entity is Item )
							validItems.Add( (Item)relEntity.Entity );

					foreach ( VendorInventory inventory in house.VendorInventories )
						foreach ( Item subItem in inventory.Items )
							validItems.Add( subItem );
				}
				else if ( item is BankBox )
				{
					BankBox box = (BankBox)item;
					Mobile owner = box.Owner;

					if ( owner == null )
					{
						items.Add( box );
						++boxes;
					}
					else if ( box.Items.Count == 0 )
					{
						items.Add( box );
						++emptyboxes;
					}

					continue;
				}
				else if ( (item.Layer == Layer.Hair || item.Layer == Layer.FacialHair) )
				{
					object rootParent = item.RootParent;

					if ( rootParent is Mobile )
					{
						Mobile rootMobile = (Mobile)rootParent;
						if ( item.Parent != rootMobile /*&& rootMobile.AccessLevel == AccessLevel.Player*/ )
						{
							items.Add( item );
							continue;
						}
						else if( item.Parent == rootMobile )
						{
							hairCleanup.Add( rootMobile );
							continue;
						}
					}
				}
				else if ( item is Container )
				{
					List<Item> contitems = item.AcquireItems();

					for ( int i = contitems.Count - 1; i >= 0; i-- )
					{
						Item child = contitems[i];

						if ( child.Parent != item )
						{
							//if ( child is Spawner && child.Parent == null )
							//	item.Items.RemoveAt( i-- );

							Console.WriteLine( "Cleanup: Detected orphan item {0} ({1}) of {2} ({3}) has parent of {4} ({5})", child.GetType().Name, child.Serial, item.GetType().Name, item.Serial, child.Parent == null ? "(-null-)" : child.Parent.GetType().Name, child.Parent is IEntity ? ((IEntity)child.Parent).Serial.ToString() : "N/A" );

							contitems.RemoveAt( i ); //Clean this up

							if ( child.Parent is Item )
							{
								Item parent = (Item)child.Parent;
								List<Item> parentitems = parent.AcquireItems();
								parentitems.Add( child );
							}
							else if ( child.Parent is Mobile )
							{
								Mobile parent = (Mobile)child.Parent;
								parent.Items.Add( child );
							}

							parents++;
						}
					}
				}
				else if ( item.RootParent is BaseTreasureChest ) //Clear out all items in a treasure chest
				{
					items.Add( item );
					continue;
				}
				else if ( item is CharacterStatueDeed )
				{
					CharacterStatueDeed deed = item as CharacterStatueDeed;
					if ( deed.Statue != null )
					{
						validMobiles.Add( deed.Statue );
						if ( deed.Statue.Plinth != null )
							validItems.Add( deed.Statue.Plinth );
					}
				}
				else if ( item is KeyRing )
					validItems.AddRange( ((KeyRing)item).Keys );

				if ( item.Parent != null || ( item.Map != null && item.Map != Map.Internal ) || item.HeldBy != null )
					continue;

				if ( item.Location != Point3D.Zero )
					continue;

				if ( !IsBuggable( item ) )
					continue;

				if ( !item.Movable )
					continue;

				items.Add( item );
			}

			for ( int i = 0; i < validItems.Count; ++i )
				items.Remove( validItems[i] );

			for ( int i = 0; i < validMobiles.Count; ++i )
				mobiles.Remove( validMobiles[i] );

			if ( items.Count > 0 )
			{
				String message = String.Format( "Cleanup: Detected {0} inaccessible items, ", items.Count );

				if ( boxes > 0 )
					message += String.Format( "including {0} bank box{1}, ", boxes, boxes != 1 ? "es" : String.Empty );

				if ( emptyboxes > 0 )
					message += String.Format( "{1}{0} empty bank box{2}, ", emptyboxes, boxes == 0 ? "including " : String.Empty, emptyboxes != 1 ? "es" : String.Empty );

				if ( emptyspawners > 0 )
					message += String.Format( "{1} {0} empty/invalid spawner{2}, ", emptyspawners, (emptyboxes == 0 && boxes == 0) ? "including " : String.Empty, emptyspawners != 1 ? "s" : String.Empty );

				message += "removing..";

				Console.WriteLine( message );

				for ( int i = 0; i < items.Count; ++i )
				{
					//Console.WriteLine( "Item: {0} - {2} ({1})", items[i].Name, items[i].Serial, items[i].GetType() );
					items[i].Delete();
				}
			}

			if ( spawners > 0 )
				Console.WriteLine( "Cleanup: Detected {0} inaccessible spawners..", spawners );

			if ( parents > 0 )
				Console.WriteLine( "Cleanup: Detected {0} invalid parent-child items, fixing references..", parents );

			if ( hairCleanup.Count > 0 )
			{
				Console.WriteLine( "Cleanup: Detected {0} hair and facial hair items being worn, converting to virtual hair..", hairCleanup.Count );

				for ( int i = 0; i < hairCleanup.Count; i++ )
					hairCleanup[i].ConvertHair();
			}

			if ( orphans.Count > 0 )
			{
				Console.WriteLine( "Cleanup: Detected {0} orphaned players, removing..", orphans.Count );

				for ( int i = 0; i < orphans.Count; ++i )
					orphans[i].Delete();
			}

			if ( stabled > 0 )
				Console.WriteLine( "Cleanup: Detected {0} pets requiring stables..", stabled );

			if ( mobiles.Count > 0 )
			{
				Console.WriteLine( "Cleanup: Detected {0} invalid mobiles, removing...", mobiles.Count );
				for ( int i = 0; i < mobiles.Count; ++i )
					mobiles[i].Delete();
			}
		}

		public static bool IsBuggable( Item item )
		{
			if ( item is Fists )
				return false;

			if ( item is ICommodity || item is Multis.BaseBoat
				|| item is Fish || item is BigFish
				|| item is BasePotion || item is Food || item is CookableFood
				|| item is SpecialFishingNet || item is BaseMagicFish
				|| item is Shoes || item is Sandals
				|| item is Boots || item is ThighBoots
				|| item is TreasureMap || item is MessageInABottle
				|| item is BaseArmor || item is BaseWeapon
				|| item is BaseClothing || item is Bone
				|| (item is BaseJewel && Core.AOS)
				|| (item is BasePotion && Core.ML))
				return true;

			return false;
		}
	}
}