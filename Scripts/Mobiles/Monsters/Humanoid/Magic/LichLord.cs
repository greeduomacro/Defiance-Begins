using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a lich corpse" )]
	public class LichLord : BaseCreature
	{
		public override string DefaultName{ get{ return "a lich lord"; } }

		[Constructable]
		public LichLord() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a lich lord";
			Body = 79;
			BaseSoundID = 412;

			SetStr( 416, 505 );
			SetDex( 146, 165 );
			SetInt( 566, 655 );

			SetHits( 250, 303 );

			SetDamage( 11, 13 );

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Cold, 60 );
			SetDamageType( ResistanceType.Energy, 40 );

			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.Necromancy, 90, 110.0 );
			SetSkill( SkillName.SpiritSpeak, 90.0, 110.0 );

			SetSkill( SkillName.EvalInt, 90.1, 100.0 );
			SetSkill( SkillName.Magery, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 150.5, 200.0 );
			SetSkill( SkillName.Tactics, 50.1, 70.0 );
			SetSkill( SkillName.Wrestling, 60.1, 80.0 );

			Fame = 18000;
			Karma = -18000;

			VirtualArmor = 50;
			PackItem( new GnarledStaff() );
			PackNecroReg( 12, 40 );
		}

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.MedScrolls, 2 );

			AddLoot( LootPack.RareStatueDrop );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 4; } }
		public override int DefaultBloodHue{ get{ return -1; } }

		private DateTime m_NextAttack;

		public override void OnActionCombat()
		{
			Mobile combatant = Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != Map || !InRange( combatant, 12 ) || !CanBeHarmful( combatant ) || !InLOS( combatant ) )
				return;

			if ( DateTime.Now >= m_NextAttack && /*AIObject.Action != ActionType.Combat &&*/ AIObject.Action != ActionType.Flee && !Paralyzed && Utility.Random( 25 ) == 0 )
			{
				SummonUndead( combatant );
				m_NextAttack = DateTime.Now + TimeSpan.FromSeconds( 300.0 + (300.0 * Utility.RandomDouble()) );
			}
		}

		public override void OnThink()
		{
			base.OnThink();

			if ( DateTime.Now >= m_NextAttack && AIObject.Action != ActionType.Combat && AIObject.Action != ActionType.Flee && !Paralyzed )
				EndPolymorph();
		}

		public void SummonUndead( Mobile target )
		{
			Point3D[] locs = new Point3D[4];

			locs[0] = Location;

			for ( int i = 1; i < 4; i++ )
			{
				bool validLocation = false;

				for ( int j = 0; !validLocation && j < 10; ++j )
				{
					int x = X + Utility.Random( 4 ) - 1;
					int y = Y + Utility.Random( 4 ) - 1;
					int z = this.Map.GetAverageZ( x, y );

					if ( validLocation = this.Map.CanFit( x, y, this.Z, 16, false, false ) )
						locs[i] = new Point3D( x, y, Z );
					else if ( validLocation = this.Map.CanFit( x, y, z, 16, false, false ) )
						locs[i] = new Point3D( x, y, z );
				}
			}

			bool movelich = false;

			for ( int i = 0;i < 4; i++ )
			{
				BaseCreature summon = null;

				if ( !movelich && ( Utility.Random( 4 ) == 0 || i == 3 ) )
				{
					summon = this;
					BodyMod = Utility.RandomList( 50, 56 );
					HueMod = 0;
					movelich = true;
				}
				else
				{
					summon = new Skeleton();
					summon.Team = this.Team;
					summon.FightMode = FightMode.Closest;
				}

				summon.MoveToWorld( locs[i], Map );
				Effects.SendLocationEffect( summon.Location, summon.Map, 0x3728, 10, 10, 0, 0 );
				summon.PlaySound( 0x48F );
				summon.PlaySound( summon.GetAttackSound() );
				summon.Combatant = target;
			}
		}

		public void EndPolymorph()
		{
			BodyMod = 0;
			HueMod = -1;
		}

		public LichLord( Serial serial ) : base( serial )
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