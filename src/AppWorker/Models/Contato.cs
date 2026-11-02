using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AppWorker.Models
{
    public class Contato
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Telefone { get; set; } = null!;
    }
}