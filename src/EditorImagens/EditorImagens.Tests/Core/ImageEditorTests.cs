using EditorImagens.Console.Core;

namespace EditorImagens.Tests.Core;

public class ImageEditorTests
{
    [Fact]
    public void Constructor_ShouldCreateImageWithCorrectDimensions()
    {
        // Arrange & Act
        var editor = new ImageEditor(1920, 1080);

        // Assert
        Assert.NotNull(editor);
    }

    [Fact]
    public void ApplyBrightness_ShouldAdjustBrightness()
    {
        // Arrange
        var editor = new ImageEditor(800, 600);
        var initialMemento = editor.Save();

        // Act
        editor.ApplyBrightness(20);
        var updatedMemento = editor.Save();

        // Assert
        Assert.NotEqual(initialMemento.Brightness, updatedMemento.Brightness);
        Assert.Equal(20, updatedMemento.Brightness);
    }

    [Fact]
    public void ApplyBrightness_MultipleTimes_ShouldAccumulateValues()
    {
        // Arrange
        var editor = new ImageEditor(800, 600);

        // Act
        editor.ApplyBrightness(10);
        editor.ApplyBrightness(15);
        editor.ApplyBrightness(-5);
        var memento = editor.Save();

        // Assert
        Assert.Equal(20, memento.Brightness);
    }

    [Fact]
    public void ApplyFilter_ShouldSetFilterName()
    {
        // Arrange
        var editor = new ImageEditor(1024, 768);

        // Act
        editor.ApplyFilter("Sepia");
        var memento = editor.Save();

        // Assert
        Assert.Equal("Sepia", memento.FilterApplied);
    }

    [Fact]
    public void ApplyFilter_MultipleTimes_ShouldUpdateToLatestFilter()
    {
        // Arrange
        var editor = new ImageEditor(1024, 768);

        // Act
        editor.ApplyFilter("Sepia");
        editor.ApplyFilter("BlackAndWhite");
        var memento = editor.Save();

        // Assert
        Assert.Equal("BlackAndWhite", memento.FilterApplied);
    }

    [Fact]
    public void Rotate_ShouldAdjustRotation()
    {
        // Arrange
        var editor = new ImageEditor(800, 600);

        // Act
        editor.Rotate(90);
        var memento = editor.Save();

        // Assert
        Assert.Equal(90, memento.Rotation);
    }

    [Fact]
    public void Rotate_MultipleTimes_ShouldAccumulateRotation()
    {
        // Arrange
        var editor = new ImageEditor(800, 600);

        // Act
        editor.Rotate(45);
        editor.Rotate(90);
        editor.Rotate(-15);
        var memento = editor.Save();

        // Assert
        Assert.Equal(120, memento.Rotation);
    }

    [Fact]
    public void Crop_ShouldUpdateDimensions()
    {
        // Arrange
        var editor = new ImageEditor(1920, 1080);

        // Act
        editor.Crop(1280, 720);
        var memento = editor.Save();

        // Assert
        Assert.Equal(1280, memento.Width);
        Assert.Equal(720, memento.Height);
    }

    [Fact]
    public void Crop_ShouldResizePixelsArray()
    {
        // Arrange
        var editor = new ImageEditor(1920, 1080);
        var initialMemento = editor.Save();
        var initialPixelCount = initialMemento.Pixels.Length;

        // Act
        editor.Crop(960, 540);
        var croppedMemento = editor.Save();
        var croppedPixelCount = croppedMemento.Pixels.Length;

        // Assert
        Assert.NotEqual(initialPixelCount, croppedPixelCount);
        Assert.Equal(960 * 540 * 3, croppedPixelCount);
    }

    [Fact]
    public void Save_ShouldCreateMemento()
    {
        // Arrange
        var editor = new ImageEditor(1024, 768);
        editor.ApplyBrightness(15);
        editor.ApplyFilter("Vintage");
        editor.Rotate(45);

        // Act
        var memento = editor.Save();

        // Assert
        Assert.NotNull(memento);
        Assert.Equal(1024, memento.Width);
        Assert.Equal(768, memento.Height);
        Assert.Equal(15, memento.Brightness);
        Assert.Equal("Vintage", memento.FilterApplied);
        Assert.Equal(45, memento.Rotation);
    }

