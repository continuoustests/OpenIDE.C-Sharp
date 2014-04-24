using System;
using System.IO;
using CSharp.FileSystem;
using CSharp.EditorEngine;
using CSharp.Responses;
using CSharp.Crawlers.TypeResolvers;

namespace CSharp.Commands
{
    class GetWordFromCaretHandler : ICommandHandler
    {
        private string _token;

        public string Usage {
            get {
                return
                    Command + "|\"Returns word under cursor\" end ";
            }
        }

        public string Command { get { return "get-word"; } }

        public GetWordFromCaretHandler(string token)
        {
            _token = token;
        }

        public void Execute(IResponseWriter writer, string[] args)
        {
            var instance = new EngineLocator(new FS()).GetInstance(_token);
            if (instance == null)
                return;
            var caret = instance.GetCaret();
            if (caret == "")
                return;
            var lines = caret.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            var chunks = lines[0].Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            if (chunks.Length != 3)
                return;
            try {
                var file = chunks[0]; 
                var line = int.Parse(chunks[1]);
                var column = int.Parse(chunks[2]);
                
                var start = caret.IndexOf(Environment.NewLine);
                if (start == -1)
                    return;
                start += Environment.NewLine.Length;
                var name = new TypeUnderPositionResolver()
                    .GetTypeName(caret.Substring(start, caret.Length - start), line, column);
                if (name == null || name == "")
                    return;
                writer.Write(name);
            } catch (Exception ex) {
                writer.Write(ex.ToString());
            }
        }
    }
}
