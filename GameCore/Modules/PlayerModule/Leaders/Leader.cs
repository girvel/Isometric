﻿using System;
using GameCore.Modules.TickModule;
using GameCore.Modules.TimeModule;
using CompressedStructures;

namespace GameCore.Modules.PlayerModule.Leaders
{
    // TODO leader dna

	[Serializable]
	public class Leader : IIndependentChanging, IResourcesBonusChanging
	{
		public LeaderPattern Pattern { get; set; }

		public Name Name { get; set; }

		public GameDate Age { get; set; }

		public GameDate AgeMax { get; set; }



		public Leader() {}

		public Leader(LeaderPattern pattern)
		{
			Pattern = pattern;
			this.Name = NamesGenerator.Instance.Generate();

			Age = SingleRandom.Next(
                Player.Data.MinimalNewLeaderAge,
                Player.Data.MaximalNewLeaderAge);

			AgeMax = Age + SingleRandom.Next(
                Player.Data.MinimalLeaderLifeDuration,
                Player.Data.MaximalLeaderLifeDuration);
		}

		public void Die()
		{
			throw new NotImplementedException();
		}



		#region Interfaces

		void IIndependentChanging.Tick()
		{
            Age += ClocksManager.Data.DaysInTick;

            if (Age > AgeMax)
            {
                Die();
            }
		}

        void IResourcesBonusChanging.Tick(ref Resources resources)
        {
            Pattern.BonusTick(this, ref resources);
        }

		#endregion
	}
}

