using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Ethics
{
	public class EthicDefinition
	{
		private int m_PrimaryHue;

		private TextDefinition m_Title;
		private TextDefinition m_NPCAdjunct;

		private TextDefinition m_JoinPhrase;

		private Power[] m_Powers;

		private RankDefinition[] m_Ranks;

		public int PrimaryHue { get { return m_PrimaryHue; } }

		public TextDefinition Title { get { return m_Title; } }
		public TextDefinition NPCAdjunct { get { return m_NPCAdjunct; } }

		public TextDefinition JoinPhrase { get { return m_JoinPhrase; } }

		public Power[] Powers { get { return m_Powers; } }

		public RankDefinition[] Ranks{ get{ return m_Ranks; } }

		public EthicDefinition( int primaryHue, TextDefinition title, TextDefinition npcadjunct, TextDefinition joinPhrase, Power[] powers, RankDefinition[] ranks )
		{
			m_PrimaryHue = primaryHue;

			m_Title = title;
			m_NPCAdjunct = npcadjunct;

			m_JoinPhrase = joinPhrase;

			m_Powers = powers;

			m_Ranks = ranks;
		}
	}
}