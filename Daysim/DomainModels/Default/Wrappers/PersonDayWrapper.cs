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
using Daysim.Framework.Factories;
using Ninject;

namespace Daysim.DomainModels.Default.Wrappers {
	[Factory(Factory.WrapperFactory, Category = Category.Wrapper, DataType = DataType.Default)]
	public class PersonDayWrapper : IPersonDayWrapper {
		private IPersonDay _personDay;

		private readonly IPersisterExporter _exporter;

		private readonly IPersonDayCreator _personDayCreator;

		private readonly IPersisterReader<ITour> _tourReader;
		private readonly ITourCreator _tourCreator;

		private int _tourSequence;

		[UsedImplicitly]
		public PersonDayWrapper(IPersonDay personDay, IPersonWrapper personWrapper, IHouseholdDayWrapper householdDayWrapper) {
			_personDay = personDay;

			_exporter =
				Global
					.Kernel
					.Get<IPersistenceFactory<IPersonDay>>()
					.Exporter;

			// person day fields

			_personDayCreator =
				Global
					.Kernel
					.Get<IWrapperFactory<IPersonDayCreator>>()
					.Creator;

			// tour fields

			_tourReader =
				Global
					.Kernel
					.Get<IPersistenceFactory<ITour>>()
					.Reader;

			_tourCreator =
				Global
					.Kernel
					.Get<IWrapperFactory<ITourCreator>>()
					.Creator;

			// relations properties

			Household = personWrapper.Household;
			Person = personWrapper;
			HouseholdDay = householdDayWrapper;

			// domain model properies

			SetExpansionFactor();

			// flags/choice model/etc. properties

			TimeWindow = new TimeWindow();
		}

		#region relations properties

		public IHouseholdWrapper Household { get; set; }

		public IPersonWrapper Person { get; set; }

		public IHouseholdDayWrapper HouseholdDay { get; set; }

		public List<ITourWrapper> Tours { get; set; }

		#endregion

		#region domain model properies

		public int Id {
			get { return _personDay.Id; }
			set { _personDay.Id = value; }
		}

		public int PersonId {
			get { return _personDay.PersonId; }
			set { _personDay.PersonId = value; }
		}

		public int HouseholdDayId {
			get { return _personDay.HouseholdDayId; }
			set { _personDay.HouseholdDayId = value; }
		}

		public int HouseholdId {
			get { return _personDay.HouseholdId; }
			set { _personDay.HouseholdId = value; }
		}

		public int PersonSequence {
			get { return _personDay.PersonSequence; }
			set { _personDay.PersonSequence = value; }
		}

		public int Day {
			get { return _personDay.Day; }
			set { _personDay.Day = value; }
		}

		public int DayBeginsAtHome {
			get { return _personDay.DayBeginsAtHome; }
			set { _personDay.DayBeginsAtHome = value; }
		}

		public int DayEndsAtHome {
			get { return _personDay.DayEndsAtHome; }
			set { _personDay.DayEndsAtHome = value; }
		}

		public int HomeBasedTours {
			get { return _personDay.HomeBasedTours; }
			set { _personDay.HomeBasedTours = value; }
		}

		public int WorkBasedTours {
			get { return _personDay.WorkBasedTours; }
			set { _personDay.WorkBasedTours = value; }
		}

		public int UsualWorkplaceTours {
			get { return _personDay.UsualWorkplaceTours; }
			set { _personDay.UsualWorkplaceTours = value; }
		}

		public int WorkTours {
			get { return _personDay.WorkTours; }
			set { _personDay.WorkTours = value; }
		}

		public int SchoolTours {
			get { return _personDay.SchoolTours; }
			set { _personDay.SchoolTours = value; }
		}

		public int EscortTours {
			get { return _personDay.EscortTours; }
			set { _personDay.EscortTours = value; }
		}

		public int PersonalBusinessTours {
			get { return _personDay.PersonalBusinessTours; }
			set { _personDay.PersonalBusinessTours = value; }
		}

		public int ShoppingTours {
			get { return _personDay.ShoppingTours; }
			set { _personDay.ShoppingTours = value; }
		}

		public int MealTours {
			get { return _personDay.MealTours; }
			set { _personDay.MealTours = value; }
		}

		public int SocialTours {
			get { return _personDay.SocialTours; }
			set { _personDay.SocialTours = value; }
		}

		public int RecreationTours {
			get { return _personDay.RecreationTours; }
			set { _personDay.RecreationTours = value; }
		}

		public int MedicalTours {
			get { return _personDay.MedicalTours; }
			set { _personDay.MedicalTours = value; }
		}

