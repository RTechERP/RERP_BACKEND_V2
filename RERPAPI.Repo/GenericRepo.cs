using Microsoft.EntityFrameworkCore;
using RERPAPI.IRepo;
using RERPAPI.Model.Context;
using RERPAPI.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.Repo
{
    public class GenericRepo<T> : IGenericRepo<T> where T : class, new()
    {
        protected RTCContext db { get; set; }
        protected DbSet<T> table = null;
        public GenericRepo()
        {
            db = new RTCContext();
            table = db.Set<T>();
        }

        public GenericRepo(RTCContext db)
        {
            this.db = db;
            table = db.Set<T>();
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

        /*public int Update(T item)
        {
            try
            {
                table.Attach(item);
                db.Entry(item).State = EntityState.Modified;
                return db.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
            }
        }*/

        public int Update(T item)
        {
            //try
            //{
            //    table.Attach(item);
            //    db.Entry(item).State = EntityState.Modified;
            //    return db.SaveChanges();
            //}
            //catch (Exception ex)
            //{

            //    throw new Exception(ex.ToString());
            //}

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

                // Lưu thay đổi vào cơ sở dữ liệu
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
                return db.SaveChanges();
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
                table.Attach(item);
                db.Entry(item).State = EntityState.Modified;
                return await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.ToString());
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



        public int UpdateFieldsByID(int ID, T item)
        {
            try
            {
                var fieldValues = new Dictionary<string, object>();


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
                var entity = db.Set<T>().Find(ID);
                if (entity == null)
                {
                    throw new Exception($"Entity with ID {ID} not found.");
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

                // Lưu thay đổi vào cơ sở dữ liệu
                return db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating entity: {ex.Message}", ex);
            }
        }
    }
}
