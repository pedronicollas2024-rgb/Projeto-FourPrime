using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using FourPrime.Application.Abstractions.Repositories;
using FourPrime.Domain.Entities;
using FourPrime.Infrastructure.Persistence;
using FourPrime.Infrastructure.Repositories;
using FourPrime.UI;
using Microsoft.EntityFrameworkCore;

namespace FourPrime.Ul
{
    public partial class FrmCarroForm : Form
    {
        public int? CarroIdEmEdicao { get; private set; }
        private int? _marcaIdEdicao;
        private int? _categoriaIdEdicao;
        public FrmCarroForm()
        {
            InitializeComponent();
            this.Load += FrmCarroForm_Load;
        }

        private async void FrmCarroForm_Load(object? sender, EventArgs e)
        {
            await CarregarCombosAsync();
            if (_marcaIdEdicao.HasValue)
                cmbMarca.SelectedValue = _marcaIdEdicao.Value;

            if (_categoriaIdEdicao.HasValue)
                cmbCategoria.SelectedValue = _categoriaIdEdicao.Value;
        }

        private async Task CarregarCombosAsync()
        {

            var conn = ConfigHelper.GetDefaultConnection();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(conn)
                .Options;

            using var context = new AppDbContext(options);

            IMarcaRepository marcaRepo = new MarcaRepository(context);
            ICategoriaRepository categoriaRepo = new CategoriaRepository(context);

            var marcas = await marcaRepo.GetAllAsync();
            var categorias = await categoriaRepo.GetAllAsync();

            cmbMarca.DisplayMember = "Nome";
            cmbMarca.ValueMember = "Id";
            cmbMarca.DataSource = marcas;

            cmbCategoria.DisplayMember = "Nome";
            cmbCategoria.ValueMember = "Id";
            cmbCategoria.DataSource = categorias;
        }

        private async void btnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtModelo.Text))
            {
                MessageBox.Show("Informe o modelo.");
                return;
            }

            var anoTexto = txtAno.Text.Trim();
            if (!int.TryParse(anoTexto, out var ano))
            {
                MessageBox.Show($"Ano inválido: \"{txtAno.Text}\"");
                return;
            }

            var precoTexto = TxtPreco.Text.Trim();
            if (!decimal.TryParse(precoTexto, out var preco))
            {
                MessageBox.Show("Preço inválido.");
                return;
            }

            if (cmbMarca.SelectedValue is null || cmbCategoria.SelectedValue is null)
            {
                MessageBox.Show("Selecione marca e categoria.");
                return;
            }

            var carro = new Carro
            {
                Id = CarroIdEmEdicao ?? 0,
                Modelo = txtModelo.Text.Trim(),
                Ano = ano,
                Preco = preco,
                MarcaId = (int)cmbMarca.SelectedValue,
                CategoriaId = (int)cmbCategoria.SelectedValue,

                Cor = "",
                Combustivel = "",
                Quilometragem = 0,
                ImagemUrl = "",
                Descricao = "",
                IsDestaque = false,
                DestaqueTipo = null
            };

            var conn = ConfigHelper.GetDefaultConnection();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(conn)
                .Options;

            using var context = new AppDbContext(options);
            ICarroRepository repo = new CarroRepository(context);

            if (CarroIdEmEdicao.HasValue)
                await repo.UpdateAsync(carro);
            else
                await repo.AddAsync(carro);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        public void CarregarParaEdicao(Carro carro)
        {
            CarroIdEmEdicao = carro.Id;

            txtModelo.Text = carro.Modelo;
            txtAno.Text = carro.Ano.ToString();
            TxtPreco.Text = carro.Preco.ToString();

            _marcaIdEdicao = carro.MarcaId;
            _categoriaIdEdicao = carro.CategoriaId;
        }

        private void txtPaisOrigem_TextChanged(object sender, EventArgs e)
        {

        }

        private void TxtPreco_TextChanged(object sender, EventArgs e)
        {

        }
    }
}