		public int WorkStops {
			get { return _personDay.WorkStops; }
			set { _personDay.WorkStops = value; }
		}

		public int SchoolStops {
			get { return _personDay.SchoolStops; }
			set { _personDay.SchoolStops = value; }
		}

		public int EscortStops {
			get { return _personDay.EscortStops; }
			set { _personDay.EscortStops = value; }
		}

		public int PersonalBusinessStops {
			get { return _personDay.PersonalBusinessStops; }
			set { _personDay.PersonalBusinessStops = value; }
		}

		public int ShoppingStops {
			get { return _personDay.ShoppingStops; }
			set { _personDay.ShoppingStops = value; }
		}

		public int MealStops {
			get { return _personDay.MealStops; }
			set { _personDay.MealStops = value; }
		}

		public int SocialStops {
			get { return _personDay.SocialStops; }
			set { _personDay.SocialStops = value; }
		}

		public int RecreationStops {
			get { return _personDay.RecreationStops; }
			set { _personDay.RecreationStops = value; }
		}

		public int MedicalStops {
			get { return _personDay.MedicalStops; }
			set { _personDay.MedicalStops = value; }
		}

		public int WorkAtHomeDuration {
			get { return (_personDay).WorkAtHomeDuration; }
			set { _personDay.WorkAtHomeDuration = value; }
		}

		public double ExpansionFactor {
			get { return (_personDay).ExpansionFactor; }
			set { _personDay.ExpansionFactor = value; }
		}

		//JLB 20160323
		public int WorkHomeAllDay {
			get { return _personDay.WorkHomeAllDay; }
			set { _personDay.WorkHomeAllDay = value; }
		}

		public int MinutesStudiedHome {
			get { return _personDay.MinutesStudiedHome; }
			set { _personDay.MinutesStudiedHome = value; }
		}

		public int DiaryWeekday {
			get { return _personDay.DiaryWeekday; }
			set { _personDay.DiaryWeekday = value; }
		}

		public int DiaryDaytype {
			get { return _personDay.DiaryDaytype; }
			set { _personDay.DiaryDaytype = value; }
		}

		public int DayStartPurpose {
			get { return _personDay.DayStartPurpose; }
			set { _personDay.DayStartPurpose = value; }
		}

		public int DayJourneyType {
			get { return _personDay.DayJourneyType; }
			set { _personDay.DayJourneyType = value; }
		}

		public int BusinessTours {
			get { return _personDay.BusinessTours; }
			set { _personDay.BusinessTours = value; }
		}

		public int BusinessStops {
			get { return _personDay.BusinessStops; }
			set { _personDay.BusinessStops = value; }
		}
		
		#endregion

		#region flags/choice model/etc. properties

		public ITimeWindow TimeWindow { get; set; }

		public int CreatedWorkTours { get; set; }

		public int CreatedSchoolTours { get; set; }

		public int CreatedEscortTours { get; set; }

		public int CreatedPersonalBusinessTours { get; set; }

		public int CreatedShoppingTours { get; set; }

		public int CreatedMealTours { get; set; }

		public int CreatedSocialTours { get; set; }

		public int CreatedRecreationTours { get; set; }

		public int CreatedMedicalTours { get; set; }

		public int CreatedWorkBasedTours { get; set; }

		public int SimulatedHomeBasedTours { get; set; }

		public int SimulatedWorkTours { get; set; }

		public int SimulatedSchoolTours { get; set; }

		public int SimulatedEscortTours { get; set; }

		public int SimulatedPersonalBusinessTours { get; set; }

		public int SimulatedShoppingTours { get; set; }

		public int SimulatedMealTours { get; set; }

		public int SimulatedSocialTours { get; set; }

		public int SimulatedRecreationTours { get; set; }

		public int SimulatedMedicalTours { get; set; }

		public int SimulatedWorkStops { get; set; }

		public int SimulatedSchoolStops { get; set; }

		public int SimulatedEscortStops { get; set; }

		public int SimulatedPersonalBusinessStops { get; set; }

		public int SimulatedShoppingStops { get; set; }

		public int SimulatedMealStops { get; set; }

		public int SimulatedSocialStops { get; set; }

		public int SimulatedRecreationStops { get; set; }

		public int SimulatedMedicalStops { get; set; }

		public bool IsValid { get; set; }

		public int AttemptedSimulations { get; set; }

		public int PatternType { get; set; }

		public bool HasMandatoryTourToUsualLocation { get; set; }

		public int EscortFullHalfTours { get; set; }

		public int WorksAtHomeFlag { get; set; }

		public int JointTours { get; set; }

		public int EscortJointTours { get; set; }

		public int PersonalBusinessJointTours { get; set; }

		public int ShoppingJointTours { get; set; }

