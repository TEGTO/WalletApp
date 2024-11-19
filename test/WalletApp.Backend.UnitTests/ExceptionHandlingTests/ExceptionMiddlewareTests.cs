using EntityFramework.Exceptions.Common;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace ExceptionHandling.Tests
{
    [TestFixture]
    internal class ExceptionMiddlewareTests
    {
        private Mock<ILogger<ExceptionMiddleware>> loggerMock;
        private DefaultHttpContext httpContext;

        [SetUp]
        public void Setup()
        {
            loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
            httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
        }
        private ExceptionMiddleware CreateMiddleware(RequestDelegate next)
        {
            return new ExceptionMiddleware(next, loggerMock.Object);
        }
        [Test]
        public async Task InvokeAsync_ValidationException_StatusCodeBadRequest()
        {
            // Arrange
            var exception = new ValidationException("Validation error.");
            RequestDelegate next = context => throw exception;
            var exceptionMiddleware = CreateMiddleware(next);
            // Act
            await exceptionMiddleware.InvokeAsync(httpContext);
            // Assert
            Assert.That(httpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            loggerMock.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ), Times.Exactly(1));
        }
        [Test]
        public async Task InvokeAsync_UniqueConstraintException_StatusCodeConflict()
        {
            // Arrange
            var exception = new UniqueConstraintException("Constraint error.");
            RequestDelegate next = context => throw exception;
            var exceptionMiddleware = CreateMiddleware(next);
            // Act
            await exceptionMiddleware.InvokeAsync(httpContext);
            // Assert
            Assert.That(httpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.Conflict));
            loggerMock.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ), Times.Exactly(1));
        }
        [Test]
        public async Task InvokeAsync_InvalidDataException_StatusBadRequest()
        {
            // Arrange
            var exception = new InvalidDataException("Invalid data error.");
            RequestDelegate next = context => throw exception;
            var exceptionMiddleware = CreateMiddleware(next);
            // Act
            await exceptionMiddleware.InvokeAsync(httpContext);
            // Assert
            Assert.That(httpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            loggerMock.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ), Times.Exactly(1));
        }
        [Test]
        public async Task InvokeAsync_AuthorizationException_StatusCodeConflict()
        {
            // Arrange
            var exception = new AuthorizationException(new string[1] { "Authorization error." });
            RequestDelegate next = context => throw exception;
            var exceptionMiddleware = CreateMiddleware(next);
            // Act
            await exceptionMiddleware.InvokeAsync(httpContext);
            // Assert
            Assert.That(httpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            loggerMock.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ), Times.Exactly(1));
        }
        [Test]
        public async Task InvokeAsync_ValidationException_ResponseBodyContainsErrors()
        {
            // Arrange
            IEnumerable<ValidationFailure> errors = new List<ValidationFailure> { new ValidationFailure("Name", "Name is required.") };
            var exception = new ValidationException(errors);
            RequestDelegate next = context => throw exception;
            var exceptionMiddleware = CreateMiddleware(next);
            // Act
            await exceptionMiddleware.InvokeAsync(httpContext);
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = new StreamReader(httpContext.Response.Body).ReadToEnd();
            var expectedResponseBody = "400";
            // Assert
            StringAssert.Contains(expectedResponseBody, responseBody);
            loggerMock.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ), Times.Exactly(1));
        }
        [Test]
        public async Task InvokeAsync_GenericException_StatusCodeInternalServerError()
        {
            // Arrange
            var exception = new Exception("Internal Server Error.");
            RequestDelegate next = context => throw exception;
            var exceptionMiddleware = CreateMiddleware(next);
            // Act
            await exceptionMiddleware.InvokeAsync(httpContext);
            // Assert
            Assert.That(httpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
            loggerMock.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ), Times.Exactly(1));
        }
        [Test]
        public async Task InvokeAsync_GenericException_ResponseBodyContainsErrorMessage()
        {
            // Arrange
            var exception = new Exception("Internal Server Error.");
            RequestDelegate next = context => throw exception;
            var exceptionMiddleware = CreateMiddleware(next);
            // Act
            await exceptionMiddleware.InvokeAsync(httpContext);
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = new StreamReader(httpContext.Response.Body).ReadToEnd();
            var expectedResponseBody = "500";
            // Assert
            StringAssert.Contains(expectedResponseBody, responseBody);
            loggerMock.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ), Times.Exactly(1));
        }
        [Test]
        public async Task InvokeAsync_UnauthorizedAccessException_StatusCodeUnauthorized()
        {
            // Arrange
            var exception = new UnauthorizedAccessException("Invalid Authentication.");
            RequestDelegate next = context => throw exception;
            var exceptionMiddleware = CreateMiddleware(next);
            // Act
            await exceptionMiddleware.InvokeAsync(httpContext);
            // Assert
            Assert.That(httpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
            loggerMock.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ), Times.Exactly(1));
        }
        [Test]
        public async Task InvokeAsync_InvalidDataException_ResponseBodyContainsErrorMessage()
        {
            // Arrange
            var exception = new Exception("Invalid Data.");
            RequestDelegate next = context => throw exception;
            var exceptionMiddleware = CreateMiddleware(next);
            // Act
            await exceptionMiddleware.InvokeAsync(httpContext);
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = new StreamReader(httpContext.Response.Body).ReadToEnd();
            var expectedResponseBody = "500";
            // Assert
            StringAssert.Contains(expectedResponseBody, responseBody);
            loggerMock.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ), Times.Exactly(1));
        }
    }
}