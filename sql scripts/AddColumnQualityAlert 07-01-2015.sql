use PnG_Db_Dev_Thien

EXEC sp_RENAME 'QualityAlert.SAPLotOrSupplierLot' , 'SAPLot', 'COLUMN'

ALTER TABLE QualityAlert
ADD SupplierLot nvarchar(MAX),
	Unit nvarchar(MAX),
	SupplierId int not null default 0,
	MaterialId int not null default 0,
	CategoryId int not null default 0,
	ComplaintTypeId int not null default 0,
	ClassificationDefectId int not null default 0,
	DefectRepeatId int not null default 0,
	SupplierReplyDate datetime2(7) not null default '0001-01-01 00:00:00',
	CostImpacted float not null default 0,
	PRLossPercent float not null default 0,
	QuantityReturn int not null default 0,
	NumStop int not null default 0,
	DownTime datetime2(7) not null default '0001-01-01 00:00:00'

