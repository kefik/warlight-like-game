namespace GameHandlersLib
{
    using System;

    /// <summary>
    /// Exception that should be used to display various notifications
    /// to user in front-end.
    /// </summary>
    /// <example>
    /// Notify user that he exceeded the deployed units limit:
    /// <code>
    ///  throw new NotifyUserException("You have exceeded the limit for deployed units in the region Alberta");
    /// </code>
    /// </example>
    public class NotifyUserException : Exception
    {
        public NotifyUserException()
        {
        }

        public NotifyUserException(string message)
            : base(message)
        {
        }

        public NotifyUserException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}