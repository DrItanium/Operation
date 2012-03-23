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
	public delegate void UnfulfilledHookResolver(Operation next);
	public abstract class Operation : List<Operation>
	{
		public int Index { get; set; }
		private Queue<UnfulfilledHookResolver> unfulfilledHooks;
		public Operation()
		{
			unfulfilledHooks = new Queue<UnfulfilledHookResolver>();
		}
		
		public abstract Operation Coalesce();
		///<summary>
		///Performs an initial fulfill by linking
		///nodes together that can be linked together
		///within the confines of the current operation
		///Unfulfillable hooks need to be registered through
		///the RegisterHook method so they can be linked to the
		///next node within the parent operation of this operation
		///</summary>
		public abstract void InitialFulfill();
		public void RegisterHook(UnfulfilledHookResolver resolver)
		{
			unfulfilledHooks.Enqueue(resolver);
		}
		///<summary>
		///Returns the head node of the current operation chain
		///</summary>
		public abstract Operation First();
		public Operation OnTrueHook { get; set; }
		public Operation OnFalseHook { get; set; }
		public void ResolveHooks(Operation next)
		{
			if(unfulfilledHooks.Count > 0)
			{
				do
				{
				  var hook = unfulfilledHooks.Dequeue();
					hook(next);
				}while(unfulfilledHooks.Count > 0);
			}
		}
		public abstract void Build(IGraphBuilder builder);
		public virtual void Enumerate(IGraphBuilder builder)
		{
			Index = builder.Index(this);
		}
	}
}
