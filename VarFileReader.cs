namespace VarFileLib; 

using System;
using System.IO;

public sealed class VarFileReader {
	public static VarElement Read(string path) {
		var reader = new VarFileReader();
		using TextReader textReader = File.OpenText(path);
		return reader.StartReader(textReader);
	}
	
	private VarFileReader() {}

	private VarElement StartReader(TextReader stream) {
		VarElement vars = VarElement.CreateElement();
		ParseString(ref vars, stream.ReadToEnd());
		
		return vars;
	}

	private void ParseString(ref VarElement vars, string str) {
		int lineNumber = 0;
		int charIndex = 0;
		string curLine = str[charIndex..str.IndexOf('\n', 1)];
		bool lastIteration = false;
		while(str.Length > charIndex) {
			curLine = curLine.Trim();

			if (curLine.StartsWith('[')) {
				int closing = FindClosingBracket(curLine);
				string name = curLine[1..closing];
				string lineValue = curLine[(closing + 2)..];

				// If the entire line is a string literal, skip everything and just save the string
				if (lineValue.StartsWith('\"') && lineValue.EndsWith('\"')) {
					vars.SetElement(name, VarElement.CreateVar(lineValue[1..^1]));
				}
				else // If the line opens with a curly bracket, start doin some recusive shit
				if (lineValue.StartsWith('{')) {
					int indx = str.IndexOf('{', charIndex) + 1;
					int indxclose = str.IndexOf('}', charIndex);
					string chop = str[indx..indxclose];
					VarElement element = VarElement.CreateElement();
					ParseString(ref element, chop);
					vars.SetElement(name, element);

					charIndex = str.IndexOf('\n', indxclose);
					if (str.IndexOf('\n', indxclose + 1) == -1) {
						lastIteration = true;
						curLine = str[(str.LastIndexOf('\n') + 1)..];
					}
					else {
						curLine = str[(indxclose + 1)..(str.IndexOf('\n', indxclose + 1) - 1)];
					}

					continue;
				}
				else if (lineValue.Contains(':')) {
					VarElement element = VarElement.CreateElement();
					string[] vals = lineValue.Split(',');
					foreach (var val in vals) {
						var val2 = val.Trim();
						if (val2.Length == 0) continue;
						int c = val2.IndexOf(':');
						string val3 = val2[(c + 1)..];
						if (val3.StartsWith('"') && val3.EndsWith('"')) val3 = val3[1..^1];
						
						element.SetElement(val2[..c], VarElement.CreateVar(val3));
					}
					vars.SetElement(name, element);
				} else {
					vars.SetElement(name, VarElement.CreateVar(lineValue));
				}
			} else if (curLine.Length > 0) {
				//throw Error("Invalid token");
			}
			
			
			if (lastIteration) break;
			charIndex = str.IndexOf('\n', charIndex+1);
			if (str.IndexOf('\n', charIndex + 1) == -1) {
				lastIteration = true;
				curLine = str[(str.LastIndexOf('\n')+1)..];
			}
			else {
				curLine = str[(charIndex + 1)..(str.IndexOf('\n', charIndex + 1) - 1)];
			}

			lineNumber += 1;
		}
	}

	private int FindClosingBracket(string str) {
		int indx = str.IndexOf("]=", StringComparison.Ordinal);
		if (indx == -1) throw Error("No closing bracket/assign operator found");
		return indx;
	}

	private static Exception Error(string reason) {
		return new Exception($"Failed to read vars file: {reason}");
	}
}