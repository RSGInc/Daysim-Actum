﻿// Copyright 2005-2008 Mark A. Bradley and John L. Bowman
// Copyright 2011-2013 John Bowman, Mark Bradley, and RSG, Inc.
// You may not possess or use this file without a License for its use.
// Unless required by applicable law or agreed to in writing, software
// distributed under a License for its use is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daysim.ChoiceModels;
using Daysim.Framework.Core;
using Daysim.Framework.DomainModels.Creators;
using Daysim.Framework.DomainModels.Models;
using Daysim.Framework.DomainModels.Persisters;
using Daysim.Framework.DomainModels.Wrappers;
using Daysim.Framework.Exceptions;
using Daysim.Framework.Factories;
using Daysim.Framework.Sampling;
using Daysim.PathTypeModels;
using Ninject;

namespace Daysim.DomainModels.Default.Wrappers {
	[Factory(Factory.WrapperFactory, Category = Category.Wrapper, DataType = DataType.Default)]
	public class TourWrapper : ITourWrapper, ISamplingTour {
		private readonly ITour _tour;

		private readonly IPersisterExporter _exporter;

		private readonly ITourCreator _tourCreator;

		[UsedImplicitly]
		public TourWrapper(ITour tour, IPersonWrapper personWrapper, IPersonDayWrapper personDayWrapper, IParcelWrapper originParcel, IParcelWrapper destinationParcel, int destinationArrivalTime, int destinationDepartureTime, int destinationPurpose) {
			_tour = tour;

			_exporter =
				Global
					.Kernel
					.Get<IPersistenceFactory<ITour>>()
					.Exporter;

			_tourCreator =
				Global
					.Kernel
					.Get<IWrapperFactory<ITourCreator>>()
					.Creator;

			// relations properties

			Household = personWrapper.Household;
			Person = personWrapper;
			PersonDay = personDayWrapper;

			OriginParcel = originParcel;
			DestinationParcel = destinationParcel;
			DestinationPurpose = destinationPurpose;
			DestinationArrivalTime = destinationArrivalTime;
			DestinationDepartureTime = destinationDepartureTime;
			DestinationPurpose = destinationPurpose;

			// flags/choice model/etc. properties

			SetValueOfTimeCoefficients(destinationPurpose, true);
		}

		[UsedImplicitly]
		public TourWrapper(ITour subtour, ITourWrapper tourWrapper, int purpose, bool suppressRandomVOT) {
			_tour = subtour;

			_exporter =
				Global
					.Kernel
					.Get<IPersistenceFactory<ITour>>()
					.Exporter;

			_tourCreator =
				Global
					.Kernel
					.Get<IWrapperFactory<ITourCreator>>()
					.Creator;

			// relations properties

			Household = tourWrapper.Household;
			Person = tourWrapper.Person;
			PersonDay = tourWrapper.PersonDay;
			ParentTour = tourWrapper;

			SetParcelRelationships(subtour);

			// flags/choice model/etc. properties

			SetValueOfTimeCoefficients(purpose, suppressRandomVOT);
		}

		[UsedImplicitly]
		public TourWrapper(ITour tour, IPersonDayWrapper personDayWrapper, int purpose, bool suppressRandomVOT) {
			_tour = tour;

			_exporter =
				Global
					.Kernel
					.Get<IPersistenceFactory<ITour>>()
					.Exporter;

			_tourCreator =
				Global
					.Kernel
					.Get<IWrapperFactory<ITourCreator>>()
					.Creator;

			// relations properties

			Household = personDayWrapper.Household;
			Person = personDayWrapper.Person;
			PersonDay = personDayWrapper;
			Subtours = new List<ITourWrapper>();

			SetParcelRelationships(tour);

			// flags/choice model/etc. properties

			SetValueOfTimeCoefficients(purpose, suppressRandomVOT);

			IsHomeBasedTour = true;
			TimeWindow = new TimeWindow();
		}

		#region relations properties

		public IHouseholdWrapper Household { get; set; }

		public IPersonWrapper Person { get; set; }

		public IPersonDayWrapper PersonDay { get; set; }

		public ITourWrapper ParentTour { get; set; }

		public List<ITourWrapper> Subtours { get; set; }

		public IHalfTour HalfTourFromOrigin { get; set; }

		public IHalfTour HalfTourFromDestination { get; set; }

		public IParcelWrapper OriginParcel { get; set; }

		IParcel ISamplingTour.OriginParcel {
			get { return OriginParcel; }
		}

		public IParcelWrapper DestinationParcel { get; set; }

		#endregion

		#region domain model properies

		public int Id {
			get { return _tour.Id; }
			set { _tour.Id = value; }
		}

		public int PersonId {
			get { return _tour.PersonId; }
			set { _tour.PersonId = value; }
		}

		public int PersonDayId {
			get { return _tour.PersonDayId; }
			set { _tour.PersonDayId = value; }
		}

		public int HouseholdId {
			get { return _tour.HouseholdId; }
			set { _tour.HouseholdId = value; }
		}

		public int PersonSequence {
			get { return _tour.PersonSequence; }
			set { _tour.PersonSequence = value; }
		}

