using System.ComponentModel.DataAnnotations;

namespace MockSurveyService.DTOs
{
    public class CreateFormSubmissionDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "There must be at least one answer.")]
        public Dictionary<Guid, string> Answers { get; set; } // (KV) => QuestionId, QuestionAnswer
    }
}
