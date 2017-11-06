﻿namespace GameObjectsLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameMap;
    using ProtoBuf;

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class LinearizedGameBeginningRound : ILinearizedRound
    {
        public IList<Seize> SelectedRegions { get; } = new List<Seize>();

        // ReSharper disable once UnusedMember.Local
        private LinearizedGameBeginningRound()
        {
        }

        public LinearizedGameBeginningRound(IList<Seize> list)
        {
            SelectedRegions = list ?? throw new ArgumentException();
        }
    }
}