		public int MealJointTours { get; set; }

		public int SocialJointTours { get; set; }

		public int RecreationJointTours { get; set; }

		public int MedicalJointTours { get; set; }

		public bool IsMissingData { get; set; }

		//JLB 20160323
		public int CreatedBusinessTours { get; set; }

		public int SimulatedBusinessTours { get; set; }

		public int SimulatedBusinessStops { get; set; }


		#endregion

		#region wrapper methods

		public virtual int GetTotalTours() {
			return
				WorkTours +
				SchoolTours +
				EscortTours +
				PersonalBusinessTours +
				ShoppingTours +
				MealTours +
				SocialTours +
				RecreationTours +
				MedicalTours +
				BusinessTours;
		}

		public virtual int GetTotalToursExcludingWorkAndSchool() {
			return
				EscortTours +
				PersonalBusinessTours +
				ShoppingTours +
				MealTours +
				SocialTours +
				RecreationTours +
				MedicalTours;
		}

		public virtual int GetCreatedNonMandatoryTours() {
			return
				CreatedEscortTours +
				CreatedPersonalBusinessTours +
				CreatedShoppingTours +
				CreatedMealTours +
				CreatedSocialTours +
				CreatedRecreationTours +
				CreatedMedicalTours;
		}

		public virtual int GetTotalCreatedTours() {
			return
				CreatedWorkTours +
				CreatedSchoolTours +
				CreatedEscortTours +
				CreatedPersonalBusinessTours +
				CreatedShoppingTours +
				CreatedMealTours +
				CreatedSocialTours +
				CreatedRecreationTours +
				CreatedMedicalTours +
				CreatedWorkBasedTours + 
				BusinessTours;
		}

		public virtual int GetTotalCreatedTourPurposes() {
			return
				Math.Min(1, CreatedWorkTours) +
				Math.Min(1, CreatedSchoolTours) +
				Math.Min(1, CreatedEscortTours) +
				Math.Min(1, CreatedPersonalBusinessTours) +
				Math.Min(1, CreatedShoppingTours) +
				Math.Min(1, CreatedMealTours) +
				Math.Min(1, CreatedSocialTours) +
				Math.Min(1, CreatedRecreationTours) +
				Math.Min(1, CreatedMedicalTours) +
				Math.Min(1, CreatedBusinessTours);
		}

		public virtual int GetTotalSimulatedTours() {
			return
				SimulatedWorkTours +
				SimulatedSchoolTours +
				SimulatedEscortTours +
				SimulatedPersonalBusinessTours +
				SimulatedShoppingTours +
				SimulatedMealTours +
				SimulatedSocialTours +
				SimulatedRecreationTours +
				SimulatedMedicalTours +
				SimulatedBusinessTours;
		}

		public virtual int GetTotalStops() {
			return
				WorkStops +
				SchoolStops +
				EscortStops +
				PersonalBusinessStops +
				ShoppingStops +
				MealStops +
				SocialStops +
				RecreationStops +
				MedicalStops +
				BusinessStops;
		}

		public virtual int GetTotalStopsExcludingWorkAndSchool() {
			return
				EscortStops +
				PersonalBusinessStops +
				ShoppingStops +
				MealStops +
				SocialStops +
				RecreationStops +
				MedicalStops;
		}

		public virtual int GetTotalSimulatedStops() {
			return
				SimulatedWorkStops +
				SimulatedSchoolStops +
				SimulatedEscortStops +
				SimulatedPersonalBusinessStops +
				SimulatedShoppingStops +
				SimulatedMealStops +
				SimulatedSocialStops +
				SimulatedRecreationStops +
				SimulatedMedicalStops +
				SimulatedBusinessStops;
		}

		public virtual int GetTotalStopPurposes() {
			return
				(WorkStops > 0 ? 1 : 0) +
				(SchoolStops > 0 ? 1 : 0) +
				(EscortStops > 0 ? 1 : 0) +
				(PersonalBusinessStops > 0 ? 1 : 0) +
				(ShoppingStops > 0 ? 1 : 0) +
				(MealStops > 0 ? 1 : 0) +
				(SocialStops > 0 ? 1 : 0) +
				(RecreationStops > 0 ? 1 : 0) +
				(MedicalStops > 0 ? 1 : 0);
		}

		public virtual bool GetIsWorkOrSchoolPattern() {
			return WorkTours + SchoolTours > 0;
		}

		public virtual bool GetIsOtherPattern() {
			return WorkTours + SchoolTours == 0;
		}

		public virtual bool HomeBasedToursExist() {
			return HomeBasedTours > 1;
		}

