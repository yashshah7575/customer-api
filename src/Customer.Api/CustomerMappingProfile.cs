using AutoMapper;
using Customer.Common.Models.Customer;
using Customer.Repository;

public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        CreateMap<CustomerEntity, CustomerResponse>().ReverseMap();
        CreateMap<CreateCustomerRequest, CustomerEntity>().ReverseMap();
    }
}