using Microsoft.Data.SqlClient;

namespace WebApplication1.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private static int idCounter = 0;
        private readonly IConfiguration _configuration;

        public WarehouseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> CzyProduktIstnieje(int id)
        {
            return await SprawdzCzyIstnieje("Product", "IdProduct", id);
        }

        public async Task<bool> CzyMagazynIstnieje(int id)
        {
            return await SprawdzCzyIstnieje("WareHouse", "idWarehouse", id);
        }

        public async Task<bool> CzyZrealizowane(int id)
        {
            return await SprawdzCzyIstnieje("Product_Warehouse", "IdOrder", id);
        }

        public async Task DodajProdukt(int id, int idWarehouse, int amount, string createdAt)
        {
            string query = @"INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
                            VALUES (@idWarehouse, @id, 1, @Amount, (SELECT price FROM product WHERE IdProduct = @id), @CreatedAt );";
            idCounter++;
            using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ID", id);
            command.Parameters.AddWithValue("@idWarehouse", idWarehouse);
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@CreatedAt", createdAt);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            query = "SELECT SCOPE_IDENTITY()";
            command.CommandText = query;
            await command.ExecuteScalarAsync();
        }

        public async Task<bool> CzyWKolejnosci(int id)
        {
            return await SprawdzCzyIstnieje("[Order]", "IdProduct", id);
        }

        public async Task AktualizacjaDanych(int id)
        {
            string query = "UPDATE [Order] SET FulfilledAt = GETDATE() WHERE IdProduct = @ID";
            using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ID", id);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        private async Task<bool> SprawdzCzyIstnieje(string table, string column, int id)
        {
            string query = $"SELECT 1 FROM {table} WHERE {column} = @ID";
            using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ID", id);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            return result is not null;
        }
    }
}
