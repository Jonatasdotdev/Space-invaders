using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CapstoneProg3.utils;

namespace CapstoneProg3.background
{
    public class CyclingBackgroundHardcore
    {
        private WriteableBitmap writeableImage;
        private int width, height;
        private byte[] pixels;
        private readonly Canvas canvas;
        private Image image;
        private readonly byte[] firstRowsBuffer;
        private const int LinesToMove = 4;

        public CyclingBackgroundHardcore(Canvas canvas)
        {
            this.canvas = canvas;

            BitmapImage bitmap =
                new BitmapImage(new Uri(PathUtils.GetFullPath("assets/Background3.png"), UriKind.Relative));
            writeableImage = new WriteableBitmap(bitmap);
            width = writeableImage.PixelWidth;
            height = writeableImage.PixelHeight;
            pixels = new byte[width * height * 4];

            writeableImage.CopyPixels(pixels, width * 4, 0);

            image = new Image
            {
                Stretch = Stretch.Fill,
                Source = writeableImage
            };
            this.canvas.Children.Add(image);
            Panel.SetZIndex(image, -1);

            this.canvas.SizeChanged += OnCanvasSizeChanged;
            
            firstRowsBuffer = new byte[LinesToMove * width * 4];
        }

        private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            image.Width = canvas.ActualWidth;
            image.Height = canvas.ActualHeight;
        }

        public void OnTick(object sender, EventArgs e)
        {
            _ = OnTickAsync();
        }

        private async Task OnTickAsync()
        {
            await Task.Run(ProcessImage);
            
            Application.Current.Dispatcher.InvokeAsync(UpdateImage);
        }

        private void ProcessImage()
        {
            var rowSize = width * 4;
            var blockSize = LinesToMove * rowSize;

            Array.Copy(pixels, 0, firstRowsBuffer, 0, blockSize);

            Array.Copy(pixels, blockSize, pixels, 0, (height - LinesToMove) * rowSize);

            Array.Copy(firstRowsBuffer, 0, pixels, (height - LinesToMove) * rowSize, blockSize);
        }


        private void UpdateImage()
        {
            writeableImage.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);
        }
    }
}