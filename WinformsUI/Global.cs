namespace WinformsUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameObjectsLib.GameUser;

    /// <summary>
    ///     Represents global utils for the given client.
    /// </summary>
    internal static class Global
    {
        private static User myUser = new LocalUser("Me");

        /// <summary>
        ///     Instance of this represents user instance of the client.
        /// </summary>
        public static User MyUser
        {
            get { return myUser; }
            set
            {
                myUser = value;

                Action<User> userChangedEvent = OnUserChanged;
                if (userChangedEvent == null)
                {
                    return;
                }

                Action<User> onUserChanged = OnUserChanged;

                if (onUserChanged == null)
                {
                    return;
                }

                IEnumerable<Action<User>> invocationList = onUserChanged.GetInvocationList().OfType<Action<User>>();

                foreach (Action<User> eventToInvoke in invocationList)
                {
                    try
                    {
                        eventToInvoke(value);
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        ///     Methods invoked when value in MyUser is changed.
        /// </summary>
        public static event Action<User> OnUserChanged;
    }
}
