using Customer.Common.Models.Customer;

namespace Customer.Service.Interface
{
    public interface ICustomerService
    {
        /// <summary>
        /// Get customer by id
        /// </summary>
        /// <param name="id">Customer Id</param>
        /// <returns>Returns customer entity if exist</returns>
        Task<CustomerResponse?> GetByIdAsync(Guid id);

        /// <summary>
        /// Get all customers from data store
        /// </summary>
        /// <returns>Returns list of customer entity</returns>
        Task<IEnumerable<CustomerResponse>?> GetAllAsync();

        /// <summary>
        /// Add Customer in DB and return saved entity
        /// </summary>
        /// <param name="customerRequest">Customer Entity</param>
        /// <returns>Returns Customer Entity</returns>
        Task<CustomerResponse> AddAsync(CreateCustomerRequest customerRequest);

        /// <summary>
        /// Updates customer
        /// </summary>
        /// <param name="customer">Customer to update</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(UpdateCustomerRequest customer, Guid id);

        /// <summary>
        /// Deletes the customer by id
        /// </summary>
        /// <param name="id">Id of customer to delete</param>
        /// <returns>Bool</returns>
        Task<bool> DeleteAsync(Guid id);
    }
}