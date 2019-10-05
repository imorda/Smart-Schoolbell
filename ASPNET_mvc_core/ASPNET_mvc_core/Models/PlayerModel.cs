using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNET_mvc_core.Models
{
    public class PlayerModel
    {
        private static List<(TimeSpan, TimeSpan)> LessonStart;
        private static List<(TimeSpan, TimeSpan)> LessonEnd;
        private static List<(TimeSpan, TimeSpan)> Announcement;
        private static List<(string, TimeSpan)> Playlist;



        private static TimeSpan TrackEndPoint = new TimeSpan();

        public static (string, TimeSpan) current
        {
            set
            {
                current = value;
            }
            get
            {
                if (current.Item1 != null)
                    return current;
                else
                {
                    (string, TimeSpan) x = NextSource;
                    return (x.Item1, x.Item2);
                }
            }
        }

        private static string NextEvent
        {
            get
            {
                List<(TimeSpan, TimeSpan)> ClosestEventsList = new List<(TimeSpan, TimeSpan)>();
                ClosestEventsList.Append(GetClosestEvent(Announcement));
                ClosestEventsList.Append(GetClosestEvent(LessonStart));
                ClosestEventsList.Append(GetClosestEvent(LessonEnd));

                if ((GetMinSpan(ClosestEventsList).Item1 - DateTime.Now.TimeOfDay > TrackEndPoint - DateTime.Now.TimeOfDay) || TrackEndPoint == new TimeSpan())
                    return "track";
                else
                    if (ClosestEventsList.IndexOf(GetMinSpan(ClosestEventsList)) == 0)
                    return "announcement";
                else if (ClosestEventsList.IndexOf(GetMinSpan(ClosestEventsList)) == 1)
                    return "lesStart";
                else
                    return "lesEnd";
            }
        }

        private static (TimeSpan, TimeSpan) GetMinSpan(List<(TimeSpan, TimeSpan)> inputValue)
        {
            List<TimeSpan> result = new List<TimeSpan>();
            List<TimeSpan> result2 = new List<TimeSpan>();
            foreach ((TimeSpan, TimeSpan) i in inputValue)
            {
                if (i.Item1 != new TimeSpan())
                {
                    result.Append(i.Item1);
                }
                result2.Append(i.Item1);
            }
            return inputValue[result2.IndexOf(result.Min())];
        }
        private static (TimeSpan, TimeSpan) GetClosestEvent(List<(TimeSpan, TimeSpan)> inputList)
        {
            foreach ((TimeSpan, TimeSpan) i in inputList)
                if (i.Item1 > DateTime.Now.TimeOfDay)
                    return i;
            return (new TimeSpan(), new TimeSpan());
        }
        public static int getDelayMS()
        {
            List<(TimeSpan, TimeSpan)> ClosestEventsList = new List<(TimeSpan, TimeSpan)>();
            ClosestEventsList.Append(GetClosestEvent(Announcement));
            ClosestEventsList.Append(GetClosestEvent(LessonStart));
            ClosestEventsList.Append(GetClosestEvent(LessonEnd));

            if (GetMinSpan(ClosestEventsList).Item1 - DateTime.Now.TimeOfDay > TrackEndPoint - DateTime.Now.TimeOfDay)
                return (int)((TrackEndPoint - DateTime.Now.TimeOfDay).TotalMilliseconds);
            else
            {
                return (int)((GetMinSpan(ClosestEventsList).Item1 - DateTime.Now.TimeOfDay).TotalMilliseconds);
            }
        }
        private static void SetTrackEndPoint(string cause)
        {
            switch (cause)
            {
                case ("track"):
                    TrackEndPoint = DateTime.Now.TimeOfDay + current.Item2;
                    break;
                case ("announcement"):
                    TrackEndPoint = DateTime.Now.TimeOfDay + Announcement[Announcement.IndexOf(GetClosestEvent(Announcement))].Item2;
                    break;
                case ("lesStart"):
                    TrackEndPoint = GetClosestEvent(LessonEnd).Item1 + TimeSpan.FromMilliseconds(500);
                    break;
                case ("lesEnd"):
                    TrackEndPoint = DateTime.Now.TimeOfDay + LessonEnd[LessonEnd.IndexOf(GetClosestEvent(LessonEnd))].Item2;
                    break;
                default:
                    if (GetClosestEvent(LessonEnd).Item1 > GetClosestEvent(LessonStart).Item1)
                        TrackEndPoint = GetClosestEvent(LessonEnd).Item1 + TimeSpan.FromMilliseconds(500);
                    else
                    {
                        List<(TimeSpan, TimeSpan)> ClosestEventsList = new List<(TimeSpan, TimeSpan)>();
                        ClosestEventsList.Append(GetClosestEvent(Announcement));
                        ClosestEventsList.Append(GetClosestEvent(LessonStart));
                        ClosestEventsList.Append(GetClosestEvent(LessonEnd));
                        TrackEndPoint = GetMinSpan(ClosestEventsList).Item1 + TimeSpan.FromMilliseconds(500);
                    }
                    break;
            }
        }

        private static (string, TimeSpan) GetEventName(string cause)
        {
            switch (cause)
            {
                case ("announcement"):
                    return (Announcement.IndexOf(GetClosestEvent(Announcement)).ToString() + ".mp3", GetClosestEvent(Announcement).Item2);
                case ("lesStart"):
                    return (LessonStart.IndexOf(GetClosestEvent(LessonStart)).ToString() + ".mp3", GetClosestEvent(LessonStart).Item2);
                default:
                    return (LessonEnd.IndexOf(GetClosestEvent(LessonEnd)).ToString() + ".mp3", GetClosestEvent(LessonEnd).Item2);
            }
        }

        public static (string, TimeSpan) NextSource
        {
            get
            {
                switch (NextEvent)
                {
                    case ("track"):

                        if (Playlist == null)
                        {
                            Playlist = new List<(string, TimeSpan)>();
                            SetTrackEndPoint("none");
                            return ("", new TimeSpan());
                        }
                        if (Playlist.Count == 0)
                        {
                            SetTrackEndPoint("none");
                            return ("", new TimeSpan());
                        }
                        try
                        {
                            current = Playlist[0];
                            Playlist.RemoveAt(0);
                            SetTrackEndPoint("track");
                            return ("/playlist/" + current.Item1, current.Item2);
                        }
                        catch
                        {
                            SetTrackEndPoint("none");
                            return ("", new TimeSpan());
                        }
                    case ("announcement"):
                        SetTrackEndPoint("announcement");
                        var x = GetEventName("announcement");
                        return ("/announcements/" + x.Item1, x.Item2);
                    case ("lesStart"):
                        SetTrackEndPoint("lesStart");
                        var y = GetEventName("lesStart");
                        return ("/lesStart/" + y.Item1, y.Item2);
                    default:
                        SetTrackEndPoint("lesEnd");
                        var z = GetEventName("lesEnd");
                        return ("/lesEnd/" + z.Item1, z.Item2);
                }
            }
            set
            {
                if (Playlist == null)
                    Playlist = new List<(string, TimeSpan)>();
                Playlist.Add(value);
            }
        }
    }
}
