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
	public class WhileLoopOperation : NonStandardOperation
	{
		public override Operation First()
		{
			return Condition.First();
		}
		public BooleanOperation Condition
		{
			get
			{
				return (BooleanOperation)this[0];
			}
			set
			{
				this[0] = value;
			}
		}
		public Block Body
		{
			get
			{
				return (Block)this[1];
			}
			set
			{
				this[1] = value;
			}
		}
		public WhileLoopOperation(BooleanOperation condition,
				Block body)
		{
			Add(condition);
			Add(body);
		}
		public override Operation Coalesce()
		{
			Condition = (BooleanOperation)Condition.Coalesce();
			Body = (Block)Body.Coalesce();
			return this;
		}

		public override void InitialFulfill()
		{
			Condition.OnTrueHook = Body.First();
			Condition.InitialFulfill();
			RegisterHook((x) => Condition.ResolveHooks(x));
			Body.InitialFulfill();
			Body.ResolveHooks(Condition.First()); //link back
#if DEBUG
			Console.WriteLine("Body.First().Index = {0}", Body.First().Index);
			Console.WriteLine("Body[0].OnTrueHook.Index = {0}, Body[0].OnTrueHook.First().Index = {1}", Body.First().OnTrueHook.Index,
					Body.First().OnTrueHook.First().Index);
#endif
		}
		public override void Enumerate(IGraphBuilder builder)
		{
			Index = builder.Index(this);
			foreach(var v in this)
				v.Enumerate(builder);
		}
		public override void Build(IGraphBuilder builder)
		{
			//lets make sure...
			Condition.Build(builder);
			Body.Build(builder);
		}
	}
	public class DoWhileLoopOperation : NonStandardOperation
	{
		public Block Body { get { return (Block)this[0]; } set { this[0] = value; } }
		public BooleanOperation Condition { get { return (BooleanOperation)this[1]; } set { this[1] = value; } }

		public DoWhileLoopOperation(Block body, BooleanOperation condition)
		{
			Add(body);
			Add(condition);
		}
		public override Operation First()
		{
			return Body.First();
		}
		public override void Enumerate(IGraphBuilder builder)
		{
			Index = builder.Index(this);
			foreach(var v in this)
				v.Enumerate(builder);
		}
		public override Operation Coalesce()
		{
			Body = (Block)Body.Coalesce();
			Condition = (BooleanOperation)Condition.Coalesce();
			return this;
		}
		public override void Build(IGraphBuilder builder)
		{
			Body.Build(builder);
			Condition.Build(builder);
		}
		public override void InitialFulfill()
		{
			Condition.OnTrueHook = Body.First();
			Condition.InitialFulfill();
			Body.InitialFulfill();
			RegisterHook((x) => Condition.ResolveHooks(x));
			Body.ResolveHooks(Condition.First());
		}
	}
}
