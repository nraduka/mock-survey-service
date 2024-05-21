using System.ComponentModel.DataAnnotations;

namespace MockSurveyService.DTOs
{
    public class CreateQuestionDto
    {
        [Required]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Question description must be between 5 and 200 characters.")]
        public string Description { get; set; }
    }
}
