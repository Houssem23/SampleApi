using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations;

namespace SampleApi.WebApi.Models
{
    [DynamoDBTable("SampleTable0001")]
    public class Product
    {
        [Required]
        [DynamoDBHashKey]
        public int Id { get; set; }
        [Required]
        [DynamoDBGlobalSecondaryIndexHashKey]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string ProductName { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string? ProductDescription { get; set; }
        public int Rank { get; set; }

    }
}
