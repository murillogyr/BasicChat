using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace ChatBasicoSetores
{
    public partial class Form1 : Form
    {
        // definir o IP do PC onde a pasta está compartilhada
        private string pastaCompartilhada = @"\\endereço ipv4\pastaCompartilhada";
        private string arquivoMensagens = "mensagens.txt";
        private string caminhoArquivo => Path.Combine(pastaCompartilhada, arquivoMensagens);

        // guarda as linhas que já foram exibidas
        private int ultimaLinhaExibida = 0;

        public Form1()
        {
            this.AutoScaleMode = AutoScaleMode.None; // trava a escala
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // config visual do txtMensagem
            txtMensagem.Multiline = false;
            txtMensagem.Font = new Font(txtMensagem.Font.FontFamily, 36);
            txtMensagem.Height = 60;
            txtMensagem.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // listBox ocupa todo espaço disponível
            listMensagens.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // configura ListBox para OwnerDraw
            listMensagens.DrawMode = DrawMode.OwnerDrawVariable;
            listMensagens.MeasureItem += ListMensagens_MeasureItem;
            listMensagens.DrawItem += ListMensagens_DrawItem;

            // verifica se a pasta compartilhada existe
            if (!Directory.Exists(pastaCompartilhada))
                Directory.CreateDirectory(pastaCompartilhada);

            // cria o arquivo de mensagens se não existir
            if (!File.Exists(caminhoArquivo))
                File.WriteAllText(caminhoArquivo, "");

            AtualizarMensagens();
            timer1.Interval = 2000; // 🔹 atualiza a cada 2 segundos
            timer1.Start();
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMensagem.Text)) return;

            string msg = $"[{DateTime.Now:dd/MM/yyyy|HH:mm}] {Environment.MachineName}: {txtMensagem.Text}";

            try
            {
                File.AppendAllText(caminhoArquivo, msg + Environment.NewLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar mensagem: " + ex.Message);
            }

            txtMensagem.Clear();
            CarregarMensagens();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CarregarMensagens();
        }

        private void CarregarMensagens()
        {
            try
            {
                if (!File.Exists(caminhoArquivo))
                {
                    listMensagens.Items.Clear();
                    ultimaLinhaExibida = 0;
                    return;
                }

                var linhas = File.ReadAllLines(caminhoArquivo);
                var novasMensagens = linhas.Skip(ultimaLinhaExibida).ToArray();
                if (novasMensagens.Length == 0) return;

                listMensagens.BeginUpdate();
                foreach (var l in novasMensagens)
                    listMensagens.Items.Add(l);
                if (listMensagens.Items.Count > 0)
                    listMensagens.TopIndex = listMensagens.Items.Count - 1;
                listMensagens.EndUpdate();

                ultimaLinhaExibida = linhas.Length;
            }
            catch { }
        }

        private void AtualizarMensagens()
        {
            CarregarMensagens();
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            if (listMensagens.SelectedIndex >= 0)
            {
                // remove apenas os itens selecionados
                listMensagens.Items.RemoveAt(listMensagens.SelectedIndex);
            }
            else
            {
                // limpa todo o listBox
                listMensagens.Items.Clear();
            }
        }

        private void btnAbrirLog_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(pastaCompartilhada))
                    Directory.CreateDirectory(pastaCompartilhada);

                if (!File.Exists(caminhoArquivo))
                    File.WriteAllText(caminhoArquivo, "");

                var psi = new ProcessStartInfo
                {
                    FileName = "notepad.exe",
                    Arguments = $"\"{caminhoArquivo}\"",
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao abrir log: " + ex.Message);
            }
        }

        private void txtMensagem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnEnviar.PerformClick();
            }
        }

        // eventos para OwnerDraw do listBox
        private void ListMensagens_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index < 0) return;
            string texto = listMensagens.Items[e.Index].ToString();
            SizeF tamanho = e.Graphics.MeasureString(texto, listMensagens.Font, listMensagens.Width);
            e.ItemHeight = (int)tamanho.Height + 4; // adiciona um padding
        }

        private void ListMensagens_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            e.DrawBackground();
            string texto = listMensagens.Items[e.Index].ToString();
            using (Brush brush = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(texto, e.Font, brush, e.Bounds);
            }
            e.DrawFocusRectangle();
        }

        // eventos vazios do Design
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void txtMensagem_TextChanged(object sender, EventArgs e) { }
        private void By_Murillo_Carvalho_TextChanged(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label1_Click_1(object sender, EventArgs e) { }
    }
}
