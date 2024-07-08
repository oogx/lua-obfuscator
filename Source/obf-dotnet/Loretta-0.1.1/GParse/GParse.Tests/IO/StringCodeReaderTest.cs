using System;
using System.Text.RegularExpressions;
using GParse.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GParse.Tests.IO
{
    [TestClass]
    public class StringCodeReaderTest
    {
        [TestMethod]
        public void AdvanceTest ( )
        {
            var reader = new StringCodeReader ( "stri\nng\r\nanother\r\nstring" );
            var expectedLines = new[]
            {
                /*s :*/ "B 00 L 1 C 1",
                /*t :*/ "B 01 L 1 C 2",
                /*r :*/ "B 02 L 1 C 3",
                /*i :*/ "B 03 L 1 C 4",
                /*\n:*/ "B 04 L 1 C 5",
                /*n :*/ "B 05 L 2 C 1",
                /*g :*/ "B 06 L 2 C 2",
                /*\r:*/ "B 07 L 2 C 3",
                /*\n:*/ "B 08 L 2 C 4",
                /*a :*/ "B 09 L 3 C 1",
                /*n :*/ "B 10 L 3 C 2",
                /*o :*/ "B 11 L 3 C 3",
                /*t :*/ "B 12 L 3 C 4",
                /*h :*/ "B 13 L 3 C 5",
                /*e :*/ "B 14 L 3 C 6",
                /*r :*/ "B 15 L 3 C 7",
                /*\r:*/ "B 16 L 3 C 8",
                /*\n:*/ "B 17 L 3 C 9",
                /*s :*/ "B 18 L 4 C 1",
                /*t :*/ "B 19 L 4 C 2",
                /*r :*/ "B 20 L 4 C 3",
                /*i :*/ "B 21 L 4 C 4",
                /*n :*/ "B 22 L 4 C 5",
                /*g :*/ "B 23 L 4 C 6",
            };
            var i = 0;
            while ( reader.Position != reader.Length )
            {
                SourceLocation l = reader.Location;
                reader.Advance ( 1 );
                Assert.AreEqual ( expectedLines[i++], $"B {l.Byte:00} L {l.Line} C {l.Column}" );
            }
            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.Advance ( 1 ) );
            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.Advance ( -1 ) );
        }

        [TestMethod]
        public void OffsetOfCharTest ( )
        {
            var reader = new StringCodeReader ( "aaa bbb" );
            Assert.AreEqual ( 0, reader.FindOffset ( 'a' ) );
            Assert.AreEqual ( 4, reader.FindOffset ( 'b' ) );

            reader.Advance ( 2 );
            Assert.AreEqual ( 0, reader.FindOffset ( 'a' ) );
            Assert.AreEqual ( 2, reader.FindOffset ( 'b' ) );
            Assert.AreEqual ( -1, reader.FindOffset ( 'c' ) );
        }

        [TestMethod]
        public void OffsetOfStringTest ( )
        {
            var reader = new StringCodeReader ( "aaa bbb" );
            Assert.AreEqual ( 0, reader.FindOffset ( "aaa" ) );
            Assert.AreEqual ( 4, reader.FindOffset ( "bbb" ) );

            reader.Advance ( 1 );
            Assert.AreEqual ( 0, reader.FindOffset ( "aa" ) );
            Assert.AreEqual ( -1, reader.FindOffset ( "aaa" ) );
            Assert.AreEqual ( 3, reader.FindOffset ( "bbb" ) );
            Assert.AreEqual ( -1, reader.FindOffset ( "ccc" ) );
        }

        [TestMethod]
        public void OffsetOfPredicateTest ( )
        {
            var reader = new StringCodeReader ( "a b c d" );
            Assert.AreEqual ( 0, reader.FindOffset ( c => c == 'a' ) );
            Assert.AreEqual ( 2, reader.FindOffset ( c => c == 'b' ) );
            Assert.AreEqual ( 4, reader.FindOffset ( c => c == 'c' ) );
            Assert.AreEqual ( 6, reader.FindOffset ( c => c == 'd' ) );

            reader.Advance ( 2 );
            Assert.AreEqual ( -1, reader.FindOffset ( c => c == 'a' ) );
            Assert.AreEqual ( 0, reader.FindOffset ( c => c == 'b' ) );
            Assert.AreEqual ( 2, reader.FindOffset ( c => c == 'c' ) );
            Assert.AreEqual ( 4, reader.FindOffset ( c => c == 'd' ) );
        }

        [TestMethod]
        public void IsNextCharTest ( )
        {
            var reader = new StringCodeReader ( "aa bb" );
            Assert.IsTrue ( reader.IsNext ( 'a' ) );

            reader.Advance ( 3 );
            Assert.IsTrue ( reader.IsNext ( 'b' ) );
        }

        [TestMethod]
        public void IsNextStringTest ( )
        {
            var reader = new StringCodeReader ( "aa bb" );
            Assert.IsTrue ( reader.IsNext ( "aa " ) );
            Assert.IsFalse ( reader.IsNext ( "aaa" ) );
            Assert.IsFalse ( reader.IsNext ( "aa bbc" ) );

            reader.Advance ( 3 );
            Assert.IsTrue ( reader.IsNext ( "b" ) );
            Assert.IsTrue ( reader.IsNext ( "bb" ) );
        }

