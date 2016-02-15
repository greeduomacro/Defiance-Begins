using System;
using System.Collections.Generic;
using System.Text;
using Server.Factions;

namespace Server.Ethics.Hero
{
	public sealed class HeroEthic : Ethic
	{
		public HeroEthic()
		{
			m_Definition = new EthicDefinition(
					1150,
					"Hero", "(Spirit of the Just)",
					"I will defend the virtues",
					new Power[]
					{
						new HolySense(),
						new HolyItem(),
						new SummonFamiliar(),
						new HolyBlade(),
						//new Bless(),
						//new HolyShield(),
						new HolySteedPower()//,
						//new HolyWord()
					},
					new RankDefinition[]
					{
						new RankDefinition(     0,	"(Hero: Paige)" ),
						new RankDefinition(   500,	"(Hero: Squire)" ),
						new RankDefinition(  1500,	"(Hero: Errant)" ),
						new RankDefinition(  3000,	"(Hero: Lance)" ),
						new RankDefinition(  5250,	"(Hero: Knight)" ),
						new RankDefinition(  8250,	"(Hero: Sergeant)" ),
						new RankDefinition( 12250,	"(Hero: Commander)" ),
						new RankDefinition( 17250,	"(Hero: Crusader)" ),
						new RankDefinition( 23500,	"(Hero: Prophet)" ),
						new RankDefinition( 31000,	"(Hero: Marshal)" ),
						new RankDefinition( 40000,	"(Hero: Paladin)" )
					}
				);
		}

		public override bool IsEligible( Mobile mob )
		{
			if ( mob.SkillsTotal >= 5000 && Ethic.HasEligibleSkill( mob ) )
			{
				Faction fac = Faction.Find( mob );

				return mob.AccessLevel == AccessLevel.Player && !( fac is Minax || fac is Shadowlords );
			}

			return false;
		}
	}
}