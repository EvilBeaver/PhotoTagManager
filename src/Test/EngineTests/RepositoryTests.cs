using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tagger.Engine;
using Tagger.Engine.DAL;

namespace EngineTests
{
    [TestClass]
    public class RepositoryTests
    {
        private string _dbPath;
        private IDatabase _db;

        public RepositoryTests()
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
            CleanUpDatabase();
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

            DatabaseService.FileRepository.Remove(readed);
            readed = DatabaseService.FileRepository.FindByKey(dbObject.Key);
            Assert.IsTrue(readed == default(FileLink));

            CleanUpDatabase();

        }

        [TestMethod]
        public void FavoritesRepoCheck()
        {
            CleanUpDatabase();
            _db = CreateDBInstance();
            _db.Init();

            var record = new FavoritesStreamReference()
            {
                TableName = "folder_refs",
                id = new Identifier(1)
            };

            var repo = FavoritesRepository.Create(_db);
            var recordKey = repo.CreateKey();
            recordKey["id"] = record.id;
            recordKey["TableName"] = record.TableName;

            var found = repo.FindByKey(recordKey);
            Assert.IsTrue(found.Equals(default(FavoritesStreamReference)));

            repo.Write(record);
            found = repo.FindByKey(recordKey);
            Assert.IsTrue(found.Equals(record));

            repo.Remove(recordKey);
            found = repo.FindByKey(recordKey);
            Assert.IsTrue(found.Equals(default(FavoritesStreamReference)));

            CleanUpDatabase();
        }

        [TestMethod]
        public void FolderRefRepoCheck()
        {
            CleanUpDatabase();
            _db = CreateDBInstance();
            _db.Init();

            DatabaseService.RegisterInstance(_db);

            var folderRef = new FolderRefEntity();
            folderRef.Path = @"C:\Users\";

            DatabaseService.FolderRefRepository.Write(folderRef);

            Assert.IsFalse(folderRef.Key.IsEmpty());

            var readed = DatabaseService.FolderRefRepository.FindByKey(folderRef.Key);
            Assert.AreEqual(folderRef.Key.Value, readed.Key.Value);

            DatabaseService.FolderRefRepository.Remove(folderRef);
            Assert.IsNull(DatabaseService.FolderRefRepository.FindByKey(folderRef.Key));

            CleanUpDatabase();
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
            var db = new SQLiteDatabase(_dbPath);
            return db;
        }
        
    }
}
