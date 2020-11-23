using SQLite;

namespace GolfScorekeeper
{
    public class CurrentGame
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string CourseName { get; set; }
        public string Scorecard { get; set; }
        public int CurrentHole { get; set; }
        public int FurthestHole { get; set; }
        public int CurrentCourseScore { get; set; }
        public int Strokes { get; set; }
    }
}