		public virtual bool TwoOrMoreWorkToursExist() {
			return WorkTours > 1;
		}

		public virtual bool WorkStopsExist() {
			return WorkStops >= 1;
		}

		public virtual bool SimulatedToursExist() {
			return GetTotalSimulatedTours() > 1;
		}

		public virtual bool OnlyHomeBasedToursExist() {
			return HomeBasedTours == 1;
		}

		public virtual bool IsFirstSimulatedHomeBasedTour() {
			return SimulatedHomeBasedTours == 1;
		}

		public virtual bool IsLaterSimulatedHomeBasedTour() {
			return SimulatedHomeBasedTours > 1;
		}

		public virtual bool SimulatedWorkStopsExist() {
			return SimulatedWorkStops > 0;
		}

		public virtual bool SimulatedSchoolStopsExist() {
			return SimulatedSchoolStops > 0;
		}

		public virtual bool SimulatedEscortStopsExist() {
			return SimulatedEscortStops > 0;
		}

		public virtual bool SimulatedPersonalBusinessStopsExist() {
			return SimulatedPersonalBusinessStops > 0;
		}

		public virtual bool SimulatedShoppingStopsExist() {
			return SimulatedShoppingStops > 0;
		}

		public virtual bool SimulatedMealStopsExist() {
			return SimulatedMealStops > 0;
		}

		public virtual bool SimulatedSocialStopsExist() {
			return SimulatedSocialStops > 0;
		}

		public virtual int GetJointTourParticipationPriority() {
			if (PatternType == Global.Settings.PatternTypes.Home || GetCreatedNonMandatoryTours() >= 3 || GetTotalCreatedTourPurposes() >= 3) {
				return 9;
			}

			if (Person.PersonType == Global.Settings.PersonTypes.FullTimeWorker) {
				return 8;
			}

			if (Person.PersonType == Global.Settings.PersonTypes.PartTimeWorker) {
				return 5;
			}

			if (Person.PersonType == Global.Settings.PersonTypes.RetiredAdult) {
				return 4;
			}

			if (Person.PersonType == Global.Settings.PersonTypes.NonWorkingAdult) {
				return 2;
			}

			if (Person.PersonType == Global.Settings.PersonTypes.UniversityStudent) {
				return 6;
			}

			if (Person.PersonType == Global.Settings.PersonTypes.DrivingAgeStudent) {
				return 7;
			}

			if (Person.PersonType == Global.Settings.PersonTypes.ChildAge5Through15) {
				return 3;
			}

			if (Person.PersonType == Global.Settings.PersonTypes.ChildUnder5) {
				return 1;
			}

			return 9;
		}

		public virtual int GetJointHalfTourParticipationPriority() {
			if (PatternType == Global.Settings.PatternTypes.Home) {
				return 9;
			}

			if (!Person.IsDrivingAge && PatternType == Global.Settings.PatternTypes.Optional) {
				return 9;
			}

			if (Person.PersonType == Global.Settings.PersonTypes.FullTimeWorker) {
				return 5;
			}

			if (Person.PersonType == Global.Settings.PersonTypes.PartTimeWorker) {
				return 6;
			}

			if (Person.PersonType == Global.Settings.PersonTypes.RetiredAdult) {
				return 8;
			}

			if (Person.PersonType == Global.Settings.PersonTypes.NonWorkingAdult) {
				return 7;
			}

			if (Person.PersonType == Global.Settings.PersonTypes.UniversityStudent) {
				return 4;
			}

			if (Person.PersonType == Global.Settings.PersonTypes.DrivingAgeStudent) {
				return 3;
			}

			if (Person.PersonType == Global.Settings.PersonTypes.ChildAge5Through15) {
				return 1;
			}

			if (Person.PersonType == Global.Settings.PersonTypes.ChildUnder5) {
				return 2;
			}

			return 9;
		}

		public virtual void InitializeTours() {
			Tours =
				Global.Configuration.IsInEstimationMode
					? GetTourSurveyData()
					: new List<ITourWrapper>();
		}

		public virtual void SetTours() {
			Tours =
				Global.Configuration.IsInEstimationMode
					? GetTourSurveyData()
					: GetTourSimulatedData();
		}

