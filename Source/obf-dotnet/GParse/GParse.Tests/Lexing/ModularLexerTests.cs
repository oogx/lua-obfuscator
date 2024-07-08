using System;
using GParse.Errors;
using GParse.Lexing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GParse.Tests.Lexing
{
    [TestClass]
    public class ModularLexerTests
    {
        private static void AssertToken<T> ( Token<T> token, String id, T type, String raw, Object value, SourceRange range )
            where T : notnull
        {
            Assert.AreEqual ( id, token.Id );
            Assert.AreEqual ( type, token.Type );
            Assert.AreEqual ( raw, token.Raw );
            Assert.AreEqual ( value, token.Value );
            Assert.AreEqual ( false, token.IsTrivia );
            Assert.AreEqual ( range, token.Range );
        }

        [TestMethod]
        public void LiteralModuleConsumesProperly ( )
        {
            var builder = new ModularLexerBuilder<Int32> ( );
            builder.AddLiteral ( "id", 1, "raw" );
            var progress = new Progress<Diagnostic> ( );
            ILexer<Int32> lexer = builder.BuildLexer ( "raw", progress );
            AssertToken ( lexer.Consume ( ), "id", 1, "raw", "raw", new SourceRange ( new SourceLocation ( 1, 1, 0 ), new SourceLocation ( 1, 4, 3 ) ) );
            AssertToken ( lexer.Consume ( ), "EOF", default, String.Empty, String.Empty, new SourceRange ( new SourceLocation ( 1, 4, 3 ), new SourceLocation ( 1, 4, 3 ) ) );
            lexer = builder.BuildLexer ( "rawraw", progress );
            AssertToken ( lexer.Consume ( ), "id", 1, "raw", "raw", new SourceRange ( new SourceLocation ( 1, 1, 0 ), new SourceLocation ( 1, 4, 3 ) ) );
            AssertToken ( lexer.Consume ( ), "id", 1, "raw", "raw", new SourceRange ( new SourceLocation ( 1, 4, 3 ), new SourceLocation ( 1, 7, 6 ) ) );
            AssertToken ( lexer.Consume ( ), "EOF", default, String.Empty, String.Empty, new SourceRange ( new SourceLocation ( 1, 7, 6 ), new SourceLocation ( 1, 7, 6 ) ) );
            lexer = builder.BuildLexer ( "notraw", progress );
            Assert.ThrowsException<FatalParsingException> ( ( ) => lexer.Consume ( ), "No registered modules can consume the rest of the input." );
        }

        [TestMethod]
        public void RegexModuleConsumesProperly ( )
        {
            var progress = new Progress<Diagnostic> ( );
            var builder = new ModularLexerBuilder<Int32> ( );
            builder.AddRegex ( "id", 1, @"num:(\d+)", "num:", m => Int32.Parse ( m.Groups[1].Value ), false );

            // Test 01
            ILexer<Int32> lexer = builder.BuildLexer ( "num:1", progress );
            AssertToken ( lexer.Consume ( ), "id", 1, "num:1", 1, new SourceRange ( new SourceLocation ( 1, 1, 0 ), new SourceLocation ( 1, 6, 5 ) ) );
            AssertToken ( lexer.Consume ( ), "EOF", default, String.Empty, String.Empty, new SourceRange ( new SourceLocation ( 1, 6, 5 ), new SourceLocation ( 1, 6, 5 ) ) );

            // Test 02
            lexer = builder.BuildLexer ( "num:12", progress );
            AssertToken ( lexer.Consume ( ), "id", 1, "num:12", 12, new SourceRange ( new SourceLocation ( 1, 1, 0 ), new SourceLocation ( 1, 7, 6 ) ) );
            AssertToken ( lexer.Consume ( ), "EOF", 0, String.Empty, String.Empty, new SourceRange ( new SourceLocation ( 1, 7, 6 ), new SourceLocation ( 1, 7, 6 ) ) );

            // Test 03
            lexer = builder.BuildLexer ( "num:1234", progress );
            AssertToken ( lexer.Consume ( ), "id", 1, "num:1234", 1234, new SourceRange ( new SourceLocation ( 1, 1, 0 ), new SourceLocation ( 1, 9, 8 ) ) );
            AssertToken ( lexer.Consume ( ), "EOF", default, String.Empty, String.Empty, new SourceRange ( new SourceLocation ( 1, 9, 8 ), new SourceLocation ( 1, 9, 8 ) ) );

            // Test 04
            lexer = builder.BuildLexer ( "num:1234num:1", progress );
            AssertToken ( lexer.Consume ( ), "id", 1, "num:1234", 1234, new SourceRange ( new SourceLocation ( 1, 1, 0 ), new SourceLocation ( 1, 9, 8 ) ) );
            AssertToken ( lexer.Consume ( ), "id", 1, "num:1", 1, new SourceRange ( new SourceLocation ( 1, 9, 8 ), new SourceLocation ( 1, 14, 13 ) ) );
            AssertToken ( lexer.Consume ( ), "EOF", default, String.Empty, String.Empty, new SourceRange ( new SourceLocation ( 1, 14, 13 ), new SourceLocation ( 1, 14, 13 ) ) );

            // Test 05
            lexer = builder.BuildLexer ( "num:notnum", progress );
            Assert.ThrowsException<FatalParsingException> ( ( ) => lexer.Consume ( ), "No registered modules can consume the rest of the input." );
        }
    }
}