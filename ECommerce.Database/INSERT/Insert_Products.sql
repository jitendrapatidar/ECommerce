INSERT INTO [dbo].[Products]
           ([Name], [Description], [Price], [StockQuantity], [CategoryId], [CreatedAt], [UpdatedAt])
VALUES
    ('iPhone 15 Pro', 'Apple iPhone 15 Pro with A17 chip, 128GB storage', 1299.99, 50, 1, GETDATE(), GETDATE()), -- Electronics
    ('Men’s Leather Jacket', 'Premium leather jacket for men, black color', 199.99, 30, 2, GETDATE(), GETDATE()), -- Clothing
    ('Non-stick Cookware Set', '10-piece non-stick cookware set with heat-resistant handles', 89.99, 75, 3, GETDATE(), GETDATE()), -- Home & Kitchen
    ('The Great Gatsby', 'Classic novel by F. Scott Fitzgerald, paperback edition', 9.99, 120, 4, GETDATE(), GETDATE()), -- Books
    ('Yoga Mat', 'High-density, non-slip yoga mat for exercise and meditation', 24.99, 200, 5, GETDATE(), GETDATE()); -- Sports & Outdoors
