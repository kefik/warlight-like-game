namespace GameObjectsLib.GameUser
{
    using System;
    using ProtoBuf;

    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class LocalUser : User
    {
        public override UserType UserType
        {
            get { return UserType.LocalUser; }
        }

        private LocalUser()
        {
        }

        public LocalUser(string name = "") : base(name)
        {
        }
    }
}
