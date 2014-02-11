using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tagger.Engine.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CreateDatabase()
        {
            // arrange
            var filename = "test.db3";
            var dir = System.IO.Path.GetDirectoryName(typeof(Tagger.Engine.DAL.Database).Assembly.Location);
            var fullname = System.IO.Path.Combine(dir, filename);
            System.IO.File.Delete(fullname);

            // act
            var db = new Tagger.Engine.DAL.Database(filename);
            db.Create();

            // assert
            Assert.IsTrue(System.IO.File.Exists(fullname));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateOnExistingDB()
        {
            // arrange
            var filename = "test.db3";
            var dir = System.IO.Path.GetDirectoryName(typeof(Tagger.Engine.DAL.Database).Assembly.Location);
            var fullname = System.IO.Path.Combine(dir, filename);
            System.IO.File.Delete(fullname);

            // act
            var db = new Tagger.Engine.DAL.Database(filename);
            db.Create();
            db.Create(); // assert
        }
    }
}
