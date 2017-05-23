
GO

/****** Object:  Table [dbo].[Frequency]    Script Date: 4/24/2017 4:05:01 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Frequency](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Mark] [int] NOT NULL,
 CONSTRAINT [PK_Frequency] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

INSERT INTO PermissionRecord (PermissionRecord.Name, PermissionRecord.SystemName, PermissionRecord.Category, PermissionRecord.[Order])
VALUES ('Frequency', 'Frequency', 'QualityAlert', 26)


/* Create table with data  */
GO
/****** Object:  Table [dbo].[Frequency]    Script Date: 4/26/2017 11:05:04 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Frequency](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Mark] [int] NOT NULL,
	[Code] [nvarchar](max) NULL,
 CONSTRAINT [PK_Frequency] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[Frequency] ON 

INSERT [dbo].[Frequency] ([Id], [Name], [Mark], [Code]) VALUES (1, N'Remote - first known occurrence', 2, N'Remote')
INSERT [dbo].[Frequency] ([Id], [Name], [Mark], [Code]) VALUES (2, N'Occasionally - deviation occurs intermittently (1-5 similar issues in the last)', 4, N'Occasionally')
INSERT [dbo].[Frequency] ([Id], [Name], [Mark], [Code]) VALUES (3, N'Probably - deviation is likely to occur (>5 similar issues in the last year)', 6, N'Probably')
INSERT [dbo].[Frequency] ([Id], [Name], [Mark], [Code]) VALUES (4, N'Frequency - deviation occurs repeatedly (>10 similar issues in past year)', 8, N'Frequency')
INSERT [dbo].[Frequency] ([Id], [Name], [Mark], [Code]) VALUES (5, N'Always - deviation occurs constantly (>10 similar issues in the last 3 months) (or >10)', 10, N'Always')
SET IDENTITY_INSERT [dbo].[Frequency] OFF


