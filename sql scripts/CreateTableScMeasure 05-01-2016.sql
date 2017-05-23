use PnG_Db_Dev_Thien

CREATE TABLE ScMeasure
(
Id int IDENTITY(1,1) PRIMARY KEY,
Name nvarchar(MAX) not null,
Note nvarchar(MAX),
DisplayOrder int default 0,
Target nvarchar(MAX)
);