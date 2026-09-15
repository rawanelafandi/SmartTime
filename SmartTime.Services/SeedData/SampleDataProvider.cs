using SmartTime.Services.DTOs;

namespace SmartTime.Services.SeedData;

// Hard-codes the exact sample dataset given in the task sheet (Tasks and
// Assignments table + Time Entries table), so /sync/push-sample-data has
// something deterministic to push into Clockify and persist locally.
public static class SampleDataProvider
{
    public static readonly IReadOnlyDictionary<string, string> UserEmails = new Dictionary<string, string>
    {
        ["Alice Johnson"] = "alice.johnson@example.com",
        ["Bob Martinez"] = "bob.martinez@example.com",
        ["Carla Nguyen"] = "carla.nguyen@example.com",
        ["David Lee"] = "david.lee@example.com",
        ["Emma Smith"] = "emma.smith@example.com",
    };

    public static IReadOnlyList<SeedTask> Tasks { get; } = new List<SeedTask>
    {
        new("Website Redesign", "Homepage Mockup", "Alice Johnson", 7),
        new("Website Redesign", "Content Migration", "Bob Martinez", 10),
        new("Website Redesign", "SEO Optimization", "Carla Nguyen", 12),
        new("Mobile App Launch", "API Integration", "David Lee", 5),
        new("Mobile App Launch", "User Testing", "Emma Smith", 3),
        new("Mobile App Launch", "Bug Fixing", "Alice Johnson", 8),
        new("Marketing Campaign", "Ad Design", "Bob Martinez", 9),
        new("Marketing Campaign", "Social Media Scheduling", "Carla Nguyen", 14),
        new("Marketing Campaign", "Email Outreach", "David Lee", 24),
    };

    public static IReadOnlyList<SeedTimeEntry> TimeEntries { get; } = new List<SeedTimeEntry>
    {
        new("Alice Johnson", "Website Redesign", "Homepage Mockup",
            DateTime.Parse("2025-07-20T09:15:00Z").ToUniversalTime(), DateTime.Parse("2025-07-20T11:45:00Z").ToUniversalTime()),
        new("Bob Martinez", "Website Redesign", "Content Migration",
            DateTime.Parse("2025-07-21T13:00:00Z").ToUniversalTime(), DateTime.Parse("2025-07-21T15:20:00Z").ToUniversalTime()),
        new("Carla Nguyen", "Website Redesign", "SEO Optimization",
            DateTime.Parse("2025-07-22T10:05:00Z").ToUniversalTime(), DateTime.Parse("2025-07-22T12:50:00Z").ToUniversalTime()),
        new("David Lee", "Mobile App Launch", "API Integration",
            DateTime.Parse("2025-07-23T11:10:00Z").ToUniversalTime(), DateTime.Parse("2025-07-23T13:35:00Z").ToUniversalTime()),
        new("Emma Smith", "Mobile App Launch", "User Testing",
            DateTime.Parse("2025-07-24T14:05:00Z").ToUniversalTime(), DateTime.Parse("2025-07-24T16:35:00Z").ToUniversalTime()),
        new("Alice Johnson", "Mobile App Launch", "Bug Fixing",
            DateTime.Parse("2025-07-25T09:15:00Z").ToUniversalTime(), DateTime.Parse("2025-07-25T11:45:00Z").ToUniversalTime()),
        new("Bob Martinez", "Marketing Campaign", "Ad Design",
            DateTime.Parse("2025-07-26T10:10:00Z").ToUniversalTime(), DateTime.Parse("2025-07-26T12:10:00Z").ToUniversalTime()),
        new("Carla Nguyen", "Marketing Campaign", "Social Media Scheduling",
            DateTime.Parse("2025-07-27T09:05:00Z").ToUniversalTime(), DateTime.Parse("2025-07-27T10:35:00Z").ToUniversalTime()),
        new("David Lee", "Marketing Campaign", "Email Outreach",
            DateTime.Parse("2025-07-28T15:10:00Z").ToUniversalTime(), DateTime.Parse("2025-07-28T17:05:00Z").ToUniversalTime()),
    };
}