using System;
using Server;

namespace Server.Multis
{
    public delegate void TownHouseCommandHandler(CommandInfo info);

    public class CommandInfo
    {
        private Mobile m_Mobile;
        private string m_Command;
        private string m_ArgString;
        private string[] m_Arguments;

        public Mobile Mobile { get { return m_Mobile; } }
        public string Command { get { return m_Command; } }
        public string ArgString { get { return m_ArgString; } }
        public string[] Arguments { get { return m_Arguments; } }

        public CommandInfo(Mobile m, string com, string args, string[] arglist)
        {
            m_Mobile = m;
            m_Command = com;
            m_ArgString = args;
            m_Arguments = arglist;
        }

        public string GetString(int num)
        {
            if (m_Arguments.Length > num)
                return m_Arguments[num];

            return "";
        }
    }
}