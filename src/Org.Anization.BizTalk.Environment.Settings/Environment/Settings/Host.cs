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

using Be.Stateless.BizTalk.Dsl.Binding.Convention;
using Org.Anization.BizTalk.Environment.Settings.Convention;

namespace Org.Anization.BizTalk.Environment.Settings
{
	public static class Host
	{
		public static HostResolutionPolicy B2B => _policy.B2B;

		public static HostResolutionPolicy Default => _policy;

		public static HostResolutionPolicy Intranet => _policy.Intranet;

		private static readonly AnyNetworkZoneHostResolutionPolicy _policy = new();
	}
}
