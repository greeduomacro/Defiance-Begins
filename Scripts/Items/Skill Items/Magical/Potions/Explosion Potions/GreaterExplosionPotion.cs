using System;
using Server;

namespace Server.Items
{
	public class GreaterExplosionPotion : BaseExplosionPotion
	{
//		public override int MinDamage { get { return /*Core.AOS ? 20 :*/ 15; } }
//		public override int MaxDamage { get { return /*Core.AOS ? 40 :*/ 30; } }
		public override int Damage{ get{ return Utility.Dice( 3, 5, 12 ); } } // 17 - 27
		public override double Delay{ get{ return 5.0; } }

		[Constructable]
		public GreaterExplosionPotion() : base( PotionEffect.ExplosionGreater )
		{
		}

		public GreaterExplosionPotion( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}