		public int Day {
			get { return _tour.Day; }
			set { _tour.Day = value; }
		}

		public int Sequence {
			get { return _tour.Sequence; }
			set { _tour.Sequence = value; }
		}

		public int JointTourSequence {
			get { return _tour.JointTourSequence; }
			set { _tour.JointTourSequence = value; }
		}

		public int ParentTourSequence {
			get { return _tour.ParentTourSequence; }
			set { _tour.ParentTourSequence = value; }
		}

		public int TotalSubtours {
			get { return _tour.TotalSubtours; }
			set { _tour.TotalSubtours = value; }
		}

		public int DestinationPurpose {
			get { return _tour.DestinationPurpose; }
			set { _tour.DestinationPurpose = value; }
		}

		public int OriginDepartureTime {
			get { return _tour.OriginDepartureTime.ToMinutesAfter3AM(); }
			set { _tour.OriginDepartureTime = value.ToMinutesAfterMidnight(); }
		}

		public int DestinationArrivalTime {
			get { return _tour.DestinationArrivalTime.ToMinutesAfter3AM(); }
			set { _tour.DestinationArrivalTime = value.ToMinutesAfterMidnight(); }
		}

		public int DestinationDepartureTime {
			get { return _tour.DestinationDepartureTime.ToMinutesAfter3AM(); }
			set { _tour.DestinationDepartureTime = value.ToMinutesAfterMidnight(); }
		}

		public int OriginArrivalTime {
			get { return _tour.OriginArrivalTime.ToMinutesAfter3AM(); }
			set { _tour.OriginArrivalTime = value.ToMinutesAfterMidnight(); }
		}

		public int OriginAddressType {
			get { return _tour.OriginAddressType; }
			set { _tour.OriginAddressType = value; }
		}

		public int DestinationAddressType {
			get { return _tour.DestinationAddressType; }
			set { _tour.DestinationAddressType = value; }
		}

		public int OriginParcelId {
			get { return _tour.OriginParcelId; }
			set { _tour.OriginParcelId = value; }
		}

		public int OriginZoneKey {
			get { return _tour.OriginZoneKey; }
			set { _tour.OriginZoneKey = value; }
		}

		public int DestinationParcelId {
			get { return _tour.DestinationParcelId; }
			set { _tour.DestinationParcelId = value; }
		}

		public int DestinationZoneKey {
			get { return _tour.DestinationZoneKey; }
			set { _tour.DestinationZoneKey = value; }
		}

		public int Mode {
			get { return _tour.Mode; }
			set { _tour.Mode = value; }
		}

		public int PathType {
			get { return _tour.PathType; }
			set { _tour.PathType = value; }
		}

		public double AutoTimeOneWay {
			get { return _tour.AutoTimeOneWay; }
			set { _tour.AutoTimeOneWay = value; }
		}

		public double AutoCostOneWay {
			get { return _tour.AutoCostOneWay; }
			set { _tour.AutoCostOneWay = value; }
		}

		public double AutoDistanceOneWay {
			get { return _tour.AutoDistanceOneWay; }
			set { _tour.AutoDistanceOneWay = value; }
		}

		public int HalfTour1Trips {
			get { return _tour.HalfTour1Trips; }
			set { _tour.HalfTour1Trips = value; }
		}

		public int HalfTour2Trips {
			get { return _tour.HalfTour2Trips; }
			set { _tour.HalfTour2Trips = value; }
		}

		public int PartialHalfTour1Sequence {
			get { return _tour.PartialHalfTour1Sequence; }
			set { _tour.PartialHalfTour1Sequence = value; }
		}

		public int PartialHalfTour2Sequence {
			get { return _tour.PartialHalfTour2Sequence; }
			set { _tour.PartialHalfTour2Sequence = value; }
		}

		public int FullHalfTour1Sequence {
			get { return _tour.FullHalfTour1Sequence; }
			set { _tour.FullHalfTour1Sequence = value; }
		}

		public int FullHalfTour2Sequence {
			get { return _tour.FullHalfTour2Sequence; }
			set { _tour.FullHalfTour2Sequence = value; }
		}

		public double ExpansionFactor {
			get { return _tour.ExpansionFactor; }
			set { _tour.ExpansionFactor = value; }
		}

		#endregion

		#region flags/choice model/etc. properties

		public bool IsHomeBasedTour { get; set; }

		public ITimeWindow TimeWindow { get; set; }

		public double IndicatedTravelTimeToDestination { get; set; }

		public double IndicatedTravelTimeFromDestination { get; set; }

		public int EarliestOriginDepartureTime { get; set; }

		public int LatestOriginArrivalTime { get; set; }

		public IMinuteSpan DestinationDepartureBigPeriod { get; set; }

		public IMinuteSpan DestinationArrivalBigPeriod { get; set; }

		public double TimeCoefficient { get; set; }

		public double CostCoefficient { get; set; }

		public int ParkAndRideNodeId { get; set; }

		public int ParkAndRideOriginStopAreaKey { get; set; }

		public int ParkAndRideDestinationStopAreaKey { get; set; }

