using DomainModel;
using DomainServices;
using Infrastructure;

namespace api.Data {
    public class Query {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Packet> packets([Service] IRepository _repository) =>
            _repository.GetAllPackets().AsQueryable();

        public IQueryable<Product> products([Service] IRepository _repository) =>
        _repository.GetAllProducts().AsQueryable();
    }
}
