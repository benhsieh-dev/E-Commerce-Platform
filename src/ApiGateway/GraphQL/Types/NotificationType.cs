using GraphQL.Types;
using ApiGateway.Models;

namespace ApiGateway.GraphQL.Types
{
    public class NotificationType : ObjectGraphType<Notification>
    {
        public NotificationType()
        {
            Name = "Notification";
            Description = "A notification sent to a user";

            Field(n => n.Id, type: typeof(IdGraphType)).Description("The unique identifier of the notification");
            Field(n => n.UserId, type: typeof(IdGraphType)).Description("The unique identifier of the user");
            Field(n => n.Type, type: typeof(NotificationTypeEnumType)).Description("The type of notification");
            Field(n => n.Channel, type: typeof(NotificationChannelType)).Description("The notification channel");
            Field(n => n.Title).Description("Notification title");
            Field(n => n.Content).Description("Notification content");
            Field(n => n.HtmlContent, nullable: true).Description("HTML content for rich notifications");
            Field(n => n.Status, type: typeof(NotificationStatusType)).Description("Notification status");
            Field(n => n.Priority, type: typeof(NotificationPriorityType)).Description("Notification priority");
            Field(n => n.RecipientEmail, nullable: true).Description("Recipient email address");
            Field(n => n.RecipientPhone, nullable: true).Description("Recipient phone number");
            Field(n => n.ScheduledAt, type: typeof(DateTimeGraphType), nullable: true).Description("When the notification is scheduled to be sent");
            Field(n => n.SentAt, type: typeof(DateTimeGraphType), nullable: true).Description("When the notification was sent");
            Field(n => n.DeliveredAt, type: typeof(DateTimeGraphType), nullable: true).Description("When the notification was delivered");
            Field(n => n.ReadAt, type: typeof(DateTimeGraphType), nullable: true).Description("When the notification was read");
            Field(n => n.CreatedAt, type: typeof(DateTimeGraphType)).Description("When the notification was created");
            Field(n => n.UpdatedAt, type: typeof(DateTimeGraphType)).Description("When the notification was last updated");
            Field(n => n.ErrorMessage, nullable: true).Description("Error message if delivery failed");
            Field(n => n.RetryCount).Description("Number of retry attempts");
            Field(n => n.BookingId, type: typeof(IdGraphType), nullable: true).Description("Related booking ID");
            Field(n => n.PropertyId, type: typeof(IdGraphType), nullable: true).Description("Related property ID");
            Field(n => n.PaymentId, type: typeof(IdGraphType), nullable: true).Description("Related payment ID");
            Field(n => n.ReviewId, type: typeof(IdGraphType), nullable: true).Description("Related review ID");
            Field(n => n.TrackingId, nullable: true).Description("Tracking ID for delivery");
            Field(n => n.IsRead).Description("Whether the notification has been read");
            Field(n => n.IsArchived).Description("Whether the notification is archived");

            Field<UserType>("user", resolve: context => context.Source.User);
            Field<BookingType>("booking", resolve: context => context.Source.Booking);
            Field<PropertyType>("property", resolve: context => context.Source.Property);
        }
    }

    public class NotificationPreferenceType : ObjectGraphType<NotificationPreference>
    {
        public NotificationPreferenceType()
        {
            Name = "NotificationPreference";
            Description = "User notification preferences";

            Field(np => np.Id, type: typeof(IdGraphType)).Description("The unique identifier of the preference");
            Field(np => np.UserId, type: typeof(IdGraphType)).Description("The unique identifier of the user");
            Field(np => np.NotificationType, type: typeof(NotificationTypeEnumType)).Description("The type of notification");
            Field(np => np.EmailEnabled).Description("Whether email notifications are enabled");
            Field(np => np.SmsEnabled).Description("Whether SMS notifications are enabled");
            Field(np => np.PushEnabled).Description("Whether push notifications are enabled");
            Field(np => np.InAppEnabled).Description("Whether in-app notifications are enabled");
            Field(np => np.CreatedAt, type: typeof(DateTimeGraphType)).Description("When the preference was created");
            Field(np => np.UpdatedAt, type: typeof(DateTimeGraphType)).Description("When the preference was last updated");

            Field<UserType>("user", resolve: context => context.Source.User);
        }
    }

