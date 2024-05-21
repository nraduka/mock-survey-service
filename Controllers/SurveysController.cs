using MockSurveyService.DTOs;
using MockSurveyService.Services;
using Microsoft.AspNetCore.Mvc;

namespace MockSurveyService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SurveysController : ControllerBase
    {
        private readonly ISurveyService _surveySerivce;

        public SurveysController(ISurveyService surveySerivce)
        {
            _surveySerivce = surveySerivce;
        }

        [HttpPost("CreateSurvey")]
        public async Task<IActionResult> CreateSurveyAsync([FromBody] CreateSurveyDto createSurveyDto)
        {
            var result = await _surveySerivce.CreateSurveyAsync(createSurveyDto);
            return Ok(result);
        }

        [HttpGet("{surveyId}")]
        public async Task<IActionResult> GetSurveyAsync(Guid surveyId)
        {
            var result = await _surveySerivce.GetSurveyByIdAsync(surveyId);
            return Ok(result);
        }

        [HttpPost("{surveyId}/Activate")]
        public async Task<IActionResult> ActivateSurveyAsync(Guid surveyId)
        {
            await _surveySerivce.ActivateSurveyAsync(surveyId);
            return Ok();
        }

        [HttpPost("{surveyId}/CreateFormSubmission")]
        public async Task<IActionResult> CreateFormSubmissionAsync(Guid surveyId, [FromBody] CreateFormSubmissionDto createFormSubmissionDto)
        {
            await _surveySerivce.CreateFromSubmissionAsync(surveyId, createFormSubmissionDto);
            return Ok();
        }


        [HttpGet("{surveyId}/Answers")]
        public async Task<IActionResult> GetSurveyAnswersAsync(Guid surveyId)
        {
            var result = await _surveySerivce.GetSurveyAnswersByIdAsync(surveyId);
            return Ok(result);
        }
    }
}
