﻿// Copyright 2005-2008 Mark A. Bradley and John L. Bowman
// Copyright 2011-2013 John Bowman, Mark Bradley, and RSG, Inc.
// You may not possess or use this file without a License for its use.
// Unless required by applicable law or agreed to in writing, software
// distributed under a License for its use is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.


using System;
using System.Collections.Generic;
using System.Linq;
using Daysim.ChoiceModels.Shared.Models;
using Daysim.DomainModels;
using Daysim.DomainModels.Default;
using Daysim.DomainModels.Default.Models;
using Daysim.DomainModels.Default.Wrappers;
using Daysim.DomainModels.Extensions;
using Daysim.Framework.ChoiceModels;
using Daysim.Framework.Coefficients;
using Daysim.Framework.Core;
using Daysim.Framework.DomainModels.Models;
using Daysim.Framework.DomainModels.Wrappers;
using Daysim.Framework.Roster;

namespace Daysim.ChoiceModels.Actum.Models {
	public class OtherHomeBasedTourModeModel : ChoiceModel {
		private const string CHOICE_MODEL_NAME = "ActumOtherHomeBasedTourModeModel";
		private const int TOTAL_NESTED_ALTERNATIVES = 4;
		private const int TOTAL_LEVELS = 2;
		private const int MAX_PARAMETER = 199;
		private const int THETA_PARAMETER = 99;

		private readonly int[] _nestedAlternativeIds = new[] { 0, 19, 19, 20, 21, 21, 22, 0, 0 };
		private readonly int[] _nestedAlternativeIndexes = new[] { 0, 0, 0, 1, 2, 2, 3, 0, 0 };

		public override void RunInitialize(ICoefficientsReader reader = null)
		{
			Initialize(CHOICE_MODEL_NAME, Global.GetInputPath(Global.Configuration.OtherHomeBasedTourModeModelCoefficients), Global.Settings.Modes.TotalModes, TOTAL_NESTED_ALTERNATIVES, TOTAL_LEVELS, MAX_PARAMETER);
		}

		public void Run(TourWrapper tour) {
			if (tour == null) {
				throw new ArgumentNullException("tour");
			}

			tour.PersonDay.ResetRandom(40 + tour.Sequence - 1);

			if (Global.Configuration.IsInEstimationMode) {
				if (Global.Configuration.EstimationModel != CHOICE_MODEL_NAME) {
					return;
				}
			}

			var choiceProbabilityCalculator = _helpers[ParallelUtility.GetBatchFromThreadId()].GetChoiceProbabilityCalculator(tour.Id);

			if (_helpers[ParallelUtility.GetBatchFromThreadId()].ModelIsInEstimationMode) {
				if (tour.DestinationParcel == null || tour.Mode <= Global.Settings.Modes.None || tour.Mode > Global.Settings.Modes.Transit) {
					return;
				}

				var pathTypeModels =
					PathTypeModel.RunAll(
					tour.Household.RandomUtility,
						tour.OriginParcel,
						tour.DestinationParcel,
						tour.DestinationArrivalTime,
						tour.DestinationDepartureTime,
						tour.DestinationPurpose,
						tour.CostCoefficient,
						tour.TimeCoefficient,
						tour.Person.IsDrivingAge,
						tour.Household.VehiclesAvailable,
						tour.Person.GetTransitFareDiscountFraction(),
						false);

				var pathTypeModel = pathTypeModels.First(x => x.Mode == tour.Mode);

				if (!pathTypeModel.Available) {
					return;
				}

				RunModel(choiceProbabilityCalculator, tour, pathTypeModels, tour.DestinationParcel, tour.Mode);

				choiceProbabilityCalculator.WriteObservation();
			}
			else {
				var pathTypeModels =
					PathTypeModel.RunAll(
					tour.Household.RandomUtility,
						tour.OriginParcel,
						tour.DestinationParcel,
						tour.DestinationArrivalTime,
						tour.DestinationDepartureTime,
						tour.DestinationPurpose,
						tour.CostCoefficient,
						tour.TimeCoefficient,
						tour.Person.IsDrivingAge,
						tour.Household.VehiclesAvailable,
						tour.Person.GetTransitFareDiscountFraction(),
						false);

				RunModel(choiceProbabilityCalculator, tour, pathTypeModels, tour.DestinationParcel);

				var chosenAlternative = choiceProbabilityCalculator.SimulateChoice(tour.Household.RandomUtility);

				if (chosenAlternative == null) {
					Global.PrintFile.WriteNoAlternativesAvailableWarning(CHOICE_MODEL_NAME, "Run", tour.PersonDay.Id);
					tour.Mode = Global.Settings.Modes.Hov3;
					if (!Global.Configuration.IsInEstimationMode) {
						tour.PersonDay.IsValid = false;
					}
					return;
				}

				var choice = (int) chosenAlternative.Choice;

				tour.Mode = choice;
				var chosenPathType = pathTypeModels.First(x => x.Mode == choice);
				tour.PathType = chosenPathType.PathType;
				tour.ParkAndRideNodeId = choice == Global.Settings.Modes.ParkAndRide ? chosenPathType.PathParkAndRideNodeId : 0;
			}
		}

