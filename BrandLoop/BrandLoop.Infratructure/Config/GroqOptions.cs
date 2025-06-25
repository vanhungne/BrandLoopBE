using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Config
{
    public sealed class GroqOptions
    {
        public const string SectionName = "Groq";
        public string ApiKey { get; init; } = default!;
        public string BaseUrl { get; init; } = "https://api.groq.com/openai/v1";
        public string Model { get; init; } = "llama3-70b-8192";
    }
}
