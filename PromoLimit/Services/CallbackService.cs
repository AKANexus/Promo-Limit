using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MlSuite.EntityFramework.EntityFramework;
using Npgsql.Internal.TypeHandlers;
using PromoLimit.Models.Local;
using PromoLimit.Models.MercadoLivre;

namespace PromoLimit.Services
{
    public class CallbackService
    {
        private readonly IServiceScopeFactory _resolver;

        public CallbackService(IServiceScopeFactory resolver)
        {
            _resolver = resolver;
        }

        public async Task RunCallbackChecks(Guid orderGuid)
        {
            IServiceProvider scopedProvider = _resolver.CreateScope().ServiceProvider;

            LoggingDataService logger = scopedProvider.GetRequiredService<LoggingDataService>();
            MlInfoDataService mlInfoDataService = scopedProvider.GetRequiredService<MlInfoDataService>();
            ProdutoDataService produtoDataService = scopedProvider.GetRequiredService<ProdutoDataService>();
            MlApiService mlApiService = scopedProvider.GetRequiredService<MlApiService>();
            TrilhaDbContext context = scopedProvider.GetRequiredService<TrilhaDbContext>();
            Guid operation = Guid.NewGuid();
            var orderInfo = await context.Pedidos.AsNoTracking().FirstAsync(x => x.Uuid == orderGuid);
            int userId = (int)orderInfo.SellerId;
            string orderId = orderInfo.Id.ToString();
            try
            {
                await logger.LogTrace("Running Callback Checks", $"RunCallbackChecks ({operation})");
            }
            catch (Exception e)
            {
                await logger.LogError(e.Message, $"RunCallbackChecks ({operation})");
                throw;
            }

            try
            {
                //using IServiceScope scope = provider.CreateScope();
                //MlInfoDataService scopedMlInfoDataService =
                //	provider.GetRequiredService<MlInfoDataService>();
                //ProdutoDataService scopedProdutoDataService =
                //	provider.GetRequiredService<ProdutoDataService>();

                var users = await mlInfoDataService.GetAll();
                var produtos = await produtoDataService.GetAllProdutos();

                if (users.Any(x => x.UserId == userId))
                {
                    var order = await mlApiService.GetOrderInfo(
                         long.Parse(orderId), userId, logger);
                    if (!order.Item1)
                    {
                        await logger.LogInformation($"order.Item1 was false", $"RunCallbackChecks ({operation})");

                        return;
                    }
                    if (order.Item2.Status != "paid")
                    {
                        await logger.LogInformation($"order.Item2.status was not paid", $"RunCallbackChecks ({operation})");

                        return;
                    }
                    await logger.LogInformation($"TRACE>> itens: {order.Item2.OrderItems.Count}", $"RunCallbackChecks ({operation})");

                    foreach (OrderItem orderItem in order.Item2.OrderItems)
                    {
                        await logger.LogInformation($"TRACE>> orderItem: {orderItem.Item.Id}, variation: {orderItem.Item.VariationId.ToString() ?? "--"}", $"RunCallbackChecks ({operation})");

                        Produto? prodTentativo;
                        if (orderItem.Item.VariationId is not null && orderItem.Item.VariationId != 0)
                        {
                            prodTentativo = produtos.FirstOrDefault(y =>
                                y.MLB == orderItem.Item.Id && y.Variacao == orderItem.Item.VariationId);
                        }
                        else
                        {
                            prodTentativo = produtos.FirstOrDefault(y => y.MLB == orderItem.Item.Id);
                        }

                        if (prodTentativo is null) continue;

                        await logger.LogInformation($"TRACE>> tentativo was not null", $"RunCallbackChecks ({operation})");

                        var mlItem = await mlApiService.GetProdutoFromMlb(prodTentativo.MLB);
                        await logger.LogInformation($"TRACE>> mlItem encontrado: {mlItem.Item2.Title}", $"RunCallbackChecks ({operation})");
                        if (mlItem.Item1 && mlItem.Item2.AvailableQuantity == prodTentativo.QuantidadeAVenda)
                        {
                            await logger.LogInformation($"TRACE>> mlItem.Item2.AvailableQuantity == prodTentativo.QuantidadeAVenda", $"RunCallbackChecks ({operation})");

                            return;
                        }
                        /*
                        try
                        {
                            var sku = mlItem.Item2.Attributes?.FirstOrDefault(x => x.Id == "SELLER_SKU");
                            if (sku != null)
                            {
                                await logger.LogTrace($"SKU {sku.ValueName}", $"RunCallbackChecks ({operation})");
                                var tinyTentative = await _tinyApiService.ProcuraEstoquePorCodigo(sku.ValueName);
                                if (tinyTentative is not null)
                                {
                                    await logger.LogTrace($"Existe um produto no Tiny com esse SKU ({sku.ValueName}). Vamos verificar o estoque", $"RunCallbackChecks ({operation})");

                                    var saldo = (int?)tinyTentative["saldo"];
                                    if (saldo is null)
                                    {
                                        await logger.LogTrace(
                                            $"Houve um erro ao puxar estoque do Tiny.\n[\"saldo\"] era nulo.\nIgnorando...",
                                            $"RunCallbackChecks ({operation})");
                                    }
                                    else
                                    {
                                        await logger.LogTrace($"Saldo no Tiny: {saldo}",
                                            $"RunCallbackChecks ({operation})");
                                        prodTentativo.Estoque = (int)saldo;
                                    }
                                }
                                else
                                {
                                    await logger.LogTrace(
                                        $"Não existe um produto no Tiny com esse SKU ({sku.ValueName}).\nIgnorando...",
                                        $"RunCallbackChecks ({operation})");
                                }

                            }
                            else
                            {
                                await logger.LogTrace($"SKU era nulo",
                                    $"RunCallbackChecks ({operation})");
                            }
                        }
                        catch (Exception e)
                        {
                            await logger.LogError($"Houve um erro ao puxar estoque do Tiny.\n{e.Message}\nIgnorando...",
                                $"RunCallbackChecks ({operation})");
                        }
                        */

                        if (prodTentativo.Estoque < prodTentativo.QuantidadeAVenda)
                        {
                            await logger.LogInformation($"TRACE>> Não há estoque {prodTentativo.Estoque} o bastante para repor na venda", $"RunCallbackChecks ({operation})");
                            return;
                        }

                        if (prodTentativo.Estoque >= prodTentativo.QuantidadeAVenda)
                        {
                            await logger.LogInformation("Atualizando estoque no ML", $"RunCallbackChecks ({operation})");
                            await logger.LogInformation($"prodTentativo.MLB: {prodTentativo.MLB}", $"RunCallbackChecks ({operation})");
                            await logger.LogInformation($"prodTentativo.QuantidadeAVenda: {prodTentativo.QuantidadeAVenda}", $"RunCallbackChecks ({operation})");
                            await logger.LogInformation($"notification.UserId: {userId}", $"RunCallbackChecks ({operation})");
                            await logger.LogInformation($"prodTentativo.Variacao: {prodTentativo.Variacao}", $"RunCallbackChecks ({operation})");

                            await mlApiService.AtualizaEstoqueDisponivel(prodTentativo.MLB,
                                prodTentativo.QuantidadeAVenda,
                                userId,
                                prodTentativo.Variacao, logger);
                        }

                        prodTentativo.Estoque -= orderItem.Quantity;


                        await logger.LogInformation($"TRACE>> Estoque:{prodTentativo.Estoque}, OrderQuant: {orderItem.Quantity}", $"RunCallbackChecks ({operation})");
                        await produtoDataService.AddOrUpdate(prodTentativo);
                    }
                }
                else
                {
                    await logger.LogWarning("Usuário do ML não cadastrado, pulando notificação", $"RunCallbackChecks ({operation})");
                }
            }
            catch (Exception e)
            {
                await logger.LogError(e.Message, $"RunCallbackChecks ({operation})");
            }
        }
    }
}
