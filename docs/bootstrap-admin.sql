-- Виконайте після реєстрації облікового запису, який має стати адміністратором.
-- Замініть admin_username на фактичний Username.
UPDATE dbo.Users
SET IsAdmin = 1,
    IsRegistrationConfirmed = 1
WHERE Username = N'admin_username';

-- Переконайтеся, що оновився рівно один запис.
SELECT Id, Username, IsAdmin, IsRegistrationConfirmed
FROM dbo.Users
WHERE Username = N'admin_username';
