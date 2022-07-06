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

using System.ComponentModel;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Org.Anization.BizTalk.Environment.Settings
{
	public class NetworkZonesFixture
	{
		[Theory]
		[InlineData(NetworkZones.All, NetworkZones.All, true)]
		[InlineData(NetworkZones.All, NetworkZones.B2B, true)]
		[InlineData(NetworkZones.All, NetworkZones.Intranet, true)]
		[InlineData(NetworkZones.All, NetworkZones.None, false)]
		[InlineData(NetworkZones.B2B, NetworkZones.All, false)]
		[InlineData(NetworkZones.B2B, NetworkZones.B2B, true)]
		[InlineData(NetworkZones.B2B, NetworkZones.Intranet, false)]
		[InlineData(NetworkZones.B2B, NetworkZones.None, false)]
		[InlineData(NetworkZones.None, NetworkZones.B2B, false)]
		[InlineData(NetworkZones.None, NetworkZones.None, false)]
		public void Match(NetworkZones networkZones, NetworkZones otherNetworkZones, bool result)
		{
			networkZones.Match(otherNetworkZones).Should().Be(result);
		}

		[Theory]
		[InlineData(1 << 7, NetworkZones.None)]
		[InlineData(NetworkZones.None, 1 << 3)]
		public void MatchThrows(NetworkZones networkZones, NetworkZones otherNetworkZones)
		{
			Invoking(() => networkZones.Match(otherNetworkZones)).Should().Throw<InvalidEnumArgumentException>();
		}

		[Theory]
		[InlineData(NetworkZones.All)]
		[InlineData(NetworkZones.B2B | NetworkZones.Intranet)]
		[InlineData((1 << 6) + (1 << 3))]
		[InlineData(NetworkZones.None)]
		public void SingleReturnsFalse(NetworkZones networkZones)
		{
			networkZones.Single().Should().BeFalse();
		}

		[Theory]
		[InlineData(NetworkZones.B2B)]
		[InlineData(NetworkZones.Intranet)]
		[InlineData(1 << 0)]
		[InlineData(1 << 1)]
		[InlineData(1 << 2)]
		[InlineData(1 << 3)]
		[InlineData(1 << 4)]
		[InlineData(1 << 5)]
		[InlineData(1 << 6)]
		[InlineData(1 << 7)]
		[InlineData(1 << 8)]
		public void SingleReturnsTrue(NetworkZones networkZones)
		{
			networkZones.Single().Should().BeTrue();
		}
	}
}
