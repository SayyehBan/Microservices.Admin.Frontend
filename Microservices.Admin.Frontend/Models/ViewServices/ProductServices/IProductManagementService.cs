using Microservices.Admin.Frontend.Models.Dto;
using Microsoft.AspNetCore.Authentication;
using RestSharp;
using System.Text.Json;

namespace Microservices.Admin.Frontend.Models.ViewServices.ProductServices;

public interface IProductManagementService
{
    List<ProductDto> GetProducts();
    ResultDto UpdateName(UpdateProductDto updateProduct);
}

public class RProductManagementService : IProductManagementService
{

    private readonly RestClient restClient;
    private readonly IHttpContextAccessor httpContextAccessor;

    public RProductManagementService(RestClient restClient, IHttpContextAccessor httpContextAccessor)
    {
        this.restClient = restClient;
        this.httpContextAccessor = httpContextAccessor;
    }

    public List<ProductDto> GetProducts()
    {
        var request = new RestRequest("/api/ProductManagement", Method.Get);
        var accsessToken = httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
        request.AddHeader("Authorization", $"Bearer {accsessToken}");
        var response = restClient.Execute(request);
        var products = JsonSerializer.Deserialize<List<ProductDto>>(response.Content);
        return products;
    }

    public ResultDto UpdateName(UpdateProductDto updateProduct)
    {
        var request = new RestRequest($"/api/ProductManagement", Method.Put);

        var accsessToken = httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
        request.AddHeader("Authorization", $"Bearer {accsessToken}");

        request.AddHeader("Content-Type", "application/json");
        string serializeModel = JsonSerializer.Serialize(updateProduct);
        request.AddParameter("application/json", serializeModel, ParameterType.RequestBody);
        var response = restClient.Execute(request);
        return GetResponseStatusCode(response);
    }

    private static ResultDto GetResponseStatusCode(RestResponse response)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return new ResultDto(false);
        }
        else
        {
            return new ResultDto(false, response.ErrorMessage);
        }
    }


}

public record UpdateProductDto(Guid ProductId, string Name);

public record ProductDto(Guid id, string? name, string? description,
    string? image, int price,
    ProductCategoryDto? productCategory);
public record ProductCategoryDto(Guid categoryId, string category);

