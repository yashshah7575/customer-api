using System.Net.Http.Json;
using Customer.Api.Client;

public class Helper
{
    private readonly HttpClient _httpClient;

    public Helper(string baseUrl)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task<List<CustomerDto>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<CustomerDto>>("api/customers") ?? [];
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<CustomerDto>($"api/customers/{id}");
    }

    public async Task<CustomerDto?> CreateAsync(CustomerDto customer)
    {
        var response = await _httpClient.PostAsJsonAsync("api/customers", customer);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<CustomerDto>()
            : null;
    }

    public async Task<bool> UpdateAsync(CustomerDto customer)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/customers/{customer.Id}", customer);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/customers/{id}");
        return response.IsSuccessStatusCode;
    }
}
