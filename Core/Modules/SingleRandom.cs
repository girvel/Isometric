﻿using System;
using Isometric.Core.Modules.TimeModule;
using Isometric.Vector;

namespace Isometric.Core.Modules
{
    [Serializable]
    public class SingleRandom
    {
        #region Singleton-part

        private static Random _instance;
        public static Random Instance => _instance ?? (_instance = CustomSeed ? new Random(Seed) : new Random());

        private SingleRandom() {}

        #endregion



        private static int _seed;
        public static int Seed
        {
            get
            {
                return _seed;
            }
            set
            {
#if DEBUG
                if (_instance != null)
                {
                    throw new ArgumentException(
                        "SingleRandom.Instance is already set; seed can not be changed");
                }
#endif

                _seed = value;
                CustomSeed = true;
            }
        }

        public static bool CustomSeed { get; set; }



        public static GameDate Next(GameDate min, GameDate max)
        {
            return new GameDate(
                Instance.Next(
                    min.TotalDay,
                    max.TotalDay));
        }

        public static IntVector Next(IntVector min, IntVector max)
        {
#if DEBUG
            if (min.Dimensions != max.Dimensions)
            {
                throw new ArgumentException("min and max vectors have different numbers of dimensions");
            }
#endif
            var result = new IntVector(min.Coordinates);
            for (var i = 0; i < min.Dimensions; i++)
            {
#if DEBUG
                if (min[i] > max[i])
                {
                    throw new ArgumentException($"min[{i}] is bigger than max[{i}]");
                }
#endif
                result[i] = Instance.Next(min[i], max[i]);
            }

            return result;
        }

        public static IntVector Next(IntVector max) => Next(new IntVector(0, 0), max);
    }
}

