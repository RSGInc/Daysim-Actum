﻿// Copyright 2005-2008 Mark A. Bradley and John L. Bowman
// Copyright 2011-2013 John Bowman, Mark Bradley, and RSG, Inc.
// You may not possess or use this file without a License for its use.
// Unless required by applicable law or agreed to in writing, software
// distributed under a License for its use is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

using System.Runtime.InteropServices;
using Daysim.Framework.DomainModels.Models;
using Daysim.Framework.Factories;
using Daysim.Framework.Persistence;

namespace Daysim.DomainModels.Actum.Models {
	[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
	[Factory(Factory.PersistenceFactory, Category = Category.Model, DataType = DataType.Actum)]
	public sealed class ParcelNode : IParcelNode {
		[ColumnName("id")]
		public int Id { get; set; }

		[ColumnName("node_id")]
		public int NodeId { get; set; }
	}
}