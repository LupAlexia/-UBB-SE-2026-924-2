using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface IRepository<TKey, TValue>
        where TValue : class
    {
        TValue GetById(TKey id);
        TKey CreateNewEntity(TValue elem);
        void DeleteById(TKey id);
        void UpdateById(TKey id, TValue elem);
        IEnumerable<TValue> GetAll();
    }
}