		public int ParkAndRidePathType { get; set; }

		public double ParkAndRideTransitTime { get; set; }

		public double ParkAndRideTransitDistance { get; set; }

		public double ParkAndRideTransitCost { get; set; }

		public double ParkAndRideTransitGeneralizedTime { get; set; }


		public bool DestinationModeAndTimeHaveBeenSimulated { get; set; }

		public bool HalfTour1HasBeenSimulated { get; set; }

		public bool HalfTour2HasBeenSimulated { get; set; }

		public bool IsMissingData { get; set; }

		//JLB 20160323
		public int HalfTour1AccessMode { get; set; }

		public int HalfTour1AccessPathType { get; set; }

		public double HalfTour1AccessTime { get; set; }

		public double HalfTour1AccessCost { get; set; }

		public double HalfTour1AccessDistance { get; set; }

		public int HalfTour1AccessStopAreaKey { get; set; }

		public int HalfTour1EgressMode { get; set; }

		public int HalfTour1EgressPathType { get; set; }

		public double HalfTour1EgressTime { get; set; }

		public double HalfTour1EgressCost { get; set; }

		public double HalfTour1EgressDistance { get; set; }

		public int HalfTour1EgressStopAreaKey { get; set; }
	
		public int HalfTour2AccessMode { get; set; }

		public int HalfTour2AccessPathType { get; set; }

		public double HalfTour2AccessTime { get; set; }

		public double HalfTour2AccessCost { get; set; }

		public double HalfTour2AccessDistance { get; set; }

		public int HalfTour2AccessStopAreaKey { get; set; }

		public int HalfTour2EgressMode { get; set; }

		public int HalfTour2EgressPathType { get; set; }

		public double HalfTour2EgressTime { get; set; }

		public double HalfTour2EgressCost { get; set; }

		public double HalfTour2EgressDistance { get; set; }

		public int HalfTour2EgressStopAreaKey { get; set; }

		public double HalfTour1TravelTime { get; set; }

		public double HalfTour2TravelTime { get; set; }

		public double TravelCostForPTBikeTour { get; set; }

		public double TravelDistanceForPTBikeTour { get; set; }



		#endregion

		#region wrapper methods

		public virtual bool IsWorkPurpose() {
			return DestinationPurpose == Global.Settings.Purposes.Work;
		}

		public virtual bool IsSchoolPurpose() {
			return DestinationPurpose == Global.Settings.Purposes.School;
		}

		public virtual bool IsEscortPurpose() {
			return DestinationPurpose == Global.Settings.Purposes.Escort;
		}

		public virtual bool IsPersonalBusinessPurpose() {
			return DestinationPurpose == Global.Settings.Purposes.PersonalBusiness;
		}

		public virtual bool IsShoppingPurpose() {
			return DestinationPurpose == Global.Settings.Purposes.Shopping;
		}

		public virtual bool IsMealPurpose() {
			return DestinationPurpose == Global.Settings.Purposes.Meal;
		}

		public virtual bool IsSocialPurpose() {
			return DestinationPurpose == Global.Settings.Purposes.Social;
		}

		public virtual bool IsRecreationPurpose() {
			return DestinationPurpose == Global.Settings.Purposes.Recreation;
		}

		public virtual bool IsMedicalPurpose() {
			return DestinationPurpose == Global.Settings.Purposes.Medical;
		}

		public virtual bool IsPersonalBusinessOrMedicalPurpose() {
			return DestinationPurpose == Global.Settings.Purposes.PersonalBusiness || DestinationPurpose == Global.Settings.Purposes.Medical;
		}

		public virtual bool IsSocialOrRecreationPurpose() {
			return DestinationPurpose == Global.Settings.Purposes.Social || DestinationPurpose == Global.Settings.Purposes.Recreation;
		}

		//JLB 20160323
		public virtual bool IsBusinessPurpose() {
			return DestinationPurpose == Global.Settings.Purposes.Business;
		}


		public virtual bool IsWalkMode() {
			return Mode == Global.Settings.Modes.Walk;
		}

		public virtual bool IsBikeMode() {
			return Mode == Global.Settings.Modes.Bike;
		}

		public virtual bool IsSovMode() {
			return Mode == Global.Settings.Modes.Sov;
		}

		public virtual bool IsHov2Mode() {
			return Mode == Global.Settings.Modes.Hov2;
		}

		public virtual bool IsHov3Mode() {
			return Mode == Global.Settings.Modes.Hov3;
		}

		public virtual bool IsTransitMode() {
			return Mode == Global.Settings.Modes.Transit;
		}

		public virtual bool IsParkAndRideMode() {
			return Mode == Global.Settings.Modes.ParkAndRide;
		}

		public virtual bool IsSchoolBusMode() {
			return Mode == Global.Settings.Modes.SchoolBus;
		}

		public virtual bool IsWalkOrBikeMode() {
			return Mode == Global.Settings.Modes.Walk || Mode == Global.Settings.Modes.Bike;
		}

		//JLB 20160323
		public virtual bool IsHovDriverMode() {
			return Mode == Global.Settings.Modes.HovDriver;
		}
		public virtual bool IsHovPassengerMode() {
			return Mode == Global.Settings.Modes.HovPassenger;
		}

