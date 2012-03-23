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
	public interface IGraphLink 
	{
		int From { get; }
		int To { get; }
		string Label { get; }
		string ToString();
	}
	public abstract class GraphBuilder<T> : List<T>, IGraphBuilder
		where T : IGraphLink
	{
		public const string DEFAULT_GRAPH_NAME = "whatever";
		private List<Operation> operations; 
		public GraphBuilder()
		{
			operations = new List<Operation>();
		}
		public int Index(Operation op)
		{
			int index = operations.Count;
			operations.Add(op);
			return index; 
		}
		public abstract void Link(int i0, int i1, string label);
		public void Link(int i0, int i1)
		{
			Link(i0,i1,string.Empty);
		}
		public abstract string ToString(string graphName);
		public override string ToString()
		{
			return ToString(DEFAULT_GRAPH_NAME);
		}
		public void PrintRegisteredOperations()
		{
			for(int i = 0; i < operations.Count; i++)
				Console.WriteLine("/*{0}: {1}*/",i, operations[i]);
		}
	}
}
