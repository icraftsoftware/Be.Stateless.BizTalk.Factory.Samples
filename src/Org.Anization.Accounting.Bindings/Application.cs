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

using Be.Stateless.BizTalk.Component;
using Be.Stateless.BizTalk.Dsl.Binding;
using Be.Stateless.BizTalk.Dsl.Binding.Adapter;
using Be.Stateless.BizTalk.MicroComponent;
using Be.Stateless.BizTalk.MicroPipelines;
using Be.Stateless.BizTalk.Schema;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Utilities;
using Microsoft.XLANGs.BaseTypes;

namespace Org.Anization.Accounting
{
	public class Application : ApplicationBinding
	{
		public Application()
		{
			Name = "Accounting";
			Description = "Org.Anization's BizTalk Server Accounting Application";

			ReceivePorts.Add(
				ReceivePort(
					rp => {
						rp.Name = "Billing Receive Port";
						rp.ReceiveLocations.Add(
							ReceiveLocation(
								rl => {
									rl.Name = "Credit Note Receive Location";
									rl.ReceivePipeline = new ReceivePipeline<XmlReceive>(
										p => {
											p.Decoder<MicroPipelineComponent>(
													c => {
														c.Components = new IMicroComponent[] {
															new ContextBuilder { BuilderType = typeof(int) }
														};
													})
												.Disassembler<XmlDasmComp>(
													d => {
														d.AllowUnrecognizedMessage = true;
														d.RecoverableInterchangeProcessing = true;
														d.DocumentSpecNames = new SchemaList() {
															new SchemaWithNone(SchemaMetadata.For<Any>().DocumentSpec.DocSpecStrongName)
														};
													})
												.Validator<MicroPipelineComponent>(
													c => {
														c.Components = new IMicroComponent[] {
															new XsltRunner { MapType = typeof(string) }
														};
													});
										});
									rl.Transport.Adapter = new FileAdapter.Inbound(a => { a.ReceiveFolder = @"c:\file\billing\credits"; });
									rl.Transport.Host = "BizTalkServerApplication";
								})
						);
					}),
				new EvilBankReceivePort()
			);
		}
	}
}
