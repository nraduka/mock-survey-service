using MockSurveyService.DTOs;
using MockSurveyService.Events;
using MockSurveyService.Models;
using MockSurveyService.Repositories;

namespace MockSurveyService.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IEventPublisher _eventPublisher;

        public SurveyService(ISurveyRepository surveyRepository, IEventPublisher eventPublisher)
        {
            _surveyRepository = surveyRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task<Guid> CreateSurveyAsync(CreateSurveyDto createSurveyDto)
        {
            var survey = new Survey
            {
                Id = Guid.NewGuid(),
                Name = createSurveyDto.Name,
                Questions = createSurveyDto.Questions.ConvertAll(x =>
                    new Question
                    {
                        Id = Guid.NewGuid(),
                        Description = x.Description
                    }),
                IsActive = false,
                ActivationTime = null
            };

            await _surveyRepository.AddSurveyAsync(survey);
            return survey.Id;
        }

        public async Task ActivateSurveyAsync(Guid surveyId)
        {
            var survey = await GetSurveyAsync(surveyId);

            if (survey.IsActive)
                throw new Exception("Survey is already active!");

            survey.IsActive = true;
            survey.ActivationTime = DateTime.UtcNow;

            await _surveyRepository.UpdateSurveyAsync(survey);

            var surveyEvent = new SurveyActivatedEvent()
            {
                ActivateionTime = survey.ActivationTime.Value,
                SurveyId = survey.Id,
            };
            await _eventPublisher.PublishSurveyActivatedEventAsync(surveyEvent);
        }

        public async Task CreateFromSubmissionAsync(Guid surveyId, CreateFormSubmissionDto createFormSubmissionDto)
        {
            var survey = await GetSurveyAsync(surveyId);

            if (!survey.IsActive ||
                survey.ActivationTime == null ||
                survey.ActivationTime.Value.AddHours(24) < DateTime.UtcNow)
                throw new Exception("Survey is not active or has expired");

            var surveyQuestionIds = survey.Questions?.Select(x => x.Id).ToList();
            var unexpectedQuestions = createFormSubmissionDto.Answers.Keys.Except(surveyQuestionIds);
            if (unexpectedQuestions.Any())
                throw new Exception($"The following questions are not valid for this survey ({surveyId}): {string.Join(" , ", unexpectedQuestions)}");

            var formSubmission = new FormSubmission
            {
                Id = Guid.NewGuid(),
                SurveyId = survey.Id,
                Answers = createFormSubmissionDto.Answers
            };

            await _surveyRepository.AddFormSubmissionAsync(formSubmission);

            var submissionEvent = new FormSubmissionCreatedEvent()
            {
                FormSubmissionId = formSubmission.Id,
                SurveyId = formSubmission.SurveyId,
            };
            await _eventPublisher.PublishFormSubmissionCreatedEventAsync(submissionEvent);
        }

        public async Task<SurveyDto> GetSurveyByIdAsync(Guid surveyId)
        {
            var survey = await GetSurveyAsync(surveyId);

            var dto = new SurveyDto()
            {
                Name = survey.Name,
                SurveyId = survey.Id,
                Questions = survey.Questions.ConvertAll(x =>
                    new QuestionDto
                    {
                        Id = x.Id,
                        Question = x.Description
                    })
            };

            return dto;
        }

        public async Task<SurveyAnswersDto> GetSurveyAnswersByIdAsync(Guid surveyId)
        {
            var survey = await GetSurveyAsync(surveyId);
            var answers = await _surveyRepository.GetFormSubmissionByIdAsync(surveyId);
            var dto = new SurveyAnswersDto()
            {
                Name = survey.Name,
                SurveyId = survey.Id,
                Questions = survey.Questions.ConvertAll(x =>
                    new QuestionDto
                    {
                        Id = x.Id,
                        Question = x.Description
                    }),
                Answers = answers.ConvertAll(x => new AnswersDto { SubmissionId = x.Id, Answers = x.Answers })
            };

            return dto;
        }

        private async Task<Survey> GetSurveyAsync(Guid surveyId)
        {
            return await _surveyRepository.GetSurveyByIdAsync(surveyId)
                ?? throw new Exception("Survey not found!");
        }
    }
}
