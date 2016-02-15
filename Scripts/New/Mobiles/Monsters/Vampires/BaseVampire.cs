using System;
using System.Collections.Generic;
using System.Reflection;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Misc;

namespace Server.Mobiles
{
/*
	public interface IAnimated
	{
		void Reanimate( Corpse c );
		//Really you could get the corpse from Mobile.Corpse
		//Except that there are two corpses, and to not confuse future expansion, we will feed it the correct one.
		void ReanimateEffect( Point3D location, Map map );
	}
*/
	public abstract class BaseVampire : BaseCreature
	{
		private bool m_Unconscience;
		private AnimateTimer m_AnimTimer;

		public BaseVampire( AIType aiType, FightMode fightMode, int rangePerception, int rangeFight, double activeSpeed, double passiveSpeed ) : base( aiType, fightMode, rangePerception, rangeFight, activeSpeed, passiveSpeed )
		{
			SpeechHue = Utility.RandomDyedHue();

			if ( this.Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
				Title = "the vampiress";
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
				Title = "the vampire";
			}

			Hue = 1072;
			HairItemID = 0x203C;
			HairHue = 1072;
		}

		public override int GetHurtSound()
		{
			if ( Body.IsFemale )
				return Utility.Random( 331, 5 ); // 331 - 334
			else
				return Utility.Random( 340, 6 ); // 340 - 345
		}

		public override int GetDeathSound()
		{
			if ( Body.IsFemale )
				return Utility.Random( 336, 4 ); // 336 - 339
			else
				return Utility.Random( 346, 4 ); // 346 - 349
		}

		public override bool ClickTitle{ get{ return false; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lesser; } }
		public abstract int VampireTimer{ get; }
		public bool Unconscience{ get{ return m_Unconscience; } set{ m_Unconscience = value; if ( m_Unconscience ) Hidden = true; } }

		public BaseVampire( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
			writer.Write( (bool) m_Unconscience );

			//Reanimate();
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
			m_Unconscience = reader.ReadBool();

			//Reanimate();
		}

		public virtual void GeneratePack()
		{
			return;
		}

		public virtual void Reanimate( Corpse c )
		{
			Hits = HitsMaxSeed;
			Stam = StamMaxSeed;
			Mana = ManaMaxSeed;
		}

		public void ReanimateEffect( Point3D location, Map map )
		{
			m_Unconscience = Hidden = Blessed = CantWalk = false;
			PlaySound( 494 );
			Animate( 21, 6, 1, false, false, 1 );
		}

		public virtual double StakeVariance( Stake stake, double stakechance )
		{
			return stakechance;
		}

		public override void RevealingAction()
		{
			if ( !m_Unconscience )
				base.RevealingAction();
		}

		public override bool OnBeforeDeath()
		{
			PlaySound( GetHurtSound() );
			return !m_Unconscience || base.OnBeforeDeath();
		}

		public override void OnDeath( Container c )
		{
			if ( !m_Unconscience )
			{
				m_AnimTimer = new AnimateTimer( (VampireCorpse)c, VampireTimer, this );
				m_AnimTimer.Start();
				m_Unconscience = Hidden = Blessed = CantWalk = true;
			}
			else
			{
				c.Visible = false;

				base.OnDeath( c );

				//When they die, they create a new corpse, lets move the items!

				for( int i = c.Items.Count; i > 0; i-- )
					m_AnimTimer.Corpse.DropItem( (Item)c.Items[0] );

				c.Delete();
				m_AnimTimer.Stop();
			}
		}

		private class AnimateTimer : Timer
		{
			private VampireCorpse m_Corpse;
			private BaseVampire m_Vampire;

			[CommandProperty( AccessLevel.GameMaster )]
			public VampireCorpse Corpse
			{
				get{ return m_Corpse; }
				set{ m_Corpse = value; }
			}

			public AnimateTimer( VampireCorpse corpse, int vamptimer, BaseVampire vampire ) : base( TimeSpan.FromSeconds( vamptimer ), TimeSpan.FromSeconds( vamptimer ), 1 )
			{
				Priority = TimerPriority.OneSecond;

				m_Corpse = corpse;
				m_Vampire = vampire;
			}

			protected override void OnTick()
			{
				if ( m_Corpse.Deleted || m_Corpse == null )
				{
					m_Vampire.Delete();
					Stop();
					return;
				}
				else if ( m_Vampire.Deleted || m_Vampire == null )
					return;

				List<Item> items = new List<Item>( m_Corpse.Items );

				foreach ( Item item in items )
				{
					if ( item.RootParent == m_Corpse )
					{
						if ( item.GetSavedFlag( 0x1 ) ) //Not Lootable
							item.Movable = true;

						if ( m_Corpse.EquipItems.Contains( item ) )
						{
							if ( m_Vampire is ShadowVampire || m_Vampire is ShadowVampireLord )
								item.Hue += 16384;

							m_Vampire.AddItem( item );
						}
						else
							m_Vampire.PackItem( item );
					}
				}

				m_Vampire.Reanimate( m_Corpse );
				m_Vampire.ReanimateEffect( m_Corpse.Location, m_Corpse.Map );

				m_Vampire.MoveToWorld( m_Corpse.Location, m_Corpse.Map );
				m_Vampire.Direction = m_Corpse.Direction;

				m_Corpse.Delete();

				Stop();
			}
		}
	}
}