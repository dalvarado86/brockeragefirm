using System;

namespace Application.Common.Exceptions
{
    /// <summary>
    /// ApplicationException.
    /// </summary>
    public class ApplicationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ApplicationException(string message)
            : base(message)
        {
        }
    }
}
