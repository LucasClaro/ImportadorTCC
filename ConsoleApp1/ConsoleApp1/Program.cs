using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

// See https://aka.ms/new-console-template for more information


public class Class1
{
    public string idFirebase { get; set; }
    public Conexo[] conexoes { get; set; }
    public string sala { get; set; }
    public string setor { get; set; }
}

public class Conexo
{
    public string bssid { get; set; }
    public string rssi { get; set; }
    public string ssid { get; set; }
}



namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            List<Class1> registros = new List<Class1>();

            var diretorio = new DirectoryInfo("C:\\Users\\Lucas\\Desktop");
            var files = diretorio.GetFiles("*.json");

            registros = JsonConvert.DeserializeObject<List<Class1>>(File.ReadAllText(files[0].FullName));

            SqlConnection cn = new SqlConnection();
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
            sb.DataSource = "DESKTOP-5LI3LJ1";
            sb.UserID = "sa";
            sb.Password = "senhaSpenser11";
            sb.InitialCatalog = "TCC";
            sb.Encrypt = false;
            cn.ConnectionString = sb.ConnectionString;

            cn.Open();

            if (registros != null)
            {
                foreach (Class1 registro in registros)
                {   
                    if (registro.conexoes != null)
                    {
                        SqlCommand cmd = cn.CreateCommand();
                        cmd.CommandText = "declare @x table (id int); insert into Registro2 (idFirebase, sala, setor) output inserted.id into @x values (@fire, @sala, @setor); select id from @x";
                        cmd.Parameters.Add("@fire", System.Data.SqlDbType.VarChar, 25).Value = registro.idFirebase;
                        cmd.Parameters.Add("@sala", System.Data.SqlDbType.VarChar, 20).Value = registro.sala;
                        cmd.Parameters.Add("@setor", System.Data.SqlDbType.VarChar, 15).Value = registro.setor;

                        SqlDataReader dr = cmd.ExecuteReader();

                        dr.Read();
                        int codigo = (int)dr[0];
                        dr.Close();

                        foreach (Conexo conexo in registro.conexoes)
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = @"insert into Conexao2 (bssid, rssi, ssid, idRegistro) values
                                (@bssid, @rssi, @ssid, @idRegistro)";
                            cmd.Parameters.Add("@bssid", System.Data.SqlDbType.Char, 17).Value = conexo.bssid;
                            cmd.Parameters.Add("@rssi", System.Data.SqlDbType.Int).Value = conexo.rssi;
                            cmd.Parameters.Add("@ssid", System.Data.SqlDbType.VarChar, 50).Value = conexo.ssid;
                            cmd.Parameters.Add("@idRegistro", System.Data.SqlDbType.Int).Value = codigo;

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            cn.Close();
        }
    }
}