    public class NotificationTypeEnumType : EnumerationGraphType<Models.NotificationType>
    {
        public NotificationTypeEnumType()
        {
            Name = "NotificationTypeEnum";
            Description = "The type of notification";
        }
    }

    public class NotificationChannelType : EnumerationGraphType<NotificationChannel>
    {
        public NotificationChannelType()
        {
            Name = "NotificationChannel";
            Description = "The channel for sending notifications";
        }
    }

    public class NotificationStatusType : EnumerationGraphType<NotificationStatus>
    {
        public NotificationStatusType()
        {
            Name = "NotificationStatus";
            Description = "The status of a notification";
        }
    }

    public class NotificationPriorityType : EnumerationGraphType<NotificationPriority>
    {
        public NotificationPriorityType()
        {
            Name = "NotificationPriority";
            Description = "The priority of a notification";
        }
    }

    public class NotificationInputType : InputObjectGraphType<Notification>
    {
        public NotificationInputType()
        {
            Name = "NotificationInput";
            Description = "Input for creating a notification";

            Field(n => n.UserId, type: typeof(IdGraphType)).Description("The unique identifier of the user");
            Field(n => n.Type, type: typeof(NotificationTypeEnumType)).Description("The type of notification");
            Field(n => n.Channel, type: typeof(NotificationChannelType)).Description("The notification channel");
            Field(n => n.Title).Description("Notification title");
            Field(n => n.Content).Description("Notification content");
            Field(n => n.HtmlContent, nullable: true).Description("HTML content for rich notifications");
            Field(n => n.Priority, type: typeof(NotificationPriorityType)).Description("Notification priority");
            Field(n => n.RecipientEmail, nullable: true).Description("Recipient email address");
            Field(n => n.RecipientPhone, nullable: true).Description("Recipient phone number");
            Field(n => n.ScheduledAt, type: typeof(DateTimeGraphType), nullable: true).Description("When the notification should be sent");
            Field(n => n.BookingId, type: typeof(IdGraphType), nullable: true).Description("Related booking ID");
            Field(n => n.PropertyId, type: typeof(IdGraphType), nullable: true).Description("Related property ID");
            Field(n => n.PaymentId, type: typeof(IdGraphType), nullable: true).Description("Related payment ID");
            Field(n => n.ReviewId, type: typeof(IdGraphType), nullable: true).Description("Related review ID");
        }
    }

    public class NotificationPreferenceInputType : InputObjectGraphType<NotificationPreference>
    {
        public NotificationPreferenceInputType()
        {
            Name = "NotificationPreferenceInput";
            Description = "Input for updating notification preferences";

            Field(np => np.NotificationType, type: typeof(NotificationTypeEnumType)).Description("The type of notification");
            Field(np => np.EmailEnabled).Description("Whether email notifications are enabled");
            Field(np => np.SmsEnabled).Description("Whether SMS notifications are enabled");
            Field(np => np.PushEnabled).Description("Whether push notifications are enabled");
            Field(np => np.InAppEnabled).Description("Whether in-app notifications are enabled");
        }
    }

    public class NotificationFilterInputType : InputObjectGraphType
    {
        public NotificationFilterInputType()
        {
            Name = "NotificationFilterInput";
            Description = "Filters for searching notifications";

            Field<IdGraphType>("userId");
            Field<NotificationTypeEnumType>("type");
            Field<NotificationChannelType>("channel");
            Field<NotificationStatusType>("status");
            Field<NotificationPriorityType>("priority");
            Field<BooleanGraphType>("isRead");
            Field<BooleanGraphType>("isArchived");
            Field<DateTimeGraphType>("createdFrom");
            Field<DateTimeGraphType>("createdTo");
            Field<DateTimeGraphType>("sentFrom");
            Field<DateTimeGraphType>("sentTo");
        }
    }
}