using System;
using System.Linq;
using System.Collections.Generic;

namespace CSharp.Crawlers.TypeResolvers
{
	public class FileContextAnalyzer
	{
        private IOutputWriter _globalCache;
		private IOutputWriter _cache;
        private List<ICodeReference> _references;
        private List<ICodeReference> _referenceContainers;

		public FileContextAnalyzer(IOutputWriter globalCache, IOutputWriter cache)
		{
            _globalCache = globalCache;
			_cache = cache;
            buildReferenceMap();
		}

		public string GetEnclosingSignature(string file, int line, int column)
		{
            var match = getParent(file, line, column);

            if (match == null)
                return null;
            
            return match.Signature;
		}

        public string GetSignatureFromNameAndPosition(string file, string name, int line, int column)
        {
            var parent = getParent(file, line, column);

            Func<string,string> getType = (ns) => {
                var signature = ns + "." + name;
                if (_cache.ContainsType(signature) || _globalCache.ContainsType(signature))
                    return signature;
                var end = ns.LastIndexOf(".");
                if (end != -1) {
                    var parentNS = ns.Substring(0, end);
                    return getType(parentNS);
                }
                return null;
            };

            Func<ICodeReference,string> getSignature = (parentRef) => {
                if (parentRef == null)
                    return null;
                var match = _references.FirstOrDefault(
                    x => 
                        x.Parent == parentRef.ToNamespaceSignature() && 
                        x.Name == name);
                if (match != null)
                    return match.Signature;
                if (parentRef.Parent != null && parentRef.Parent != "") {
                    return getSignature(_referenceContainers.FirstOrDefault(x => x.ToNamespaceSignature() == parentRef.Parent));
                } else {
                    return getType(parentRef.ToNamespaceSignature());
                }
            };
            
            var resolvedSignature = getSignature(parent);
            if (resolvedSignature != null)
                return resolvedSignature;
            foreach (var ns in _cache.Usings) {
                var useSignature = getType(ns.Name);
                if (useSignature != null)
                    return useSignature;
            }
            return null;
        }

        private ICodeReference getParent(string file, int line, int column)
        {
            if (_referenceContainers.Count == 0)
                return null;

            var insideOf = _referenceContainers
                    .Where(x => x.Line <= line && x.EndLine >= line)
                    .ToArray();
            if (insideOf.Length == 0)
                return null;

            return _referenceContainers
                .FirstOrDefault(x => x.Line == insideOf.Max(y => y.Line));
        }

        private void buildReferenceMap()
        {
            _referenceContainers = new List<ICodeReference>();
            _references = new List<ICodeReference>();

            _referenceContainers.AddRange(_cache.Namespaces);
            _referenceContainers.AddRange(_cache.Classes);
            _referenceContainers.AddRange(_cache.Interfaces);
            _referenceContainers.AddRange(_cache.Structs);
            _referenceContainers.AddRange(_cache.Enums);
            _referenceContainers.AddRange(_cache.Methods);

            _references.AddRange(_referenceContainers);
            _references.AddRange(_cache.Parameters);
            _references.AddRange(_cache.Fields);
            _references.AddRange(_cache.Variables);
        }
	}
}