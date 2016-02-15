using System;
using Server;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x26BD, 0x26C7 )]
	public class BloodVampire : BladedStaff
	{
		public override string DefaultName{ get{ return "Blood of the Vampire"; } }
		public override int LabelNumber{ get{ return 0; } }

		public override int AosStrengthReq{ get{ return 50; } }
		public override int AosMinDamage{ get{ return 16; } }
		public override int AosMaxDamage{ get{ return 18; } }
		public override int AosSpeed{ get{ return 35; } }

		public override int ArtifactRarity{ get{ return 10; } }

		[Constructable]
		public BloodVampire() : base()
		{
			Weight = 5.0;
			Hue = 1194;
			Attributes.Luck = 95;
			WeaponAttributes.ResistColdBonus = Utility.RandomMinMax(0,10);
			//WeaponAttributes.BloodLust = 1;
			//WeaponAttributes.Knockback = 1;
			WeaponAttributes.UseBestSkill = 1;
			WeaponAttributes.HitLeechHits = Utility.RandomMinMax(25,50);
			WeaponAttributes.HitLeechMana = Utility.RandomMinMax(25,50);
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			fire = pois = nrgy = chaos = direct = 0;
			phys = 70;
			cold = 30;
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			base.AddNameProperty( list );
			list.Add( 600286 );
		}

		public BloodVampire( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			//if ( OneLineTag == "Count Dracula's Bladed Staff" )
			//	OneLineTag = null;
			//if ( Name == "Blood of the Vampire" )
			//	Name = null;
		}
	}
}