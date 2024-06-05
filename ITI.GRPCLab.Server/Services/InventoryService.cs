using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ITI.AuthDemo.Server;
using ITI.GRPCLab.Server.Protos;
using Microsoft.AspNetCore.Authorization;
using static ITI.GRPCLab.Server.Protos.InventoryService;

namespace ITI.GRPCLab.Server.Services
{
    public class InventoryService : InventoryServiceBase
    {
        
        static List<Product> Products { get; set; }

        public List<RecerviedProduct> recerviedProducts { get; set; }

        public InventoryService()
        {
            Products = new List<Product>();
            recerviedProducts = new List<RecerviedProduct>()
            {
                new RecerviedProduct { Id = 1, Name = "Product 1", Quantity = 10 },
                new RecerviedProduct { Id = 2, Name = "Product 2", Quantity = 20 },
                new RecerviedProduct { Id = 3, Name = "Product 3", Quantity = 30 },
                new RecerviedProduct { Id = 4, Name = "Product 4", Quantity = 40 },

            };
        }

        [Authorize(AuthenticationSchemes = Consts.ApiKeySchemeName)]
        public override async Task<IsProductExisted> GetProductById(Id request, ServerCallContext context)
        {
            var prodcut = Products.FirstOrDefault(p => p.Id == request.Id_);

            if (prodcut != null)
            {
                return await Task.FromResult(new IsProductExisted
                {
                    IsExistd = true,
                });
            }

            return await Task.FromResult(new IsProductExisted
            {
                IsExistd = false,
            });
        }

        [Authorize(AuthenticationSchemes = Consts.ApiKeySchemeName)]
        public override async Task<Product> AddProduct(Product request, ServerCallContext context)
        {
            Products.Add(request);

            return await Task.FromResult(request);
        }

        [Authorize(AuthenticationSchemes = Consts.ApiKeySchemeName)]
        public override async Task<Product> UpdateProduct(Product request, ServerCallContext context)
        {
            var product = Products.FirstOrDefault(p => p.Id == request.Id);

            product.Name = request.Name;
            product.Descripton = request.Descripton;
            product.Quantity = request.Quantity;

            return await Task.FromResult(product);
        }

        [Authorize(AuthenticationSchemes = Consts.ApiKeySchemeName)]
        public override async Task<NumberOfProducts> AddBulkProducts(IAsyncStreamReader<RecerviedProduct> requestStream, ServerCallContext context)
        {
            int count = 0;
            await foreach(var product in requestStream.ReadAllAsync())
            {
                recerviedProducts.Add(product);
                ++count;
            }
            return await Task.FromResult(new NumberOfProducts { Count = count });
        }

        [Authorize(AuthenticationSchemes = Consts.ApiKeySchemeName)]
        public override async Task GetProductReport(Empty request, IServerStreamWriter<RecerviedProduct> responseStream, ServerCallContext context)
        {
            foreach (var product in recerviedProducts)
            {
                await responseStream.WriteAsync(product);
            }
            await Task.CompletedTask;
        }
    }
}
