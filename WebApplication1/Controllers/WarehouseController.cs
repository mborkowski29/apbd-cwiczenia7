using Microsoft.AspNetCore.Mvc;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers;

public class WarehouseController : ControllerBase
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseController(IWarehouseRepository wareHouseRepository)
    {
        _warehouseRepository = wareHouseRepository;
    }
    
    [HttpPost("AddProduct/{productId}")]
    public async Task<IActionResult> DodajProduktDoWarehouse(int productId, int idWarehouse, int amount, string createdAt)
    {
        // walidacja danych
        if (productId <= 0 || idWarehouse <= 0 || amount <= 0 || string.IsNullOrEmpty(createdAt))
        {
            return BadRequest("Invalid input data.");
        }

        // sprawdzenie istnienia magazynu, produktu i czy zrealizowane
        if (!await _warehouseRepository.CzyMagazynIstnieje(idWarehouse) || 
            !await _warehouseRepository.CzyProduktIstnieje(productId) || 
            !await _warehouseRepository.CzyZrealizowane(idWarehouse))
        {
            return NotFound();
        }
        
        // sprawdzenie czy produkt jest w zamowieniu
        if (!await _warehouseRepository.CzyWKolejnosci(productId))
        {
            // aktualizacja
            _warehouseRepository.AktualizacjaDanych(productId);
        }

        // dodanie produktu
        await _warehouseRepository.DodajProdukt(productId, idWarehouse, amount, createdAt);
        return Ok();
    }
}

