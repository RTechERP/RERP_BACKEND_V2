using System.Linq.Expressions;

namespace RERPAPI.IRepo
{
    public interface IGenericRepo<T> where T : class
    {
        List<T> GetAll(Expression<Func<T, bool>> predicate = null);

        T GetByID(int id);

        int Create(T item);

        int CreateRange(List<T> items);

        int Update(T item);

        int Delete(int id);

        int DeleteRange(List<T> items);

        Task<int> CreateAsync(T item);

        Task<int> CreateRangeAsync(List<T> items);

        Task<int> UpdateAsync(T item);

        Task<int> DeleteAsync(int id);

        Task<int> DeleteRangeAsync(List<T> items);

        int UpdateFieldByAttribute<TValue>(Expression<Func<T, bool>> predicate, Dictionary<Expression<Func<T, object>>, TValue> updatedFields);
        Task<int> UpdateFieldByAttributeAsync<TValue>(Expression<Func<T, bool>> predicate, Dictionary<Expression<Func<T, object>>, TValue> updatedFields);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        //void SetClaim(Dictionary<string, string> claim);
    }
}