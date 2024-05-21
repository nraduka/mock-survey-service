using System.ComponentModel.DataAnnotations;

namespace MockSurveyService.DTOs
{
    public class CreateSurveyDto
    {
        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Survey name must be between 5 and 100 characters.")]
        public string Name { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "There must be at least one question.")]
        public List<CreateQuestionDto> Questions { get; set; }
    }
}
