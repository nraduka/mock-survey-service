namespace MockSurveyService.Events
{
    public class SurveyActivatedEvent
    {
        public Guid SurveyId { get; set; }
        public DateTime ActivateionTime { get; set; }
    }
}
