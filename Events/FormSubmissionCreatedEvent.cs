namespace MockSurveyService.Events
{
    public class FormSubmissionCreatedEvent
    {
        public Guid FormSubmissionId { get; set; }
        public Guid SurveyId { get; set; }
    }
}
