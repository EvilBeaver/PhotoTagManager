using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tagger.Engine;
using Tagger.Engine.DAL;

namespace EngineTests
{
    [TestClass]
    public class UnitTest1
    {
        private string _dbPath;
        private IDatabase _db;

        public UnitTest1()
        {
            _dbPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "TESTDB.DB3");
        }

        [TestMethod]
        public void DatabaseCreated()
        {
            CleanUpDatabase();
            _db = CreateDBInstance();
            _db.Init();
            
            Assert.IsTrue(System.IO.File.Exists(_dbPath));
            CleanUpDatabase(true);
        }

        [TestMethod]
        public void FileRepoCheck()
        {
            CleanUpDatabase();
            _db = CreateDBInstance();
            _db.Init();

            DatabaseService.RegisterInstance(_db);

            var file = FileLink.Create(@"C:\dummy_test_file.txt");
            DatabaseService.FileRepository.Write(file);

            var dbObject = (IPersistable)file;
            Assert.IsFalse(dbObject.Key.IsEmpty());

            var readed = DatabaseService.FileRepository.FindByKey(dbObject.Key);
            Assert.IsFalse(readed == default(FileLink));
            Assert.IsTrue(readed.FullName == file.FullName);

            CleanUpDatabase(true);

        }

        private void CleanUpDatabase(bool ignoreErrors = false)
        {
            if (System.IO.File.Exists(_dbPath))
            {
                try
                {
                    DatabaseService.ShutdownInstance();
                    _db = null;
                    System.IO.File.Delete(_dbPath); 
                }
                catch 
                {
                    if (!ignoreErrors)
                    {
                        throw;
                    }
                }
            }
        }

        private IDatabase CreateDBInstance()
        {
            var db = new Database(_dbPath);
            return db;
        }
        
    }
}