		public virtual bool IsPaidRideShareMode() {
			return Mode == Global.Settings.Modes.PaidRideShare;
		}
		
		public virtual bool SubtoursExist() {
			return Subtours.Count > 0;
		}

		public virtual bool IsAnHovMode() {
			return IsHov2Mode() || IsHov3Mode();
		}

		public virtual bool IsAnAutoMode() {
			return IsSovMode() || IsHov2Mode() || IsHov3Mode();
		}

		public virtual bool UsesTransitModes() {
			return IsTransitMode() || IsParkAndRideMode();
		}

		public virtual int GetTotalToursByPurpose() {
			if (DestinationPurpose == Global.Settings.Purposes.Work) {
				return PersonDay.WorkTours;
			}

			if (DestinationPurpose == Global.Settings.Purposes.School) {
				return PersonDay.SchoolTours;
			}

			if (DestinationPurpose == Global.Settings.Purposes.Escort) {
				return PersonDay.EscortTours;
			}

			if (DestinationPurpose == Global.Settings.Purposes.PersonalBusiness) {
				return PersonDay.PersonalBusinessTours;
			}

			if (DestinationPurpose == Global.Settings.Purposes.Shopping) {
				return PersonDay.ShoppingTours;
			}

			if (DestinationPurpose == Global.Settings.Purposes.Meal) {
				return PersonDay.MealTours;
			}

			if (DestinationPurpose == Global.Settings.Purposes.Social) {
				return PersonDay.SocialTours;
			}

			return 0;
		}

		public virtual int GetTotalSimulatedToursByPurpose() {
			if (DestinationPurpose == Global.Settings.Purposes.Work) {
				return PersonDay.SimulatedWorkTours;
			}

			if (DestinationPurpose == Global.Settings.Purposes.School) {
				return PersonDay.SimulatedSchoolTours;
			}

			if (DestinationPurpose == Global.Settings.Purposes.Escort) {
				return PersonDay.SimulatedEscortTours;
			}

			if (DestinationPurpose == Global.Settings.Purposes.PersonalBusiness) {
				return PersonDay.SimulatedPersonalBusinessTours;
			}

			if (DestinationPurpose == Global.Settings.Purposes.Shopping) {
				return PersonDay.SimulatedShoppingTours;
			}

			if (DestinationPurpose == Global.Settings.Purposes.Meal) {
				return PersonDay.SimulatedMealTours;
			}

			if (DestinationPurpose == Global.Settings.Purposes.Social) {
				return PersonDay.SimulatedSocialTours;
			}

			return 0;
		}

		public virtual int GetTourPurposeSegment() {
			return
				IsHomeBasedTour
					? Global.Settings.Purposes.HomeBasedComposite
					: Global.Settings.Purposes.WorkBased;
		}

		public virtual int GetTourCategory() {
			var tourCategory =
				IsHomeBasedTour
					? PersonDay.SimulatedHomeBasedTours == 1
						? Global.Settings.TourCategories.Primary
						: Global.Settings.TourCategories.Secondary
					: Global.Settings.TourCategories.WorkBased;

			if (tourCategory == Global.Settings.TourCategories.Secondary && (IsWorkPurpose() && !Person.IsFulltimeWorker) || (IsSchoolPurpose() && Person.IsAdult)) {
				tourCategory = Global.Settings.TourCategories.HomeBased;
			}

			return tourCategory;
		}

		//JLB 20160323
		public virtual int GetVotALSegment() {
			var segment =
				(DestinationPurpose == Global.Settings.Purposes.Work || DestinationPurpose == Global.Settings.Purposes.School || DestinationPurpose == Global.Settings.Purposes.Escort)
					? Global.Settings.VotALSegments.Medium
					: (DestinationPurpose == Global.Settings.Purposes.Business)
						? Global.Settings.VotALSegments.High
						: Global.Settings.VotALSegments.Low;

			return segment;
		}

//		public virtual int GetVotALSegment() {
//			var segment =
//				((60 * TimeCoefficient) / CostCoefficient < Global.Settings.VotALSegments.VotLowMedium)
//					? Global.Settings.VotALSegments.Low
//					: ((60 * TimeCoefficient) / CostCoefficient < Global.Settings.VotALSegments.VotMediumHigh)
//						? Global.Settings.VotALSegments.Medium
//						: Global.Settings.VotALSegments.High;
//
//			return segment;
//		}

		public virtual void SetHomeBasedIsSimulated() {
			PersonDay.IncrementSimulatedTours(DestinationPurpose);
		}

		public virtual void SetWorkBasedIsSimulated() {
			PersonDay.IncrementSimulatedStops(DestinationPurpose);
		}

		public virtual void SetHalfTours(int direction) {
			if (direction == Global.Settings.TourDirections.OriginToDestination) {
				HalfTourFromOrigin = new HalfTour(this);

				HalfTourFromOrigin.SetTrips(direction);
			}
			else if (direction == Global.Settings.TourDirections.DestinationToOrigin) {
				HalfTourFromDestination = new HalfTour(this);

				HalfTourFromDestination.SetTrips(direction);
			}
		}

