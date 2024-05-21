using MockSurveyService.Models;

namespace MockSurveyService.Repositories
{
    public class MockSurveyRepo : ISurveyRepository
    {
        private readonly Dictionary<Guid, Survey> _surveys = [];
        private readonly List<FormSubmission> _formSubmissions = [];

        public Task AddSurveyAsync(Survey survey)
        {
            _surveys.Add(survey.Id, survey);
            return Task.CompletedTask;
        }

        public async Task<Survey?> GetSurveyByIdAsync(Guid id)
        {
            _surveys.TryGetValue(id, out var survey);
            return await Task.FromResult(survey);
        }

        public Task UpdateSurveyAsync(Survey survey)
        {
            _surveys[survey.Id] = survey;
            return Task.CompletedTask;
        }

        public Task AddFormSubmissionAsync(FormSubmission formSubmission)
        {
            _formSubmissions.Add(formSubmission);
            return Task.CompletedTask;
        }

        public async Task<List<FormSubmission>> GetFormSubmissionByIdAsync(Guid surveyId)
        {
            return [.. _formSubmissions.FindAll(x => x.SurveyId == surveyId)];
        }
    }
}
