using Microblogging.Domain.ValueObjects;

namespace Microblogging.Domain.Entities;


public class Tweet
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public UserId AuthorId { get; private set; }
    public string Content { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public Tweet(UserId authorId, string content)
    {
        AuthorId = authorId;
        Content = content;
    }
}