		public virtual ITimeWindow GetRelevantTimeWindow(IHouseholdDayWrapper householdDay) {
			var timeWindow = new TimeWindow();
			if (JointTourSequence > 0) {
				foreach (var pDay in householdDay.PersonDays) {
					var tInJoint =
						pDay
							.Tours
							.Find(t => t.JointTourSequence == JointTourSequence);

					if (tInJoint != null) {
						// set jointTour time window
						timeWindow.IncorporateAnotherTimeWindow(tInJoint.PersonDay.TimeWindow);
					}
				}
			}
			else if (FullHalfTour1Sequence > 0 || FullHalfTour2Sequence > 0) {
				if (FullHalfTour1Sequence > 0) {
					foreach (var pDay in householdDay.PersonDays) {
						var tInJoint =
							pDay
								.Tours
								.Find(t => t.FullHalfTour1Sequence == FullHalfTour1Sequence);

						if (tInJoint != null) {
							// set jointTour time window
							timeWindow.IncorporateAnotherTimeWindow(tInJoint.PersonDay.TimeWindow);
						}
					}
				}

				if (FullHalfTour2Sequence <= 0) {
					return timeWindow;
				}

				foreach (var pDay in householdDay.PersonDays) {
					var tInJoint =
						pDay
							.Tours
							.Find(t => t.FullHalfTour2Sequence == FullHalfTour2Sequence);

					if (tInJoint != null) {
						// set jointTour time window
						timeWindow.IncorporateAnotherTimeWindow(tInJoint.PersonDay.TimeWindow);
					}
				}
			}
			else if (ParentTour == null) {
				timeWindow.IncorporateAnotherTimeWindow(PersonDay.TimeWindow);
			}
			else {
				timeWindow.IncorporateAnotherTimeWindow(ParentTour.TimeWindow);
			}

			return timeWindow;
		}

		public virtual void SetOriginTimes(int direction = 0) {
			if (Global.Configuration.IsInEstimationMode) {
				return;
			}

			// sets origin departure time
			if (direction != Global.Settings.TourDirections.DestinationToOrigin) {
				OriginDepartureTime = HalfTourFromOrigin.Trips.Last().ArrivalTime;
			}

			// sets origin arrival time
			if (direction != Global.Settings.TourDirections.OriginToDestination) {
				OriginArrivalTime = HalfTourFromDestination.Trips.Last().ArrivalTime;
			}
		}

		public virtual void UpdateTourValues() {
			if (Global.Configuration.IsInEstimationMode || (Global.Configuration.ShouldRunTourModels && !Global.Configuration.ShouldRunTourTripModels) || (Global.Configuration.ShouldRunSubtourModels && !Global.Configuration.ShouldRunSubtourTripModels)) {
				return;
			}

			IEnumerable<dynamic> pathTypeModels =
			PathTypeModelFactory.Model
				.Run(Household.RandomUtility, OriginParcel, DestinationParcel, DestinationArrivalTime, DestinationDepartureTime, DestinationPurpose, CostCoefficient, TimeCoefficient, true, 1, Person.TransitPassOwnership, Household.OwnsAutomatedVehicles>0, Person.GetTransitFareDiscountFraction(), false, Global.Settings.Modes.Sov);
			
			var autoPathRoundTrip =		pathTypeModels.First();

			AutoTimeOneWay = autoPathRoundTrip.PathTime / 2.0;
			AutoCostOneWay = autoPathRoundTrip.PathCost / 2.0;
			AutoDistanceOneWay = autoPathRoundTrip.PathDistance / 2.0;

			if (HalfTourFromOrigin == null || HalfTourFromDestination == null) {
				return;
			}

			var trips =
				HalfTourFromOrigin
					.Trips
					.Union(HalfTourFromDestination.Trips)
					.ToList();

			for (var i = 0; i < trips.Count; i++) {
				// sets the driver or passenger flag for each trip
				trips[i].SetDriverOrPassenger(trips);

				// sets the activity end time for each trip
				if (trips[i].IsHalfTourFromOrigin) {
					// for trips to intermediate stops on first half tour, use "arrival" time at stop
					if (i > 0) {
						trips[i].SetActivityEndTime(trips[i - 1].ArrivalTime);
					}
						// for trip to primary destination on tours without subtours, use departure time from prim.dest.
					else if (Subtours == null || Subtours.Count == 0) {
						trips[i].SetActivityEndTime(DestinationDepartureTime);
					}
						// for trip to primary destination on tours with subtours, use departure time for first subtour
					else {
						var nextDepartureTime = Global.Settings.Times.MinutesInADay;

						foreach (var subtour in Subtours) {
							if (subtour != null && subtour.OriginDepartureTime > trips[i].DepartureTime && subtour.OriginDepartureTime < nextDepartureTime) {
								nextDepartureTime = subtour.OriginDepartureTime;
							}
						}

						trips[i].SetActivityEndTime(nextDepartureTime);
					}
				}
				else {
					//second half tour
					// for trips to intermediate stops on second half tour, use departure time from stop
					if (i < trips.Count - 1) {
						trips[i].SetActivityEndTime(trips[i + 1].DepartureTime);
					}
						// if a home-based tour, look for start of next home-based tour}
					else if (ParentTour == null) {
						var nextDepartureTime = Global.Settings.Times.MinutesInADay;

						foreach (var otherTour in PersonDay.Tours) {
							if (otherTour != null && otherTour != this
							    && otherTour.OriginDepartureTime > trips[i].ArrivalTime
							    && otherTour.OriginDepartureTime < nextDepartureTime) {
								nextDepartureTime = otherTour.OriginDepartureTime;
							}
						}

						trips[i].SetActivityEndTime(nextDepartureTime);
					}
						// otherwise, a subtour - look for other subtours, else departure from the parent tour dest.
					else {
						var nextDepartureTime = ParentTour.DestinationDepartureTime;
						foreach (var otherSubtour in ParentTour.Subtours) {
							if (otherSubtour != null && otherSubtour != this && otherSubtour.OriginDepartureTime > trips[i].ArrivalTime && otherSubtour.OriginDepartureTime < nextDepartureTime) {
								nextDepartureTime = otherSubtour.OriginDepartureTime;
							}
						}

						trips[i].SetActivityEndTime(nextDepartureTime);
					}
				}
			}
		}

