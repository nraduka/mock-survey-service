namespace MockSurveyService.Models
{
    public class Survey
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Question> Questions { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ActivationTime { get; set; }
    }
}