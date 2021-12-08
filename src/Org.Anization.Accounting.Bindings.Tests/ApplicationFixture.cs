#region Copyright & License

// Copyright © 2012 - 2021 François Chabot
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

using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Be.Stateless.BizTalk.Install;
using Be.Stateless.BizTalk.Unit.Dsl.Binding;
using FluentAssertions;
using Xunit;
using static FluentAssertions.FluentActions;

namespace Org.Anization.Accounting
{
	public class ApplicationFixture : ApplicationBindingFixture<Application>
	{
		[Theory]
		[InlineData(TargetEnvironment.DEVELOPMENT)]
		[InlineData(TargetEnvironment.BUILD)]
		[InlineData(TargetEnvironment.ACCEPTANCE)]
		[InlineData(TargetEnvironment.PRODUCTION)]
		public void GenerateApplicationBinding(string targetEnvironment)
		{
			Invoking(() => GenerateApplicationBindingForTargetEnvironment(targetEnvironment)).Should().NotThrow();
		}

		[Theory(Skip = "To be run manually.")]
		[InlineData(TargetEnvironment.DEVELOPMENT)]
		[InlineData(TargetEnvironment.BUILD)]
		[InlineData(TargetEnvironment.ACCEPTANCE)]
		[InlineData(TargetEnvironment.PRODUCTION)]
		public void GenerateApplicationBindingFile(string targetEnvironment)
		{
			var path = Path.Combine(
				GetRepoRootPath(),
				$"Org.Anization.Accounting.{targetEnvironment}.bindings.xml"
			);
			File.WriteAllText(path, GenerateApplicationBindingForTargetEnvironment(targetEnvironment), Encoding.Unicode);
		}

		private string GetRepoRootPath([CallerFilePath] string sourceFilePath = "")
		{
			return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(sourceFilePath)!, @"..\..\"));
		}
	}
}
