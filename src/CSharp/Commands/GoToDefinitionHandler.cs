using System;
using System.IO;
using CSharp.FileSystem;
using CSharp.EditorEngine;
using CSharp.Responses;
using CSharp.Crawlers.TypeResolvers;

namespace CSharp.Commands
{
	class GoToDefinitionHandler : ICommandHandler
	{
		private string _token;
        private IOutputWriter _globalCache;

		public string Usage {
			get {
				return
					Command + "|\"Moves to where the signature under the cursor is defined\"" +
						"POSITION|\"File position as FILE|LINE|COLUMN\" end " +
					"end ";
			}
		}

		public string Command { get { return "go-to-definition"; } }

		public GoToDefinitionHandler(string token, IOutputWriter globalCache)
        {
        	_token = token;
            _globalCache = globalCache;
        }

		public void Execute(IResponseWriter writer, string[] args)
		{
			Logger.Write("Looking for editor instance at " + _token);
            var instance = new EngineLocator(new FS()).GetInstance(_token);
            if (instance == null)
            	return;
            Logger.Write("Got editor instance for " + _token);
            var caret = instance.GetCaret();
            if (caret == "")
            	return;
            Logger.Write("Got caret");
            var lines = caret.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
			var chunks = lines[0].Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
			if (chunks.Length != 3)
				return;
			try {
				var file = chunks[0];
				var line = int.Parse(chunks[1]);
				var column = int.Parse(chunks[2]);
				Logger.Write("Caret is {0}:{1}:{2}", file, line, column);
				
				var cache = 
	                new DirtyFileParser(
	                    _globalCache,
	                    (fileName) => File.ReadAllText(fileName),
						(fileName) => File.Delete(fileName),
                        (fileName) => {
                            return instance.GetDirtyFiles(fileName);
                        }).Parse(file);

				var name = new TypeUnderPositionResolver()
					.GetTypeName(file, File.ReadAllText(file), line, column);
				Logger.Write("Name is " + name);
				var signature = new FileContextAnalyzer(_globalCache, cache)
					.GetSignatureFromNameAndPosition(file, name, line, column);
				if (signature != null) {
					Logger.Write("Found signature " + signature);
					var pos = cache.PositionFromSignature(signature);
					if (pos != null) {
						writer.Write("command|editor goto {0}|{1}|{2}", pos.Fullpath, pos.Line, pos.Column);
						return;
					}
					pos = _globalCache.PositionFromSignature(signature);
					if (pos != null)
						writer.Write("command|editor goto {0}|{1}|{2}", pos.Fullpath, pos.Line, pos.Column);
				}
			} catch (Exception ex) {
				writer.Write(ex.ToString());
			}
		}
	}
}
