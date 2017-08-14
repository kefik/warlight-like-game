namespace WinformsUI
{
    using System;
    using System.Linq;
    using GameObjectsLib.GameUser;

    /// <summary>
    /// Represents global utils for the given client.
    /// </summary>
    static class Global
    {
        static User myUser = new LocalUser("Me");
        /// <summary>
        /// Instance of this represents user instance of the client.
        /// </summary>
        public static User MyUser
        {
            get
            {
                return myUser;
            }
            set
            {
                myUser = value;

                var userChangedEvent = OnUserChanged;
                if (userChangedEvent == null) return;

                var invocationList = OnUserChanged.GetInvocationList().OfType<Action<User>>();

                foreach (var eventToInvoke in invocationList)
                {
                    try
                    {
                        eventToInvoke(value);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        /// <summary>
        /// Methods invoked when value in MyUser is changed.
        /// </summary>
        public static event Action<User> OnUserChanged;
    }
}
