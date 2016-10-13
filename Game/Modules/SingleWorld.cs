﻿using System;
using Isometric.Core.Modules;
using Isometric.Core.Modules.WorldModule;

namespace Isometric.Game.Modules
{
    public class SingleWorld
    {
        private static World _instance;
        public static World Instance => _instance ?? (_instance = new World(SingleRandom.Instance.Next()));
    }
}