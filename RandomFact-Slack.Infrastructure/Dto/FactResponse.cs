using System;

namespace RandomFact_Slack.Core.Dto
{
    public class FactResponse
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public string Source { get; set; }
        public string Source_url { get; set; }
        public string Language { get; set; }
        public string Permalink { get; set; }

    }
}