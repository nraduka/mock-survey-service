using MockSurveyService.Models;

namespace MockSurveyService.Repositories
{
    public interface ISurveyRepository
    {
        Task AddSurveyAsync(Survey survey);
        Task<Survey?> GetSurveyByIdAsync(Guid surveyId);
        Task UpdateSurveyAsync(Survey survey);
        Task AddFormSubmissionAsync(FormSubmission formSubmission);
        Task<List<FormSubmission>> GetFormSubmissionByIdAsync(Guid surveyId);
    }
}
