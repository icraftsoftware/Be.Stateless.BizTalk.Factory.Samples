#region Copyright & License

// Copyright © 2012 - 2022 François Chabot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

extern alias ExplorerOM;
using System;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.Dsl.Binding.Scheduling;
using Be.Stateless.BizTalk.MicroPipelines;
using ExplorerOM::Microsoft.BizTalk.BtsScheduleHelper;

namespace Org.Anization.Accounting
{
	public class BribeReceiveLocation : ReceiveLocation
	{
		public BribeReceiveLocation()
		{
			Name = "Bribe Receive Location";
			ReceivePipeline = new ReceivePipeline<PassThruReceive>();
			Transport.Adapter = new FileAdapter.Inbound(
				a => {
					// ReSharper disable once StringLiteralTypo
					a.ReceiveFolder = @"c:\file\evil\bribes";
					a.FileMask = "*.xml";
					a.RenameReceivedFiles = true;
				});
			Transport.Host = "BizTalkServerApplication";
			Transport.Schedule = new Schedule {
				AutomaticallyAdjustForDaylightSavingTime = true,
				StartDate = new DateTime(2022, 1, 20),
				StopDate = new DateTime(2022, 2, 14),
				TimeZone = TimeZoneInfo.Utc,

				#region RecurringServiceWindow

				//ServiceWindow = RecurringServiceWindow.None

				#endregion

				#region CalendricalMonthlyServiceWindow

				//ServiceWindow = new CalendricalMonthlyServiceWindow {
				//	StartTime = new Time(8, 0),
				//	StopTime = new Time(20, 0),
				//	Months = Month.January | Month.April | Month.July | Month.October,
				//	Days = MonthDay.Day01 | MonthDay.Day08 | MonthDay.Day15 | MonthDay.Day22 | MonthDay.Day29,
				//	OnLastDay = true
				//}

				#endregion

				#region DailyServiceWindow

				//ServiceWindow = new DailyServiceWindow {
				//	StartTime = new Time(8, 0),
				//	StopTime = new Time(20, 0),
				//	From = new DateTime(2022, 1, 20),
				//	Interval = 4
				//}

				#endregion

				#region OrdinalMonthlyServiceWindow

				//ServiceWindow = new OrdinalMonthlyServiceWindow {
				//	StartTime = new Time(8, 0),
				//	StopTime = new Time(20, 0),
				//	Months = Month.January | Month.March | Month.June,
				//	Ordinality = OrdinalType.First,
				//	WeekDays = BtsDayOfWeek.Monday | BtsDayOfWeek.Friday
				//}

				#endregion

				#region WeeklyServiceWindow

				ServiceWindow = new WeeklyServiceWindow {
					StartTime = new Time(8, 0),
					StopTime = new Time(20, 0),
					From = new DateTime(2022, 1, 20),
					Interval = 2,
					WeekDays = BtsDayOfWeek.Monday | BtsDayOfWeek.Friday
				}

				#endregion
			};
		}
	}
}
