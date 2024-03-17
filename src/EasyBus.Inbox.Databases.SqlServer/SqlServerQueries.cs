namespace EasyBus.Inbox.Infrastructure;

public static class SqlServerQueries
{
    public const string Insert =
        "INSERT INTO [dbo].[Inboxes] (MessageId, Type, Data) VALUES (@MessageId, @Type, @Data)";

    public const string UpdateTries =
        "UPDATE [dbo].[Inboxes] SET TriesCount = @TriesCount, PickupDate = @PickupDate WHERE Id = @Id";

    public const string SetReceived = "UPDATE [dbo].[Inboxes] SET Received = 1 WHERE Id = @Id";

    public const string GetUnprocessed =
        "SELECT Id, Type, Data, TriesCount, InsertDate FROM [dbo].[Inboxes] WHERE Received = 0 AND PickupDate <= GETDATE() AND TriesCount < 11";

    public const string DeleteOldProcessed = "DELETE FROM [dbo].[Inboxes] WHERE Received = 1 AND InsertDate < @Date";

    public const string CreateTable =
        """
            IF (NOT EXISTS (SELECT *
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_SCHEMA = 'dbo'
                    AND  TABLE_NAME = 'Inboxes'))
            BEGIN
                SET ANSI_NULLS ON
                
                SET QUOTED_IDENTIFIER ON
                
                CREATE TABLE [dbo].[Inboxes](
        	        [Id] [uniqueidentifier] NOT NULL,
        	        [MessageId] [nvarchar](450) NOT NULL,
        	        [Type] [nvarchar](max) NOT NULL,
        	        [Data] [nvarchar](max) NOT NULL,
        	        [Received] [bit] NOT NULL,
        	        [TriesCount] [int] NOT NULL,
        	        [PickupDate] [datetime2](7) NOT NULL,
        	        [InsertDate] [datetime2](7) NOT NULL,
                 CONSTRAINT [PK_Inboxes] PRIMARY KEY CLUSTERED
                (
        	        [Id] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                
                ALTER TABLE [dbo].[Inboxes] ADD  DEFAULT ((0)) FOR [Received]
                
                ALTER TABLE [dbo].[Inboxes] ADD  DEFAULT ((0)) FOR [TriesCount]
                
                ALTER TABLE [dbo].[Inboxes] ADD  DEFAULT (getdate()) FOR [PickupDate]
                
                ALTER TABLE [dbo].[Inboxes] ADD  DEFAULT (getdate()) FOR [InsertDate]
                
                ALTER TABLE [dbo].[Inboxes] ADD  DEFAULT (newid()) FOR [Id];
            END
        """;
}