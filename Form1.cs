using System.Globalization;

namespace RentCar_Express;

public partial class Form1 : Form
{
    private const decimal SeguroCompleto = 300m;
    private const decimal CargoGpsDiario = 100m;
    private const decimal CargoDomicilio = 500m;
    private const decimal TasaItbis = 0.18m;
    private const decimal TasaDescuento = 0.15m;

    // Acumula el importe de todos los alquileres agregados.
    private decimal subtotal = 0m;

    public Form1()
    {
        InitializeComponent();
        Text = "RentCar Express - Alquileres";
        StartPosition = FormStartPosition.CenterScreen;
        nudDias.Minimum = 1;
        nudDias.Maximum = 30;
        nudDias.Value = 1;
        dtpSalida.Value = DateTime.Today;
        rbtBasico.Checked = true;
        rbtSucursal.Checked = true;

        lstAlquileres.View = View.Details;
        lstAlquileres.FullRowSelect = true;
        lstAlquileres.GridLines = true;
        lstAlquileres.MultiSelect = false;
        lstAlquileres.Columns.Clear();
        lstAlquileres.Columns.Add("Salida", 85);
        lstAlquileres.Columns.Add("Cliente", 105);
        lstAlquileres.Columns.Add("Vehículo", 105);
        lstAlquileres.Columns.Add("Días", 45);
        lstAlquileres.Columns.Add("Total", 95);

        ActualizarResultados(0m, 0m, 0m, 0m, 0m);
    }

    private void btnAgregar_Click(object? sender, EventArgs e)
    {
        string cliente = txtCliente.Text.Trim();
        string vehiculo = txtVehiculo.Text.Trim();

        if (string.IsNullOrWhiteSpace(cliente))
        {
            MessageBox.Show("Escribe el nombre del cliente.", "Dato requerido",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtCliente.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(vehiculo))
        {
            MessageBox.Show("Escribe el vehículo que se va a alquilar.", "Dato requerido",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtVehiculo.Focus();
            return;
        }

        if (!TryLeerTarifa(txtTarifa.Text, out decimal tarifa) || tarifa <= 0m)
        {
            MessageBox.Show("Introduce una tarifa diaria válida mayor que cero.", "Tarifa inválida",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTarifa.Focus();
            txtTarifa.SelectAll();
            return;
        }

        int dias = Decimal.ToInt32(nudDias.Value);
        decimal cargoSeguro = rbtCompleto.Checked ? SeguroCompleto : 0m;
        decimal cargoGps = chkGps.Checked ? CargoGpsDiario * dias : 0m;
        decimal totalAlquiler = (tarifa * dias) + cargoSeguro + cargoGps;

        var fila = new ListViewItem(dtpSalida.Value.ToShortDateString());
        fila.SubItems.Add(cliente);
        fila.SubItems.Add(vehiculo);
        fila.SubItems.Add(dias.ToString(CultureInfo.CurrentCulture));
        fila.SubItems.Add(totalAlquiler.ToString("C2", CultureInfo.GetCultureInfo("es-DO")));
        fila.Tag = totalAlquiler;
        lstAlquileres.Items.Add(fila);

        subtotal += totalAlquiler;
        ActualizarResultados(subtotal, 0m, 0m, 0m, 0m);

        // Deja los datos del cliente para poder agregar otro vehículo al mismo cliente.
        txtVehiculo.Clear();
        txtTarifa.Clear();
        nudDias.Value = 1;
        rbtBasico.Checked = true;
        chkGps.Checked = false;
        txtVehiculo.Focus();
    }

    private void btnCalcular_Click(object? sender, EventArgs e)
    {
        if (lstAlquileres.Items.Count == 0)
        {
            MessageBox.Show("Agrega al menos un vehículo antes de calcular.", "Sin alquileres",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        decimal descuento = chkCorporativo.Checked ? subtotal * TasaDescuento : 0m;
        decimal domicilio = rbtDomicilio.Checked ? CargoDomicilio : 0m;
        decimal baseImponible = subtotal - descuento + domicilio;
        decimal itbis = baseImponible * TasaItbis;
        decimal total = baseImponible + itbis;

        ActualizarResultados(subtotal, descuento, domicilio, itbis, total);
        MessageBox.Show($"Total a pagar: {total.ToString("C2", CultureInfo.GetCultureInfo("es-DO"))}",
            "Cálculo realizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private static bool TryLeerTarifa(string texto, out decimal tarifa)
    {
        return decimal.TryParse(texto, NumberStyles.Number | NumberStyles.AllowCurrencySymbol,
                   CultureInfo.CurrentCulture, out tarifa)
               || decimal.TryParse(texto, NumberStyles.Number | NumberStyles.AllowCurrencySymbol,
                   CultureInfo.InvariantCulture, out tarifa);
    }

    private void btnLimpiar_Click(object? sender, EventArgs e)
    {
        txtCliente.Clear();
        txtVehiculo.Clear();
        txtTarifa.Clear();
        chkCorporativo.Checked = false;
        chkGps.Checked = false;
        rbtBasico.Checked = true;
        rbtSucursal.Checked = true;
        nudDias.Value = 1;
        dtpSalida.Value = DateTime.Today;
        lstAlquileres.Items.Clear();
        subtotal = 0m;
        ActualizarResultados(0m, 0m, 0m, 0m, 0m);
        txtCliente.Focus();
    }

    private void btnSalir_Click(object? sender, EventArgs e)
    {
        if (MessageBox.Show("¿Deseas salir del programa?", "Confirmar salida",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            Close();
        }
    }

    private void ActualizarResultados(decimal sub, decimal desc,
        decimal domicilio, decimal itbis, decimal total)
    {
        var moneda = CultureInfo.GetCultureInfo("es-DO");
        lblSubtotal.Text = sub.ToString("C2", moneda);
        lblDescuento.Text = desc.ToString("C2", moneda);
        lblDomicilio.Text = domicilio.ToString("C2", moneda);
        lblItbis.Text = itbis.ToString("C2", moneda);
        lblTotal.Text = total.ToString("C2", moneda);
    }
}
