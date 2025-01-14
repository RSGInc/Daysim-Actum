﻿// Copyright 2005-2008 Mark A. Bradley and John L. Bowman
// Copyright 2011-2013 John Bowman, Mark Bradley, and RSG, Inc.
// You may not possess or use this file without a License for its use.
// Unless required by applicable law or agreed to in writing, software
// distributed under a License for its use is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

namespace Daysim.Framework.DomainModels.Models {
	public interface IHouseholdDay : IModel {
		int HouseholdId { get; set; }

		int Day { get; set; }

		int DayOfWeek { get; set; }

		int JointTours { get; set; }

		int PartialHalfTours { get; set; }

		int FullHalfTours { get; set; }

		double ExpansionFactor { get; set; }

		//JLB20160323
		int SharedActivityHomeStays { get; set; }

		int NumberInLargestSharedHomeStay { get; set; }

		int StartingMinuteSharedHomeStay { get; set; }

		int DurationMinutesSharedHomeStay { get; set; }

		int AdultsInSharedHomeStay { get; set; }

		int ChildrenInSharedHomeStay { get; set; }

		int PrimaryPriorityTimeFlag { get; set; }

	}
}