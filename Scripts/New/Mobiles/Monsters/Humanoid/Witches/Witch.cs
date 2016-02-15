using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Misc;

namespace Server.Mobiles
{
	public class Witch : BaseCreature
	{
		[Constructable]
		public Witch() : base( AIType.AI_Mage, FightMode.Closest, 10, 8, 0.25, 0.25 )
		{
			Body = 0x191;
			Hue = 0x599;
			Name = NameList.RandomName( "female" );
			Female = true;
			Title = "the witch";
			SpeechHue = Utility.RandomDyedHue();
			HairItemID = 0x203C;
			HairHue = Utility.RandomGreyHue();

			SetSkill( SkillName.Magery, 55.0, 75.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.MagicResist, 65.0, 90.0 );
			SetSkill( SkillName.Parry, 55.0, 75.0 );
			SetSkill( SkillName.Wrestling, 55.0, 75.0 );

			Fame = 4500;
			Karma = -4500;

			SetStr( 350, 400 );
			SetDex( 75, 96 );
			SetInt( 126, 168 );

			SetHits( 275, 315 );

			SetDamage( 10, 20 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 65, 75 );
			SetResistance( ResistanceType.Cold, 55, 65 );
			SetResistance( ResistanceType.Poison, 65, 75 );
			SetResistance( ResistanceType.Energy, 55, 65 );

			SetDamageType( ResistanceType.Cold, 100 - m_BreathPoison );
			SetDamageType( ResistanceType.Energy, m_BreathPoison );

			VirtualArmor = 35;

			AddItem( new WizardsHat( 1 ) );
			AddItem( new FancyShirt() );
			AddItem( new Boots( 1 ) );
			AddItem( new Skirt( 1 ) );

			if ( 0.02 > Utility.RandomDouble() )
				AddItem( new BlackBookOfSpells( 0.05 > Utility.RandomDouble() ? ulong.MaxValue : 0ul ) );
			else
				AddItem( new Spellbook() );

			PackItem( new Gold( Utility.Random( 50, 80 ) ) );
		}

		private int m_BreathPoison = Utility.Random( 50, 35 );

		public override bool ClickTitle{ get{ return false; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override bool ReacquireOnMovement{ get{ return true; } }
		public override bool HasBreath{ get{ return true; } } // it breaths poison?
		public override double BreathMinDelay{ get{ return 20.0; } }
		public override double BreathMaxDelay{ get{ return 25.0; } }
		public override double BreathStallTime{ get{ return 0.0; } }
		public override double BreathEffectDelay{ get{ return 0.65; } }
		public override int BreathFireDamage{ get{ return 0; } }
		public override int BreathPoisonDamage{ get{ return m_BreathPoison; } }
		public override int BreathEnergyDamage{ get{ return 100 - m_BreathPoison; } }

		public override bool OnBeforeDeath()
		{
			//PackNecroReg( 1, 3 );
			//PackNecroReg( 1, 3 );
			//PackNecroReg( 1, 3 );
			PackPotion(); PackPotion();
			//if ( 0.10 > Utility.RandomDouble() )
			//	PackItem( new BallOfReputation() );
			return base.OnBeforeDeath();
		}

		public Witch( Serial serial ) : base( serial )
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
		}
	}
}