using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carrier.CCP.Common.Database.Interface
{
    public interface IRepository<T>
    {
        #region Query Based Operation

        //Task<int> CountAsync(string schema = null);

        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync(string schema);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        Task<T> GetAsync(object name, dynamic id, string schema);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="filters">The filters.</param>
        /// <returns></returns>
        Task<T> GetAsync(string schema = null, IFilter filters = null);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="selectPropertyName"></param>
        /// <returns></returns>
        Task<T> GetAsync(string schema = null, IFilter filters = null, IList<string> selectPropertyName = null);

        /// <summary>
        /// Gets the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        T Get(object name, dynamic id, string schema);

        /// <summary>
        /// Gets the list asynchronous.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="sort">The sort.</param>
        /// <param name="selectPropertyName">Name of the select property.</param>
        /// <param name="distinct">if set to <c>true</c> [distinct].</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetListAsync(string schema = null, IFilter filters = null, IList<IOrder> sort = null,
            IList<string> selectPropertyName = null, bool distinct = false);

        /// <summary>
        /// Gets the paged asynchronous.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="filters">The filters.</param>
        /// <param name="sort">The sort.</param>
        /// <param name="selectPropertyName">Name of the select property.</param>
        /// <param name="distinct">if set to <c>true</c> [distinct].</param>
        /// <returns></returns>
        Task<(IEnumerable<T>, long)> GetPagedAsync(int limit, int offset, string schema = null, IFilter filters = null,
            IList<IOrder> sort = null, IList<string> selectPropertyName = null, bool distinct = false);

        #endregion Query Based Operation

        #region Command Based Operation

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="filters">The filters.</param>
        /// <returns>created entiry</returns>
        Task<T> AddAsync(T entity, string schema, IFilter filters = null);

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        Task<T> UpdateAsync(T entity, string schema, IFilter filters = null, IFilter existsFilter = null);

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        Task<IEnumerable<T>> UpdateAsync(T entity, string schema, IFilter filters, IEnumerable<string> cols, IFilter existsFilter = null);

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        Task<int> DeleteAsync(object name, dynamic id, string schema);
        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        Task<int> DeleteAsync(T entity, string schema = null);
        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="filters">The filters.</param>
        /// <returns></returns>
        Task<IEnumerable<T>> DeleteAsync(string schema, IFilter filters);

        #endregion Command Based Operation

        #region Stored Procedure API

        #region SP Query Operation

        /// <summary>
        /// Sps the get all asynchronous.
        /// </summary>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        Task<IEnumerable<T>> SP_GetListAsync(string spSchema = null, string storedProc = null);

        /// <summary>
        /// Sps the get all by filter asynchronous.
        /// </summary>
        /// <param name="dbInputs">The database inputs.</param>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        Task<IEnumerable<T>> SP_GetListByFilterAsync(IDictionary<object, dynamic> dbInputs, string spSchema = null, string storedProc = null);

        /// <summary>
        /// Sps the get all by entity asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        Task<IEnumerable<T>> SP_GetListByEntityAsync(T entity, string spSchema = null, string storedProc = null);

        /// <summary>
        /// Sps the get by identifier asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        Task<T> SP_GetByIdAsync(object name, dynamic id, string spSchema = null, string storedProc = null);

        /// <summary>
        /// Sps the get scalar by identifier asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        Task<dynamic> SP_GetColumnByIdAsync(object name, dynamic id, string spSchema, string storedProc);

        #endregion SP Query Operation

        #region SP Add

        /// <summary>
        /// Sps the add asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="entitySchema">The entity schema.</param>
        /// <returns></returns>        
        Task<IEnumerable<TResult>> SP_AddAsync<TResult>(T entity, string storedProc, string spSchema = null);
        #endregion SP Add

        #region SP Update

        /// <summary>
        /// Sps the update asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier or Data Table.</param>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        Task<IEnumerable<TResult>> SP_UpdateByTVPAsync<TResult>(object name, dynamic id, string spSchema = null, string storedProc = null);
        /// <summary>
        /// Sps the update asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        Task<IEnumerable<TResult>> SP_UpdateByEntityAsync<TResult>(T entity, string spSchema = null, string storedProc = null);

        #endregion SP Update

        #region SP Delete

        /// <summary>
        /// Sps the delete by table asynchronous.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        Task<int> SP_DeleteByTVPAsync(object name, dynamic id, string spSchema = null, string storedProc = null);
        /// <summary>
        /// Sps the delete by entity asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="spSchema">The sp schema.</param>
        /// <param name="storedProc">The stored proc.</param>
        /// <returns></returns>
        Task<int> SP_DeleteByEntityAsync(T entity, string spSchema = null, string storedProc = null);

        #endregion SP Delete

        #endregion Stored Procedure API
    }
}