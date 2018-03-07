namespace GameObjectsLib.GameRestrictions
{
    using System.Collections.Generic;
    using ProtoBuf;

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class GameRestrictions
    {
        public ICollection<GameBeginningRestriction> GameBeginningRestrictions { get; set; }
    }
}