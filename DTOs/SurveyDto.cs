namespace MockSurveyService.DTOs
{
    public class SurveyDto
    {
        public Guid SurveyId { get; set; }
        public string Name { get; set; }
        public List<QuestionDto> Questions { get; set; }
    }
}
