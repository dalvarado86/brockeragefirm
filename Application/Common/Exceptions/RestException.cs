using System;
using System.Net;

namespace Application.Common.Exceptions
{
    /// <summary>
    /// RestException.
    /// </summary>
    public class RestException : Exception
    {
        /// <summary>
        /// Gets the http status code.
        /// </summary>
        public HttpStatusCode Code { get; }
        
        /// <summary>
        /// Gets the errors.
        /// </summary>
        public object Errors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestException"/> class.
        /// </summary>
        /// <param name="code">The http status code.</param>
        /// <param name="errors">The errors.</param>
        public RestException(HttpStatusCode code, object errors = null)
        {
            Code = code;
            Errors = errors;
        }
    }
}
