use PnG_Db_New

CREATE TABLE Supplier
(
Id int IDENTITY(1,1) PRIMARY KEY,
Name nvarchar(MAX) not null,
Note nvarchar(MAX),
VendorCode nvarchar(MAX) not null,
VendorPrefixCode nvarchar(MAX) not null,
VendorContact nvarchar(MAX),
LocationTypeId int default 0,
DisplayOrder int default 0,
);