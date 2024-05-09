namespace WebApplication1.Repositories;

public interface IWarehouseRepository
{
    Task<bool> CzyMagazynIstnieje(int id);
    Task<bool> CzyProduktIstnieje(int id);
    Task<bool> CzyZrealizowane(int id);
    Task<bool> CzyWKolejnosci(int id);
    Task AktualizacjaDanych(int id);
    Task DodajProdukt(int id, int idWarehouse, int amount, string createdAt);
}