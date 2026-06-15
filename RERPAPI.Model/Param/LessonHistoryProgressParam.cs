namespace RERPAPI.Model.Param
{
    public class LessonHistoryProgressParam
    {
        public int LessonID { get; set; } // id bài học
        public int MaxWatchedSecond { get; set; } // mốc time xem cao nhất
        public int LastWatchedSecond { get; set; }// thời điểm cuối cùng xem
        public int VideoDuration { get; set; }
    }
}