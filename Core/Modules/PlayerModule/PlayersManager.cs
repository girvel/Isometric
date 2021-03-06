﻿using System.Collections.Generic;
using Isometric.Core.Modules.TickModule;

namespace Isometric.Core.Modules.PlayerModule
{
    public class PlayersManager : IIndependentChanging
    {
        public HashSet<Player> Players { get; set; }



        public PlayersManager()
        {
            Players = new HashSet<Player>();
        }



        public void Tick()
        {
            foreach (var player in Players)
            {
                player.Tick();
            }
        }
    }
}

