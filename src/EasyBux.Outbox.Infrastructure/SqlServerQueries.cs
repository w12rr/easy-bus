namespace EasyBux.Outbox.Infrastructure;

public static class SqlServerQueries
{
    public const string Delete = "DELETE FROM [dbo].Outboxes WHERE Id = @Id";
    public const string GetNext = "SELECT Id, Type, Data FROM [dbo].Outboxes";
    public const string Insert = "INSERT INTO [dbo].[Outboxes] (Type, Data) VALUES (@Type, @Data)";

    public const string CreateTable = """
                                          IF (NOT EXISTS (SELECT *
                                                  FROM INFORMATION_SCHEMA.TABLES
                                                  WHERE TABLE_SCHEMA = 'dbo'
                                                  AND  TABLE_NAME = 'Outboxes'))
                                          BEGIN
                                              CREATE TABLE [dbo].[Outboxes](
                                      	        [Id] [uniqueidentifier] NOT NULL,
                                      	        [Type] [nvarchar](max) NOT NULL,
                                      	        [Data] [nvarchar](max) NOT NULL,
                                      	        [InsertDate] [datetime] NOT NULL DEFAULT GETDATE(),
                                                  CONSTRAINT [PK_Outboxes] PRIMARY KEY CLUSTERED
                                                  (
                                      	            [Id] ASC
                                                  ) WITH (
                                                          PAD_INDEX = OFF,
                                                          STATISTICS_NORECOMPUTE = OFF,
                                                          IGNORE_DUP_KEY = OFF,
                                                          ALLOW_ROW_LOCKS = ON,
                                                          ALLOW_PAGE_LOCKS = ON,
                                                          OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
                                                      ) ON [PRIMARY]
                                              ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
                                              
                                              
                                              ALTER TABLE [dbo].[Outboxes] ADD  DEFAULT (newid()) FOR [Id];
                                              
                                          END
                                      """;
}