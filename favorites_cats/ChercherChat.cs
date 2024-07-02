
using favorites_cats.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace favorites_cats
{
    public partial class ChercherChat : Form
    {
        public static string PathUrlImage;
        public ChercherChat()
        {
            InitializeComponent();
        }

        private void ChercherChat_Load(object sender, EventArgs e)
        {
            GetGatos();
            GetGatosPreferidos();
        }

        private void btnChercher_Click(object sender, EventArgs e)
        {
            lblObs.MaximumSize = new Size(400, 0);
            lblObs.AutoSize = true;
            lblObs.BorderStyle = BorderStyle.FixedSingle;
            ChercherChatInfo();
            btnFavGato.Visible = true;
            
        }

        private void GetGatosPreferidos()
        {
            DataTable listaDados = Data.DAOConexao.GetFavoritos();
            foreach (DataRow row in listaDados.Rows)
            {
                string nome = row["Nome"].ToString();
                Image img = DownloadImage(row["UrlImage"].ToString());
                object[] objGato = new object[] { nome, img };
                gridGatosPref.Rows.Add(objGato);
            }

        }

        private Image DownloadImage(string url)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(url);
                using (MemoryStream mem = new MemoryStream(data))
                {
                    return Image.FromStream(mem);
                }
            }
        }

        private void GetGatos()
        {
            string key = "live_J2Oz0Ve4fhAwhk3nzw9HrH1cP2WkwbrDKKBScOLJjZufQ7hCRtKdOfqxhbUMVVst";
            try
            {
                using (HttpClient http = new HttpClient())
                {
                    using (HttpResponseMessage resposta = http.GetAsync($"https://api.thecatapi.com/v1/breeds?api_key={key}").Result)
                    {
                        dynamic json = JsonConvert.DeserializeObject<dynamic>(resposta.Content.ReadAsStringAsync().Result);

                        List<string> names = new List<string>();
                        List<string> ids = new List<string>();

                        foreach (JObject obj in json)
                        {

                            var item = new ComboboxItem
                            {
                                Text = obj["name"]?.ToString(),
                                Value = obj["id"]?.ToString(),
                            };
                            cbChats.Items.Add(item);
                            cbChats.DisplayMember = item.Text;
                            cbChats.ValueMember = item.Value.ToString();

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}");
            }

        }

        private void ChercherChatInfo()
        {
            string idGato = (cbChats.SelectedItem as ComboboxItem).Value.ToString();
            string key = "live_J2Oz0Ve4fhAwhk3nzw9HrH1cP2WkwbrDKKBScOLJjZufQ7hCRtKdOfqxhbUMVVst";
            try
            {
                using (HttpClient http = new HttpClient())
                {
                
                    using (HttpResponseMessage resposta = http.GetAsync($"https://api.thecatapi.com/v1/images/search?breed_ids={idGato}&api_key={key}").Result)
                    {
                        dynamic json = JsonConvert.DeserializeObject<dynamic>(resposta.Content.ReadAsStringAsync().Result);
                        foreach (JObject obj in json)
                        {
                            PathUrlImage = obj["url"]?.ToString();
                            string urlFoto = obj["url"]?.ToString();
                            foreach (var item in obj["breeds"])
                            {
                                lblTemperamento.Text = item["temperament"].ToString();
                                lblOrigem.Text = item["origin"].ToString();
                                lblObs.Text = item["description"].ToString();
                            }
                            picFotoGato.Image = LoadImageFromUrl(picFotoGato, urlFoto);
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}");
            }

        }

        private System.Drawing.Image LoadImageFromUrl(PictureBox picFotoGato, string url)
        {
            Image image = null;
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    byte[] imageBytes = webClient.DownloadData(url);
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(imageBytes))
                    {
                       image = Image.FromStream(ms);
                    }
                }

                return image;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao baixar a imagem: " + ex.Message);
                return null;
            }

            
        }

        public class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        private void btnFavGato_Click(object sender, EventArgs e)
        {
            string idGato = (cbChats.SelectedItem as ComboboxItem).Value.ToString();

            try
            {
                Gato gato = new Gato()
                {
                    Id = idGato.ToString(),
                    Nome = cbChats.Text,
                    UrlImage = PathUrlImage.ToString(),
                };

                Data.DAOConexao.InsertGatoFavotito(gato);
                MessageBox.Show("Chat add à mes preferences!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

}
