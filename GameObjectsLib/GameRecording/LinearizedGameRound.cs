namespace GameObjectsLib.GameRecording
{
    using ProtoBuf;

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class LinearizedGameRound : ILinearizedRound
    {
        public Deploying Deploying { get; set; }
        public Attacking Attacking { get; set; }

        // ReSharper disable once UnusedMember.Local
        private LinearizedGameRound() { }

        public LinearizedGameRound(Deploying deploying, Attacking attacking)
        {
            Deploying = deploying;
            Attacking = attacking;
        }
    }
}