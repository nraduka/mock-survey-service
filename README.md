## Mock Survey Service

The Survey Service is a web service for managing surveys and their submissions. It allows users to create surveys, activate them, and submit responses to these surveys. The service ensures that surveys are only active for 24 hours once activated. It follows an event-driven architecture to notify external services about key events such as survey activations and form submissions.

## Features

- **Create Survey**: Create a new survey with a list of questions.
- **Activate Survey**: Activate a survey to allow submissions for 24 hours.
- **Get Survey**: Get survey information to view questions.
- **Create Form Submission**: Submit answers to an active survey.
- **Get Survey Answers**: Retrieve all answers for a specific survey.
- **Event Publishing**: Notify external systems when a survey is activated or a form submission is created.

## API Endpoints

Can be viewed using Swagger by running the API and going to the [locahost swagger page.](http://localhost:5221/swagger/index.html)

## Development Setup

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Visual Studio](https://visualstudio.microsoft.com/downloads/) or any other C# IDE

### Running the Service
1. Clone the repository.
2. Navigate to the project directory.
3. Run the service using the command:
  `dotnet run`

### Running Tests
1. Navigate to the project directory.
2. Run the tests using the command:
  `dotnet test`
