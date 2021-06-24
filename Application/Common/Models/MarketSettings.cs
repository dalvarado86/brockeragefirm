using System;

namespace Application.Common.Models
{
    /// <summary>
    /// MarketSettings.
    /// </summary>
    public class MarketSettings
    {
        /// <summary>
        /// Gets or sets the time open.
        /// </summary>
        public TimeSpan TimeOpen { get; set; }

        /// <summary>
        /// Gets or sets the time close.
        /// </summary>
        public TimeSpan TimeClose { get; set; }        
    }
}
