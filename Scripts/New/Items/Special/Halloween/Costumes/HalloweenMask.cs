using System;
using System.Collections.Generic;
using Server;
using Server.Misc;
using Server.Spells;
using Server.Mobiles;

namespace Server.Items
{
	[FlipableAttribute( 0x19BC, 0x19BD )]
	public abstract class HalloweenMask : Item
	{
		public abstract int BodyMod{ get; }
		public abstract int HueMod{ get; }
		public virtual int Rarity{ get{ return 1; } }

		public override bool DisplayLootType{ get{ return false; } }

		[Constructable]
		public HalloweenMask() : base ( 0x19BC )
		{
			Layer = Layer.FirstValid;
			Weight = 1.0;
			LootType = LootType.Blessed;
		}

		public HalloweenMask( Serial serial ) : base ( serial )
		{
		}

		public override bool OnEquip( Mobile from )
		{
			if ( (from.BodyMod == 0 && !TransformationSpellHelper.UnderTransformation( from ) && BaseHalloweenGiftGiver.IsHalloween()) || from.AccessLevel >= AccessLevel.GameMaster )
				return base.OnEquip( from );
			else
			{
				from.SendMessage( "You do not believe it would be appropriate to wear the costume now." );
				return false;
			}
		}

		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );
			AddEffect(); //parent checks are done here
		}

		public override void OnRemoved( object parent )
		{
			base.OnRemoved( parent );

			if ( parent is Mobile )
			{
				Mobile from = (Mobile)parent;
				TransformationSpellHelper.RemoveContext( from ); //assume resetGraphics = true
			}
		}

		public virtual void AddEffect()
		{
			if ( Parent is Mobile )
			{
				Mobile from = (Mobile)Parent;
				if ( from.Mounted )
				{
					IMount mount = (IMount)from.Mount;
					mount.Rider = null;
				}

				from.BodyMod = BodyMod;
				from.HueMod = HueMod;
				TransformationSpellHelper.AddContext( from, new TransformContext( null, new List<ResistanceMod>(), typeof(HalloweenMask), null ) );
			}
		}

		private void ForceUnEquip()
		{
			if ( Parent is Mobile )
			{
				Mobile from = (Mobile)Parent;
				if ( from.AddToBackpack( this ) )
					from.SendMessage( "The spirit of halloween has left.  You put the halloween mask in your backpack." );
				else
				{
					from.BankBox.AddItem( this );
					from.SendMessage( "The spirit of halloween has left.  You put the halloween mask in your bankbox." );
				}
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

			if ( !BaseHalloweenGiftGiver.IsHalloween() )
				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( ForceUnEquip ) );
			//else
			//	Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( AddEffect ) );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			if ( !BaseHalloweenGiftGiver.IsHalloween() )
				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( ForceUnEquip ) );
			else
				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerCallback( AddEffect ) );
		}
	}
}