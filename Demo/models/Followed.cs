
using Demo.Models;

public interface IFollowable
{

}

public class Followed
{
    public string ContentId { get; private set; } = "";
    public DateTime FollowingSince { get; private set; }

    public void Follow<T>(T content) where T : ICosmosResource, IFollowable
    {
        ContentId = content.Id;
        FollowingSince = DateTime.UtcNow;
    }
}
