using System.Collections.Generic;

namespace BetBuddy.Backend.Api.Dtos
{
    public class SelectionDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Odds { get; set; }
    }

    public class MarketDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string MarketType { get; set; }
        public ICollection<SelectionDto> Selections { get; set; }
    }

    public class MatchDto
    {
        public long Id { get; set; }
        public string Home { get; set; }
        public string Away { get; set; }
        public string Competition { get; set; }
        public System.DateTimeOffset StartTime { get; set; }
        public string Game { get; set; }
        public IDictionary<long, MarketDto> Markets { get; set; }
    }
}
