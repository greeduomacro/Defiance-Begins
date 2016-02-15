using System;
using Server;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
	public class EvilTwin : BaseCreature
	{
		private Mobile m_Original;
		public Mobile Original{ get{ return m_Original; } }

		public EvilTwin( Mobile orig ) : base( AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4 )
		{
			m_Original = orig;

			Body = orig.RawBody;

			//Hue = orig.Hue;
			Hue = 1157;
			Female = orig.Female;

			Name = orig.RawName;
			NameHue = orig.NameHue;

			Title = orig.Title;
			Kills = orig.Kills;

			HairItemID = orig.HairItemID;
			HairHue = orig.HairHue;

			FacialHairItemID = orig.FacialHairItemID;
			FacialHairHue = orig.FacialHairHue;

			for ( int i = 0; i < orig.Skills.Length; ++i )
			{
				Skills[i].Base = orig.Skills[i].Base;
				Skills[i].Cap = orig.Skills[i].Cap;
			}

			for( int i = 0; i < orig.Items.Count; i++ )
				AddItem( CloneItem( orig.Items[i] ) );

			Warmode = true;
			Summoned = true;
			Combatant = orig;
			FocusMob = orig;

			TimeSpan duration = TimeSpan.FromMinutes( 5.0 );

			new UnsummonTimer( orig, this, duration ).Start();
			SummonEnd = DateTime.Now + duration;
		}

		protected override BaseAI ForcedAI { get { return new EvilTwinAI( this ); } }

		public override bool IsHumanInTown() { return false; }

		private Item CloneItem( Item item )
		{
			Item newItem = new Item( item.ItemID );
			newItem.Hue = item.Hue;
			newItem.Layer = item.Layer;
			newItem.Movable = false;

			return newItem;
		}

		public override void AlterMeleeDamageFrom( Mobile from, ref int damage )
		{
			if ( from != m_Original )
				damage = 0;
		}

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			if ( caster != m_Original )
				reflect = true; // Every spell is reflected back to the caster
		}

		//Breath Immune to pets
		public override void AlterAbilityDamageFrom( Mobile from, ref int damage )
		{
			if ( !(from is BaseCreature && ((BaseCreature)from).ControlMaster == m_Original) )
				damage = 0;
		}

		public override bool DeleteCorpseOnDeath { get { return true; } }

		public override bool IsDispellable { get { return false; } }
		public override bool Commandable { get { return false; } }
		public override bool Unprovokable{ get{ return true; } }

		public EvilTwin( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version

			writer.Write( m_Original );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_Original = reader.ReadMobile();
		}
	}

	public class EvilTwinAI : BaseAI
	{
		public EvilTwin m_Twin;

		public EvilTwinAI( EvilTwin m ) : base ( m )
		{
			m_Twin = m;
			m.CurrentSpeed = m.ActiveSpeed;
		}

		public override bool Think()
		{
			Mobile master = m_Twin.Original;

			if ( master != null )
			{
				if ( master.Map == m_Mobile.Map && master.InRange( m_Mobile, m_Mobile.RangePerception ) )
				{
					int iCurrDist = (int)m_Mobile.GetDistanceToSqrt( master );
					WalkMobileRange( master, 2, true, 0, 1 );
				}
				else
					TeleportTo( master );

				if ( m_Twin.FocusMob != master || m_Twin.Combatant != master )
				{
					m_Twin.FocusMob = master;
					m_Twin.Combatant = master;
				}
			}
			else
				m_Mobile.Kill();

			return true;
		}

		private void TeleportTo( Mobile target )
		{
			Point3D from = m_Mobile.Location;
			Point3D to = target.Location;
			m_Mobile.Location = to;
			Effects.SendLocationParticles( EffectItem.Create( from, m_Mobile.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
			Effects.SendLocationParticles( EffectItem.Create(   to, m_Mobile.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );
			m_Mobile.PlaySound( 0x1FE );
		}

		public override bool CanDetectHidden { get { return true; } }
	}
}