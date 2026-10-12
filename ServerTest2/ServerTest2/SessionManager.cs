using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    static class SessionManager
    {
        static Dictionary<int, Session> _sessions = new Dictionary<int, Session>();

        static readonly object _lock = new object();

        public static void AddSession(int id, Session session)
        {
            lock (_lock)
            {
                _sessions[id] = session;
            }
        }
        public static Session GetSession(int id) 
        {
            lock (_lock)
            {
                return _sessions[id];
            }
        }
        public static void RemoveSession(int id) 
        {
            lock (_lock)
            {
                _sessions.Remove(id);
            }
        }

        public static async Task BroadcastAsync(string message)
        {
            IReadOnlyCollection<Session> SessionsCapture;

            lock (_lock)
            {
                SessionsCapture = _sessions.Values.ToList();
            }

            foreach(Session session in SessionsCapture)
            {
                await session.SendAsync(message);
            }
        }
    }
}
