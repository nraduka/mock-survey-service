namespace MockSurveyService.DTOs
{
    public class SurveyAnswersDto
    {
        public Guid SurveyId { get; set; }
        public string Name { get; set; }
        public List<QuestionDto> Questions { get; set; }
        public List<AnswersDto> Answers { get; set; }
    }
}