		public virtual void GetMandatoryTourSimulatedData(IPersonDayWrapper personDay, List<ITourWrapper> tours) {
			tours.AddRange(CreateToursByPurpose(Global.Settings.Purposes.Work, personDay.UsualWorkplaceTours));
			//JLB 20160323
			foreach (var tour in tours) {
				tour.DestinationParcel = personDay.Person.UsualWorkParcel;
				tour.DestinationParcelId = personDay.Person.UsualWorkParcelId;
				tour.DestinationZoneKey = ChoiceModelFactory.ZoneKeys[personDay.Person.UsualWorkParcel.ZoneId];
				tour.DestinationAddressType = Global.Settings.AddressTypes.UsualWorkplace;
			}

			tours.AddRange(CreateToursByPurpose(Global.Settings.Purposes.Business, ((PersonDayWrapper) personDay).BusinessTours));
			tours.AddRange(CreateToursByPurpose(Global.Settings.Purposes.School, personDay.SchoolTours));

			foreach (var tour in tours.Where(tour => tour.DestinationPurpose == Global.Settings.Purposes.School)) {
				tour.DestinationParcel = personDay.Person.UsualSchoolParcel;
				tour.DestinationParcelId = personDay.Person.UsualSchoolParcelId;
				tour.DestinationZoneKey = ChoiceModelFactory.ZoneKeys[personDay.Person.UsualSchoolParcel.ZoneId];
				tour.DestinationAddressType = Global.Settings.AddressTypes.UsualSchool;
			}
		}

//			foreach (var tour in tours) {
//				tour.DestinationParcel = personDay.Person.UsualWorkParcel;
//				tour.DestinationParcelId = personDay.Person.UsualWorkParcelId;
//				tour.DestinationZoneKey = ChoiceModelFactory.ZoneKeys[personDay.Person.UsualWorkParcel.ZoneId];
//				tour.DestinationAddressType = Global.Settings.AddressTypes.UsualWorkplace;
//			}
//
//			tours.AddRange(CreateToursByPurpose(Global.Settings.Purposes.Work, personDay.WorkTours - personDay.UsualWorkplaceTours));
//			tours.AddRange(CreateToursByPurpose(Global.Settings.Purposes.School, personDay.SchoolTours));
//
//			foreach (var tour in tours.Where(tour => tour.DestinationPurpose == Global.Settings.Purposes.School)) {
//				tour.DestinationParcel = personDay.Person.UsualSchoolParcel;
//				tour.DestinationParcelId = personDay.Person.UsualSchoolParcelId;
//				tour.DestinationZoneKey = ChoiceModelFactory.ZoneKeys[personDay.Person.UsualSchoolParcel.ZoneId];
//				tour.DestinationAddressType = Global.Settings.AddressTypes.UsualSchool;
//			}
//		}

		public virtual void GetIndividualTourSimulatedData(IPersonDayWrapper personDay, List<ITourWrapper> tours) {
			tours.AddRange(CreateToursByPurpose(Global.Settings.Purposes.Escort, CreatedEscortTours - EscortFullHalfTours));
			tours.AddRange(CreateToursByPurpose(Global.Settings.Purposes.PersonalBusiness, CreatedPersonalBusinessTours));
			tours.AddRange(CreateToursByPurpose(Global.Settings.Purposes.Shopping, CreatedShoppingTours));
			tours.AddRange(CreateToursByPurpose(Global.Settings.Purposes.Meal, CreatedMealTours));
			tours.AddRange(CreateToursByPurpose(Global.Settings.Purposes.Social, CreatedSocialTours));
			tours.AddRange(CreateToursByPurpose(Global.Settings.Purposes.Recreation, CreatedRecreationTours));
			tours.AddRange(CreateToursByPurpose(Global.Settings.Purposes.Medical, CreatedMedicalTours));
		}

		public virtual void IncrementSimulatedTours(int destinationPurpose) {
			SimulatedHomeBasedTours++;

			if (destinationPurpose == Global.Settings.Purposes.Work) {
				SimulatedWorkTours++;
			}
			else if (destinationPurpose == Global.Settings.Purposes.School) {
				SimulatedSchoolTours++;
			}
			else if (destinationPurpose == Global.Settings.Purposes.Escort) {
				SimulatedEscortTours++;
			}
			else if (destinationPurpose == Global.Settings.Purposes.PersonalBusiness) {
				SimulatedPersonalBusinessTours++;
			}
			else if (destinationPurpose == Global.Settings.Purposes.Shopping) {
				SimulatedShoppingTours++;
			}
			else if (destinationPurpose == Global.Settings.Purposes.Meal) {
				SimulatedMealTours++;
			}
			else if (destinationPurpose == Global.Settings.Purposes.Social) {
				SimulatedSocialTours++;
			}
			else if (destinationPurpose == Global.Settings.Purposes.Recreation) {
				SimulatedRecreationTours++;
			}
			else if (destinationPurpose == Global.Settings.Purposes.Medical) {
				SimulatedMedicalTours++;
			}
		}

