using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a drake corpse" )]
	public class Drake : BaseCreature
	{
		public override string DefaultName{ get{ return "a drake"; } }

		[Constructable]
		public Drake () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = Utility.Random( 60, 2 );
			Hue = ( Body == 2 && 0.02 > Utility.RandomDouble() ) ? Utility.RandomList( 1445, 1436, 2006, 2001 ) : 0;
			BaseSoundID = 362;

			SetStr( 401, 430 );
			SetDex( 133, 152 );
			SetInt( 101, 140 );

			SetHits( 241, 258 );

			SetDamage( 11, 17 );

			SetDamageType( ResistanceType.Physical, 80 );
			SetDamageType( ResistanceType.Fire, 20 );

			SetResistance( ResistanceType.Physical, 45, 50 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.MagicResist, 65.1, 80.0 );
			SetSkill( SkillName.Tactics, 65.1, 90.0 );
			SetSkill( SkillName.Wrestling, 65.1, 80.0 );

			Fame = 5500;
			Karma = -5500;

			VirtualArmor = 46;

			Tamable = true;
			ControlSlots = 2;
			MinTameSkill = 84.3;

			PackReg( 3 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override bool ReacquireOnMovement{ get{ return true; } }
		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int TreasureMapLevel{ get{ return 2; } }
		public override int Meat{ get{ return 10; } }
		public override int Hides{ get{ return 20; } }
		public override HideType HideType{ get{ return HideType.Horned; } }
		public override int Scales{ get{ return 2; } }
		public override ScaleType ScaleType{ get{ return Hue == 0 ? ( Body == 60 ? ScaleType.Yellow : ScaleType.Red ) : ScaleType.Green; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish; } }
		public override int DefaultBloodHue{ get{ return -2; } }
		public override int BloodHueTemplate{ get{ return Utility.RandomGreyHue(); } }

		public Drake( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}