using System;
using System.Collections.Generic;
using System.Text;

namespace PlayListLib.Model
{
    public class Track
    {
        public string Performer { get; set; }
        public string Title { get; set; }
        public int TrackNumber { get; set; }
        public TrackIndex Index { get; set; }

        public override string ToString()
        {
            return $"TRACK {TrackNumber} AUDIO{Environment.NewLine}" +
                $"  PERFORMER \"{Performer}\"{Environment.NewLine}" +
                $"  TITLE \"{Title}\"{Environment.NewLine}" +
                $"  {Index}";
        }

    }

    public class TrackIndex
    {
        public int Index { get; set; }
        public TimeSpan StartTime { get; set; }
        public override string ToString()
        {
            var ms = (long)StartTime.TotalMilliseconds % 1000;
            var s = ((long)StartTime.TotalSeconds) % 60;
            var m = (long)StartTime.TotalMinutes;

            return $"INDEX {Index:D2} {m:d2}:{s:d2}:{ms / 10:d2}";
        }
    }
}
