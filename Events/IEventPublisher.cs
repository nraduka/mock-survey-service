namespace MockSurveyService.Events
{
    public interface IEventPublisher
    {
        Task PublishSurveyActivatedEventAsync(SurveyActivatedEvent surveyActivatedEvent);
        Task PublishFormSubmissionCreatedEventAsync(FormSubmissionCreatedEvent formSubmissionCreatedEvent);
    }
}
