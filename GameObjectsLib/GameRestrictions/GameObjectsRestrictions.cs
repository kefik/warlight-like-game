namespace GameObjectsLib.GameRestrictions
{
    using System.Collections.Generic;
    using ProtoBuf;

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class GameObjectsRestrictions
    {
        public ICollection<GameObjectsBeginningRestriction> GameBeginningRestrictions { get; set; }
    }
}