using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using ITI.GRPCLab.Server.Protos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using System.Net;
using static ITI.GRPCLab.Server.Protos.InventoryService;

namespace ITI.GRPCLab.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> ManageProduct(Product product)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7080");
            var client = new InventoryServiceClient(channel);

            var productResponse = await client.GetProductByIdAsync(new Id { Id_= product.Id});

            if (!productResponse.IsExistd)
            {
                var addedProduct = await client.AddProductAsync(product);
                return Ok(addedProduct);
            }
            
            var updatedProduct = await client.UpdateProductAsync(product); 
            return Ok(updatedProduct);
        }

        [HttpPost("ProductsAdd")]
        public async Task<ActionResult> AddBulkProducts(List<RecerviedProduct> products)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7080");
            var client = new InventoryServiceClient(channel);

            var response = client.AddBulkProducts();
            foreach (var item in products)
            {
                await response.RequestStream.WriteAsync(item);
            }
            await response.RequestStream.CompleteAsync();
            var result = await response.ResponseAsync;
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult> GetReport()
        {
            List<RecerviedProduct> productToAdds = new List<RecerviedProduct>();

            var channel = GrpcChannel.ForAddress("https://localhost:7080");
            var client = new InventoryServiceClient(channel);

            var call = client.GetProductReport(new Empty());

            while (await call.ResponseStream.MoveNext(CancellationToken.None))
            {
                productToAdds.Add(call.ResponseStream.Current);
            }

            return Ok(productToAdds);
        }
    }
}