#if HAS_SPAN
        [TestMethod]
        public void IsNextSpanTest ( )
        {
            var reader = new StringCodeReader ( "aa bb" );
            Assert.IsTrue ( reader.IsNext ( "aa ".AsSpan ( ) ) );
            Assert.IsFalse ( reader.IsNext ( "aaa".AsSpan ( ) ) );
            Assert.IsFalse ( reader.IsNext ( "aa bbc".AsSpan ( ) ) );

            reader.Advance ( 3 );
            Assert.IsTrue ( reader.IsNext ( "b".AsSpan ( ) ) );
            Assert.IsTrue ( reader.IsNext ( "bb".AsSpan ( ) ) );
        }
#endif

        [TestMethod]
        public void PeekTest ( )
        {
            var reader = new StringCodeReader ( "ab" );
            Assert.AreEqual ( 'a', reader.Peek ( ) );

            reader.Advance ( 1 );
            Assert.AreEqual ( 'b', reader.Peek ( ) );

            reader.Advance ( 1 );
            Assert.AreEqual ( null, reader.Peek ( ) );
        }

        [TestMethod]
        public void PeekInt32Test ( )
        {
            var reader = new StringCodeReader ( "abc" );
            Assert.AreEqual ( 'b', reader.Peek ( 1 ) );
            Assert.AreEqual ( 'c', reader.Peek ( 2 ) );
            Assert.AreEqual ( null, reader.Peek ( 20 ) );

            reader.Advance ( 1 );
            Assert.AreEqual ( 'c', reader.Peek ( 1 ) );
            Assert.AreEqual ( null, reader.Peek ( 2 ) );

            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.Peek ( -1 ) );
        }

        [TestMethod]
        public void PeekRegexStringTest ( )
        {
            var reader = new StringCodeReader ( "123abc" );
            Assert.AreEqual ( "1", reader.PeekRegex ( /*lang=regex*/ @"\d" ).Value );
            Assert.AreEqual ( "12", reader.PeekRegex ( /*lang=regex*/ @"\d{2}" ).Value );
            Assert.AreEqual ( "123", reader.PeekRegex ( /*lang=regex*/ @"\d+" ).Value );
            Assert.IsFalse ( reader.PeekRegex ( "[a-z]" ).Success );

            reader.Advance ( 3 );
            Assert.IsFalse ( reader.PeekRegex ( /*lang=regex*/ @"\d+" ).Success );
            Assert.AreEqual ( "a", reader.PeekRegex ( /*lang=regex*/ @"[a-z]" ).Value );
            Assert.AreEqual ( "ab", reader.PeekRegex ( /*lang=regex*/ @"[a-z]{2}" ).Value );
            Assert.AreEqual ( "abc", reader.PeekRegex ( /*lang=regex*/ @"[a-z]+" ).Value );
        }

        [TestMethod]
        public void PeekRegexRegexTest ( )
        {
            var reader = new StringCodeReader ( "123abc" );
            var regex1 = new Regex ( @"\G\d+", RegexOptions.Compiled );
            var regex2 = new Regex ( @"\G[a-z]+", RegexOptions.Compiled );
            var regex3 = new Regex ( @"[a-z]+", RegexOptions.Compiled );

            Assert.AreEqual ( "123", reader.PeekRegex ( regex1 ).Value );
            Assert.IsFalse ( reader.PeekRegex ( regex2 ).Success );

            reader.Advance ( 3 );
            Assert.IsFalse ( reader.PeekRegex ( regex1 ).Success );
            Assert.AreEqual ( "abc", reader.PeekRegex ( regex2 ).Value );

            reader.Reset ( );
            Assert.ThrowsException<ArgumentException> ( ( ) => reader.PeekRegex ( regex3 ) );
        }

        [TestMethod]
        public void PeekStringTest ( )
        {
            var reader = new StringCodeReader ( "abc abc" );
            Assert.AreEqual ( "abc", reader.PeekString ( 3 ) );

            reader.Advance ( 3 );
            Assert.AreEqual ( " abc", reader.PeekString ( 4 ) );

            reader.Advance ( 1 );
            Assert.AreEqual ( null, reader.PeekString ( 4 ) );
            Assert.AreEqual ( "abc", reader.PeekString ( 3 ) );
            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.PeekString ( -1 ) );
        }

#if HAS_SPAN
        [TestMethod]
        public void PeekSpanTest ( )
        {
            var str = "abc def";
            var reader = new StringCodeReader ( str );
            ReadOnlySpan<Char> span = str.AsSpan ( );

            Assert.IsTrue ( span.Slice ( 0, 3 ).Equals ( reader.PeekSpan ( 3 ), StringComparison.Ordinal ) );

            reader.Advance ( 3 );
            Assert.IsTrue ( span.Slice ( 3, 4 ).Equals ( reader.PeekSpan ( 4 ), StringComparison.Ordinal ) );
        }
#endif

        [TestMethod]
        public void ReadTest ( )
        {
            var reader = new StringCodeReader ( "abc " );
            Assert.AreEqual ( 'a', reader.Read ( ) );
            Assert.AreEqual ( 'c', reader.Read ( 1 ) );
            Assert.AreEqual ( null, reader.Read ( 1 ) );
            Assert.AreEqual ( ' ', reader.Read ( ) );
            Assert.AreEqual ( null, reader.Read ( ) );
            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.Read ( -1 ) );
        }

        [TestMethod]
        public void ReadStringTest ( )
        {
            var reader = new StringCodeReader ( "abcabc" );
            Assert.AreEqual ( "abc", reader.ReadString ( 3 ) );
            Assert.AreEqual ( "abc", reader.ReadString ( 3 ) );
            Assert.AreEqual ( null, reader.ReadString ( 3 ) );
            Assert.ThrowsException<ArgumentOutOfRangeException> ( ( ) => reader.ReadString ( -3 ) );
        }
    }
}