		public virtual void IncrementSimulatedStops(int destinationPurpose) {
			//JLB 20160323
			if (destinationPurpose == Global.Settings.Purposes.Work) {
				SimulatedWorkStops++;
			}
			else if (destinationPurpose == Global.Settings.Purposes.Business) {
				SimulatedBusinessStops++;
			}
			else if (destinationPurpose == Global.Settings.Purposes.School) {
				SimulatedSchoolStops++;
			}
			else if (destinationPurpose == Global.Settings.Purposes.Escort) {
				SimulatedEscortStops++;
			}
			else if (destinationPurpose == Global.Settings.Purposes.PersonalBusiness) {
				SimulatedPersonalBusinessStops++;
			}
			else if (destinationPurpose == Global.Settings.Purposes.Shopping) {
				SimulatedShoppingStops++;
			}
			else if (destinationPurpose == Global.Settings.Purposes.Meal) {
				SimulatedMealStops++;
			}
			else if (destinationPurpose == Global.Settings.Purposes.Social) {
				SimulatedSocialStops++;
			}
			else if (destinationPurpose == Global.Settings.Purposes.Recreation) {
				SimulatedRecreationStops++;
			}
			else if (destinationPurpose == Global.Settings.Purposes.Medical) {
				SimulatedMedicalStops++;
			}
		}

	
			//if (destinationPurpose == Global.Settings.Purposes.Work) {
			//	SimulatedWorkStops++;
			//}
			//else if (destinationPurpose == Global.Settings.Purposes.School) {
			//	SimulatedSchoolStops++;
			//}
			//else if (destinationPurpose == Global.Settings.Purposes.Escort) {
			//	SimulatedEscortStops++;
			//}
			//else if (destinationPurpose == Global.Settings.Purposes.PersonalBusiness) {
			//	SimulatedPersonalBusinessStops++;
			//}
			//else if (destinationPurpose == Global.Settings.Purposes.Shopping) {
			//	SimulatedShoppingStops++;
			//}
			//else if (destinationPurpose == Global.Settings.Purposes.Meal) {
			//	SimulatedMealStops++;
			//}
			//else if (destinationPurpose == Global.Settings.Purposes.Social) {
			//	SimulatedSocialStops++;
			//}
			//else if (destinationPurpose == Global.Settings.Purposes.Recreation) {
			//	SimulatedRecreationStops++;
			//}
			//else if (destinationPurpose == Global.Settings.Purposes.Medical) {
			//	SimulatedMedicalStops++;
			//}
		//}

		public virtual ITourWrapper GetEscortTour(int originAddressType, int originParcelId, int originZoneKey) {
			var tour = CreateTour(originAddressType, originParcelId, originZoneKey, Global.Settings.Purposes.Escort);

			_personDay.EscortTours++;

			Tours.Add(tour);

			return tour;
		}

		public virtual ITourWrapper GetNewTour(int originAddressType, int originParcelId, int originZoneKey, int purpose) {
			var tour = CreateTour(originAddressType, originParcelId, originZoneKey, purpose);

			Tours.Add(tour);

			if (purpose == Global.Settings.Purposes.Escort) {
				_personDay.EscortTours++;
			}
			else if (purpose == Global.Settings.Purposes.PersonalBusiness) {
				_personDay.PersonalBusinessTours++;
			}
			else if (purpose == Global.Settings.Purposes.Shopping) {
				_personDay.ShoppingTours++;
			}
			else if (purpose == Global.Settings.Purposes.Meal) {
				_personDay.MealTours++;
			}
			else if (purpose == Global.Settings.Purposes.Social) {
				_personDay.SocialTours++;
			}
			else if (purpose == Global.Settings.Purposes.Recreation) {
				_personDay.RecreationTours++;
			}
			else if (purpose == Global.Settings.Purposes.Medical) {
				_personDay.MedicalTours++;
			}

			return tour;
		}

		public virtual int GetNextTourSequence() {
			return ++_tourSequence;
		}

		public virtual int GetCurrentTourSequence() {
			return _tourSequence;
		}

		public virtual void SetHomeBasedNonMandatoryTours() {
			HomeBasedTours = GetTotalCreatedTours();
			EscortTours = CreatedEscortTours;
			PersonalBusinessTours = CreatedPersonalBusinessTours;
			ShoppingTours = CreatedShoppingTours;
			MealTours = CreatedMealTours;
			SocialTours = CreatedSocialTours;
			RecreationTours = CreatedRecreationTours;
			MedicalTours = CreatedMedicalTours;
		}

		//JLB 20150323
		public virtual bool SimulatedBusinessStopsExist() {
			return SimulatedBusinessStops > 0;
		}



		#endregion

		#region persistence methods

		private IEnumerable<ITour> LoadToursFromFile() {
			return
				_tourReader
					.Seek(_personDay.Id, "person_day_fk");
		}

