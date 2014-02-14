CREATE TABLE [files] ( 
	[id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
	[name] varchar(100) NOT NULL,
	[fullname] varchar(260) NOT NULL,
	[md5] char(32) NOT NULL
);

CREATE INDEX [idx_file_md5] ON [files] ([md5]);
CREATE UNIQUE INDEX [idx_file_path] ON [files] ([fullname]);