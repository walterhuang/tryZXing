using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZXing;

namespace TryZXing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const char dataMatrixGS = (char)29;
        private const char code128GS = (char)0x00F1;

        // Target sample
        // <GS>4020614141<GS>1234567890
        // <GS>9012KSCAC12345678901234567890

        // http://www.idautomation.com/barcode-faq/gs1-128/
        // <GS>8100712345<GS>2112345678

        // https://stackoverflow.com/questions/45603051/generate-gs1-128-using-zxing-net
        // {GS}019501234567890310000123{GS}17150801

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerateBarcodeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BarcodeFormat barcodeFormat;
                Enum.TryParse(BarcodeFormatSelector.Text, out barcodeFormat);
                var GS = barcodeFormat == BarcodeFormat.CODE_128 ? code128GS : dataMatrixGS;
                string code = BarcodeContentTextBox.Text.Replace("<GS>", GS.ToString());
                BarcodeWriter writer = new BarcodeWriter { Format = barcodeFormat };
                var img = writer.Write(code);
                img.Save("barcode.png", ImageFormat.Png);
                BarcodeImage.Source = loadBitmap(img);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CreateGS1DataMatrixButton_Click(object sender, RoutedEventArgs e)
        {
            var GS = (char)29;
            BarcodeWriter writer = new BarcodeWriter { Format = BarcodeFormat.DATA_MATRIX };
            var img = writer.Write($"{GS}019501234567890310000123{GS}17150801");
            img.Save("gs1datamatrix.png", ImageFormat.Png);
            BarcodeImage.Source = loadBitmap(img);
        }

        private void CreateGS1128Button_Click(object sender, RoutedEventArgs e)
        {
            var GS = (char)0x00F1;
            BarcodeWriter writer = new BarcodeWriter { Format = BarcodeFormat.CODE_128, };
            var img = writer.Write($"{GS}019501234567890310000123{GS}17150801");
            img.Save("gs1128.png", ImageFormat.Png);
            BarcodeImage.Source = loadBitmap(img);
        }

        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        public static BitmapSource loadBitmap(System.Drawing.Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty,
                   System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        }


    }
}
