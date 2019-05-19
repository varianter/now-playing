using System;
using System.Collections.Generic;
using System.Linq;

namespace Functions.Models.Orchestrator
{
    public class OrchestratorData
    {
        public List<MusicListener> MusicListeners { get; set; } = new List<MusicListener>();
        public int Interval { get; internal set; }
        public DateTime? LastActivity
        {
            get
            {
                return MusicListeners.Select(m => m.LastSeenActivity).Max();
            }
        }
    }
}