    [Fact]
    public void Save_ShouldCreateIndependentCopy()
    {
        // Arrange
        var editor = new ImageEditor(800, 600);
        var memento1 = editor.Save();

        // Act
        editor.ApplyBrightness(20);
        var memento2 = editor.Save();

        // Assert
        Assert.NotEqual(memento1.Brightness, memento2.Brightness);
        Assert.Equal(0, memento1.Brightness);
        Assert.Equal(20, memento2.Brightness);
    }

    [Fact]
    public void Restore_ShouldRestorePreviousState()
    {
        // Arrange
        var editor = new ImageEditor(1920, 1080);
        editor.ApplyBrightness(10);
        editor.ApplyFilter("Sepia");
        var memento = editor.Save();

        // Act
        editor.ApplyBrightness(30);
        editor.ApplyFilter("Grayscale");
        editor.Rotate(90);
        editor.Restore(memento);
        var restoredMemento = editor.Save();

        // Assert
        Assert.Equal(10, restoredMemento.Brightness);
        Assert.Equal("Sepia", restoredMemento.FilterApplied);
        Assert.Equal(0, restoredMemento.Rotation);
    }

    [Fact]
    public void Restore_ShouldRestoreDimensions()
    {
        // Arrange
        var editor = new ImageEditor(1920, 1080);
        var originalMemento = editor.Save();

        // Act
        editor.Crop(640, 480);
        editor.Restore(originalMemento);
        var restoredMemento = editor.Save();

        // Assert
        Assert.Equal(1920, restoredMemento.Width);
        Assert.Equal(1080, restoredMemento.Height);
    }

    [Fact]
    public void Restore_ShouldRestorePixelArray()
    {
        // Arrange
        var editor = new ImageEditor(1024, 768);
        var originalMemento = editor.Save();
        var originalPixelLength = originalMemento.Pixels.Length;

        // Act
        editor.Crop(512, 384);
        editor.Restore(originalMemento);
        var restoredMemento = editor.Save();

        // Assert
        Assert.Equal(originalPixelLength, restoredMemento.Pixels.Length);
    }

    [Fact]
    public void ComplexWorkflow_ShouldMaintainCorrectState()
    {
        // Arrange
        var editor = new ImageEditor(1920, 1080);

        // Act & Assert - Initial state
        var state1 = editor.Save();
        Assert.Equal(1920, state1.Width);
        Assert.Equal(1080, state1.Height);
        Assert.Equal(0, state1.Brightness);
        Assert.Equal("None", state1.FilterApplied);
        Assert.Equal(0, state1.Rotation);

        // Act & Assert - Apply brightness
        editor.ApplyBrightness(25);
        var state2 = editor.Save();
        Assert.Equal(25, state2.Brightness);

        // Act & Assert - Apply filter
        editor.ApplyFilter("HDR");
        var state3 = editor.Save();
        Assert.Equal("HDR", state3.FilterApplied);

        // Act & Assert - Rotate
        editor.Rotate(180);
        var state4 = editor.Save();
        Assert.Equal(180, state4.Rotation);

        // Act & Assert - Crop
        editor.Crop(1280, 720);
        var state5 = editor.Save();
        Assert.Equal(1280, state5.Width);
        Assert.Equal(720, state5.Height);
        Assert.Equal(25, state5.Brightness);
        Assert.Equal("HDR", state5.FilterApplied);
        Assert.Equal(180, state5.Rotation);
    }

    [Fact]
    public void Memento_ShouldBeImmutableAfterCreation()
    {
        // Arrange
        var editor = new ImageEditor(800, 600);
        editor.ApplyBrightness(10);
        var memento = editor.Save();
        var originalBrightness = memento.Brightness;

        // Act
        editor.ApplyBrightness(20);

        // Assert
        Assert.Equal(originalBrightness, memento.Brightness);
        Assert.Equal(10, memento.Brightness);
    }

    [Fact]
    public void InitialState_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var editor = new ImageEditor(1024, 768);
        var memento = editor.Save();

        // Assert
        Assert.Equal(1024, memento.Width);
        Assert.Equal(768, memento.Height);
        Assert.Equal(0, memento.Brightness);
        Assert.Equal(0, memento.Contrast);
        Assert.Equal(0, memento.Saturation);
        Assert.Equal("None", memento.FilterApplied);
        Assert.Equal(0, memento.Rotation);
        Assert.Equal(1024 * 768 * 3, memento.Pixels.Length);
    }
}

