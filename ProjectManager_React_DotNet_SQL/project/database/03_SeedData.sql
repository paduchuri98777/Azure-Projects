USE ProjectManagerDb;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Projects)
BEGIN
    INSERT INTO dbo.Projects (Name, Description, Status)
    VALUES
        (N'Website Development', N'Company website development project', N'Active'),
        (N'Employee Portal', N'Internal employee management portal', N'Active'),
        (N'IT Automation', N'Automate common system administration tasks', N'On Hold');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Tasks)
BEGIN
    DECLARE @WebsiteProjectId INT =
        (SELECT TOP 1 Id FROM dbo.Projects WHERE Name = N'Website Development');

    DECLARE @PortalProjectId INT =
        (SELECT TOP 1 Id FROM dbo.Projects WHERE Name = N'Employee Portal');

    INSERT INTO dbo.Tasks (ProjectId, Title, Description, Status)
    VALUES
        (@WebsiteProjectId, N'Create React UI', N'Build the main React screens', N'In Progress'),
        (@WebsiteProjectId, N'Create .NET API', N'Build REST API endpoints', N'Todo'),
        (@PortalProjectId, N'Database Design', N'Create SQL Server database tables', N'Completed');
END
GO