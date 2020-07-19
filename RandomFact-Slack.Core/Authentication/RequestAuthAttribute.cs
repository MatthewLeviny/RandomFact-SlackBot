using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace RandomFact_Slack.Core.Authentication
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class RequestAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "X-API-KEY";
        private const string SlackHeaderName = "X-Slack-Signature";
        private const string SlackTimestampHeaderName = "X-Slack-Request-Timestamp";
        private const string SlackVersion = "v0:";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey);
            context.HttpContext.Request.Headers.TryGetValue(SlackHeaderName, out var potentialSlackKey);


            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = configuration.GetSection(ApiKeyHeaderName).Value;

            if (!string.IsNullOrEmpty(potentialApiKey) && apiKey.Equals(potentialApiKey))
            {
                await next();
            }

            context.HttpContext.Request.Headers.TryGetValue(SlackTimestampHeaderName, out var slackRequestTimestamp);
            string requestBody;
            using (var reader = new StreamReader(context.HttpContext.Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            var slackSigningKey = configuration.GetSection(SlackHeaderName).Value;

            if (!string.IsNullOrEmpty(potentialSlackKey) &&
                IsSecret(potentialSlackKey, slackRequestTimestamp, requestBody, slackSigningKey))
            {
                await next();
            }

            context.Result = new UnauthorizedResult();
        }

        private bool IsSecret(in StringValues potentialSlackKey, in StringValues slackRequestTimestamp,
            string requestBody, string slackSigningKey)
        {
            //check timestamp for replay - within 5 minutes
            //if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - Convert.ToInt64(slackRequestTimestamp.ToString()) > 60 * 5)
            //{
            //    return false;
            //}

            //concat version timestamp and body
            var concatenatedValue = SlackVersion + slackRequestTimestamp + ":" + requestBody;

            //hash the result
            var mySignature = "v0=" + HashEncode(HashHMAC(Encoding.ASCII.GetBytes(slackSigningKey),
                Encoding.ASCII.GetBytes(concatenatedValue)));

            return potentialSlackKey.Equals(mySignature);
        }


        private static byte[] HashHMAC(byte[] key, byte[] message)
        {
            var hash = new HMACSHA256(key);
            return hash.ComputeHash(message);
        }

        private static string HashEncode(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}