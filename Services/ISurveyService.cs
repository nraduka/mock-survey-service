using MockSurveyService.DTOs;

namespace MockSurveyService.Services
{
    public interface ISurveyService
    {
        Task<Guid> CreateSurveyAsync(CreateSurveyDto createSurveyDto);
        Task<SurveyDto> GetSurveyByIdAsync(Guid surveyId);
        Task<SurveyAnswersDto> GetSurveyAnswersByIdAsync(Guid surveyId);
        Task ActivateSurveyAsync(Guid surveyId);
        Task CreateFromSubmissionAsync(Guid surveyId, CreateFormSubmissionDto createFormSubmissionDto);
    }
}
