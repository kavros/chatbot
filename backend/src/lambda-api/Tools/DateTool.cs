using System.ComponentModel;

namespace Project1.Tools
{
    public class DateTool
    {
        [Description("A tool for retrieving the current date in UTC format.")]

        public Task<string> GetCurrentDateAsync()
        {
            return Task.FromResult(DateTime.UtcNow.ToString("yyyy-MM-dd"));
        }
    }
}