using DomainModel;
using DomainServices;
using HotChocolate.Authorization;
using Infrastructure;


namespace api.Data {
    [Authorize]
    public class Query {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Packet> packets([Service] IRepository _repository) =>
            _repository.GetAllPackets().AsQueryable();

        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Product> products([Service] IRepository _repository) =>
        _repository.GetAllProducts().AsQueryable();
    }
}
