
-- file table

CREATE TABLE [files] ( 
	[id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
	[name] varchar(100) NOT NULL,
	[fullname] varchar(260) NOT NULL,
	[md5] char(32) NOT NULL
);

CREATE INDEX [idx_file_md5] ON [files] ([md5]);
CREATE UNIQUE INDEX [idx_file_path] ON [files] ([fullname]);

-- tags table

CREATE TABLE [tags](
	[id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
	[name] varchar(150) NOT NULL,
	[description] varchar(260)
);

-- properties table

CREATE TABLE [imageProps](
	[file_id] integer PRIMARY KEY NOT NULL,
	[tags_string] varchar(200)
);

CREATE TABLE [tagsmap](
	[file_id] integer NOT NULL,
	[tag_id] integer NOT NULL,
	PRIMARY KEY ([file_id],[tag_id])
);