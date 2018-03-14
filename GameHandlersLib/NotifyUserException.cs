namespace GameHandlersLib
{
    using System;
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