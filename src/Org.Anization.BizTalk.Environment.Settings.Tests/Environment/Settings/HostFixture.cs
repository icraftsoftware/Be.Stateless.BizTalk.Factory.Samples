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

using FluentAssertions;
using Org.Anization.BizTalk.Environment.Settings.Convention;
using Xunit;

namespace Org.Anization.BizTalk.Environment.Settings
{
	[Collection("TargetEnvironment")]
	public class HostFixture
	{
		[Fact]
		public void B2BPolicy()
		{
			Host.B2B.Should().BeOfType<NetworkZoneBoundHostResolutionPolicy>();
		}

		[Fact]
		public void DefaultPolicy()
		{
			Host.Default.Should().BeOfType<AnyNetworkZoneHostResolutionPolicy>();
		}

		[Fact]
		public void IntranetPolicy()
		{
			Host.B2B.Should().BeOfType<NetworkZoneBoundHostResolutionPolicy>();
		}
	}
}
