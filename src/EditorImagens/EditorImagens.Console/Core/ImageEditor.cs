namespace EditorImagens.Console.Core;

public class ImageEditor
{
    // Estado interno COMPLETAMENTE encapsulado
    private byte[] _pixels;
    private int _width;
    private int _height;
    private int _brightness;
    private int _contrast;
    private int _saturation;
    private string _filterApplied;
    private double _rotation;

    public ImageEditor(int width, int height)
    {
        _width = width;
        _height = height;
        _pixels = new byte[width * height * 3];
        _brightness = 0;
        _contrast = 0;
        _saturation = 0;
        _filterApplied = "None";
        _rotation = 0;

        System.Console.WriteLine($"[Editor] Imagem criada: {width}x{height}");
    }

    // ==============================
    // OPERAÇÕES
    // ==============================

    public void ApplyBrightness(int value)
    {
        _brightness += value;
        System.Console.WriteLine($"[Editor] Brilho ajustado para {_brightness}");
    }

    public void ApplyFilter(string filter)
    {
        _filterApplied = filter;
        System.Console.WriteLine($"[Editor] Filtro aplicado: {filter}");
    }

    public void Rotate(double degrees)
    {
        _rotation += degrees;
        System.Console.WriteLine($"[Editor] Rotação: {_rotation}°");
    }

    public void Crop(int newWidth, int newHeight)
    {
        _width = newWidth;
        _height = newHeight;
        Array.Resize(ref _pixels, newWidth * newHeight * 3);

        System.Console.WriteLine($"[Editor] Imagem cortada para {newWidth}x{newHeight}");
    }

    // ==============================
    // MEMENTO
    // ==============================

    public ImageMemento Save()
    {
        System.Console.WriteLine("[Editor] Estado capturado (Memento)");

        return new ImageMemento(
            (byte[])_pixels.Clone(), // cópia defensiva
            _width,
            _height,
            _brightness,
            _contrast,
            _saturation,
            _filterApplied,
            _rotation
        );
    }

    public void Restore(ImageMemento memento)
    {
        System.Console.WriteLine("[Editor] Restaurando estado...");

        _pixels = (byte[])memento.Pixels.Clone();
        _width = memento.Width;
        _height = memento.Height;
        _brightness = memento.Brightness;
        _contrast = memento.Contrast;
        _saturation = memento.Saturation;
        _filterApplied = memento.FilterApplied;
        _rotation = memento.Rotation;
    }

    public void DisplayInfo()
    {
        System.Console.WriteLine("\n=== Estado Atual ===");
        System.Console.WriteLine($"Dimensões: {_width}x{_height}");
        System.Console.WriteLine($"Brilho: {_brightness}");
        System.Console.WriteLine($"Filtro: {_filterApplied}");
        System.Console.WriteLine($"Rotação: {_rotation}°\n");
    }

    // Classe interna → protege encapsulamento
    public class ImageMemento
    {
        internal byte[] Pixels { get; }
        internal int Width { get; }
        internal int Height { get; }
        internal int Brightness { get; }
        internal int Contrast { get; }
        internal int Saturation { get; }
        internal string FilterApplied { get; }
        internal double Rotation { get; }

        internal ImageMemento(
            byte[] pixels,
            int width,
            int height,
            int brightness,
            int contrast,
            int saturation,
            string filterApplied,
            double rotation)
        {
            Pixels = pixels;
            Width = width;
            Height = height;
            Brightness = brightness;
            Contrast = contrast;
            Saturation = saturation;
            FilterApplied = filterApplied;
            Rotation = rotation;
        }
    }
}