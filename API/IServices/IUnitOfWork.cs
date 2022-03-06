using System.Threading.Tasks;

namespace API.IServices
{
    public interface IUnitOfWork
    {
         IMessageRepository MessageRepository {get;}
         IUserRepository UserRepository {get;}
         ILikeRepository LikeRepository{get;}
         Task<bool> Complete();
         bool HasChanges();
    }
}