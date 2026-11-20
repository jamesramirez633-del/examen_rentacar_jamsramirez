using System;
using System.Windows.Forms;

namespace RentCar_Express
{
    public partial class Form1 : Form
    {
        decimal subtotal = 0;

        const decimal SEGURO = 300m;
        const decimal GPS = 100m;
        const decimal DOMICILIO = 500m;
        const decimal ITBIS = 0.18m;
        const decimal DESCUENTO = 0.15m;

        public Form1()
        {
            InitializeComponent();
        }

        // Aquí implementarás:
        // Form1_Load
        // btnAgregar_Click
        // btnCalcular_Click
        // btnLimpiar_Click
        // btnSalir_Click
    }
}
