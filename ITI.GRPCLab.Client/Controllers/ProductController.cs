using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using ITI.GRPCLab.Server.Protos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ITI.GRPCLab.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private const string ApiKey = "AIzaSyD7Q6Q6-4";

        [HttpPost]
        public async Task<ActionResult> ManageProduct(Product product)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7080", new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.SecureSsl
            });

            var callCredentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add("x-api-key", ApiKey);
                return Task.CompletedTask;
            });

            var client = new InventoryService.InventoryServiceClient(channel);

            var productResponse = await client.GetProductByIdAsync(new Id { Id_ = product.Id }, new CallOptions(credentials: callCredentials));

            if (!productResponse.IsExistd)
            {
                var addedProduct = await client.AddProductAsync(product, new CallOptions(credentials: callCredentials));
                return Ok(addedProduct);
            }

            var updatedProduct = await client.UpdateProductAsync(product, new CallOptions(credentials: callCredentials));
            return Ok(updatedProduct);
        }

        [HttpPost("ProductsAdd")]
        public async Task<ActionResult> AddBulkProducts(List<RecerviedProduct> products)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7080", new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.SecureSsl
            });

            var callCredentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add("x-api-key", ApiKey);
                return Task.CompletedTask;
            });

            var client = new InventoryService.InventoryServiceClient(channel);

            using var response = client.AddBulkProducts(new CallOptions(credentials: callCredentials));
            foreach (var item in products)
            {
                await response.RequestStream.WriteAsync(item);
            }
            await response.RequestStream.CompleteAsync();
            var result = await response.ResponseAsync;
            return Ok(result);
        }

        [HttpGet("GetReport")]
        public async Task<ActionResult> GetReport()
        {
            List<RecerviedProduct> productToAdds = new List<RecerviedProduct>();

            var channel = GrpcChannel.ForAddress("https://localhost:7080", new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.SecureSsl
            });

            var callCredentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                metadata.Add("x-api-key", ApiKey);
                return Task.CompletedTask;
            });

            var client = new InventoryService.InventoryServiceClient(channel);

            using var call = client.GetProductReport(new Empty(), new CallOptions(credentials: callCredentials));

            while (await call.ResponseStream.MoveNext(CancellationToken.None))
            {
                productToAdds.Add(call.ResponseStream.Current);
            }

            return Ok(productToAdds);
        }
    }
}
