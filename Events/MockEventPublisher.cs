namespace MockSurveyService.Events
{
    public class MockEventPublisher : IEventPublisher
    {
        public Task PublishFormSubmissionCreatedEventAsync(FormSubmissionCreatedEvent formSubmissionEvent)
        {
            Console.WriteLine($"FormSubmissionCreated event publisher for FormSubmissionId ({formSubmissionEvent.FormSubmissionId}) and SurveyId ({formSubmissionEvent.SurveyId})");
            return Task.CompletedTask;
        }

        public Task PublishSurveyActivatedEventAsync(SurveyActivatedEvent surveyActivatedEvent)
        {
            Console.WriteLine($"SurveyActivated event published for SurveyId ({surveyActivatedEvent.SurveyId}) and ActivationTime {surveyActivatedEvent.ActivateionTime}");
            return Task.CompletedTask;
        }
    }
}