		public virtual ITourWrapper CreateSubtour(int originAddressType, int originParcelId, int originZoneKey, int destinationPurpose) {
			TotalSubtours++;

			var model = _tourCreator.CreateModel();

			model.Id = PersonDayId * 10 + PersonDay.GetNextTourSequence();
			model.PersonId = PersonId;
			model.PersonDayId = PersonDayId;
			model.HouseholdId = HouseholdId;
			model.PersonSequence = PersonSequence;
			model.Day = Day;
			model.Sequence = PersonDay.GetCurrentTourSequence();
			model.OriginAddressType = originAddressType;
			model.OriginParcelId = originParcelId;
			model.OriginZoneKey = originZoneKey;
			model.DestinationPurpose = destinationPurpose;
			model.OriginDepartureTime = 180;
			model.DestinationArrivalTime = 180;
			model.DestinationDepartureTime = 180;
			model.OriginArrivalTime = 180;
			model.PathType = 1;
			model.ExpansionFactor = Household.ExpansionFactor;

			return _tourCreator.CreateWrapper(model, this, destinationPurpose, false);
		}

		public virtual IHalfTour GetHalfTour(int direction) {
			if (direction == Global.Settings.TourDirections.OriginToDestination) {
				// the half-tour from the origin to destination
				return HalfTourFromOrigin;
			}
			else if (direction == Global.Settings.TourDirections.DestinationToOrigin) {
				// the half-tour from the destination to origin
				return HalfTourFromDestination;
			}
			else {
				throw new InvalidTourDirectionException();
			}
		}

		public virtual ITourModeImpedance[] GetTourModeImpedances() {
			var modeImpedances = new ITourModeImpedance[DayPeriod.SmallDayPeriods.Length];

			var availableMinutes =
				IsHomeBasedTour
					? PersonDay.TimeWindow
					: ParentTour.TimeWindow;

			for (var i = 0; i < DayPeriod.SmallDayPeriods.Length; i++) {
				var period = DayPeriod.SmallDayPeriods[i];
				var modeImpedance = GetTourModeImpedance(period.Middle);

				modeImpedances[i] = modeImpedance;

				modeImpedance.AdjacentMinutesBefore = availableMinutes.AdjacentAvailableMinutesBefore(period.Start) / ChoiceModelFactory.SmallPeriodDuration;
				modeImpedance.MaxMinutesBefore = availableMinutes.MaxAvailableMinutesBefore(period.Start) / ChoiceModelFactory.SmallPeriodDuration;
				modeImpedance.TotalMinutesBefore = availableMinutes.TotalAvailableMinutesBefore(period.Start) / ChoiceModelFactory.SmallPeriodDuration;

				modeImpedance.AdjacentMinutesAfter = availableMinutes.AdjacentAvailableMinutesAfter(period.End) / ChoiceModelFactory.SmallPeriodDuration;
				modeImpedance.MaxMinutesAfter = availableMinutes.MaxAvailableMinutesAfter(period.End) / ChoiceModelFactory.SmallPeriodDuration;
				modeImpedance.TotalMinutesAfter = availableMinutes.TotalAvailableMinutesAfter(period.End) / ChoiceModelFactory.SmallPeriodDuration;
			}

			return modeImpedances;
		}

		public virtual void SetParentTourSequence(int parentTourSequence) {
			ParentTourSequence = parentTourSequence;
		}

