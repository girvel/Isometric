﻿using System;
using CommonStructures;
using IsometricCore.Modules.WorldModule.Buildings;

namespace IsometricImplementation.Modules.PatternsRealization
{
    [Serializable]
    public static class BuildingPatterns
    {
        public static BuildingPattern Plain { get; private set; }
        public static BuildingPattern Rock { get; private set; }
        public static BuildingPattern Water { get; private set; }
        public static BuildingPattern Forest { get; private set; }
        public static BuildingPattern WoodHouse { get; private set; }
        public static BuildingPattern WoodHouse2 { get; private set; }
            


        internal static void Init()
        {
            Plain = new BuildingPattern(
                "Plain",
                new Resources(),
                new Resources(),
                new TimeSpan(0),
                BuildingType.Space);
            
            Rock = new BuildingPattern(
                "Rock",
                new Resources(stone: 10000),
                new Resources(),
                new TimeSpan(0));

            Water = new BuildingPattern(
                "Water",
                new Resources(),
                new Resources(),
                new TimeSpan(0));

            Forest = new BuildingPattern(
                "Forest",
                new Resources(wood: 1000),
                new Resources(),
                new TimeSpan(0));

            WoodHouse = new BuildingPattern(
                "Wood house I",
                new Resources(),
                new Resources(wood: 500),
                new TimeSpan(0, 1, 0),
                BuildingType.Building);

            WoodHouse2 = new BuildingPattern(
                "Wood house II", 
                new Resources(),
                new Resources(wood: 300, stone: 100),
                new TimeSpan(0, 1, 0),
                BuildingType.Building);
        }
    }
}