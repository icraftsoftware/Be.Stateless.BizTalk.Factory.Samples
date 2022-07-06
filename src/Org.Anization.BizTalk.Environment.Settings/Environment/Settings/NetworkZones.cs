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

using System;
using System.ComponentModel;

namespace Org.Anization.BizTalk.Environment.Settings
{
	[Flags]
	public enum NetworkZones
	{
		None = 0,
		Intranet = 1 << 0,
		B2B = 1 << 1,
		All = Intranet | B2B
	}

	public static class NetworkZonesExtensions
	{
		public static bool Match(this NetworkZones networkZones, NetworkZones otherNetworkZones)
		{
			if (!Enum.IsDefined(typeof(NetworkZones), networkZones)) throw new InvalidEnumArgumentException(nameof(networkZones), (int) networkZones, typeof(NetworkZones));
			if (!Enum.IsDefined(typeof(NetworkZones), otherNetworkZones)) throw new InvalidEnumArgumentException(nameof(otherNetworkZones), (int) otherNetworkZones, typeof(NetworkZones));
			return otherNetworkZones != 0 && (networkZones & otherNetworkZones) == otherNetworkZones;
		}

		/// <summary>
		/// Returns whether one single flag is set.
		/// </summary>
		/// <remarks>
		/// The statement checks if the value of <paramref name="networkZones"/> is a power of two. The idea is that only single
		/// flag values will be power of two. Setting more than one flag will result in a non power of two value of <paramref
      /// name="networkZones"/>.
		/// </remarks>
		/// <seealso href="https://stackoverflow.com/a/8949772/1789441"/>
		/// <seealso href="https://stackoverflow.com/a/1662162/1789441"/>
		/// <seealso href="https://stackoverflow.com/a/51094793/1789441"/>
		/// <seealso href="https://stackoverflow.com/a/12483864/1789441"/>
		public static bool Single(this NetworkZones networkZones)
		{
			return networkZones != 0 && (networkZones & (networkZones - 1)) == 0;
		}
	}
}
