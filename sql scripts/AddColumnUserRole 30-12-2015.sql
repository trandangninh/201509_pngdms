use PnG_Db_New
ALTER TABLE UserRole ADD IsSystem bit not null default 1
ALTER TABLE UserRole ADD IsActive bit not null default 1
INSERT INTO PermissionRecord values ('Admin area. Quality Alert','QualityAlert','QualityAlert',6)