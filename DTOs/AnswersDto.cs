using System.ComponentModel.DataAnnotations;

namespace MockSurveyService.DTOs
{
    public class AnswersDto
    {
        public Guid SubmissionId { get; set; }
        public Dictionary<Guid, string> Answers { get; set; }
    }
}
