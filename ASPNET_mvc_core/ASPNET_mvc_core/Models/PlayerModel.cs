using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNET_mvc_core.Models
{
    public static class PlayerModel
    {
        public static List<(TimeSpan, TimeSpan)> LessonStart = new List<(TimeSpan, TimeSpan)>();
        public static List<(TimeSpan, TimeSpan)> LessonEnd = new List<(TimeSpan, TimeSpan)>();
        public static List<(TimeSpan, TimeSpan)> Announcement = new List<(TimeSpan, TimeSpan)>();
        public static List<(string, TimeSpan)> Playlist = new List<(string, TimeSpan)>();

        public static float EventVolume = 1;
        public static float TrackVolume = 0.5f;

        private static bool nil = true;
        private static bool noEvents = false;
        private static (string, TimeSpan,float) curr;
        private static string CurrEvent = "none";
        private static (string, TimeSpan, float, TimeSpan) CurrTrack; //url, duration, volume, time(start point)

        private static TimeSpan TrackEndPoint = new TimeSpan();


        static PlayerModel()
        {
            
            LessonEnd.Add((TimeSpan.FromSeconds(21 * 60 * 60 + 0* 60 + 0), TimeSpan.FromSeconds(6)));
            LessonStart.Add((TimeSpan.FromSeconds(21 * 60 * 60 + 0 * 60 + 15), TimeSpan.FromSeconds(6)));
            LessonEnd.Add((TimeSpan.FromSeconds(21 * 60*60+ 0 * 60+30), TimeSpan.FromSeconds(6)));
            Announcement.Add((TimeSpan.FromSeconds(21 * 60*60 + 0 * 60 + 45), TimeSpan.FromSeconds(6)));
            Announcement.Add((TimeSpan.FromSeconds(21 * 60*60 + 0 * 60 + 55), TimeSpan.FromSeconds(6)));
            Playlist.Add(("15.mp3", TimeSpan.FromSeconds(30)));
            Playlist.Add(("16.mp3", TimeSpan.FromSeconds(30)));
            Playlist.Add(("17.mp3", TimeSpan.FromSeconds(30))); //ТЕСТОВОЕ РАСПИСАНИЕ*/
    }

        public static void SetCurrent((string, TimeSpan,float) value)
        {
            nil = false;
            curr = value;
        }

        public static (string, TimeSpan,float,TimeSpan) GetCurrent()
        {

            if (!nil)
                if (CurrEvent == "track")
                    return (curr.Item1, curr.Item2, curr.Item3, CurrTrack.Item2 - (TrackEndPoint - DateTime.Now.TimeOfDay));
                else
                    return (curr.Item1, curr.Item2, curr.Item3,new TimeSpan());
            else
            {
                return (NextSource);
            }
            
        }

        private static string NextEvent
        {
            get
            {
                List<(TimeSpan, TimeSpan)> ClosestEventsList = new List<(TimeSpan, TimeSpan)>();
                ClosestEventsList.Add(GetClosestEvent(Announcement, DateTime.Now.TimeOfDay - TimeSpan.FromSeconds(1)));
                ClosestEventsList.Add(GetClosestEvent(LessonStart, DateTime.Now.TimeOfDay - TimeSpan.FromSeconds(1)));
                ClosestEventsList.Add(GetClosestEvent(LessonEnd, DateTime.Now.TimeOfDay - TimeSpan.FromSeconds(1)));
                GetMinSpan(ClosestEventsList);
                if (noEvents)
                    return "none";
                if (GetClosestEvent(LessonEnd, DateTime.Now.TimeOfDay).Item1 < GetClosestEvent(LessonStart, DateTime.Now.TimeOfDay).Item1)
                    return "lesEnd2"; //если программа включена во время урока
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
                    result.Add(i.Item1);
                }
                result2.Add(i.Item1);
            }
            try
            {
                noEvents = false;
                return inputValue[result2.IndexOf(result.Min())];
            }
            catch
            {
                noEvents = true;
                return (new TimeSpan(), new TimeSpan());
            }
            }
        private static (TimeSpan, TimeSpan) GetClosestEvent(List<(TimeSpan, TimeSpan)> inputList, TimeSpan time)
        {
            foreach ((TimeSpan, TimeSpan) i in inputList)
                if (i.Item1 > time)
                    return i;
            return (new TimeSpan(), new TimeSpan());
        }
        public static int getDelayMS()
        {
            List<(TimeSpan, TimeSpan)> ClosestEventsList = new List<(TimeSpan, TimeSpan)>();
            ClosestEventsList.Add(GetClosestEvent(Announcement, DateTime.Now.TimeOfDay));
            ClosestEventsList.Add(GetClosestEvent(LessonStart, DateTime.Now.TimeOfDay));
            ClosestEventsList.Add(GetClosestEvent(LessonEnd, DateTime.Now.TimeOfDay));
            GetMinSpan(ClosestEventsList);
            if ((GetMinSpan(ClosestEventsList).Item1 - DateTime.Now.TimeOfDay > TrackEndPoint - DateTime.Now.TimeOfDay) || noEvents)
            {
                return (int)((TrackEndPoint - DateTime.Now.TimeOfDay).TotalMilliseconds);
            }
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
                    TrackEndPoint = DateTime.Now.TimeOfDay +  GetCurrent().Item2;
                    break;
                case ("track2"):
                    TrackEndPoint = DateTime.Now.TimeOfDay + (CurrTrack.Item2 - CurrTrack.Item4);
                    break;
                case ("announcement"):
                    TrackEndPoint = DateTime.Now.TimeOfDay + Announcement[Announcement.IndexOf(GetClosestEvent(Announcement, DateTime.Now.TimeOfDay - TimeSpan.FromSeconds(1)))].Item2;
                    break;
                case ("lesStart"):
                    TrackEndPoint = GetClosestEvent(LessonEnd, DateTime.Now.TimeOfDay + TimeSpan.FromMilliseconds(500)).Item1 + TimeSpan.FromMilliseconds(500);
                    break;
                case ("lesEnd"):
                    TrackEndPoint = DateTime.Now.TimeOfDay + LessonEnd[LessonEnd.IndexOf(GetClosestEvent(LessonEnd, DateTime.Now.TimeOfDay - TimeSpan.FromSeconds(1)))].Item2;
                    break;
                case ("lesEnd2"):
                    TrackEndPoint = GetClosestEvent(LessonEnd, DateTime.Now.TimeOfDay + TimeSpan.FromMilliseconds(500)).Item1 + TimeSpan.FromMilliseconds(500);
                    break;
                default:
                    if (noEvents)
                        TrackEndPoint = DateTime.Now.TimeOfDay + TimeSpan.FromSeconds(5);
                    else
                    {
                        List<(TimeSpan, TimeSpan)> ClosestEventsList = new List<(TimeSpan, TimeSpan)>();
                        ClosestEventsList.Add(GetClosestEvent(Announcement, DateTime.Now.TimeOfDay));
                        ClosestEventsList.Add(GetClosestEvent(LessonStart, DateTime.Now.TimeOfDay));
                        ClosestEventsList.Add(GetClosestEvent(LessonEnd, DateTime.Now.TimeOfDay));
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
                    return ((Announcement.IndexOf(GetClosestEvent(Announcement, DateTime.Now.TimeOfDay - TimeSpan.FromSeconds(1)))+1).ToString() + ".mp3", GetClosestEvent(Announcement, DateTime.Now.TimeOfDay - TimeSpan.FromSeconds(1)).Item2);
                case ("lesStart"):
                    return ((LessonStart.IndexOf(GetClosestEvent(LessonStart, DateTime.Now.TimeOfDay - TimeSpan.FromSeconds(1)))+1).ToString() + ".mp3", GetClosestEvent(LessonStart, DateTime.Now.TimeOfDay - TimeSpan.FromSeconds(1)).Item2);
                default:
                    return ((LessonEnd.IndexOf(GetClosestEvent(LessonEnd, DateTime.Now.TimeOfDay - TimeSpan.FromSeconds(1)))+1).ToString() + ".mp3", GetClosestEvent(LessonEnd, DateTime.Now.TimeOfDay - TimeSpan.FromSeconds(1)).Item2);
            }
        }

        public static (string, TimeSpan,float,TimeSpan) NextSource //url,duration,volume,time (start point)
        {
            get
            {
                if (CurrEvent == "track")
                    CurrTrack.Item4 = CurrTrack.Item2 - (TrackEndPoint - DateTime.Now.TimeOfDay);
            switch (NextEvent)
                {
                    case ("none"):
                        SetTrackEndPoint("none");
                        SetCurrent(("", TimeSpan.FromMinutes(1),0));
                        CurrEvent = "none";
                        return ("", new TimeSpan(), 0, new TimeSpan());
                    case ("track"):

                        if (Playlist == null)
                        {
                            Playlist = new List<(string, TimeSpan)>();
                            SetTrackEndPoint("none");
                            SetCurrent(("", TimeSpan.FromMinutes(1),0));
                            CurrEvent = "none";
                            return ("", new TimeSpan(),0, new TimeSpan());
                        }
                        if (Playlist.Count == 0)
                        {
                            SetTrackEndPoint("none");
                            SetCurrent(("", TimeSpan.FromMinutes(1),0));
                            CurrEvent = "none";
                            return ("", new TimeSpan(),0, new TimeSpan());
                        }
                        try
                        {
                            if ((CurrTrack.Item2 - CurrTrack.Item4).TotalSeconds < 15)
                            {
                                SetCurrent(("/playlist/" + Playlist[0].Item1, Playlist[0].Item2, TrackVolume));
                                Playlist.RemoveAt(0);
                                SetTrackEndPoint("track");
                                CurrTrack = (GetCurrent().Item1, GetCurrent().Item2, GetCurrent().Item3, new TimeSpan());
                                CurrEvent = "track";
                                return (GetCurrent());
                            }
                            else
                            {
                                SetCurrent((CurrTrack.Item1, CurrTrack.Item2, CurrTrack.Item3));
                                SetTrackEndPoint("track2");
                                CurrEvent = "track";
                                return (CurrTrack);
                            }
                        }
                        catch
                        {
                            SetTrackEndPoint("none");
                            SetCurrent(("", TimeSpan.FromMinutes(1),0));
                            CurrEvent = "none";
                            return ("", new TimeSpan(),0, new TimeSpan());
                        }
                    case ("announcement"):
                        SetTrackEndPoint("announcement");
                        var x = GetEventName("announcement");
                        SetCurrent(("/announcement/" + x.Item1, x.Item2,EventVolume));
                        CurrEvent = "announcement";
                        return (GetCurrent());
                    case ("lesStart"):
                        SetTrackEndPoint("lesStart");
                        var y = GetEventName("lesStart");
                        //SetCurrent(("/lesStart/" + y.Item1, y.Item2));
                        //return (GetCurrent());
                        CurrEvent = "lesStart";
                        SetCurrent(("", TimeSpan.FromMinutes(1),0));
                        return (("/lesStart/" + y.Item1, y.Item2, EventVolume, new TimeSpan()));
                    case ("lesEnd2"):
                        SetTrackEndPoint("lesEnd2");
                        SetCurrent(("", TimeSpan.FromMinutes(1),0));
                        CurrEvent = "none";
                        return ("", new TimeSpan(),0, new TimeSpan());
                    default:
                        SetTrackEndPoint("lesEnd");
                        var z = GetEventName("lesEnd");
                        SetCurrent(("/lesEnd/" + z.Item1, z.Item2, EventVolume));
                        CurrEvent = "lesEnd";
                        return (GetCurrent());
                }
            }
        }
    }
}