		private ITourWrapper CreateTour(int originAddressType, int originParcelId, int originZoneKey, int purpose) {
			var model = _tourCreator.CreateModel();

			model.Id = _personDay.Id * 10 + GetNextTourSequence();
			model.PersonId = _personDay.PersonId;
			model.PersonDayId = _personDay.Id;
			model.HouseholdId = _personDay.HouseholdId;
			model.PersonSequence = _personDay.PersonSequence;
			model.Day = _personDay.Day;
			model.Sequence = GetCurrentTourSequence();
			model.OriginAddressType = originAddressType;
			model.OriginParcelId = originParcelId;
			model.OriginZoneKey = originZoneKey;
			model.OriginDepartureTime = 180;
			model.DestinationArrivalTime = 180;
			model.DestinationDepartureTime = 180;
			model.OriginArrivalTime = 180;
			model.DestinationPurpose = purpose;
			model.PathType = 1;
			model.ExpansionFactor = Household.ExpansionFactor;

			return _tourCreator.CreateWrapper(model, this, purpose, false);
		}

		protected IEnumerable<ITourWrapper> CreateToursByPurpose(int purpose, int totalTours) {
			var data = new List<ITourWrapper>();

			for (var i = 0; i < totalTours; i++) {
				var tour = CreateTour(Global.Settings.AddressTypes.Home, Household.ResidenceParcelId, Household.ResidenceZoneKey, purpose);

				//tour.DestinationPurpose = purpose;

				data.Add(tour);
			}

			return data;
		}

		private List<ITourWrapper> GetTourSurveyData() {
			var data = new List<ITourWrapper>();
			var toursForPersonDay = LoadToursFromFile().ToList();
			var tours = toursForPersonDay.Where(t => t.ParentTourSequence == 0);

			foreach (var tour in tours) {
				var wrapper = GetNewWrapper(tour, this);

				data.Add(wrapper);

				var parentTourSequence = tour.Sequence;
				var subtours = toursForPersonDay.Where(st => st.ParentTourSequence == parentTourSequence);

				foreach (var subtour in subtours) {
					wrapper
						.Subtours
						.Add(
							_tourCreator
								.CreateWrapper(subtour, wrapper, Global.Settings.Purposes.PersonalBusiness, false));
				}
			}

			return data;
		}

		private List<ITourWrapper> GetTourSimulatedData() {
			var data = new List<ITourWrapper>();

			data.AddRange(CreateToursByPurpose(Global.Settings.Purposes.Work, WorkTours));
			data.AddRange(CreateToursByPurpose(Global.Settings.Purposes.School, SchoolTours));
			data.AddRange(CreateToursByPurpose(Global.Settings.Purposes.Escort, EscortTours));
			data.AddRange(CreateToursByPurpose(Global.Settings.Purposes.PersonalBusiness, PersonalBusinessTours));
			data.AddRange(CreateToursByPurpose(Global.Settings.Purposes.Shopping, ShoppingTours));
			data.AddRange(CreateToursByPurpose(Global.Settings.Purposes.Meal, MealTours));
			data.AddRange(CreateToursByPurpose(Global.Settings.Purposes.Social, SocialTours));

			return data;
		}

		#endregion

		#region init/utility/export methods

		public void Export() {
			_exporter.Export(_personDay);
		}

		public virtual void Reset() {
			TimeWindow = new TimeWindow();

			_personDay = ResetPersonDay();

			_tourSequence = 0;

			SimulatedHomeBasedTours = 0;

			SimulatedWorkTours = 0;
			SimulatedSchoolTours = 0;
			SimulatedEscortTours = 0;
			SimulatedPersonalBusinessTours = 0;
			SimulatedShoppingTours = 0;
			SimulatedMealTours = 0;
			SimulatedSocialTours = 0;

			SimulatedWorkStops = 0;
			SimulatedSchoolStops = 0;
			SimulatedEscortStops = 0;
			SimulatedPersonalBusinessStops = 0;
			SimulatedShoppingStops = 0;
			SimulatedMealStops = 0;
			SimulatedSocialStops = 0;

			CreatedWorkTours = 0;
			CreatedSchoolTours = 0;
			CreatedEscortTours = 0;
			CreatedPersonalBusinessTours = 0;
			CreatedShoppingTours = 0;
			CreatedMealTours = 0;
			CreatedSocialTours = 0;
			CreatedRecreationTours = 0;
			CreatedMedicalTours = 0;

			WorkTours = 0;
			SchoolTours = 0;
			EscortTours = 0;
			PersonalBusinessTours = 0;
			ShoppingTours = 0;
			MealTours = 0;
			SocialTours = 0;
			RecreationTours = 0;
			MedicalTours = 0;

			WorkStops = 0;
			SchoolStops = 0;
			EscortStops = 0;
			PersonalBusinessStops = 0;
			ShoppingStops = 0;
			MealStops = 0;
			SocialStops = 0;
			RecreationStops = 0;
			MedicalStops = 0;

			UsualWorkplaceTours = 0;
			HomeBasedTours = 0;
			WorkBasedTours = 0;

			JointTours = 0;
			EscortJointTours = 0;
			PersonalBusinessJointTours = 0;
			ShoppingJointTours = 0;
			MealJointTours = 0;
			SocialJointTours = 0;
			RecreationJointTours = 0;
			MedicalJointTours = 0;

			EscortFullHalfTours = 0;

			HasMandatoryTourToUsualLocation = false;
			WorksAtHomeFlag = 0;

			//JLB 20160323
			CreatedBusinessTours = 0;
			SimulatedBusinessTours = 0;
			SimulatedBusinessStops = 0;
			BusinessTours = 0;
			BusinessStops = 0;

		}

