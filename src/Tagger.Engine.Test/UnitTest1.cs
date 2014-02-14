using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tagger.Engine.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CreateDatabase()
        {
            // arrange & act
            var filename = PrepareDatabase();

            // assert
            Assert.IsTrue(System.IO.File.Exists(filename));
        }

        [TestMethod]
        public void DatabaseService()
        {
            // arrange
            var filename = PrepareDatabase();

            // act
            var db = Tagger.Engine.DAL.DatabaseService.GetInstance();
            Assert.IsNotNull(db);
            Assert.IsTrue(db is Tagger.Engine.DAL.IDatabase);
        }

        private string PrepareDatabase()
        {
            var filename = "test.db3";
            var dir = System.IO.Path.GetDirectoryName(typeof(Tagger.Engine.DAL.Database).Assembly.Location);
            var fullname = System.IO.Path.Combine(dir, filename);
            System.IO.File.Delete(fullname);

            var dbInstance = new Tagger.Engine.DAL.Database(fullname);
            Tagger.Engine.DAL.DatabaseService.RegisterInstance(dbInstance);

            return fullname;

        }
    }
}
