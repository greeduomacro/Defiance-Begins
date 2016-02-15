using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a white drake corpse" )]
	public class WhiteDrake : BaseCreature
	{
		public override string DefaultName{ get{ return "a white drake"; } }

		[Constructable]
		public WhiteDrake() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = Utility.RandomList( 60, 61 );
			Hue = 1150;
			BaseSoundID = 362;

			SetStr( 373, 398 );
			SetDex( 161, 190 );
			SetInt( 84, 111 );

			SetHits( 211, 228 );

			SetDamage( 13, 19 );

			SetDamageType( ResistanceType.Physical, 80 );
			SetDamageType( ResistanceType.Fire, 20 );

			SetResistance( ResistanceType.Physical, 45, 60 );
			SetResistance( ResistanceType.Fire, 60, 70 );
			SetResistance( ResistanceType.Cold, 80, 90 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 35, 45 );

			SetSkill( SkillName.MagicResist, 99.1, 100.0 );
			SetSkill( SkillName.Tactics, 99.1, 100.0 );
			SetSkill( SkillName.Wrestling, 99.1, 100.0 );

			Fame = 7500;
			Karma = -7500;

			VirtualArmor = 50;

			Tamable = true;
			ControlSlots = 3;
			MinTameSkill = 89.4;

			PackReg( 10 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override int TreasureMapLevel{ get{ return 2; } }
		public override int Meat{ get{ return 10; } }
		public override int Hides{ get{ return 20; } }
		public override HideType HideType{ get{ return HideType.Horned; } }
		public override int Scales{ get{ return 3; } }
		public override ScaleType ScaleType{ get{ return ScaleType.White; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish; } }
		//public override int DefaultBloodHue{ get{ return -2; } }
		//public override int BloodHueTemplate{ get{ return Utility.RandomGreyHue(); } }

		public WhiteDrake( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}
}