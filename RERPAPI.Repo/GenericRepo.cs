using Microsoft.EntityFrameworkCore;
using RERPAPI.IRepo;
using RERPAPI.Model.Context;
using RERPAPI.Model.DTO;
using System.Linq.Expressions;

namespace RERPAPI.Repo
{
    public class GenericRepo<T> : IGenericRepo<T> where T : class, new()
    {
        protected RTCContext db { get; set; }
        protected DbSet<T> table;

        //public GenericRepo()
        //{
        //    db = new RTCContext();
        //    table = db.Set<T>();
        //}


        public GenericRepo(CurrentUser currentUser)
        {
            db = new RTCContext();
            db.CurrentUser = currentUser;
            table = db.Set<T>();
        }

        public GenericRepo(RTCContext db, CurrentUser currentUser)
        {
            this.db = db;
            table = db.Set<T>();
            db.CurrentUser = currentUser;
        }

        public List<T> GetAll(Expression<Func<T, bool>> predicate = null)
        {
            try
            {
                if (predicate == null) return table.ToList() ?? new List<T>();
                else return table.Where(predicate).ToList() ?? new List<T>(); ; // EF sẽ dịch sang SQL WHERE
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public T GetByID(int id)
        {
            try
            {
                T model = table.Find(id) ?? new T();
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<T> GetByIDAsync(int id)
        {
            try
            {
                T model = await table.FindAsync(id) ?? new T();
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public int Create(T item)
        {
            try
            {
                table.Add(item);
                return db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public int CreateRange(List<T> items)
        {
            try
            {
                table.AddRange(items);
                return db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }



        public int Update(T item)
        {

            try
            {
                var fieldValues = new Dictionary<string, object>();
                int id = 0;
                var propid = typeof(T).GetProperty("ID");
                if (propid != null) id = Convert.ToInt32(propid.GetValue(item));

                var properties = typeof(T).GetProperties();
                foreach (var prop in properties)
                {
                    // Bỏ qua thuộc tính ID hoặc các thuộc tính không cần cập nhật
                    if (prop.Name != "ID" && prop.CanRead)
                    {
                        var value = prop.GetValue(item);
                        if (value != null) // Chỉ thêm nếu giá trị không null
                        {
                            fieldValues.Add(prop.Name, value);
                        }
                    }
                }

                // Tìm entity theo ID
                var entity = db.Set<T>().Find(id);
                if (entity == null)
                {
                    throw new Exception($"Entity with ID {id} not found.");
                }

                // Lấy type của entity
                Type type = typeof(T);

                // Cập nhật các trường động
                foreach (var field in fieldValues)
                {
                    // Kiểm tra thuộc tính
                    var property = type.GetProperty(field.Key);
                    if (property == null || !property.CanWrite)
                    {
                        throw new Exception($"Property {field.Key} not found or is not writable.");
                    }

                    // Gán giá trị cho thuộc tính (xử lý null)
                    property.SetValue(entity, field.Value == null ? null : field.Value);
                }

                //Gán lại giá trị updatedDate = null để sử lý khi SaveChangesAsync
                var updatedDate = entity.GetType().GetProperty("UpdatedDate");
                var updatedDateValue = fieldValues.ContainsKey("UpdatedDate") ? fieldValues["UpdatedDate"] : null;
                if (updatedDateValue == null) //trường UpdatedDate có value thì thôi
                {
                    if (updatedDate != null) updatedDate.SetValue(entity, null);
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                db.Entry(entity).State = EntityState.Modified;
                return db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating entity: {ex.Message}", ex);
            }
        }


        public int Delete(int id)
        {
            try
            {
                T model = table.Find(id) ?? new T();
                table.Remove(model);
                return db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<int> CreateAsync(T item)
        {
            try
            {
                await table.AddAsync(item);
                return await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<int> UpdateAsync(T item)
        {

            try
            {
                //var claims = _userPermissionService.GetClaims();
                var fieldValues = new Dictionary<string, object>();
                int id = 0;
                var propid = typeof(T).GetProperty("ID");
                if (propid != null) id = Convert.ToInt32(propid.GetValue(item));

                var properties = typeof(T).GetProperties();
                foreach (var prop in properties)
                {
                    // Bỏ qua thuộc tính ID hoặc các thuộc tính không cần cập nhật
                    if (prop.Name != "ID" && prop.CanRead)
                    {
                        var value = prop.GetValue(item);
                        if (value != null) // Chỉ thêm nếu giá trị không null
                        {
                            fieldValues.Add(prop.Name, value);
                        }
                    }
                }

                // Tìm entity theo ID
                var entity = db.Set<T>().Find(id);
                if (entity == null)
                {
                    throw new Exception($"Entity with ID {id} not found.");
                }

                // Lấy type của entity
                Type type = typeof(T);

                // Cập nhật các trường động
                foreach (var field in fieldValues)
                {
                    // Kiểm tra thuộc tính
                    var property = type.GetProperty(field.Key);
                    if (property == null || !property.CanWrite)
                    {
                        throw new Exception($"Property {field.Key} not found or is not writable.");
                    }

                    // Gán giá trị cho thuộc tính (xử lý null)
                    property.SetValue(entity, field.Value == null ? null : field.Value);
                }

                //Gán lại giá trị updatedDate = null để sử lý khi SaveChangesAsync
                var updatedDate = entity.GetType().GetProperty("UpdatedDate");
                var updatedDateValue = fieldValues.ContainsKey("UpdatedDate") ? fieldValues["UpdatedDate"] : null;
                if (updatedDateValue == null) //trường UpdatedDate có value thì thôi
                {
                    if (updatedDate != null) updatedDate.SetValue(entity, null);
                }

                db.Entry(entity).State = EntityState.Modified;
                // Lưu thay đổi vào cơ sở dữ liệu
                return await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating entity: {ex.Message}", ex);
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            try
            {
                T model = await table.FindAsync(id) ?? new T();
                table.Remove(model);
                return await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<int> CreateRangeAsync(List<T> items)
        {
            try
            {
                await table.AddRangeAsync(items);
                return await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public int DeleteRange(List<T> items)
        {
            try
            {
                table.RemoveRange(items);
                return db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<int> DeleteRangeAsync(List<T> items)
        {
            try
            {
                table.RemoveRange(items);
                return await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<int> DeleteByAttributeAsync(string propertyName, object value)
        {
            try
            {
                // Tạo tham số x
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, propertyName);

                // Ép kiểu value về đúng kiểu của property (xử lý nullable)
                var propertyType = property.Type;
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    // Nullable<T> → ép sang kiểu T?
                    var underlyingType = Nullable.GetUnderlyingType(propertyType);
                    value = value == null ? null : Convert.ChangeType(value, underlyingType);
                }
                else
                {
                    // Kiểu thường
                    value = Convert.ChangeType(value, propertyType);
                }

                var constant = Expression.Constant(value, propertyType);

                // x => x.PropertyName == value
                var equal = Expression.Equal(property, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

                // Lấy bản ghi đầu tiên thỏa điều kiện
                var entityToDelete = await table.FirstOrDefaultAsync(lambda);

                if (entityToDelete == null)
                {
                    return 0; // Không có bản ghi nào cần xóa
                }

                // Xóa và lưu
                table.Remove(entityToDelete);
                return await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting by attribute: {ex.Message}", ex);
            }
        }
        public async Task<int> UpdateRangeAsync<TValue>(Expression<Func<T, bool>> predicate, Dictionary<Expression<Func<T, object>>, TValue> updatedFields)
        {
            try
            {
                var entities = await table.Where(predicate).ToListAsync();
                if (!entities.Any()) return 0;

                //public void SetClaim(Dictionary<string, string> claim)
                //{
                //    db.Claim = claim;
                //}
                foreach (var entity in entities)
                {
                    foreach (var field in updatedFields)
                    {
                        var propertyName = ((MemberExpression)(field.Key.Body is UnaryExpression u
                            ? u.Operand
                            : field.Key.Body)).Member.Name;

                        var property = typeof(T).GetProperty(propertyName);
                        if (property != null && property.CanWrite)
                        {
                            property.SetValue(entity, field.Value);
                        }
                    }
                }

                table.UpdateRange(entities);
                return await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating entities by attribute: {ex.Message}", ex);
            }
        }

        public int UpdateFieldByAttribute<TValue>(Expression<Func<T, bool>> predicate, Dictionary<Expression<Func<T, object>>, TValue> updatedFields)
        {
            try
            {
                var entities = table.Where(predicate).ToList();
                if (!entities.Any()) return 0;

                foreach (var entity in entities)
                {
                    foreach (var field in updatedFields)
                    {
                        var propertyName = ((MemberExpression)(field.Key.Body is UnaryExpression u ? u.Operand : field.Key.Body)).Member.Name;
                        var property = typeof(T).GetProperty(propertyName);
                        if (property != null && property.CanWrite)
                        {
                            property.SetValue(entity, field.Value);
                        }
                    }
                }

                return db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating fields: {ex.Message}", ex);
            }
        }

        public async Task<int> UpdateFieldByAttributeAsync<TValue>(Expression<Func<T, bool>> predicate, Dictionary<Expression<Func<T, object>>, TValue> updatedFields)
        {
            try
            {
                var entities = await table.Where(predicate).ToListAsync();
                if (!entities.Any()) return 0;

                foreach (var entity in entities)
                {
                    foreach (var field in updatedFields)
                    {
                        var propertyName = ((MemberExpression)(field.Key.Body is UnaryExpression u ? u.Operand : field.Key.Body)).Member.Name;
                        var property = typeof(T).GetProperty(propertyName);
                        if (property != null && property.CanWrite)
                        {
                            property.SetValue(entity, field.Value);
                        }
                    }
                }

                return await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating fields: {ex.Message}", ex);
            }
        }
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await table.AnyAsync(predicate);
        }

        //public void SetClaim(Dictionary<string, string> claim)
        //{
        //    db.Claim = claim;
        //}


    }
}