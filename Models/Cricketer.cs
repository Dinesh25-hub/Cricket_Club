namespace Cricket_Club.Models
{
    public class Cricketer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Team { get; set; }
        public int? MatchPlayed { get; set; }
        public int? TotalScore { get; set; }
        public int? Salary { get; set; }

    }
}
