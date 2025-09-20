using GraphQL.Types;
using ApiGateway.Models;

namespace ApiGateway.GraphQL.Types
{
    public class PaymentType : ObjectGraphType<Payment>
    {
        public PaymentType()
        {
            Name = "Payment";
            Description = "A payment transaction";

            Field(p => p.Id, type: typeof(IdGraphType)).Description("The unique identifier of the payment");
            Field(p => p.BookingId, type: typeof(IdGraphType)).Description("The unique identifier of the booking");
            Field(p => p.PayerId, type: typeof(IdGraphType)).Description("The unique identifier of the payer");
            Field(p => p.PayeeId, type: typeof(IdGraphType), nullable: true).Description("The unique identifier of the payee");
            Field(p => p.Type, type: typeof(PaymentTypeEnumType)).Description("The type of payment");
            Field(p => p.Method, type: typeof(PaymentMethodType)).Description("The payment method used");
            Field(p => p.Amount, type: typeof(DecimalGraphType)).Description("Payment amount");
            Field(p => p.PlatformFee, type: typeof(DecimalGraphType)).Description("Platform fee");
            Field(p => p.ProcessingFee, type: typeof(DecimalGraphType)).Description("Processing fee");
            Field(p => p.NetAmount, type: typeof(DecimalGraphType)).Description("Net amount after fees");
            Field(p => p.Currency).Description("Payment currency");
            Field(p => p.Status, type: typeof(PaymentStatusType)).Description("Payment status");
            Field(p => p.ExternalTransactionId, nullable: true).Description("External transaction ID");
            Field(p => p.PaymentIntentId, nullable: true).Description("Payment intent ID");
            Field(p => p.FailureReason, nullable: true).Description("Reason for payment failure");
            Field(p => p.ProcessedAt, type: typeof(DateTimeGraphType), nullable: true).Description("When the payment was processed");
            Field(p => p.CreatedAt, type: typeof(DateTimeGraphType)).Description("When the payment was created");
            Field(p => p.UpdatedAt, type: typeof(DateTimeGraphType)).Description("When the payment was last updated");

            Field<BookingType>("booking", resolve: context => context.Source.Booking);
            Field<UserType>("payer", resolve: context => context.Source.Payer);
            Field<UserType>("payee", resolve: context => context.Source.Payee);
        }
    }

    public class PaymentMethodType : ObjectGraphType<PaymentMethod>
    {
        public PaymentMethodType()
        {
            Name = "PaymentMethod";
            Description = "A payment method";

            Field(pm => pm.Id, type: typeof(IdGraphType)).Description("The unique identifier of the payment method");
            Field(pm => pm.UserId, type: typeof(IdGraphType)).Description("The unique identifier of the user");
            Field(pm => pm.Type, type: typeof(PaymentMethodTypeEnumType)).Description("The type of payment method");
            Field(pm => pm.Last4Digits).Description("Last 4 digits of the payment method");
            Field(pm => pm.Brand).Description("Payment method brand");
            Field(pm => pm.ExpiryMonth).Description("Expiry month");
            Field(pm => pm.ExpiryYear).Description("Expiry year");
            Field(pm => pm.BankName, nullable: true).Description("Bank name for bank accounts");
            Field(pm => pm.BankAccountType, nullable: true).Description("Bank account type");
            Field(pm => pm.IsDefault).Description("Whether this is the default payment method");
            Field(pm => pm.IsActive).Description("Whether this payment method is active");
            Field(pm => pm.ExternalMethodId).Description("External payment method ID");
            Field(pm => pm.CreatedAt, type: typeof(DateTimeGraphType)).Description("When the payment method was created");
            Field(pm => pm.UpdatedAt, type: typeof(DateTimeGraphType)).Description("When the payment method was last updated");

            Field<UserType>("user", resolve: context => context.Source.User);
        }
    }

    public class PaymentTypeEnumType : EnumerationGraphType<Models.PaymentType>
    {
        public PaymentTypeEnumType()
        {
            Name = "PaymentTypeEnum";
            Description = "The type of payment";
        }
    }

    public class PaymentStatusType : EnumerationGraphType<PaymentStatus>
    {
        public PaymentStatusType()
        {
            Name = "PaymentStatus";
            Description = "The status of a payment";
        }
    }

    public class PaymentMethodTypeEnumType : EnumerationGraphType<Models.PaymentMethodType>
    {
        public PaymentMethodTypeEnumType()
        {
            Name = "PaymentMethodTypeEnum";
            Description = "The type of payment method";
        }
    }

    public class PaymentInputType : InputObjectGraphType<Payment>
    {
        public PaymentInputType()
        {
            Name = "PaymentInput";
            Description = "Input for creating a payment";

            Field(p => p.BookingId, type: typeof(IdGraphType)).Description("The unique identifier of the booking");
            Field(p => p.PayerId, type: typeof(IdGraphType)).Description("The unique identifier of the payer");
            Field(p => p.Type, type: typeof(PaymentTypeEnumType)).Description("The type of payment");
            Field(p => p.Amount, type: typeof(DecimalGraphType)).Description("Payment amount");
            Field(p => p.Currency).Description("Payment currency");
        }
    }

    public class PaymentMethodInputType : InputObjectGraphType<PaymentMethod>
    {
        public PaymentMethodInputType()
        {
            Name = "PaymentMethodInput";
            Description = "Input for creating a payment method";

            Field(pm => pm.UserId, type: typeof(IdGraphType)).Description("The unique identifier of the user");
            Field(pm => pm.Type, type: typeof(PaymentMethodTypeEnumType)).Description("The type of payment method");
            Field(pm => pm.Last4Digits).Description("Last 4 digits of the payment method");
            Field(pm => pm.Brand).Description("Payment method brand");
            Field(pm => pm.ExpiryMonth).Description("Expiry month");
            Field(pm => pm.ExpiryYear).Description("Expiry year");
            Field(pm => pm.BankName, nullable: true).Description("Bank name for bank accounts");
            Field(pm => pm.BankAccountType, nullable: true).Description("Bank account type");
            Field(pm => pm.IsDefault).Description("Whether this is the default payment method");
            Field(pm => pm.ExternalMethodId).Description("External payment method ID");
        }
    }
}