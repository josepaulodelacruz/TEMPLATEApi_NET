﻿CREATE TABLE ACCESS_TOKENS (
	ID INT IDENTITY(1,1) PRIMARY KEY,
	USER_ID INT NOT NULL,
	TOKEN NVARCHAR(MAX),
	[TIMESTAMP] DATETIME,
	FOREIGN KEY (USER_ID) REFERENCES USERS(ID) ON DELETE CASCADE
)
