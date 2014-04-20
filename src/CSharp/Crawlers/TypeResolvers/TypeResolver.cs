using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CSharp.Projects;

namespace CSharp.Crawlers.TypeResolvers
{
    public class TypeResolver
    {
        private ICacheReader _cache;
        private bool _resolveMembers = false;

        public TypeResolver(ICacheReader cache) {
            _cache = cache;
        }

        public TypeResolver ResolveMembers() {
            _resolveMembers = true;
            return this;
        }

        public void ResolveAllUnresolved(IOutputWriter cache) {
            Logger.Write("Starting final resolve");
            var classes = cache.Classes.Where(x => !x.AllTypesAreResolved).AsQueryable();
            var interfaces = cache.Interfaces.Where(x => !x.AllTypesAreResolved).AsQueryable();
            var structs = cache.Structs.Where(x => !x.AllTypesAreResolved).AsQueryable();
            var enums = cache.Enums.Where(x => !x.AllTypesAreResolved).AsQueryable();
            var fields = cache.Fields.Where(x => !x.AllTypesAreResolved).AsQueryable();
            var methods = cache.Methods.Where(x => !x.AllTypesAreResolved).AsQueryable();
            var variables = cache.Variables.Where(x => !x.AllTypesAreResolved).AsQueryable();
            for (int i = 0; i < cache.Files.Count; i++) {
                var file = cache.Files[i];
                var partials = new List<PartialType>();
                getPartials(classes, file, partials);
                getPartials(interfaces, file, partials);
                getPartials(structs, file, partials);
                getPartials(enums, file, partials);
                if (_resolveMembers) {
                    getPartials(fields, file, partials);
                    getPartials(methods, file, partials);
                    getPartials(variables, file, partials);
                }
                _cache.ResolveMatchingType(partials.ToArray());
            }
            Logger.Write("Completed final resolve");
        }

        private static void getPartials(IQueryable<ICodeReference> codeRefs, FileRef file, List<PartialType> partials)
        {
            var unresolved = codeRefs.Where(x => x.File.File == file.File);
            foreach (var x in unresolved) {
                x.AllTypesAreResolved = true;
                partials.AddRange(
                    x.GetResolveStatements()
                        .Select(stmnt => 
                            new PartialType(
                                file,
                                new Point(x.Line, x.Column),
                                stmnt.Value,
                                stmnt.Namespace,
                                stmnt.Replace)));
            }
        }
    }

    public class PartialType
    {
        public FileRef File { get; set; }
        public Point Location { get; set; }
        public string Type { get; set; }
        public string Parent { get; set; }
        public Action<string> _resolve;

        public PartialType(FileRef file, Point location, string type, string parent, Action<string> resolve) {
            File = file;
            Location = location;
            Type = type;
            Parent = parent;
            _resolve = resolve;
        }

        public void Resolve(string type) {
            _resolve(type);
        }
    }

    public class ResolvedType
    {
        public string Partial { get; set; }
        public string Resolved { get; set; }
        public string File { get; set; }

        public ResolvedType(string partial, string resolved, string file) {
            Partial = partial;
            Resolved = resolved;
            File = file;
        }
    }
}
