namespace MockSurveyService.Models
{
    public class FormSubmission
    {
        public Guid Id { get; set; }
        public Guid SurveyId { get; set; }
        public Dictionary<Guid, string> Answers { get; set; }
    }
}
