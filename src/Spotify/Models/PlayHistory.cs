using System;

namespace Spotify.Models
{
    public class PlayHistory
    {
        public PlayHistoryItem[] items { get; set; }
    }

    public class PlayHistoryItem
    {
        public Track track { get; set; }

        public DateTime played_at { get; set; }
    }
}