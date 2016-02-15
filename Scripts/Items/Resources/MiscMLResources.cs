using System;

namespace Server.Items
{
	public class Blight : Item
	{
		[Constructable( AccessLevel.Owner )]
		public Blight()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public Blight( int amount )
			: base( 0x3183 )
		{
			Stackable = true;
			Amount = amount;
		}

		public Blight( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class LuminescentFungi : Item
	{
		[Constructable( AccessLevel.Owner )]
		public LuminescentFungi()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public LuminescentFungi( int amount )
			: base( 0x3191 )
		{
			Stackable = true;
			Amount = amount;
		}

		public LuminescentFungi( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


	public class CapturedEssence : Item
	{
		[Constructable( AccessLevel.Owner )]
		public CapturedEssence()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public CapturedEssence( int amount )
			: base( 0x318E )
		{
			Stackable = true;
			Amount = amount;
		}

		public CapturedEssence( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


	public class EyeOfTheTravesty : Item
	{
		[Constructable( AccessLevel.Owner )]
		public EyeOfTheTravesty()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public EyeOfTheTravesty( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public EyeOfTheTravesty( int amount )
			: base( 0x318D )
		{
			Stackable = true;
			Amount = amount;
		}

		public EyeOfTheTravesty( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


	public class Corruption : Item
	{
		[Constructable( AccessLevel.Owner )]
		public Corruption()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public Corruption( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public Corruption( int amount )
			: base( 0x3184 )
		{
			Stackable = true;
			Amount = amount;
		}

		public Corruption( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


	public class DreadHornMane : Item
	{
		[Constructable( AccessLevel.Owner )]
		public DreadHornMane()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public DreadHornMane( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public DreadHornMane( int amount )
			: base( 0x318A )
		{
			Stackable = true;
			Amount = amount;
		}

		public DreadHornMane( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


	public class ParasiticPlant : Item
	{
		[Constructable( AccessLevel.Owner )]
		public ParasiticPlant()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public ParasiticPlant( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public ParasiticPlant( int amount )
			: base( 0x3190 )
		{
			Stackable = true;
			Amount = amount;
		}

		public ParasiticPlant( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


	public class Muculent : Item
	{
		[Constructable( AccessLevel.Owner )]
		public Muculent()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public Muculent( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public Muculent( int amount )
			: base( 0x3188 )
		{
			Stackable = true;
			Amount = amount;
		}

		public Muculent( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


	public class DiseasedBark : Item
	{
		[Constructable( AccessLevel.Owner )]
		public DiseasedBark()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public DiseasedBark( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public DiseasedBark( int amount )
			: base( 0x318B )
		{
			Stackable = true;
			Amount = amount;
		}

		public DiseasedBark( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


	public class BarkFragment : Item
	{
		[Constructable( AccessLevel.Owner )]
		public BarkFragment()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public BarkFragment( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public BarkFragment( int amount )
			: base( 0x318F )
		{
			Stackable = true;
			Amount = amount;
		}

		public BarkFragment( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


	public class GrizzledBones : Item
	{
		[Constructable( AccessLevel.Owner )]
		public GrizzledBones()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public GrizzledBones( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public GrizzledBones( int amount )
			: base( 0x318C )
		{
			Stackable = true;
			Amount = amount;
		}

		public GrizzledBones( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if( version <= 0 && ItemID == 0x318F )
				ItemID = 0x318C;
		}
	}


	public class LardOfParoxysmus : Item
	{
		[Constructable( AccessLevel.Owner )]
		public LardOfParoxysmus()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public LardOfParoxysmus( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public LardOfParoxysmus( int amount )
			: base( 0x3189 )
		{
			Stackable = true;
			Amount = amount;
		}

		public LardOfParoxysmus( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class PerfectEmerald : Item
	{
		[Constructable( AccessLevel.Owner )]
		public PerfectEmerald()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public PerfectEmerald( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public PerfectEmerald( int amount )
			: base( 0x3194 )
		{
			Stackable = true;
			Amount = amount;
		}

		public PerfectEmerald( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class DarkSapphire : Item
	{
		[Constructable( AccessLevel.Owner )]
		public DarkSapphire()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public DarkSapphire( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public DarkSapphire( int amount )
			: base( 0x3192 )
		{
			Stackable = true;
			Amount = amount;
		}

		public DarkSapphire( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


	public class Turquoise : Item
	{
		[Constructable( AccessLevel.Owner )]
		public Turquoise()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public Turquoise( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public Turquoise( int amount )
			: base( 0x3193 )
		{
			Stackable = true;
			Amount = amount;
		}

		public Turquoise( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


	public class EcruCitrine : Item
	{
		[Constructable( AccessLevel.Owner )]
		public EcruCitrine()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public EcruCitrine( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public EcruCitrine( int amount )
			: base( 0x3195 )
		{
			Stackable = true;
			Amount = amount;
		}

		public EcruCitrine( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


	public class WhitePearl : Item
	{
		[Constructable( AccessLevel.Owner )]
		public WhitePearl()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public WhitePearl( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public WhitePearl( int amount )
			: base( 0x3196 )
		{
			Stackable = true;
			Amount = amount;
		}

		public WhitePearl( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


	public class FireRuby : Item
	{
		[Constructable( AccessLevel.Owner )]
		public FireRuby()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public FireRuby( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public FireRuby( int amount )
			: base( 0x3197 )
		{
			Stackable = true;
			Amount = amount;
		}

		public FireRuby( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


	public class BlueDiamond : Item
	{
		[Constructable( AccessLevel.Owner )]
		public BlueDiamond()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public BlueDiamond( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public BlueDiamond( int amount )
			: base( 0x3198 )
		{
			Stackable = true;
			Amount = amount;
		}

		public BlueDiamond( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


	public class BrilliantAmber : Item
	{
		[Constructable( AccessLevel.Owner )]
		public BrilliantAmber()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public BrilliantAmber( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public BrilliantAmber( int amount )
			: base( 0x3199 )
		{
			Stackable = true;
			Amount = amount;
		}

		public BrilliantAmber( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class Scourge : Item
	{
		[Constructable( AccessLevel.Owner )]
		public Scourge()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public Scourge( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public Scourge( int amount )
			: base( 0x3185 )
		{
			Stackable = true;
			Amount = amount;
			Hue = 150;
		}

		public Scourge( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


	public class Putrefication : Item
	{
		[Constructable( AccessLevel.Owner )]
		public Putrefication()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public Putrefication( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public Putrefication( int amount )
			: base( 0x3186 )
		{
			Stackable = true;
			Amount = amount;
			Hue = 883;
		}

		public Putrefication( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}


	public class Taint : Item
	{
		[Constructable( AccessLevel.Owner )]
		public Taint()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public Taint( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public Taint( int amount )
			: base( 0x3187 )
		{
			Stackable = true;
			Amount = amount;
			Hue = 731;
		}

		public Taint( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[Flipable( 0x315A, 0x315B )]
	public class PristineDreadHorn : Item
	{
		[Constructable( AccessLevel.Owner )]
		public PristineDreadHorn()
			: base( 0x315A )
		{

		}

		public PristineDreadHorn( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class SwitchItem : Item
	{
		[Constructable( AccessLevel.Owner )]
		public SwitchItem()
			: this( 1 )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public SwitchItem( int amountFrom, int amountTo )
			: this( Utility.RandomMinMax( amountFrom, amountTo ) )
		{
		}

		[Constructable( AccessLevel.Owner )]
		public SwitchItem( int amount )
			: base( 0x2F5F )
		{
			Stackable = true;
			Amount = amount;
		}

		public SwitchItem( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}