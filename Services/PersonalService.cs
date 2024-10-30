using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Module07DataAccess.Model;
using System.Data;

namespace Module07DataAccess.Services
{
    public class PersonalService
    {
        private readonly string _connectionString;

        public PersonalService()
        {
            var dbService = new DatabaseConnectionService();
            _connectionString = dbService.GetConnectionString();
        }


        //Retrieves all data
        public async Task<List<Personal>> GetAllPersonalsAsync()
        {
            var personalService = new List<Personal>();

            // Correct instantiation of MySqlConnection
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("SELECT * FROM tblemployee", conn);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        personalService.Add(new Personal
                        {
                            id = reader.GetInt32("id"),
                            name = reader.GetString("name"),
                            address = reader.GetString("address"),
                            email = reader.GetString("email"),
                            phone = reader.GetString("phone"),
                        });
                    }
                }
                return personalService;
            }
        }

        public async Task<bool> AddPersonalAsync(Personal newPerson)
        {
            try
            {   using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("INSERT INTO tblemployee (name, address, email,phone) VALUES (@name, @address, @email, @phone)", conn);
                    cmd.Parameters.AddWithValue("@name", newPerson.name);
                    cmd.Parameters.AddWithValue("@address", newPerson.address);
                    cmd.Parameters.AddWithValue("@email", newPerson.email);
                    cmd.Parameters.AddWithValue("@phone", newPerson.phone);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding personal record: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeletePersonalAsync(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("DELETE FROM tblemployee WHERE id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine ($"Error deleting personal record: {ex.Message}");
                return false;
            }
        }
    }
}