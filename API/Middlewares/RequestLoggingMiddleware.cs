using Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Models.AppConfig;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace APIs.Middlewares.Logging
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly AppConfig _appConfig;
        private readonly ICryptographyService _cryptographyService;
        public RequestLoggingMiddleware(RequestDelegate next, AppConfig appConfig, ILogger<RequestLoggingMiddleware> logger, ICryptographyService cryptographyService)
        {
            _next = next;
            _appConfig = appConfig;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _logger = logger;
            _cryptographyService = cryptographyService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (string.IsNullOrEmpty(context.Request.Path.Value) || !context.Request.Path.Value.Contains("/api/"))
            {
                await _next(context);
                return;
            }
            Guid CorrelationId = Guid.NewGuid();

            await LogRequest(context, CorrelationId);
            await LogResponse(context, CorrelationId);

        }
        private async Task LogRequest(HttpContext context, Guid CorrelationId)
        {
            string Body = string.Empty;
            string messageTemplate = "{Method}" +
                                    "{Type}" +
                                      "{Uri}" +
                                      "{Header}" +
                                      "{Body}" +
                                      "{CorrelationId}";
            if (true)
            {
                context.Request.EnableBuffering();
                await using var requestStream = _recyclableMemoryStreamManager.GetStream();
                await context.Request.Body.CopyToAsync(requestStream);

                Body = ReadStreamInChunks(requestStream);

            }
            if (true)
                _logger.LogInformation(messageTemplate, context.Request.Method, "Request", context.Request.Path, context.Response.Headers, Body, CorrelationId);

            context.Request.Body.Position = 0;
        }
        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);
            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;
            do
            {
                readChunkLength = reader.ReadBlock(readChunk,
                                                   0,
                                                   readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);
            return textWriter.ToString();
        }
        private async Task LogResponse(HttpContext context, Guid CorrelationId)
        {
            var originalBodyStream = context.Response.Body;

            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;
            DateTime SDate = DateTime.Now;
            Exception ResponseException = null;
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                ResponseException = ex;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(ex.Message, Encoding.UTF8);
            }
            finally
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                string ResponseMessageTemplate = "{Method}" +
                                        "{Type}" +
                                      "{StatusCode}" +
                                      "{Uri}" +
                                      "{Header}" +
                                      "{Body}" +
                                      "{CorrelationId}" +
                                      "{TimeElapsed}";
                if (ResponseException == null)
                    _logger.LogInformation(ResponseMessageTemplate, context.Request.Method, "Response", context.Response.StatusCode, context.Request.Path, context.Response.Headers, text, CorrelationId, DateTime.Now.Subtract(SDate).TotalMilliseconds);
                else
                    _logger.LogError($"{ResponseMessageTemplate} {{Exception}}", context.Request.Method, "Response", context.Response.StatusCode, context.Request.Path, context.Response.Headers, text, CorrelationId, DateTime.Now.Subtract(SDate).TotalMilliseconds, ResponseException);

                string RefreshToken = _cryptographyService.RefreshJwt();
                if (!string.IsNullOrEmpty(RefreshToken))
                {
                    context.Response.Headers.Add("refresh-token", RefreshToken);
                }
                await responseBody.CopyToAsync(originalBodyStream);


            }

        }

    }


}