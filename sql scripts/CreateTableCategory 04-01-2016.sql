use PnG_Db_New

CREATE TABLE Category
(
Id int IDENTITY(1,1) PRIMARY KEY,
Name nvarchar(MAX) not null,
Note nvarchar(MAX),
DisplayOrder int default 0
);