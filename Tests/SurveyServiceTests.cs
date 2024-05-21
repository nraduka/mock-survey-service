using MockSurveyService.DTOs;
using MockSurveyService.Events;
using MockSurveyService.Models;
using MockSurveyService.Repositories;
using MockSurveyService.Services;
using Moq;
using System.Text.Json;
using Xunit;

namespace MockSurveyService.Tests
{
    public class SurveyServiceTests
    {
        private readonly SurveyService _surveyService;
        private readonly Mock<ISurveyRepository> _mockSurveyRepo;
        private readonly Mock<IEventPublisher> _mockEventPublisher;
        private readonly Guid _surveyId;

        public SurveyServiceTests()
        {
            _mockSurveyRepo = new Mock<ISurveyRepository>();
            _mockEventPublisher = new Mock<IEventPublisher>();
            _surveyService = new SurveyService(_mockSurveyRepo.Object, _mockEventPublisher.Object);
            _surveyId = Guid.NewGuid();
        }

        [Fact]
        public async Task CreateSurvey_ShouldReturnGuid()
        {
            // Arrange
            var createSurveyDto = new CreateSurveyDto
            {
                Name = "Test Survey",
                Questions =
                    [
                        new CreateQuestionDto { Description = "Question 1" }
                    ]
            };

            // Act
            var result = await _surveyService.CreateSurveyAsync(createSurveyDto);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
            _mockSurveyRepo.Verify(repo => repo.AddSurveyAsync(It.IsAny<Survey>()), Times.Once);
        }

        [Fact]
        public async Task ActivateSurvey_ShouldSetSurveyAsActive_AndPublishEvent()
        {
            // Arrange
            var survey = new Survey { Id = _surveyId, IsActive = false };
            _mockSurveyRepo.Setup(repo => repo.GetSurveyByIdAsync(_surveyId)).ReturnsAsync(survey);

            // Act
            await _surveyService.ActivateSurveyAsync(_surveyId);

            // Assert
            Assert.True(survey.IsActive);
            Assert.NotNull(survey.ActivationTime);
            _mockSurveyRepo.Verify(repo => repo.UpdateSurveyAsync(survey), Times.Once);
            _mockEventPublisher.Verify(publisher => publisher.PublishSurveyActivatedEventAsync(It.IsAny<SurveyActivatedEvent>()), Times.Once);
        }

        [Fact]
        public async Task ActivateSurvey_ShouldThrowException_IfSurveyNotExist()
        {
            // Arrange
            var surveyId = Guid.NewGuid();
            Survey survey = null;
            _mockSurveyRepo.Setup(repo => repo.GetSurveyByIdAsync(surveyId)).ReturnsAsync(survey);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _surveyService.ActivateSurveyAsync(surveyId));
        }

