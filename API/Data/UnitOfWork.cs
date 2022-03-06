using System.Threading.Tasks;
using API.IServices;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public UnitOfWork(DataContext dataContext, IMapper mapper)
        {
            this._mapper = mapper;
            this._dataContext = dataContext;
            
        }
        public IMessageRepository MessageRepository => new MessageRepository(_dataContext, _mapper);

        public IUserRepository UserRepository => new UserRepository(_dataContext, _mapper);

        public ILikeRepository LikeRepository => new LikeRepository(_dataContext, _mapper);

        public async Task<bool> Complete()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _dataContext.ChangeTracker.HasChanges();
        }
    }
}