using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class Operations<T>
    {
        List<T> list = new();
        public Operations(List<T> list) 
        { 
            this.list = list;
        }
        public List<T> See(T item)
        {
            return list;
        }

        public List<T> Create(T item)
        {
            list.Add(item);

            return list;
        }
    }
}
