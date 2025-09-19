using GraphQL.Types;
using ApiGateway.Models;

namespace ApiGateway.GraphQL.Types
{
    public class PropertyType : ObjectGraphType<Property>
    {
        public PropertyType()
        {
            Name = "Property";
            Description = "A rental property in the AirBnB platform";

            Field(p => p.Id, type: typeof(IdGraphType)).Description("The unique identifier of the property");
            Field(p => p.HostId, type: typeof(IdGraphType)).Description("The unique identifier of the host");
            Field(p => p.Title).Description("The property title");
            Field(p => p.Description).Description("The property description");
            Field(p => p.PropertyType, type: typeof(PropertyTypeEnumType)).Description("The type of property");
            Field(p => p.PricePerNight, type: typeof(DecimalGraphType)).Description("Price per night");
            Field(p => p.MaxGuests).Description("Maximum number of guests");
            Field(p => p.Bedrooms).Description("Number of bedrooms");
            Field(p => p.Bathrooms).Description("Number of bathrooms");
            Field(p => p.Address).Description("Property address");
            Field(p => p.City).Description("Property city");
            Field(p => p.State).Description("Property state");
            Field(p => p.Country).Description("Property country");
            Field(p => p.PostalCode).Description("Property postal code");
            Field(p => p.Latitude, type: typeof(DecimalGraphType)).Description("Property latitude");
            Field(p => p.Longitude, type: typeof(DecimalGraphType)).Description("Property longitude");
            Field(p => p.Amenities, type: typeof(ListGraphType<StringGraphType>)).Description("Property amenities");
            Field(p => p.Images, type: typeof(ListGraphType<StringGraphType>)).Description("Property images");
            Field(p => p.HouseRules, type: typeof(ListGraphType<StringGraphType>)).Description("House rules");
            Field(p => p.IsInstantBook).Description("Whether instant booking is enabled");
            Field(p => p.MinimumStay).Description("Minimum stay in nights");
            Field(p => p.MaximumStay).Description("Maximum stay in nights");
            Field(p => p.WeeklyDiscount, type: typeof(DecimalGraphType), nullable: true).Description("Weekly discount percentage");
            Field(p => p.MonthlyDiscount, type: typeof(DecimalGraphType), nullable: true).Description("Monthly discount percentage");
            Field(p => p.Status, type: typeof(PropertyStatusType)).Description("Property status");
            Field(p => p.CreatedAt, type: typeof(DateTimeGraphType)).Description("When the property was created");
            Field(p => p.UpdatedAt, type: typeof(DateTimeGraphType)).Description("When the property was last updated");
            Field(p => p.UnavailableDates, type: typeof(ListGraphType<DateTimeGraphType>)).Description("Unavailable dates");
            Field(p => p.AverageRating, type: typeof(DecimalGraphType)).Description("Average rating from reviews");
            Field(p => p.ReviewCount).Description("Total number of reviews");

            Field<UserType>("host", resolve: context => context.Source.Host);
            Field<ListGraphType<ReviewType>>("reviews", resolve: context => context.Source.Reviews);
        }
    }

    public class PropertyTypeEnumType : EnumerationGraphType<PropertyType>
    {
        public PropertyTypeEnumType()
        {
            Name = "PropertyTypeEnum";
            Description = "The type of rental property";
        }
    }

    public class PropertyStatusType : EnumerationGraphType<PropertyStatus>
    {
        public PropertyStatusType()
        {
            Name = "PropertyStatus";
            Description = "The status of a property";
        }
    }

    public class PropertyInputType : InputObjectGraphType<Property>
    {
        public PropertyInputType()
        {
            Name = "PropertyInput";
            Description = "Input for creating or updating a property";

            Field(p => p.HostId, type: typeof(IdGraphType)).Description("The unique identifier of the host");
            Field(p => p.Title).Description("The property title");
            Field(p => p.Description).Description("The property description");
            Field(p => p.PropertyType, type: typeof(PropertyTypeEnumType)).Description("The type of property");
            Field(p => p.PricePerNight, type: typeof(DecimalGraphType)).Description("Price per night");
            Field(p => p.MaxGuests).Description("Maximum number of guests");
            Field(p => p.Bedrooms).Description("Number of bedrooms");
            Field(p => p.Bathrooms).Description("Number of bathrooms");
            Field(p => p.Address).Description("Property address");
            Field(p => p.City).Description("Property city");
            Field(p => p.State).Description("Property state");
            Field(p => p.Country).Description("Property country");
            Field(p => p.PostalCode).Description("Property postal code");
            Field(p => p.Latitude, type: typeof(DecimalGraphType)).Description("Property latitude");
            Field(p => p.Longitude, type: typeof(DecimalGraphType)).Description("Property longitude");
            Field(p => p.Amenities, type: typeof(ListGraphType<StringGraphType>)).Description("Property amenities");
            Field(p => p.Images, type: typeof(ListGraphType<StringGraphType>)).Description("Property images");
            Field(p => p.HouseRules, type: typeof(ListGraphType<StringGraphType>)).Description("House rules");
            Field(p => p.IsInstantBook).Description("Whether instant booking is enabled");
            Field(p => p.MinimumStay).Description("Minimum stay in nights");
            Field(p => p.MaximumStay).Description("Maximum stay in nights");
            Field(p => p.WeeklyDiscount, type: typeof(DecimalGraphType), nullable: true).Description("Weekly discount percentage");
            Field(p => p.MonthlyDiscount, type: typeof(DecimalGraphType), nullable: true).Description("Monthly discount percentage");
        }
    }

    public class PropertyFilterInputType : InputObjectGraphType
    {
        public PropertyFilterInputType()
        {
            Name = "PropertyFilterInput";
            Description = "Filters for searching properties";

            Field<StringGraphType>("city");
            Field<StringGraphType>("state");
            Field<StringGraphType>("country");
            Field<PropertyTypeEnumType>("propertyType");
            Field<DecimalGraphType>("minPrice");
            Field<DecimalGraphType>("maxPrice");
            Field<IntGraphType>("minGuests");
            Field<IntGraphType>("maxGuests");
            Field<DateTimeGraphType>("checkInDate");
            Field<DateTimeGraphType>("checkOutDate");
            Field<ListGraphType<StringGraphType>>("amenities");
            Field<BooleanGraphType>("instantBookOnly");
            Field<DecimalGraphType>("latitude");
            Field<DecimalGraphType>("longitude");
            Field<DecimalGraphType>("radiusKm");
        }
    }
}