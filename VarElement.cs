namespace VarFileLib; 

using System;
using System.Collections.Generic;
using System.Text;

public struct VarElement {
	private Dictionary<string, VarElement> _dictionary;
	private string _value;
	private Type _type;

	#region Element methods
	public void SetElement(string name, VarElement varElement) {
		if (_type == Type.Var) throw new Exception($"Tried to write element name '{name}' to a var!");
		_dictionary[name] = varElement;
	}
	public VarElement GetElement(string name) {
		if (_type == Type.Var) throw new Exception($"Tried to read var '{name}' as an element");
		
		int period = name.IndexOf('.');
		return period != -1 ? _dictionary[name[..period]].GetElement(name[(period+1)..]) : _dictionary[name];
	}
	private VarElement? GetElementWithoutExploding(string name) {
		if (_type == Type.Var) throw new Exception($"Tried to read var '{name}' as an element");
		
		int period = name.IndexOf('.');
		if (period != -1) {
			string subkey = name[..period];
			if (_dictionary.ContainsKey(subkey)) return _dictionary[subkey].GetElement(name[(period + 1)..]);
			return null;
		}

		if (_dictionary.ContainsKey(name)) {
			return _dictionary[name];
		}

		return null;
	}
	public bool HasElement(string name) {
		if (_type == Type.Var) throw new Exception($"Tried to read var '{name}' as an element");
		int period = name.IndexOf('.');
		return period != -1 ? _dictionary[name[..period]].HasElement(name[(period+1)..]) : _dictionary.ContainsKey(name);
	}
	#endregion

	#region Element var methods

	public string ReadElementString(string path) => GetElement(path).ReadString();
	public string ReadElementStringOrDefault(string path, string def) => HasElement(path) ? ReadElementString(path) : def;

	public int ReadElementInt(string path) => GetElement(path).ReadInt();
	public int ReadElementIntOrDefault(string path, int def) => HasElement(path) ? ReadElementInt(path) : def;
	
	public float ReadElementFloat(string path) => GetElement(path).ReadFloat();
	public float ReadElementFloatOrDefault(string path, float def) => HasElement(path) ? ReadElementFloat(path) : def;

	public bool ReadElementBool(string path) => GetElement(path).ReadBool();
	public bool ReadElementBoolOrDefault(string path, bool def) => HasElement(path) ? ReadElementBool(path) : def;


	#endregion

	#region Var methods
	public string ReadString() {
		MakeSure(Type.Var);
		return _value;
	}
	public string ReadStringOrDefault(string def) {
		return ReadString() ?? def;
	}

	public int ReadInt() {
		MakeSure(Type.Var);
		if (int.TryParse(_value, out int v)) {
			return v;
		}

		throw new Exception($"Failed to read var value as an int, for value: '{_value}'");
	}
	public int ReadIntOrDefault(int def) {
		MakeSure(Type.Var);
		if (_value != null && int.TryParse(_value, out int v)) {
			return v;
		}
		return def;
	}

	public float ReadFloat() {
		MakeSure(Type.Var);
		if (float.TryParse(_value, out float v)) {
			return v;
		}

		throw new Exception($"Failed to read var value as a float, for value: '{_value}'");
	}
	public float ReadFloatOrDefault(float def) {
		MakeSure(Type.Var);
		if (_value != null && float.TryParse(_value, out float v)) {
			return v;
		}
		return def;
	}

	public bool ReadBool() {
		MakeSure(Type.Var);
		if (bool.TryParse(_value, out bool v)) {
			return v;
		}

		throw new Exception($"Failed to read var value as a bool, for value: '{_value}'");
	}
	public bool ReadBoolOrDefault(bool def) {
		MakeSure(Type.Var);
		if (_value != null && bool.TryParse(_value, out bool v)) {
			return v;
		}
		return def;
	}
	#endregion

	private void MakeSure(Type type) {
		if (_type == Type.Element && type == Type.Var) throw new Exception("Tried to treat an element as a var");
		if (_type == Type.Var && type == Type.Element) throw new Exception("Tried to treat a var as an element");
	}

	private enum Type {
		Element,
		Var
	}

	public override string ToString() {
		StringBuilder builder = new StringBuilder();
		ToString(builder, 0);
		return builder.ToString();
	}
	
	private void ToString(StringBuilder builder,int depth) {
		for (int i = 0; i < depth; i++) {
			builder.Append("  ");
		}
		
		if (_type == Type.Element) {
			foreach (var (key, value) in _dictionary) {
				builder.Append($"[{key}]=");
				value.ToString(builder, value._type == Type.Var ? 0 : depth+1);
			}
		} else if (_type == Type.Var) {
			builder.Append(_value ?? "null");
		}
		
		builder.AppendLine();
	}

	public static VarElement CreateElement() {
		return new VarElement {
			_dictionary = new Dictionary<string, VarElement>(),
			_value = null,
			_type = Type.Element
		};
	}
	
	public static VarElement CreateVar(string value = null) {
		return new VarElement {
			_dictionary = null,
			_value = value,
			_type = Type.Var
		};
	}
}