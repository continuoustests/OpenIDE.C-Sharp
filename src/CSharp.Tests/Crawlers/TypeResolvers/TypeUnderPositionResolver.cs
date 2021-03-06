using System;
using CSharp.Responses;
using NUnit.Framework;
using CSharp.Crawlers;
using CSharp.Crawlers.TypeResolvers;

namespace CSharp.Tests.Crawlers.TypeResolvers
{
	[TestFixture]
	public class TypeUnderPositionResolverTests
	{
		private TypeUnderPositionResolver _resolver;
		
		[SetUp]
		public void Setup()
		{
			_resolver = 
				new TypeUnderPositionResolver();
		}

		[Test]
		public void Can_resolve_simple_method_argument()
		{
			var content = 
				"using System;" + Environment.NewLine +
				Environment.NewLine +
				"\tnamespace CSharp.Crawlers.TypeResolvers" + Environment.NewLine +
				"{" + Environment.NewLine +
				"\tpublic void Bleh(ISomeInterface bleh) {" + Environment.NewLine +
				"\t}" + Environment.NewLine +
				"}";
			var ns = _resolver.GetTypeName(content, 5, 24);
			Assert.That(ns, Is.EqualTo("ISomeInterface"));
		}


		[Test]
		public void Can_resolve_instance_construction()
		{
			var content = "var bleh = new SomeClass();";
			var ns = _resolver.GetTypeName(content, 1, 20);
			Assert.That(ns, Is.EqualTo("SomeClass"));
		}
		
		[Test]
		public void Can_resolve_single_line_definition_from_position()
		{
			var content = 
				"using System;" + Environment.NewLine +
				Environment.NewLine +
				"\tnamespace CSharp.Crawlers.TypeResolvers" + Environment.NewLine +
				"{" + Environment.NewLine +
				"}";
			var ns = _resolver.GetTrainwreck(content, 3, 20);
			Assert.That(ns, Is.EqualTo("CSharp.Crawlers"));
		}

		[Test]
		public void Can_resolve_multi_line_definition_from_position()
		{
			var content = 
				"using System;" + Environment.NewLine +
				Environment.NewLine +
				"\tnamespace CSharp.Crawlers.TypeResolvers" + Environment.NewLine +
				"\t{" + Environment.NewLine +
				"\t\tpublic class MyClass" + Environment.NewLine +
				"\t\t{" + Environment.NewLine +
				"\t\t\t{" + Environment.NewLine +
				"\t\t\t\tpublic void " + Environment.NewLine +
				"\t\t\t\t\tMyMethod " + Environment.NewLine +
				"\t\t\t\t\t\t() " + Environment.NewLine +
				"\t\t\t\t{" + Environment.NewLine +
				"\t\t\t\t}" + Environment.NewLine +
				"\t\t\t}" + Environment.NewLine +
				"\t\t}" + Environment.NewLine +
				"\t}";
			var ns = _resolver.GetTrainwreck(content, 10, 7);
			Assert.That(ns, Is.EqualTo("MyMethod"));
		}

		[Test]
		public void Can_type_from_variable_definition()
		{
			var content = 
				"using System;" + Environment.NewLine +
				Environment.NewLine +
				"\tnamespace CSharp.Crawlers.TypeResolvers" + Environment.NewLine +
				"\t{" + Environment.NewLine +
				"\t\tpublic class MyClass" + Environment.NewLine +
				"\t\t{" + Environment.NewLine +
				"\t\t\t{" + Environment.NewLine +
				"\t\t\t\tpublic void " + Environment.NewLine +
				"\t\t\t\t\tMyMethod " + Environment.NewLine +
				"\t\t\t\t\t\t() " + Environment.NewLine +
				"\t\t\t\t{" + Environment.NewLine +
				"\t\t\t\t\tvar str = \"\";" + Environment.NewLine +
				"\t\t\t\t}" + Environment.NewLine +
				"\t\t\t}" + Environment.NewLine +
				"\t\t}" + Environment.NewLine +
				"\t}";
			var ns = _resolver.GetTypeName(content, 12, 12);
			Assert.That(ns, Is.EqualTo("str"));
		}

		[Test]
		public void Can_resolve_names_and_trainwrecks()
		{
			validateLine("public void Bleh(ISomething meh)", 1, 31, "meh", "meh");
			validateLine("    var bleh = get.something .from.this()", 1, 38, "this", "get.something .from.this");
			validateLine("public void Bleh<SomeType>(ISomething meh)", 1, 20, "SomeType", "Bleh<SomeType>");
			validateLine("public void Bleh<SomeType,string>(ISomething meh)", 1, 20, "SomeType", "Bleh<SomeType,string>");
		}

		private void validateLine(string content, int line, int column, string expectedName, string expectedTrainwreck)
		{
			Assert.That(_resolver.GetTypeName(content, line, column), Is.EqualTo(expectedName));
			Assert.That(_resolver.GetTrainwreck(content, line, column), Is.EqualTo(expectedTrainwreck));
		}
	}
}