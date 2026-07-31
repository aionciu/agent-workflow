using AgenticWorkflows.Api.Models;
using AgenticWorkflows.Api.Services;

namespace AgenticWorkflows.Api.Tests;

public sealed class NotificationComposerTests
{
    [Fact]
    public void Created_notification_truncates_descriptions_longer_than_90_characters()
    {
        var description = new string('a', 91);
        var item = CreateWorkItem(description: description);

        var notification = NotificationComposer.BuildCreatedNotification(item);

        Assert.Contains($"Description: {new string('a', 87)}...", notification);
        Assert.DoesNotContain($"Description: {description}", notification);
    }

    [Fact]
    public void Notifications_omit_due_date_when_due_date_is_null()
    {
        var item = CreateWorkItem(dueDate: null);

        var createdNotification = NotificationComposer.BuildCreatedNotification(item);
        var dueSoonNotification = NotificationComposer.BuildDueSoonNotification(item);

        Assert.DoesNotContain("Due date:", createdNotification);
        Assert.DoesNotContain("Due date:", dueSoonNotification);
    }

    [Fact]
    public void Created_and_due_soon_notifications_include_expected_next_steps()
    {
        var item = CreateWorkItem();

        var createdNotification = NotificationComposer.BuildCreatedNotification(item);
        var dueSoonNotification = NotificationComposer.BuildDueSoonNotification(item);

        Assert.Contains("Next step: Review the backlog and assign an owner.", createdNotification);
        Assert.Contains("Next step: Confirm the item still belongs in this sprint.", dueSoonNotification);
    }

    private static WorkItem CreateWorkItem(string? description = "Description", DateOnly? dueDate = default) =>
        new(Guid.NewGuid(), "Example item", description, 3, WorkItemStatus.Todo, dueDate);
}
