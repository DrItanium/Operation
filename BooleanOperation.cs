//Copyright 2012 Joshua Scoggins. All rights reserved.
//
//Redistribution and use in source and binary forms, with or without modification, are
//permitted provided that the following conditions are met:
//
//   1. Redistributions of source code must retain the above copyright notice, this list of
//      conditions and the following disclaimer.
//
//   2. Redistributions in binary form must reproduce the above copyright notice, this list
//      of conditions and the following disclaimer in the documentation and/or other materials
//      provided with the distribution.
//
//THIS SOFTWARE IS PROVIDED BY Joshua Scoggins ``AS IS'' AND ANY EXPRESS OR IMPLIED
//WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Joshua Scoggins OR
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
//ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
//ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//The views and conclusions contained in the software and documentation are those of the
//authors and should not be interpreted as representing official policies, either expressed
//or implied, of Joshua Scoggins. 
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;
using System.Linq;

namespace Frameworks.Operation
{
	public class BooleanOperation : NonStandardOperation
	{
		public string Operation { get; set; }
		public override Operation First()
		{
			if(this[0] is StandardOperation)
				return this[0];
			else
				return this[0].First();
		}
		public BooleanOperation(Operation cond0, string operation, Operation cond1)
		{
			Operation = operation;
			if(cond0 != null)
				Add(cond0);
			if(cond1 != null)
				Add(cond1);
		}
		public override Operation Coalesce()
		{
			return this;
		}
		public override string ToString()
		{
			return GetType().ToString();
		}
		public override void InitialFulfill()
		{
			//this is where we need to do some if statements
			//to see what our contents are
			//in the end we will only register the OnTrue links
			//it's up to the upper level 
			Operation cond0 = null;
			Operation cond1 = null;
			switch(Count)
			{
				case 2:
					cond0 = this[0];
					cond1 = this[1];
					switch(Operation)
					{
						case "&&":
							RegisterHook((x) => { cond0.OnFalseHook = x; } );
							cond0.OnTrueHook = cond1;
							cond0.InitialFulfill();
							cond1.InitialFulfill();
							if(OnFalseHook != null)
								cond1.ResolveHooks(OnFalseHook.First());
								//cond1.OnFalseHook = OnFalseHook.First();
							else
							  RegisterHook((x) => { cond1.OnFalseHook = x; } );
							cond1.OnTrueHook = this.OnTrueHook;
							RegisterHook((x) => cond1.ResolveHooks(x) );
							RegisterHook((x) => cond0.ResolveHooks(x) );
							break;
						case "||":
							cond0.OnTrueHook = this.OnTrueHook;
							cond1.OnTrueHook = this.OnTrueHook;
							cond1.InitialFulfill();
							cond0.InitialFulfill();
							cond0.ResolveHooks(cond1.First()); //ensure
							cond0.OnFalseHook = cond1.First();
							//Console.WriteLine("\tcond1.First().Index = {0}, cond1.Index = {1}", cond1.First().Index, cond1.Index);
							//Console.WriteLine("\tcond0.First().Index = {0}, cond0.Index = {1}", cond0.First().Index, cond0.Index);
							if(OnFalseHook != null)
								cond1.ResolveHooks(OnFalseHook.First());
							else
								RegisterHook((x) => { cond1.OnFalseHook = x; });
							//The issue comes from the fact that the outer
							//boolean expression is not providing the correct target
							//
							RegisterHook((x) =>
									{
									cond1.ResolveHooks(x);
									});
							RegisterHook((x) => cond0.ResolveHooks(x));
							break;
						default:
							//Console.WriteLine("//We are at default");
							cond0.InitialFulfill();
							cond1.InitialFulfill();
							RegisterHook((x) => cond1.ResolveHooks(x));
							RegisterHook((x) => cond0.ResolveHooks(x));
							break;
					}
					//get those up and working
					break;
				case 1:
					cond0 = this[0];
					cond0.InitialFulfill();
					//register the operation
					RegisterHook((x) => cond0.ResolveHooks(x));
					break;
				case 0:
				default:
					RegisterHook((x) => { OnTrueHook = x; });
					break;
			}
		}
		public override void Enumerate(IGraphBuilder builder)
		{
			base.Enumerate(builder);
			foreach(var v in this) //lets forget about the number of arguments
				v.Enumerate(builder); //there we go :)
		}
		public override void Build(IGraphBuilder builder)
		{
			//alright so we need to create links
			//Console.WriteLine("We have {0} elements", Count);
			Operation v0 = this[0];
			Operation v1 = this[1];
		//	if(v0 is BinaryOperation)
		//		BuildCondition(v0, builder);
			v0.Build(builder);
		//	if(v1 is BinaryOperation)
		//		BuildCondition(v1, builder);
			v1.Build(builder);
		}
		private void BuildCondition(Operation v0,  IGraphBuilder builder)
		{
			string trueLabel = "[label = \"t\"]";
			string falseLabel = "[label = \"f\"]";
			if(v0.OnFalseHook == null)
				trueLabel = string.Empty;	
			if(v0.OnTrueHook != null)
				builder.Link(v0.Index, v0.OnTrueHook.Index, trueLabel);
			if(v0.OnFalseHook != null)
				builder.Link(v0.Index, v0.OnFalseHook.Index, falseLabel);
		}
	}
}