        [Fact]
        public async Task ActivateSurvey_ShouldThrowException_IfSurveyAlreadyActive()
        {
            // Arrange
            var survey = new Survey { Id = _surveyId, IsActive = true };
            _mockSurveyRepo.Setup(repo => repo.GetSurveyByIdAsync(_surveyId)).ReturnsAsync(survey);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _surveyService.ActivateSurveyAsync(_surveyId));
        }

        [Fact]
        public async Task CreateFormSubmission_ShouldThrowException_IfSurveyNotExist()
        {
            // Arrange
            Survey survey = null;
            _mockSurveyRepo.Setup(repo => repo.GetSurveyByIdAsync(_surveyId)).ReturnsAsync(survey);

            var formSubmissionDto = new CreateFormSubmissionDto
            {
                Answers = new Dictionary<Guid, string> { { Guid.NewGuid(), "Answer 1" } }
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _surveyService.CreateFromSubmissionAsync(_surveyId, formSubmissionDto));
        }

        [Fact]
        public async Task CreateFormSubmission_ShouldThrowException_IfSurveyIsExpired()
        {
            // Arrange
            var survey = new Survey
            {
                Id = _surveyId,
                IsActive = true,
                ActivationTime = DateTime.UtcNow.AddHours(-25)
            };
            _mockSurveyRepo.Setup(repo => repo.GetSurveyByIdAsync(_surveyId)).ReturnsAsync(survey);

            var formSubmissionDto = new CreateFormSubmissionDto
            {
                Answers = new Dictionary<Guid, string> { { Guid.NewGuid(), "Answer 1" } }
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _surveyService.CreateFromSubmissionAsync(_surveyId, formSubmissionDto));
        }

        [Fact]
        public async Task CreateFormSubmission_ShouldThrowException_IfQuestionNotExist()
        {
            // Arrange
            var survey = new Survey
            {
                Id = _surveyId,
                IsActive = true,
                ActivationTime = DateTime.UtcNow,
                Name = "Survey Name",
                Questions = [new Question() { Id = Guid.NewGuid(), Description = "Some question" }]
            };
            _mockSurveyRepo.Setup(repo => repo.GetSurveyByIdAsync(_surveyId)).ReturnsAsync(survey);

            var formSubmissionDto = new CreateFormSubmissionDto
            {
                Answers = new Dictionary<Guid, string> { { Guid.NewGuid(), "Answer 1" } }
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _surveyService.CreateFromSubmissionAsync(_surveyId, formSubmissionDto));
        }

        [Fact]
        public async Task CreateFormSubmission_ShouldPublishEvent()
        {
            // Arrange
            var questionId = Guid.NewGuid();

            var survey = new Survey
            {
                Id = _surveyId,
                IsActive = true,
                ActivationTime = DateTime.UtcNow,
                Name = "Survey Name",
                Questions = [new Question() { Id = questionId, Description = "Some question" }]
            };
            _mockSurveyRepo.Setup(repo => repo.GetSurveyByIdAsync(_surveyId)).ReturnsAsync(survey);

            var formSubmissionDto = new CreateFormSubmissionDto
            {
                Answers = new Dictionary<Guid, string> { { questionId, "Answer 1" } }
            };

            // Act
            await _surveyService.CreateFromSubmissionAsync(_surveyId, formSubmissionDto);

            // Assert
            _mockSurveyRepo.Verify(repo => repo.AddFormSubmissionAsync(It.IsAny<FormSubmission>()), Times.Once);
            _mockEventPublisher.Verify(publisher => publisher.PublishFormSubmissionCreatedEventAsync(It.IsAny<FormSubmissionCreatedEvent>()), Times.Once);
        }

        [Fact]
        public async Task GetSurveyByIdAsync_ShouldReturnDto()
        {
            // Arrange
            var survey = new Survey
            {
                Id = _surveyId,
                IsActive = true,
                ActivationTime = DateTime.UtcNow,
                Name = "Survey Name",
                Questions = [new Question() { Id = Guid.NewGuid(), Description = "Some question" }]
            };

            var expectedDto = new SurveyDto()
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

            _mockSurveyRepo.Setup(repo => repo.GetSurveyByIdAsync(_surveyId)).ReturnsAsync(survey);

            // Act
            var actualDto = await _surveyService.GetSurveyByIdAsync(survey.Id);

            // Assert
            Assert.Equal(JsonSerializer.Serialize(expectedDto), JsonSerializer.Serialize(actualDto));
        }

        [Fact]
        public async Task GetSurveyByIdAsync_ShouldThrowException_IfSurveyNotExist()
        {
            // Arrange
            Survey survey = null;
            _mockSurveyRepo.Setup(repo => repo.GetSurveyByIdAsync(_surveyId)).ReturnsAsync(survey);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _surveyService.GetSurveyByIdAsync(_surveyId));
        }

        [Fact]
        public async Task GetSurveyAnswersByIdAsync_ShouldThrowException_IfSurveyNotExist()
        {
            // Arrange
            Survey survey = null;
            _mockSurveyRepo.Setup(repo => repo.GetSurveyByIdAsync(_surveyId)).ReturnsAsync(survey);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _surveyService.GetSurveyAnswersByIdAsync(_surveyId));
        }

        [Fact]
        public async Task GetSurveyAnswersByIdAsync_ShouldReturnDto()
        {
            // Arrange
            var survey = new Survey
            {
                Id = _surveyId,
                IsActive = true,
                ActivationTime = DateTime.UtcNow,
                Name = "Survey Name",
                Questions = [new Question() { Id = Guid.NewGuid(), Description = "Some question" }]
            };

            var formSubmission = new List<FormSubmission>()
            {
                new() {
                    Id = Guid.NewGuid(),
                    SurveyId = _surveyId,
                    Answers = []
                }
            };
            formSubmission[0].Answers.Add(Guid.NewGuid(), "Some answer");

            var expectedDto = new SurveyAnswersDto()
            {
                Name = survey.Name,
                SurveyId = survey.Id,
                Questions = survey.Questions.ConvertAll(x =>
                    new QuestionDto
                    {
                        Id = x.Id,
                        Question = x.Description
                    }),
                Answers = formSubmission.ConvertAll(x => new AnswersDto { SubmissionId = x.Id, Answers = x.Answers })
            };
            _mockSurveyRepo.Setup(repo => repo.GetSurveyByIdAsync(_surveyId)).ReturnsAsync(survey);
            _mockSurveyRepo.Setup(repo => repo.GetFormSubmissionByIdAsync(_surveyId)).ReturnsAsync(formSubmission);

            // Act
            var actualDto = await _surveyService.GetSurveyAnswersByIdAsync(_surveyId);

            //Assert
            Assert.Equal(JsonSerializer.Serialize(expectedDto), JsonSerializer.Serialize(actualDto));
        }
    }
}