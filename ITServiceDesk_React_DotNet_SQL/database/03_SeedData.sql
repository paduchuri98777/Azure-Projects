USE ITServiceDeskDb;
GO
IF NOT EXISTS(SELECT 1 FROM dbo.Users) INSERT INTO dbo.Users(Name,Email,Department) VALUES
(N'Phani Kumar',N'phani@example.com',N'IT'),(N'Anand Kumar',N'anand@example.com',N'IT'),(N'Priya Sharma',N'priya@example.com',N'HR'),(N'Rahul Verma',N'rahul@example.com',N'Finance'),(N'Support Admin',N'support@example.com',N'IT');
GO
IF NOT EXISTS(SELECT 1 FROM dbo.Categories) INSERT INTO dbo.Categories(Name) VALUES
(N'Hardware'),(N'Software'),(N'Network'),(N'Email'),(N'Access / Account'),(N'Security');
GO
IF NOT EXISTS(SELECT 1 FROM dbo.Tickets)
BEGIN
DECLARE @Network INT=(SELECT Id FROM dbo.Categories WHERE Name=N'Network'),@Hardware INT=(SELECT Id FROM dbo.Categories WHERE Name=N'Hardware'),@Email INT=(SELECT Id FROM dbo.Categories WHERE Name=N'Email'),@Software INT=(SELECT Id FROM dbo.Categories WHERE Name=N'Software');
DECLARE @Phani INT=(SELECT Id FROM dbo.Users WHERE Name=N'Phani Kumar'),@Anand INT=(SELECT Id FROM dbo.Users WHERE Name=N'Anand Kumar'),@Priya INT=(SELECT Id FROM dbo.Users WHERE Name=N'Priya Sharma'),@Support INT=(SELECT Id FROM dbo.Users WHERE Name=N'Support Admin');
INSERT INTO dbo.Tickets(Title,Description,CategoryId,Priority,Status,CreatedById,AssignedToId) VALUES
(N'Laptop cannot connect to Wi-Fi',N'User laptop cannot connect to office wireless network.',@Network,N'High',N'Open',@Phani,@Anand),
(N'Outlook not receiving emails',N'Outlook is connected but new emails are not appearing.',@Email,N'Medium',N'In Progress',@Priya,@Support),
(N'Printer not working',N'Finance department printer is not printing documents.',@Hardware,N'Low',N'Resolved',@Phani,@Anand),
(N'Application installation request',N'Request to install approved development software.',@Software,N'Medium',N'Pending',@Priya,@Support);
END
GO