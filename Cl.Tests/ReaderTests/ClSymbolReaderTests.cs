using System.Collections.Generic;
using Cl.Input;
using NUnit.Framework;
using static Cl.Extensions.FpUniverse;

namespace Cl.Tests.ReaderTests
{
    [TestFixture]
    public class ClSymbolReaderTests
    {
        [Test]
        public void ReadSymbol_SkipOnlyPartOfSource()
        {
            var source = new FilteredSource("foo bar");
            using var reader = new Reader(source);

            Ignore(reader.ReadSymbol());

            Assert.That(source.ToString(), Is.EqualTo(" bar"));
        }

        [TestCaseSource(nameof(ValidSymbolsTestCases))]
        public void ReadSymbol_ReturnSymbol(string input, string expected)
        {
            using var reader = new Reader(new FilteredSource(input));

            Assert.That(reader.ReadSymbol()?.Value, Is.EqualTo(expected));
        }

        static object[] ValidSymbolsTestCases =
            {
                new object[] { "foo bar", "foo" },
                new object[] { "bar-foo", "bar"},
                new object[] { "foo", "foo" },
                new object[] { "f", "f" },
                new object[] { "bar1", "bar1" },
                new object[] { "a1b", "a1b" }
            };

        [TestCaseSource(nameof(InvalidSymbolsTestCases))]
        public void ReadString_ReturnNull_WhenSourceStartsWithInvalidSymbol(string input)
        {
            using var reader = new Reader(new FilteredSource("\"some"));

            Assert.That(() => reader.ReadSymbol(),Is.Null);
        }

        static IEnumerable<string> InvalidSymbolsTestCases()
        {
            yield return string.Empty;
            yield return "1foo";
            yield return "-bar";
            yield return "+foo";
            yield return "?bar";
        }
    }
}