		protected virtual IPersonDay ResetPersonDay() {
			//JLB 20160323
			//	_personDay = new PersonDay {
			//	Id = Id,
			//	PersonId = PersonId,
			//	HouseholdDayId = HouseholdDayId,
			//	HouseholdId = HouseholdId,
			//	PersonSequence = PersonSequence,
			//	Day = Day,
			//	DayBeginsAtHome = DayBeginsAtHome,
			//	DayEndsAtHome = DayEndsAtHome,
			//	ExpansionFactor = ExpansionFactor
			//};

			//return _personDay;
		//}

			
			var model = _personDayCreator.CreateModel();

			model.Id = _personDay.Id;
			model.PersonId = _personDay.PersonId;
			model.HouseholdDayId = _personDay.HouseholdDayId;
			model.HouseholdId = _personDay.HouseholdId;
			model.PersonSequence = _personDay.PersonSequence;
			model.Day = _personDay.Day;
			model.DayBeginsAtHome = _personDay.DayBeginsAtHome;
			model.DayEndsAtHome = _personDay.DayEndsAtHome;
			model.ExpansionFactor = _personDay.ExpansionFactor;

			return model;
		}

		public static void Close() {
			Global
				.Kernel
				.Get<IPersistenceFactory<IPersonDay>>()
				.Close();
		}

		public override string ToString() {
			var builder = new StringBuilder();

			builder
				.AppendLine(string.Format("Person Day ID: {0}, Person ID: {1}",
					_personDay.Id,
					_personDay.PersonId));

			builder
				.AppendLine(string.Format("Household ID: {0}, Person Sequence: {1}, Day: {2}",
					_personDay.HouseholdId,
					_personDay.PersonSequence,
					_personDay.Day));

			builder
				.AppendLine(string.Format("Work Tours: {0}", _personDay.WorkTours))
				.AppendLine(string.Format("School Tours: {0}", _personDay.SchoolTours))
				.AppendLine(string.Format("Escort Tours: {0}", _personDay.EscortTours))
				.AppendLine(string.Format("Personal Business Tours: {0}", _personDay.PersonalBusinessTours))
				.AppendLine(string.Format("Shopping Tours: {0}", _personDay.ShoppingTours))
				.AppendLine(string.Format("Meal Tours: {0}", _personDay.MealTours))
				.AppendLine(string.Format("Social Tours: {0}", _personDay.SocialTours));

			builder
				.AppendLine(string.Format("Work Stops: {0}", _personDay.WorkStops))
				.AppendLine(string.Format("School Stops: {0}", _personDay.SchoolStops))
				.AppendLine(string.Format("Escort Stops: {0}", _personDay.EscortStops))
				.AppendLine(string.Format("Personal Business Stops: {0}", _personDay.PersonalBusinessStops))
				.AppendLine(string.Format("Shopping Stops: {0}", _personDay.ShoppingStops))
				.AppendLine(string.Format("Meal Stops: {0}", _personDay.MealStops))
				.AppendLine(string.Format("Social Stops: {0}", _personDay.SocialStops));

			return builder.ToString();
		}

		private void SetExpansionFactor() {
			_personDay.ExpansionFactor = Household.ExpansionFactor*Global.Configuration.HouseholdSamplingRateOneInX;
		}

		private ITourWrapper GetNewWrapper(ITour tour, IPersonDayWrapper personDayWrapper) {
			return
				_tourCreator
					.CreateWrapper(tour, personDayWrapper, tour.DestinationPurpose, true);
		}

		#endregion
	}
}