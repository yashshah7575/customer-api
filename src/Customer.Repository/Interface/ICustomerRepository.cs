namespace Customer.Repository.Interface
{
    public interface ICustomerRepository
    {
        /// <summary>
        /// Get customer by id
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <returns>Returns customer entity if exist</returns>
        Task<CustomerEntity?> GetByIdAsync(Guid id);

        /// <summary>
        /// Get all customers from data store
        /// </summary>
        /// <returns>Returns list of customer entity</returns>
        Task<IEnumerable<CustomerEntity>> GetAllAsync();

        /// <summary>
        /// Add Customer in DB and return saved entity
        /// </summary>
        /// <param name="customer">Customer Entity</param>
        /// <returns>Returns Customer Entity</returns>
        Task<CustomerEntity> AddAsync(CustomerEntity customer);

        /// <summary>
        /// Updates customer
        /// </summary>
        /// <param name="customer">Customer to update</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(CustomerEntity customer);

        /// <summary>
        /// Deletes the customer by id
        /// </summary>
        /// <param name="id">Id of customer to delete</param>
        /// <returns>Bool</returns>
        Task<bool> DeleteAsync(Guid id);
    }
}