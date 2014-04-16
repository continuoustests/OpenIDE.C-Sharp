using System;
using System.IO;
using System.Linq;
using CSharp.Crawlers;
using CSharp.Projects;

namespace CSharp.Crawlers.TypeResolvers
{
	public class TypeUnderPositionResolver
	{
		class Location
		{
			public int Line;
			public int Column;

			public override string ToString() {
				return Line.ToString() + ":" + Column.ToString();
			}
		}

		private char[] _operators = new char[] 
			{
				'(',')',':','+','-','*','/','%','&','|',
				'^','!','~','=','?','+','-','{','}',';'
			};
		private char[] _whitespace = new char[]
            {'\r','\n',' ','\t'};
        private char[] _validBeforeWhitespace = new char[]
            {'.',',','<','>'};

		public string GetTypeName(string content, int line, int column) {
			var lines = content
				.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
			var location = rewindToBeginningOfWord(lines, line, column, false);
			if (location == null)
				return null;
			return readForward(lines, location.Line, location.Column, line, column, false);
		}

		public string GetTrainwreck(string content, int line, int column) {
			var lines = content
				.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
			var location = rewindToBeginningOfWord(lines, line, column, true);
			if (location == null)
				return null;
			return readForward(lines, location.Line, location.Column, line, column, true);
		}

		private TypeUnderPositionResolver.Location rewindToBeginningOfWord(string[] lines, int line, int column, bool getTrainwreck) {
			var startLine = line;
			var startColumn = column;
			var lastchar = '.';
			var exit = false;
			for (int j = line - 1; j >= 0; j--) {
				var content = lines[j];
				var chars = content.ToArray();
				startLine = j + 1;
				if (j + 1 == line)
					startColumn = column;
				else
					startColumn = chars.Length;
				for (int i = startColumn - 2; i >= 0; i--) {
					var c = chars[i];
					if (getTrainwreck) {
						if (_whitespace.Contains(c) && !_validBeforeWhitespace.Contains(lastchar)) {
							exit = true;
							break;
						}

						if (_operators.Contains(c)) {
							exit = true;
							break;
						}
					} else {
						if (_whitespace.Contains(c)) {
							exit = true;
							break;
						}

						if (_operators.Contains(c) || _validBeforeWhitespace.Contains(c)) {
							exit = true;
							break;
						}
					}
					if (!_whitespace.Contains(c))
						lastchar = c;
					startColumn--;
				}
				if (exit)
					break;
			}
			return new TypeUnderPositionResolver.Location()  { 
					Line = startLine,
					Column = startColumn
				};
		}

		private string readForward(string[] lines, int line, int column, int endLine, int endColumn, bool getTrainwreck) {
			var signature = "";
			var exit = false;
			for (int j = line - 1; j >= 0; j++) {
				var content = lines[j];
				var chars = content.ToArray();
				if (j + 1 != line)
					column = 1;
				for (int i = column - 1; i < content.Length; i++) {
					var previous = ' ';
					var c = chars[i];
					if (i > 0)
						previous = chars[i-1];
					var isPastEnd = isPastEndPosition(j + 1, i + 1, endLine, endColumn);
					if ((c == '.' && isPastEnd) ||
                        _operators.Contains(c) ||
                        (!getTrainwreck && _validBeforeWhitespace.Contains(c)) ||
                        (_whitespace.Contains(c) && !_operators.Contains(previous)) && isPastEnd)
					{
						exit = true;
						break;
					}
					signature += c.ToString();
				}
				if (exit)
					break;
			}
			foreach (var c in _whitespace)
				signature = signature.Replace(c.ToString(), " ");
			return signature.Trim();
		}

        private bool isPastEndPosition(int line, int column, int endLine, int endColumn) {
            if (line > endLine)
                return true;
            return line == endLine && column > endColumn;
        }
	}
}