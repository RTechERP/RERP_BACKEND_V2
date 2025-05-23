﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RERPAPI.IRepo
{
    public interface IGenericRepo<T> where T : class
    {
        List<T> GetAll();
        T GetByID(long id);

        int Create(T item);
        int CreateRange(List<T> items);
        int Update(T item);
        int Delete(long id);
        int DeleteRange(List<T> items);

        Task<int> CreateAsync(T item);
        Task<int> CreateRangeAsync(List<T> items);
        Task<int> UpdateAsync(T item);
        Task<int> DeleteAsync(int id);
        Task<int> DeleteRangeAsync(List<T> items);

        int UpdateFieldsByID(int ID, T item);
    }
}
