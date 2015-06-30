using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RD.Entities
{
    public interface IDAO
    {
        /// <summary>
        /// Saves the object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Save();

        /// <summary>
        /// Updates the object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Update();

        /// <summary>
        /// Deletes the object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Delete();
    }
}
