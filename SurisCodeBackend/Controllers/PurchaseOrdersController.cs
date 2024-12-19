using Microsoft.AspNetCore.Mvc;
using SurisCodeBackend.Models;

namespace SurisCodeBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseOrdersController : ControllerBase
    {
        [HttpPost]
        public IActionResult SubmitOrder([FromBody] PurchaseOrder purchaseOrder)
        {
            try
            {
                ValidateSellerExists(purchaseOrder.Vendedor);
                ValidateArticlesAndTotal(purchaseOrder.Articulos);

                return Ok(new { message = "Orden cargada con �xito." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private void ValidateSellerExists(int vendedorId)
        {
            if (!Seller.GetList().Any(seller => seller.Id == vendedorId))
            {
                throw new Exception("Vendedor no v�lido o no encontrado.");
            }
        }

        private void ValidateArticlesAndTotal(List<string> articulos)
        {
            if (articulos == null || !articulos.Any())
            {
                throw new Exception("La lista de art�culos est� vac�a.");
            }

            List<string> invalidArticles = articulos
                .Where(a => !Article.GetList().Any(article => article.Codigo == a))
                .ToList();

            if (invalidArticles.Any())
            {
                throw new Exception($"Art�culos inv�lidos: {string.Join(", ", invalidArticles)}.");
            }

            double totalOrderValue = articulos
                .Sum(a => Article.GetList().First(article => article.Codigo == a).Precio);

            if (totalOrderValue == 0)
            {
                throw new Exception("El total de la orden no puede ser 0.");
            }
        }
    }
}
