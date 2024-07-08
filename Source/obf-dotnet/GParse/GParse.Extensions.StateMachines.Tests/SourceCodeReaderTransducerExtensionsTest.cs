using System;
using GParse.IO;
using GUtils.StateMachines.Transducers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GParse.Extensions.StateMachines.Tests
{
    [TestClass]
    public class SourceCodeReaderTransducerExtensionsTest
    {
        [TestMethod]
        public void ExecuteExecutesProperly ( )
        {
            var reader = new StringCodeReader ( "pass" );
            var tr1 = new Transducer<Char, Int32> ( );
            tr1.InitialState.OnInput ( new[] { 'p', 'a', 's', 's' }, 1 );
            Assert.IsTrue ( tr1.TryExecute ( reader, out var num ) );
            Assert.AreEqual ( 1, num );
            reader.Reset ( );
            reader.Advance ( 1 );
            Assert.IsFalse ( tr1.TryExecute ( reader, out _ ) );
        }
    }
}
