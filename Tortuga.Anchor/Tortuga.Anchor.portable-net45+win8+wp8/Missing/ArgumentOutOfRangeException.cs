using System;

namespace Tortuga.Anchor.Missing
{


    /// <summary>
    /// Class ArgumentOutOfRangeException.
    /// </summary>
    /// <seealso cref="System.ArgumentOutOfRangeException" />
    public class ArgumentOutOfRangeException : System.ArgumentOutOfRangeException
    {
        private readonly object m_ActualValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentOutOfRangeException"/> class.
        /// </summary>
        public ArgumentOutOfRangeException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentOutOfRangeException"/> class.
        /// </summary>
        /// <param name="paramName">The name of the parameter that causes this exception.</param>
        public ArgumentOutOfRangeException(string paramName) : base(paramName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentOutOfRangeException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for this exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ArgumentOutOfRangeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentOutOfRangeException"/> class.
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="message">The message that describes the error.</param>
        public ArgumentOutOfRangeException(string paramName, string message) : base(message, paramName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentOutOfRangeException"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="message">The message.</param>
        public ArgumentOutOfRangeException(string paramName, object actualValue, string message) : base(message, paramName)
        {
            m_ActualValue = actualValue;
        }

        /// <summary>
        /// Gets the actual value.
        /// </summary>
        /// <value>The actual value.</value>
        public virtual object ActualValue
        {
            get { return m_ActualValue; }
        }


        /// <summary>
        /// Gets the error message and the string representation of the invalid argument value, or only the error message if the argument value is null.
        /// </summary>
        /// <value>The message.</value>
        public override string Message
        {
            get
            {
                string message = base.Message;

                if (m_ActualValue == null)
                    return message;

                string resourceString = string.Format("The actual value was {0}.", m_ActualValue);

                if (message == null)
                    return resourceString;

                return (message + Environment.NewLine + resourceString);
            }
        }

    }
}
