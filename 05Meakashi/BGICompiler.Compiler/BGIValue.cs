using Antlr.Runtime.Tree;
using Assets.Scripts.Core.Buriko;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace BGICompiler.Compiler
{
	public class BGIValue
	{
		public BurikoValueType Type;

		private ITree baseTree;

		private ITree tree;

		private BurikoMathType mathType;

		public BGIValue(ITree treein)
		{
			baseTree = treein;
			tree = treein.GetChild(0);
			switch (treein.Text)
			{
			case "TYPEINT":
				Type = BurikoValueType.Int;
				break;
			case "TYPEHEX":
				Type = BurikoValueType.Int;
				break;
			case "TYPESTRING":
				Type = BurikoValueType.String;
				break;
			case "TYPEBOOL":
				Type = BurikoValueType.Bool;
				break;
			case "TYPENULL":
				Type = BurikoValueType.Null;
				break;
			case "TYPEFUNCTION":
				Type = BurikoValueType.Operation;
				break;
			case "TYPEVARIABLE":
				Type = BurikoValueType.Variable;
				baseTree = tree;
				tree = baseTree.GetChild(0);
				break;
			case "VAR":
				Type = BurikoValueType.Variable;
				break;
			}
			if (Type == BurikoValueType.None && IsMath(treein.Text))
			{
				Type = BurikoValueType.Math;
			}
		}

		public bool GetBool()
		{
			if (Type != BurikoValueType.Bool)
			{
				throw new Exception("GetBool called for variable type " + Type);
			}
			if (tree.Text == "TRUE")
			{
				return true;
			}
			if (tree.Text == "FALSE")
			{
				return false;
			}
			throw new Exception("Unexpected type found for variable type Bool!");
		}

		public bool IsMath(string s)
		{
			if (s != null)
			{
				if (_003C_003Ef__switch_0024map2 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(11);
					dictionary.Add("==", 0);
					dictionary.Add("!=", 1);
					dictionary.Add("<=", 2);
					dictionary.Add(">=", 3);
					dictionary.Add(">", 4);
					dictionary.Add("<", 5);
					dictionary.Add("+", 6);
					dictionary.Add("-", 7);
					dictionary.Add("*", 8);
					dictionary.Add("/", 9);
					dictionary.Add("%", 10);
					_003C_003Ef__switch_0024map2 = dictionary;
				}
				if (_003C_003Ef__switch_0024map2.TryGetValue(s, out int value))
				{
					switch (value)
					{
					case 0:
						mathType = BurikoMathType.Equals;
						return true;
					case 1:
						mathType = BurikoMathType.NotEquals;
						return true;
					case 2:
						mathType = BurikoMathType.LessThanOrEquals;
						return true;
					case 3:
						mathType = BurikoMathType.GreaterThanOrEquals;
						return true;
					case 4:
						mathType = BurikoMathType.GreaterThan;
						return true;
					case 5:
						mathType = BurikoMathType.LessThan;
						return true;
					case 6:
						mathType = BurikoMathType.Add;
						return true;
					case 7:
						mathType = BurikoMathType.Subtract;
						return true;
					case 8:
						mathType = BurikoMathType.Multiply;
						return true;
					case 9:
						mathType = BurikoMathType.Divide;
						return true;
					case 10:
						mathType = BurikoMathType.Modulus;
						return true;
					}
				}
			}
			return false;
		}

		public void OutputMath(BinaryWriter output)
		{
			BGIValue bGIValue = new BGIValue(baseTree.GetChild(0));
			BGIValue bGIValue2 = new BGIValue(baseTree.GetChild(1));
			output.Write((short)Type);
			output.Write((short)mathType);
			bGIValue.Output();
			bGIValue2.Output();
		}

		public void OutputVar(BinaryWriter output)
		{
			output.Write((short)Type);
			output.Write(tree.Text);
			int num = 1;
			if (baseTree.ChildCount <= 1)
			{
				output.Write((short)2);
				output.Write(-1);
				output.Write(value: false);
			}
			else
			{
				ITree child = baseTree.GetChild(num);
				if (child.Text == "INDEX")
				{
					BGIValue bGIValue = new BGIValue(child.GetChild(0));
					bGIValue.Output();
					num++;
				}
				else
				{
					output.Write((short)2);
					output.Write(-1);
				}
				if (baseTree.ChildCount <= num)
				{
					output.Write(value: false);
				}
				else
				{
					ITree child2 = baseTree.GetChild(num);
					if (child2.Text == "MEMBER")
					{
						output.Write(value: true);
						ITree child3 = child2.GetChild(0);
						BGIValue bGIValue2 = new BGIValue(child3);
						bGIValue2.Output();
					}
					else
					{
						output.Write(value: false);
					}
				}
			}
		}

		public void Output()
		{
			BinaryWriter output = BGItoMG.Instance.Output;
			switch (Type)
			{
			case BurikoValueType.Int:
			{
				int value = (!tree.Text.StartsWith("0x")) ? int.Parse(tree.Text) : int.Parse(tree.Text.Substring(2), NumberStyles.HexNumber);
				output.Write((short)Type);
				output.Write(value);
				break;
			}
			case BurikoValueType.String:
			{
				string text = tree.Text;
				output.Write((short)Type);
				output.Write(text.Substring(1, text.Length - 2).Replace("\\\"", "\""));
				break;
			}
			case BurikoValueType.Bool:
				output.Write((short)Type);
				output.Write(GetBool());
				break;
			case BurikoValueType.Null:
				output.Write((short)Type);
				break;
			case BurikoValueType.Math:
				OutputMath(output);
				break;
			case BurikoValueType.Operation:
			{
				output.Write((short)Type);
				OperationHandler operationHandler = new OperationHandler();
				operationHandler.ParseOperation(tree);
				break;
			}
			case BurikoValueType.Variable:
				OutputVar(output);
				break;
			default:
				throw new Exception("Unhandled Variable Type " + Type);
			}
		}
	}
}
