namespace GameObjectsLib.GameRecording
{
    using System.Linq;
    using ProtoBuf;

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public sealed class GameBeginningRound : Round
    {
        /// <summary>
        ///     Verifies correctness of game beginning rounds.
        /// </summary>
        /// <returns>Linearized round.</returns>
        public override ILinearizedRound Linearize()
        {
            // verify if two players chose one region, if so, return null
            {
                bool doesCollide = (from round in Turns
                                    let turn = (GameBeginningTurn)round
                                    select turn.SelectedRegions
                                    into regions
                                    from region in regions
                                    group region by region.Region).Any(g => g.Count() > 1);

                if (doesCollide)
                {
                    return null;
                }
            }

            var linearizedRegions = from round in Turns
                                    let turn = (GameBeginningTurn)round
                                    select turn.SelectedRegions
                                    into regions
                                    from region in regions
                                    select region;

            var newRound = new LinearizedGameBeginningRound(linearizedRegions.ToList());

            return newRound;
        }
    }
}