		public virtual void SetParkAndRideStay() {
			if (!Global.ParkAndRideNodeIsEnabled || !Global.Configuration.ShouldUseParkAndRideShadowPricing || Global.Configuration.IsInEstimationMode) {
				return;
			}

			var arrivalTime =
				HalfTourFromOrigin
					.Trips
					.First(x => x.OriginPurpose == Global.Settings.Purposes.ChangeMode)
					.DepartureTime;

			var mode =
				HalfTourFromOrigin
					.Trips
					.First(x => x.OriginPurpose == Global.Settings.Purposes.ChangeMode)
					.Mode;

			var departureTime =
				HalfTourFromDestination
					.Trips
					.First(x => x.OriginPurpose == Global.Settings.Purposes.ChangeMode)
					.DepartureTime;

			var parkAndRideLoad =
				ChoiceModelFactory
					.ParkAndRideNodeDao
					.Get(ParkAndRideNodeId)
					.ParkAndRideLoad;

			for (var minute = arrivalTime; minute < departureTime; minute++) {
				parkAndRideLoad[minute] += Household.ExpansionFactor / (mode == Global.Settings.Modes.Hov3 ? 3 : mode == Global.Settings.Modes.Hov2 ? 2 : 1);
			}
		}

		#endregion

		#region init/utility/export methods

		public void Export() {
			_exporter.Export(_tour);
		}

		public static void Close() {
			Global
				.Kernel
				.Get<IPersistenceFactory<ITour>>()
				.Close();
		}

		public override string ToString() {
			var builder = new StringBuilder();

			builder
				.AppendLine(string.Format("Tour ID: {0}, Person Day ID: {1}, Person ID: {2}",
					Id,
					PersonDayId,
					PersonId));

			builder
				.AppendLine(string.Format("Household ID: {0}, Person Sequence: {1}, Day: {2}, Sequence: {3}, Parent Tour Sequence: {4}",
					HouseholdId,
					PersonSequence,
					Day,
					Sequence,
					ParentTourSequence));

			builder
				.AppendLine(string.Format("Destination Parcel ID: {0}, Destination Zone Key: {1}, Destination Address Type: {2}, Destination Purpose: {3}, Mode: {4}, Destination Arrival Time: {5}, Destination Departure Time: {6}",
					DestinationParcelId,
					DestinationZoneKey,
					DestinationAddressType,
					DestinationPurpose,
					Mode,
					DestinationArrivalTime,
					DestinationDepartureTime));

			return builder.ToString();
		}

		private void SetParcelRelationships(ITour tour) {
			IParcelWrapper originParcel;

			if (tour.OriginParcelId != Constants.DEFAULT_VALUE && ChoiceModelFactory.Parcels.TryGetValue(tour.OriginParcelId, out originParcel)) {
				OriginParcel = originParcel;
			}

			IParcelWrapper destinationParcel;

			if (tour.DestinationParcelId != Constants.DEFAULT_VALUE && ChoiceModelFactory.Parcels.TryGetValue(tour.DestinationParcelId, out destinationParcel)) {
				DestinationParcel = destinationParcel;
			}
		}

		private void SetValueOfTimeCoefficients(int purpose, bool suppressRandomVOT) {
			var randomVot = !Global.Configuration.IsInEstimationMode && Global.Configuration.UseRandomVotDistribution && !suppressRandomVOT;

			var income =
				Household.Income < 0
					? Global.Configuration.Coefficients_BaseCostCoefficientIncomeLevel
					: Household.Income; // missing converted to 30K

			var incomeMultiple = Math.Min(Math.Max(income / Global.Configuration.Coefficients_BaseCostCoefficientIncomeLevel, Global.Coefficients_CostCoefficientIncomeMultipleMinimum), Global.Coefficients_CostCoefficientIncomeMultipleMaximum); // ranges for extreme values

			var incomePower =
				purpose == Global.Settings.Purposes.Work
					? Global.Configuration.Coefficients_CostCoefficientIncomePower_Work
					: Global.Configuration.Coefficients_CostCoefficientIncomePower_Other;

			var costCoefficient = Global.Coefficients_BaseCostCoefficientPerMonetaryUnit / Math.Pow(incomeMultiple, incomePower);

			CostCoefficient = costCoefficient;

			const double minimumTimeCoef = 0.001;
			const double maximumTimeCoef = 1.000;

			var mean =
				purpose == Global.Settings.Purposes.Work
					? Global.Configuration.Coefficients_MeanTimeCoefficient_Work
					: Global.Configuration.Coefficients_MeanTimeCoefficient_Other;

			double timeCoefficient;

			if (randomVot) {
				if (Global.Configuration.ShouldSynchronizeRandomSeed && PersonDay != null) {
					PersonDay.ResetRandom(10 + Sequence - 1);
				}

				var coefficient =
					purpose == Global.Settings.Purposes.Work
						? Global.Configuration.Coefficients_StdDeviationTimeCoefficient_Work
						: Global.Configuration.Coefficients_StdDeviationTimeCoefficient_Other;

				var sDev = Math.Abs(mean * coefficient);

				timeCoefficient = -1.0 * Math.Min(maximumTimeCoef, Math.Max(minimumTimeCoef, Household.RandomUtility.LogNormal(-1.0 * mean, sDev))); // converted to positive and back to negative

				if (timeCoefficient.AlmostEquals(0)) {
					throw new InvalidTimeCoefficientException(string.Format("The time coefficient is invalid where randomVot is true for mean: {0}, sDev: {1}.", mean, sDev));
				}
			}
			else {
				timeCoefficient = mean;

				if (timeCoefficient.AlmostEquals(0)) {
					throw new InvalidTimeCoefficientException(string.Format("The time coefficient is invalid where randomVot is false for mean: {0}.", mean));
				}
			}

			TimeCoefficient = timeCoefficient;

			//			if (randomVot) {
			//				var vot = (60 * timeCoefficient) / costCoefficient;
			//				Global.PrintFile.WriteLine("Value of time is {0}",vot);
			//			}
		}