		public ChoiceProbabilityCalculator.Alternative RunNested(TourWrapper tour, IParcelWrapper destinationParcel) {
			if (tour == null) {
				throw new ArgumentNullException("tour");
			}

			var choiceProbabilityCalculator = _helpers[ParallelUtility.GetBatchFromThreadId()].GetNestedChoiceProbabilityCalculator();

			var pathTypeModels =
				PathTypeModel.RunAll(
				tour.Household.RandomUtility,
					tour.OriginParcel,
					destinationParcel,
					tour.DestinationArrivalTime,
					tour.DestinationDepartureTime,
					tour.DestinationPurpose,
					tour.CostCoefficient,
					tour.TimeCoefficient,
					tour.Person.IsDrivingAge,
					tour.Household.VehiclesAvailable,
					tour.Person.GetTransitFareDiscountFraction(),
					false);

			RunModel(choiceProbabilityCalculator, tour, pathTypeModels, destinationParcel);

			return choiceProbabilityCalculator.SimulateChoice(tour.Household.RandomUtility);
		}

		private void RunModel(ChoiceProbabilityCalculator choiceProbabilityCalculator, ITourWrapper tour, IEnumerable<PathTypeModel> pathTypeModels, IParcelWrapper destinationParcel, int choice = Constants.DEFAULT_VALUE) {
			var household = tour.Household;
			var householdTotals = household.HouseholdTotals;
			var person = tour.Person;
			var personDay = tour.PersonDay;

			// household inputs
			var income0To25KFlag = household.Has0To25KIncome.ToFlag();
			var childrenUnder5 = householdTotals.ChildrenUnder5;
			var childrenAge5Through15 = householdTotals.ChildrenAge5Through15;
			var nonworkingAdults = householdTotals.NonworkingAdults;
			var retiredAdults = householdTotals.RetiredAdults;
			var onePersonHouseholdFlag = household.IsOnePersonHousehold.ToFlag();
			var twoPersonHouseholdFlag = household.IsTwoPersonHousehold.ToFlag();
			var noCarsInHouseholdFlag = household.GetFlagForNoCarsInHousehold(household.VehiclesAvailable);
			var carsLessThanDriversFlag = household.GetFlagForCarsLessThanDrivers(household.VehiclesAvailable);
			var carsLessThanWorkersFlag = household.GetFlagForCarsLessThanWorkers(household.VehiclesAvailable);

			// person inputs
			var maleFlag = person.IsMale.ToFlag();
			var ageBetween51And98Flag = person.AgeIsBetween51And98.ToFlag();

			// tour inputs
			var shoppingTourFlag = (tour.DestinationPurpose == Global.Settings.Purposes.Shopping).ToFlag();
			var mealTourFlag = (tour.DestinationPurpose == Global.Settings.Purposes.Meal).ToFlag();
			var socialOrRecreationTourFlag = (tour.DestinationPurpose == Global.Settings.Purposes.Social).ToFlag();

			// remaining inputs
			var originParcel = tour.OriginParcel;
			var parkingDuration = ChoiceModelUtility.GetParkingDuration(person.IsFulltimeWorker);
			var destinationParkingCost = destinationParcel.ParkingCostBuffer1(parkingDuration);

			double escortPercentage;
			double nonEscortPercentage;

			ChoiceModelUtility.SetEscortPercentages(personDay, out escortPercentage, out nonEscortPercentage, true);

			foreach (var pathTypeModel in pathTypeModels) {
				var mode = pathTypeModel.Mode;
				var available = pathTypeModel.Mode != Global.Settings.Modes.ParkAndRide && pathTypeModel.Available;
				var generalizedTimeLogsum = pathTypeModel.GeneralizedTimeLogsum;

				var alternative = choiceProbabilityCalculator.GetAlternative(mode, available, choice == mode);
				alternative.Choice = mode;

				alternative.AddNestedAlternative(_nestedAlternativeIds[pathTypeModel.Mode], _nestedAlternativeIndexes[pathTypeModel.Mode], THETA_PARAMETER);

				if (!available) {
					continue;
				}

				alternative.AddUtilityTerm(2, generalizedTimeLogsum * tour.TimeCoefficient);

				if (mode == Global.Settings.Modes.Transit) {
					alternative.AddUtilityTerm(20, 1);
					alternative.AddUtilityTerm(21, noCarsInHouseholdFlag);
					alternative.AddUtilityTerm(120, shoppingTourFlag);
					//						alternative.AddUtility(121, mealTourFlag);
					//						alternative.AddUtility(129, destinationParcel.MixedUse2Index1());
					alternative.AddUtilityTerm(128, destinationParcel.TotalEmploymentDensity1());
					//						alternative.AddUtility(127, destinationParcel.NetIntersectionDensity1());
					//						alternative.AddUtility(126, originParcel.NetIntersectionDensity1());
					//						alternative.AddUtility(125, originParcel.HouseholdDensity1());
					alternative.AddUtilityTerm(124, originParcel.MixedUse2Index1());
					//						alternative.AddUtility(123, Math.Log(destinationParcel.StopsTransitBuffer1+1));
					//						alternative.AddUtility(122, Math.Log(originParcel.StopsTransitBuffer1+1));
				}
				else if (mode == Global.Settings.Modes.Hov3) {
					alternative.AddUtilityTerm(1, (destinationParkingCost * tour.CostCoefficient / ChoiceModelUtility.CPFACT3));
					alternative.AddUtilityTerm(30, 1);
					alternative.AddUtilityTerm(31, childrenUnder5);
					alternative.AddUtilityTerm(32, childrenAge5Through15);
					alternative.AddUtilityTerm(34, nonworkingAdults + retiredAdults);
					alternative.AddUtilityTerm(35, pathTypeModel.PathDistance.AlmostEquals(0) ? 0 : Math.Log(pathTypeModel.PathDistance));
					alternative.AddUtilityTerm(38, onePersonHouseholdFlag);
					alternative.AddUtilityTerm(39, twoPersonHouseholdFlag);
					alternative.AddUtilityTerm(41, noCarsInHouseholdFlag);
					alternative.AddUtilityTerm(43, carsLessThanWorkersFlag);
					alternative.AddUtilityTerm(133, escortPercentage);
					alternative.AddUtilityTerm(134, nonEscortPercentage);
					alternative.AddUtilityTerm(136, shoppingTourFlag);
					alternative.AddUtilityTerm(137, mealTourFlag);
					alternative.AddUtilityTerm(138, socialOrRecreationTourFlag);
				}
				else if (mode == Global.Settings.Modes.Hov2) {
					alternative.AddUtilityTerm(1, (destinationParkingCost * tour.CostCoefficient / ChoiceModelUtility.CPFACT2));
					alternative.AddUtilityTerm(31, childrenUnder5);
					alternative.AddUtilityTerm(32, childrenAge5Through15);
					alternative.AddUtilityTerm(34, nonworkingAdults + retiredAdults);
					alternative.AddUtilityTerm(35, pathTypeModel.PathDistance.AlmostEquals(0) ? 0 : Math.Log(pathTypeModel.PathDistance));
					alternative.AddUtilityTerm(40, 1);
					alternative.AddUtilityTerm(41, noCarsInHouseholdFlag);
					alternative.AddUtilityTerm(43, carsLessThanWorkersFlag);
					alternative.AddUtilityTerm(48, onePersonHouseholdFlag);
					alternative.AddUtilityTerm(133, escortPercentage);
					alternative.AddUtilityTerm(134, nonEscortPercentage);
					alternative.AddUtilityTerm(136, shoppingTourFlag);
					alternative.AddUtilityTerm(137, mealTourFlag);
					alternative.AddUtilityTerm(138, socialOrRecreationTourFlag);
				}
				else if (mode == Global.Settings.Modes.Sov) {
					alternative.AddUtilityTerm(1, (destinationParkingCost * tour.CostCoefficient));
					alternative.AddUtilityTerm(50, 1);
					alternative.AddUtilityTerm(52, carsLessThanDriversFlag);
					alternative.AddUtilityTerm(54, income0To25KFlag);
					alternative.AddUtilityTerm(131, escortPercentage);
					alternative.AddUtilityTerm(132, nonEscortPercentage);
				}
				else if (mode == Global.Settings.Modes.Bike) {
					var class1Dist = Global.Configuration.PathImpedance_BikeUseTypeSpecificDistanceFractions ?
						ImpedanceRoster.GetValue("class1distance", mode, Global.Settings.PathTypes.FullNetwork,
							Global.Settings.ValueOfTimes.DefaultVot, tour.DestinationArrivalTime, originParcel, destinationParcel).Variable : 0;
					var class2Dist = Global.Configuration.PathImpedance_BikeUseTypeSpecificDistanceFractions ?
						ImpedanceRoster.GetValue("class2distance", mode, Global.Settings.PathTypes.FullNetwork,
							Global.Settings.ValueOfTimes.DefaultVot, tour.DestinationArrivalTime, originParcel, destinationParcel).Variable : 0;
					var worstDist = Global.Configuration.PathImpedance_BikeUseTypeSpecificDistanceFractions ?
						ImpedanceRoster.GetValue("worstdistance", mode, Global.Settings.PathTypes.FullNetwork,
							Global.Settings.ValueOfTimes.DefaultVot, tour.DestinationArrivalTime, originParcel, destinationParcel).Variable : 0;

					alternative.AddUtilityTerm(60, 1);
					alternative.AddUtilityTerm(61, maleFlag);
					alternative.AddUtilityTerm(63, ageBetween51And98Flag);
					alternative.AddUtilityTerm(160, socialOrRecreationTourFlag);
					alternative.AddUtilityTerm(169, destinationParcel.MixedUse4Index1());
					alternative.AddUtilityTerm(168, destinationParcel.TotalEmploymentDensity1());
					//						alternative.AddUtility(167, destinationParcel.NetIntersectionDensity1());
					//						alternative.AddUtility(166, originParcel.NetIntersectionDensity1());
					alternative.AddUtilityTerm(165, originParcel.HouseholdDensity1());
					alternative.AddUtilityTerm(164, originParcel.MixedUse4Index1());
					alternative.AddUtilityTerm(161, (class1Dist > 0).ToFlag());
					alternative.AddUtilityTerm(162, (class2Dist > 0).ToFlag());
					alternative.AddUtilityTerm(163, (worstDist > 0).ToFlag());
				}
				else if (mode == Global.Settings.Modes.Walk) {
					alternative.AddUtilityTerm(73, ageBetween51And98Flag);
					alternative.AddUtilityTerm(171, mealTourFlag);
					alternative.AddUtilityTerm(172, socialOrRecreationTourFlag);
					//						alternative.AddUtility(179, destinationParcel.MixedUse4Index1());
					alternative.AddUtilityTerm(178, destinationParcel.HouseholdDensity1());
					//						alternative.AddUtility(177, destinationParcel.NetIntersectionDensity1());
					//						alternative.AddUtility(176, originParcel.NetIntersectionDensity1());
					alternative.AddUtilityTerm(175, originParcel.HouseholdDensity1());
					//						alternative.AddUtility(174, originParcel.MixedUse4Index1());
				}
			}
		}
	}
}