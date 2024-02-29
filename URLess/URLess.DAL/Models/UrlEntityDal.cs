using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using URLess.Domain;

namespace URLess.DAL.Models;

public class UrlEntityDal : UrlEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public bool WasRotated { get; set; } = false;
}
