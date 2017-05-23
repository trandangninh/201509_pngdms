ALTER TABLE ScMeasure ADD Formula nvarchar(MAX) null default null

CREATE TABLE [dbo].[MqsMeasure](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ScMeasureId] [int] NOT NULL,
	[SupplierId] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_MqsMeasure] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
