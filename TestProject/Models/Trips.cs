namespace TestProject.Models
{
    public class Trips
    {
     //   public string Driver { get; set; }
        public string StartPosition { get; set; }
        public string EndPosition { get; set; }
        public DateTime DateAndTime { get; set; }
        public int Price { get; set; }
        public Rating Rating { get; set; }
        public int FreeSeats { get; set; }
    }
}
