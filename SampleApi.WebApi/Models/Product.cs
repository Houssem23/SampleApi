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
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int Rank { get; set; }

    }
}
