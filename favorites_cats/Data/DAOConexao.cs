using favorites_cats.Model;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace favorites_cats.Data
{
    public class DAOConexao
    {
        public static string path = Directory.GetCurrentDirectory() + "\\bancodedados.sqlite";
        private static SQLiteConnection sqliteConnection;

        private static SQLiteConnection DbConnection()
        {
            sqliteConnection = new SQLiteConnection("Data Source=" + path);
            sqliteConnection.Open();
            return sqliteConnection;
        }

        public static void CreateDatabase()
        {
            try
            {
                if(File.Exists(path) == false)
                {
                    SQLiteConnection.CreateFile(path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        public static void CreateTables()
        {
            try
            {
                using(var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS Gatos(Id Varchar(50) NOT NULL CONSTRAINT Gatos_pk PRIMARY KEY, Nome Varchar(50), UrlImage Varchar(200))";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        public static DataTable GetFavoritos()
        {
            SQLiteDataAdapter adapter = null;
            DataTable dt = new DataTable();
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Gatos";
                    adapter = new SQLiteDataAdapter(cmd.CommandText, DbConnection());
                    adapter.Fill(dt);
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
            return dt;
        }

        public static DataTable GetGatoFavorito(string id)
        {
            SQLiteDataAdapter adapter = null;
            DataTable dt = new DataTable();
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = $"SELECT * FROM Gatos WHERE Id={id}";
                    adapter = new SQLiteDataAdapter(cmd.CommandText, DbConnection());
                    adapter.Fill(dt);
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
            return dt;
        }

        public static void InsertGatoFavotito(Gato favGato)
        {
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Gatos(Id, Nome, UrlImage) VALUES (@Id, @Nome, @UrlImage)";
                    cmd.Parameters.AddWithValue("@Id", favGato.Id);
                    cmd.Parameters.AddWithValue("@Nome", favGato.Nome);
                    cmd.Parameters.AddWithValue("@UrlImage", favGato.UrlImage);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }
        public static void DeleteGatoFavotito(string id)
        {
            try
            {
                using (var cmd = DbConnection().CreateCommand())
                {
                    cmd.CommandText = $"DELETE FROM Gatos WHERE Id={id}";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }
    }
}
