INSERT INTO [dbo].[Categories]
           ([Name], [Description], [CreatedAt], [UpdatedAt])
VALUES
    ('Electronics', 'Devices and gadgets such as phones, laptops, and accessories', GETDATE(), GETDATE()),
    ('Clothing', 'Men, women, and kids apparel including shoes and accessories', GETDATE(), GETDATE()),
    ('Home & Kitchen', 'Furniture, appliances, and kitchenware for your home', GETDATE(), GETDATE()),
    ('Books', 'Fiction, non-fiction, educational, and children’s books', GETDATE(), GETDATE()),
    ('Sports & Outdoors', 'Sports gear, fitness equipment, and outdoor essentials', GETDATE(), GETDATE());