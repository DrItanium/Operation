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
	public class Block : Operation
	{
		public Block()
		{

		}
		public override Operation First()
		{
			return this[0];
		}
		public override void InitialFulfill()
		{
			for(int i = 0; i < Count - 1; i++)
			{
				this[i].InitialFulfill();
				this[i].ResolveHooks(this[i + 1].First());

			}
			this[Count - 1].InitialFulfill();
			RegisterHook((x) => { this[Count - 1].ResolveHooks(x); });
			RegisterHook((x) => { this[Count - 1].OnTrueHook = x; });
		}
		public override Operation Coalesce()
		{
			Block outputBlock = new Block();
			outputBlock.OnTrueHook = OnTrueHook;
			outputBlock.OnFalseHook = OnFalseHook;
			int amountSeen = 0;
			foreach(var v in this)
			{
				if(v is StandardOperation)
				{
					amountSeen++;
				}	
				else
				{
					if(amountSeen > 0)
					{
						outputBlock.Add(new StandardOperation()); 
						amountSeen = 0;
					}
					outputBlock.Add(v.Coalesce());
				}
			}
			if(amountSeen > 0) //final list of operations
				outputBlock.Add(new StandardOperation());
			return outputBlock;
		}
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(GetType().ToString());
			foreach(var v in this)
				sb.AppendLine(v.ToString());
			return sb.ToString();
		}
		public override void Enumerate(IGraphBuilder builder)
		{
			base.Enumerate(builder);
			foreach(var v in this)
				v.Enumerate(builder);
		}
		public override void Build(IGraphBuilder builder)
		{
			foreach(var v in this)
			{
				v.Build(builder); //thats it :)
			}
			//get the last item
		}
	}
}
