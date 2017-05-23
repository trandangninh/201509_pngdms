use PnG_Db_Dev_Thien

CREATE TABLE ClassificationDefect
(
Id int IDENTITY(1,1) PRIMARY KEY,
Name nvarchar(MAX) not null,
Note nvarchar(MAX),
Materials nvarchar(MAX),
DisplayOrder int default 0
);