using Api.Models.Subscribtion;
using Api.Models.User;
using AutoMapper;
using DAL;
using DAL.Entites;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class SubscribtionService
    {
        private readonly DAL.DataContext _context;
        private readonly IMapper _mapper;

        public SubscribtionService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task SubscribeToUser(CreateSubscribtionModel model)
        {
            if (await _context.Subscribtions.AnyAsync(x => x.PublisherId == model.PublisherId && x.FollowerId == model.FollowerId) || model.PublisherId == model.FollowerId)
            {
                throw new Exception("you are subscribed");
            }
            var dbSubscribtion = _mapper.Map<Subscribtion>(model);
            await _context.Subscribtions.AddAsync(dbSubscribtion);
            await _context.SaveChangesAsync();
        }

        public async Task UnSubscribeFromUser(CreateSubscribtionModel model)
        {
            var dbSubscribtion = await _context.Subscribtions.FirstOrDefaultAsync(x => x.PublisherId == model.PublisherId && x.FollowerId == model.FollowerId);
            if (dbSubscribtion != null)
            {
                _context.Subscribtions.Remove(dbSubscribtion);
                _context.SaveChanges();
            }
            else throw new Exception("subscribtion is not found");
        }

        public async Task<IEnumerable<UserModel>> GetSubscribtions(Guid userId)
        {
            var publishers = await _context.Subscribtions.Include(x => x.Publisher).ThenInclude(x => x.Avatar).Include(x => x.Publisher).ThenInclude(x => x.Subscribtions).Include(x => x.Publisher).ThenInclude(x => x.Followers).Include(x => x.Publisher).ThenInclude(x => x.Posts).Where(x => x.FollowerId == userId).AsNoTracking().OrderByDescending(x => x.SubscriptionDate).Select(x => _mapper.Map<UserModel>(x.Publisher)).ToListAsync();
            if (publishers != null)
            {
                return publishers;
            }
            else throw new Exception("users is not found");
        }

        public async Task<IEnumerable<UserModel>> GetFollowers(Guid userId)
        {
            var followers = await _context.Subscribtions.Include(x => x.Follower).ThenInclude(x => x.Avatar).Include(x => x.Follower).ThenInclude(x => x.Subscribtions).Include(x => x.Follower).ThenInclude(x => x.Followers).Include(x => x.Follower).ThenInclude(x => x.Posts).Where(x => x.PublisherId == userId).AsNoTracking().OrderByDescending(x => x.SubscriptionDate).Select(x => _mapper.Map<UserModel>(x.Follower)).ToListAsync();
            if (followers != null)
            {
                return followers;
            }
            else throw new Exception("users is not found");
        }

        public async Task<List<SubscribtionModel>> GetSubAndFol(Guid userId)
        {
            var subscribtions = await _context.Subscribtions.AsNoTracking().Where(x => x.PublisherId == userId || x.FollowerId == userId).ToListAsync();
            if (subscribtions != null)
            {
                return subscribtions.OrderByDescending(x => x.SubscriptionDate).Select(x => _mapper.Map<SubscribtionModel>(x)).ToList();
            }
            throw new Exception("subscribtions are not found");
        }
    }
}
