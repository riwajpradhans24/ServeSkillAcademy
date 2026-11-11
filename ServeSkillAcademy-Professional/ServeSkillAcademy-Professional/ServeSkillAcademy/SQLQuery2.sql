INSERT INTO Videos
    (CourseId, Title, Description, VideoUrl, DurationMinutes, OrderIndex, IsPublished, CreatedAt)
VALUES
-- Course 1: Hospitality Foundations
(1, 'Introduction to Hospitality',
 'Learn the basic concepts of hospitality and guest service.',
 'https://www.youtube.com/watch?v=pmP2RZZqIe4&pp=ygUcaW50cm9kdWN0aW9uIHRvIGhvc3BpdGFsaXR5', 25, 1, 1, GETUTCDATE()),

(1, 'Professional Customer Service',
 'Learn the fundamentals of professional Customer service.',
 'https://www.youtube.com/watch?v=Eq7Z2i7m57Y&pp=ygUdcHJvZmVzc2lvbmFsIGN1c3RvbWVyIHNlcnZpY2U=', 8, 2, 1, GETUTCDATE()),

-- Course 2: Front Desk Essentials
(2, 'Front Desk Operations',
 'Learn the basic responsibilities and operations of a hotel front desk.',
 'https://www.youtube.com/watch?v=tR8koWF3s2E&t=104s&pp=ygUWZnJvbnQgZGVzayAgb3BlcmF0aW9ucw==', 15, 1, 1, GETUTCDATE()),

(2, 'Hotel Check-in and Check-out',
 'Learn professional check-in and check-out procedures.',
 'https://www.youtube.com/watch?v=xhdKDjb_GFM&pp=ygUcSG90ZWwgQ2hlY2staW4gYW5kIENoZWNrLW91dA==', 10, 2, 1, GETUTCDATE()),

-- Course 3: Restaurant Service Fundamentals
(3, 'Restaurant Service Basics',
 'Learn the fundamentals of professional restaurant service.',
 'https://www.youtube.com/watch?v=iIIR2d_vIz0&list=PLxcVdHxGGxDG7PqajHUxL1Mcj4hTlYmJd', 10, 1, 1, GETUTCDATE()),

(3, 'Table Service Standards',
 'Learn professional table setup and service standards.',
 'https://www.youtube.com/watch?v=eadlA0l8CGQ&pp=ygUbdGFibGUgc2VydmljZSBpbiByZXN0YXVyYW50', 5, 2, 1, GETUTCDATE()),

-- Course 4: Professional Housekeeping
(4, 'Hotel Housekeeping Basics',
 'Learn the basic principles of professional hotel housekeeping.',
 'https://www.youtube.com/watch?v=ggZ8QxMN8Uw&pp=ygUUSG91c2VrZWVwaW5nIGJhc2ljcyA=', 10, 1, 1, GETUTCDATE()),

(4, 'Room Cleaning Standards',
 'Learn professional room cleaning and hygiene standards.',
 'https://www.youtube.com/watch?v=5Xva5BAdRGk&pp=ygUhcm9vbSBjbGVhbmluZyBzdGFuZGFyZHMgaW4gaG90ZWxz', 3, 2, 1, GETUTCDATE()),

-- Course 5: Advanced Guest Relations & Complaint Handling
(5, 'Guest Relations',
 'Learn advanced guest relationship and communication skills.',
 'https://www.youtube.com/watch?v=x27mkgcUQ5Y&t=406s&pp=ygUsQWR2YW5jZSBndWVzdCAgcmVsYXRpb25zIGFuZCBjb21tdW5pY2F0aW9ucyA=', 8, 1, 1, GETUTCDATE()),

(5, 'Handling Guest Complaints',
 'Learn professional techniques for handling difficult guest situations.',
 'https://www.youtube.com/watch?v=baCQpnxZxQQ&pp=ygUhaG93ICB0byBoYW5kbGUgZ3Vlc3QgY29tcGxhaW5udHMg', 10, 2, 1, GETUTCDATE()),

-- Course 6: Advanced Food & Beverage Operations
(6, 'Food and Beverage Operations',
 'Learn advanced food and beverage operational practices.',
 'https://www.youtube.com/watch?v=zabfmu0Ac0A&pp=ygUnZm9vZCBhbmQgYmV2ZXJhZ2Ugb3BlcmF0aW9uYWwgcHJhY3RpY2Vz', 5, 1, 1, GETUTCDATE()),

(6, 'Restaurant Management',
 'Learn professional restaurant management and service operations.',
 'https://www.youtube.com/watch?v=W6kbZHbEJrk&pp=ygUWIHJlc3RhdXJhbnQgbWFuYWdlbWVudA==', 8, 2, 1, GETUTCDATE()),

-- Course 7: Advanced Culinary & Food Safety
(7, 'Advanced Culinary Skills',
 'Learn advanced culinary techniques and kitchen practices.',
 'https://www.youtube.com/watch?v=8A_i-PFRieQ&pp=ygUcYWR2YW5jZWQgY3VsaW5hcnkgdGVjaG5pcXVlcw==', 10, 1, 1, GETUTCDATE()),

(7, 'Food Safety and HACCP',
 'Learn advanced food safety practices and HACCP principles.',
 'https://www.youtube.com/watch?v=j-XNzBUKOoE&pp=ygUVZm9vZCBzYWZldHkgYW5kIGhhY2Nw0gcJCS0MAYcqIYzv', 30, 2, 1, GETUTCDATE());