using System;
using System.Collections.Generic;
using System.Text;
using Server.Factions;

namespace Server.Ethics.Evil
{
	public sealed class EvilEthic : Ethic
	{
		public EvilEthic()
		{
			m_Definition = new EthicDefinition(
					0x455,
					"Evil", "(Spawn of Evil)",
					"I am evil incarnate",
					new Power[]
					{
						new UnholySense(),
						new UnholyItem(),
						new SummonFamiliar(),
						new VileBlade(),
						//new Blight(),
						//new UnholyShield(),
						new UnholySteedPower()//,
						//new UnholyWord()
					},
					new RankDefinition[]
					{
						new RankDefinition(     0,	"(Evil: Neophyte)" ),
						new RankDefinition(   500,	"(Evil: Grunt)" ),
						new RankDefinition(  1500,	"(Evil: Delinquent)" ),
						new RankDefinition(  3000,	"(Evil: Outlaw)" ),
						new RankDefinition(  5250,	"(Evil: Defiler)" ),
						new RankDefinition(  8250,	"(Evil: Invader)" ),
						new RankDefinition( 12250,	"(Evil: Plunderer)" ),
						new RankDefinition( 17250,	"(Evil: Destroyer)" ),
						new RankDefinition( 23500,	"(Evil: Harbinger)" ),
						new RankDefinition( 31000,	"(Evil: Chaosmaster)" ),
						new RankDefinition( 40000,	"(Evil: Warlord)" )
					}
				);
		}

		public override bool IsEligible( Mobile mob )
		{
			if ( mob.SkillsTotal >= 5000 && Ethic.HasEligibleSkill( mob ) )
			{
				Faction fac = Faction.Find( mob );

				return mob.AccessLevel == AccessLevel.Player && !( fac is TrueBritannians || fac is CouncilOfMages );
			}

			return false;
		}
	}
}