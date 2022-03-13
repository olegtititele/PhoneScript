using Npgsql;
using System;


namespace PostgreSQL
{
    public class PostDB
    {

        public static string userdb = "Host=94.103.88.12;Username=guessick;Password=Adminsoset3.;Database=users";
        public static string all_adsdb = "Host=94.103.88.12;Username=guessick;Password=Adminsoset3.;Database=all_users_ads";
        public static string hash_adsdb = "Host=94.103.88.12;Username=guessick;Password=Adminsoset3.;Database=hash_users_ads";

        // Получить STATE юзера
        public static int state(string user_id)
        {
            try
            {
                using var con = new NpgsqlConnection(userdb);
                con.Open();
                var sql = $"SELECT state FROM users_table WHERE UserId = '{user_id.ToString()}'";
                using var cmd = new NpgsqlCommand(sql, con);
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
                
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        public static List<string> get_users_id()
        {
            List<string> parameters= new List<string>();
            using var con = new NpgsqlConnection(userdb);
            con.Open();
            var sql = $"SELECT UserId FROM users_table";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    parameters.Add(reader.GetString(0));
                }
                con.Close();
                return parameters;
            }    

        }

        public static long get_links_length(string user_id)
        {
            try
            {
                using var con = new NpgsqlConnection(hash_adsdb);
                con.Open();
                var sql = $"SELECT * FROM h{user_id}";
                using var cmd = new NpgsqlCommand(sql, con);
                var reader = cmd.ExecuteReader();
                long length = 0;
                while (reader.Read())
                {
                    length++;
                }
                return length;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static long get_links_length_without_number(string user_id)
        {
            try
            {
                using var con = new NpgsqlConnection(hash_adsdb);
                con.Open();
                var sql = $"SELECT * FROM h{user_id} WHERE sellerphone = 'Не указан'";
                using var cmd = new NpgsqlCommand(sql, con);
                var reader = cmd.ExecuteReader();
                long length = 0;
                while (reader.Read())
                {
                    length++;
                }
                return length;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static string get_announ_count(string user_id)
        {
            using var con = new NpgsqlConnection(userdb);
            con.Open();
            var sql = $"SELECT announ_count FROM users_table WHERE UserId = '{user_id}'";
            using var cmd = new NpgsqlCommand(sql, con);
            var result = cmd.ExecuteScalar();
            return result.ToString();
        }

        public static List<string> get_links(string user_id)
        {
            List<string> parameters= new List<string>();
            using var con = new NpgsqlConnection(hash_adsdb);
            con.Open();
            var sql = $"SELECT advertisementlink FROM h{user_id} WHERE sellerphone = 'Не указан'";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    parameters.Add(reader.GetString(0));
                }
                con.Close();
                return parameters;
            }    
        }

        public static void update_phone_number_inhash(string user_id, string phone_number, string link)
        {
            using (var conn = new NpgsqlConnection(hash_adsdb))
            {
                Console.Out.WriteLine("Opening connection");
                conn.Open();

                using (var command = new NpgsqlCommand($"UPDATE h{user_id} SET sellerphone = @q WHERE advertisementlink = @n", conn))
                {
                    command.Parameters.AddWithValue("n", link);
                    command.Parameters.AddWithValue("q", phone_number);
                    int nRows = command.ExecuteNonQuery();
                    Console.Out.WriteLine(String.Format("Number of rows updated={0}", nRows));
                }
            }
        }

        public static void update_phone_number_inmain(string user_id, string phone_number, string link)
        {
            using (var conn = new NpgsqlConnection(all_adsdb))
            {
                Console.Out.WriteLine("Opening connection");
                conn.Open();

                using (var command = new NpgsqlCommand($"UPDATE a{user_id} SET sellerphone = @q WHERE advertisementlink = @n", conn))
                {
                    command.Parameters.AddWithValue("n", link);
                    command.Parameters.AddWithValue("q", phone_number);
                    int nRows = command.ExecuteNonQuery();
                    Console.Out.WriteLine(String.Format("Number of rows updated={0}", nRows));
                }
            }
        }


        public static void delete_broken_advertisement_fromhash(string user_id, string adv_link)
        {
            // DELETE FROM products WHERE price = 10;
            using var con = new NpgsqlConnection(hash_adsdb);
            con.Open();
            var sql = $"DELETE FROM h{user_id} WHERE advertisementlink = '{adv_link}'";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            Console.WriteLine($"Удалил{adv_link}");
        }

        public static void delete_broken_advertisement_frommain(string user_id, string adv_link)
        {
            // DELETE FROM products WHERE price = 10;
            using var con = new NpgsqlConnection(all_adsdb);
            con.Open();
            var sql = $"DELETE FROM a{user_id} WHERE advertisementlink = '{adv_link}'";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.ExecuteNonQuery();
            Console.WriteLine($"Удалил{adv_link}");
        }

    }
}        