		private ITourModeImpedance GetTourModeImpedance(int minute) {
			var modeImpedance = new TourModeImpedance();

			var useMode =
				Mode ==
				Global.Settings.Modes.SchoolBus
					? Global.Settings.Modes.Hov3
					: Mode;

            IEnumerable<dynamic> pathTypeModels =
                PathTypeModelFactory.Model
                    .Run(Household.RandomUtility, OriginParcel, DestinationParcel, minute, 0, DestinationPurpose, CostCoefficient, TimeCoefficient, true, 1, Person.TransitPassOwnership, Household.OwnsAutomatedVehicles>0, Person.GetTransitFareDiscountFraction(), false, useMode);
            var pathTypeFromOrigin = pathTypeModels.First();

            pathTypeModels = 
				PathTypeModelFactory.Model
					.Run(Household.RandomUtility, DestinationParcel, OriginParcel, minute, 0, DestinationPurpose, CostCoefficient, TimeCoefficient, true, 1, Person.TransitPassOwnership, Household.OwnsAutomatedVehicles>0, Person.GetTransitFareDiscountFraction(), false, useMode);
            var pathTypeFromDestination = pathTypeModels.First();

			modeImpedance.GeneralizedTimeFromOrigin = pathTypeFromOrigin.GeneralizedTimeLogsum;
			modeImpedance.GeneralizedTimeFromDestination = pathTypeFromDestination.GeneralizedTimeLogsum;

			return modeImpedance;
		}

		#endregion

		public sealed class HalfTour : IHalfTour {
			private readonly TourWrapper _t;

			private readonly IPersisterReader<ITrip> _tripReader;
			private readonly ITripCreator _tripWrapperCreator;

			public HalfTour(TourWrapper tour) {
				_t = tour;

				// trip fields

				_tripReader =
					Global
						.Kernel
						.Get<IPersistenceFactory<ITrip>>()
						.Reader;

				_tripWrapperCreator =
					Global
						.Kernel
						.Get<IWrapperFactory<ITripCreator>>()
						.Creator;
			}

			public List<ITripWrapper> Trips { get; private set; }

			public int SimulatedTrips { get; set; }

			public int OneSimulatedTripFlag {
				get { return (SimulatedTrips == 1).ToFlag(); }
			}

			public int TwoSimulatedTripsFlag {
				get { return (SimulatedTrips == 2).ToFlag(); }
			}

			public int ThreeSimulatedTripsFlag {
				get { return (SimulatedTrips == 3).ToFlag(); }
			}

			public int FourSimulatedTripsFlag {
				get { return (SimulatedTrips == 4).ToFlag(); }
			}

			public int FiveSimulatedTripsFlag {
				get { return (SimulatedTrips == 5).ToFlag(); }
			}

			public int FivePlusSimulatedTripsFlag {
				get { return (SimulatedTrips >= 5).ToFlag(); }
			}

			public void SetTrips(int direction) {
				Trips =
					Global.Configuration.IsInEstimationMode
						? GetTripSurveyData(direction)
						: GetTripSimulatedData(direction, 0);
			}

			private List<ITripWrapper> GetTripSurveyData(int direction) {
				var tripsForTours = LoadTripsFromFile().ToList();
				var data =
					tripsForTours
						.Where(trip => trip.Direction == direction)
						.Select(trip =>
							_tripWrapperCreator
								.CreateWrapper(trip, _t, this))
						.ToList();

				return
					direction == Global.Settings.TourDirections.OriginToDestination
						? data.Invert()
						: data;
			}

			private List<ITripWrapper> GetTripSimulatedData(int direction, int sequence) {
				return new List<ITripWrapper> {
					CreateTrip(direction, sequence, true)
				};
			}

			private IEnumerable<ITrip> LoadTripsFromFile() {
				return
					_tripReader
						.Seek(_t._tour.Id, "tour_fk");
			}

			private ITripWrapper CreateTrip(int direction, int sequence, bool isToTourOrigin) {
				return
					_tripWrapperCreator
						.CreateWrapper(_t, _t._tour.Id * 100 + 50 * (direction - 1) + sequence + 1, direction, sequence, isToTourOrigin, this);
			}

			public ITripWrapper CreateNextTrip(ITripWrapper trip, int intermediateStopPurpose, int destinationPurpose) {
				_t.PersonDay.IncrementSimulatedStops(intermediateStopPurpose);

				return
					_tripWrapperCreator
						.CreateWrapper(_t, trip, trip.Id + 1, intermediateStopPurpose, destinationPurpose, this);
			}
		}
	}
}