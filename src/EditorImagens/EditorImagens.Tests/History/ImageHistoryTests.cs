using EditorImagens.Console.Core;
using EditorImagens.Console.History;

namespace EditorImagens.Tests.History;

public class ImageHistoryTests
{
    [Fact]
    public void Save_ShouldStoreEditorState()
    {
        // Arrange
        var editor = new ImageEditor(1920, 1080);
        var history = new ImageHistory();
        editor.ApplyBrightness(20);

        // Act
        history.Save(editor);

        // Assert - Deve poder fazer undo
        editor.ApplyBrightness(30);
        history.Undo(editor);
        var memento = editor.Save();
        Assert.Equal(20, memento.Brightness);
    }

    [Fact]
    public void Save_MultipleTimes_ShouldStackStates()
    {
        // Arrange
        var editor = new ImageEditor(800, 600);
        var history = new ImageHistory();

        // Act
        history.Save(editor); // State 1: brightness = 0
        editor.ApplyBrightness(10);
        history.Save(editor); // State 2: brightness = 10
        editor.ApplyBrightness(10);
        history.Save(editor); // State 3: brightness = 20

        // Assert - Undo should go back through states
        editor.ApplyBrightness(10); // brightness = 30
        history.Undo(editor);
        Assert.Equal(20, editor.Save().Brightness);

        history.Undo(editor);
        Assert.Equal(10, editor.Save().Brightness);

        history.Undo(editor);
        Assert.Equal(0, editor.Save().Brightness);
    }

    [Fact]
    public void Undo_WithNoHistory_ShouldNotThrowException()
    {
        // Arrange
        var editor = new ImageEditor(1024, 768);
        var history = new ImageHistory();

        // Act & Assert
        history.Undo(editor); // Should not throw
        Assert.NotNull(editor);
    }

    [Fact]
    public void Undo_ShouldRestorePreviousState()
    {
        // Arrange
        var editor = new ImageEditor(1920, 1080);
        var history = new ImageHistory();

        // Act
        history.Save(editor);
        editor.ApplyBrightness(25);
        editor.ApplyFilter("Sepia");
        history.Undo(editor);

        // Assert
        var memento = editor.Save();
        Assert.Equal(0, memento.Brightness);
        Assert.Equal("None", memento.FilterApplied);
    }

    [Fact]
    public void Undo_ShouldEnableRedo()
    {
        // Arrange
        var editor = new ImageEditor(800, 600);
        var history = new ImageHistory();
        history.Save(editor);
        editor.ApplyBrightness(15);

        // Act
        history.Undo(editor);
        history.Redo(editor);

        // Assert
        var memento = editor.Save();
        Assert.Equal(15, memento.Brightness);
    }

    [Fact]
    public void Redo_WithNoHistory_ShouldNotThrowException()
    {
        // Arrange
        var editor = new ImageEditor(1024, 768);
        var history = new ImageHistory();

        // Act & Assert
        history.Redo(editor); // Should not throw
        Assert.NotNull(editor);
    }

    [Fact]
    public void Redo_WithoutUndo_ShouldNotChangeState()
    {
        // Arrange
        var editor = new ImageEditor(1024, 768);
        var history = new ImageHistory();
        editor.ApplyBrightness(20);
        var beforeRedo = editor.Save();

        // Act
        history.Redo(editor);

        // Assert
        var afterRedo = editor.Save();
        Assert.Equal(beforeRedo.Brightness, afterRedo.Brightness);
    }

    [Fact]
    public void Redo_ShouldRestoreUndoneState()
    {
        // Arrange
        var editor = new ImageEditor(1920, 1080);
        var history = new ImageHistory();
        history.Save(editor);
        editor.ApplyBrightness(30);
        editor.ApplyFilter("Vintage");
        editor.Rotate(90);

        // Act
        history.Undo(editor);
        history.Redo(editor);

        // Assert
        var memento = editor.Save();
        Assert.Equal(30, memento.Brightness);
        Assert.Equal("Vintage", memento.FilterApplied);
        Assert.Equal(90, memento.Rotation);
    }

    [Fact]
    public void Save_AfterUndo_ShouldClearRedoStack()
    {
        // Arrange
        var editor = new ImageEditor(800, 600);
        var history = new ImageHistory();

        history.Save(editor); // State 0: brightness=0
        editor.ApplyBrightness(10);
        history.Save(editor); // State 1: brightness=10
        editor.ApplyBrightness(10);
        history.Save(editor); // State 2: brightness=20

        // Act
        history.Undo(editor); // Restores State 2 (brightness=20)
        editor.ApplyFilter("Sepia"); // New change on top of State 2
        history.Save(editor); // New State 3: brightness=20, filter=Sepia (clears redo)

        history.Redo(editor); // Redo stack is empty, nothing happens

        // Assert - Should still be at State 3
        var memento = editor.Save();
        Assert.Equal("Sepia", memento.FilterApplied);
        Assert.Equal(20, memento.Brightness); // State 2 brightness was 20
    }

    [Fact]
    public void MultipleUndo_ShouldTraverseHistoryBackward()
    {
        // Arrange
        var editor = new ImageEditor(1024, 768);
        var history = new ImageHistory();

        // Create a history
        history.Save(editor); // State 0: brightness = 0
        editor.ApplyBrightness(5);
        history.Save(editor); // State 1: brightness = 5
        editor.ApplyBrightness(5);
        history.Save(editor); // State 2: brightness = 10
        editor.ApplyBrightness(5);
        history.Save(editor); // State 3: brightness = 15

        // Act
        editor.ApplyBrightness(5); // Current: brightness = 20
        history.Undo(editor); // Back to 15
        history.Undo(editor); // Back to 10
        history.Undo(editor); // Back to 5

        // Assert
        var memento = editor.Save();
        Assert.Equal(5, memento.Brightness);
    }

    [Fact]
    public void MultipleRedo_ShouldTraverseHistoryForward()
    {
        // Arrange
        var editor = new ImageEditor(1024, 768);
        var history = new ImageHistory();

        history.Save(editor); // State 0
        editor.ApplyBrightness(10);
        history.Save(editor); // State 1
        editor.ApplyBrightness(10);
        history.Save(editor); // State 2

        // Act - Undo multiple times
        editor.ApplyBrightness(10); // Current: brightness = 30
        history.Undo(editor); // Back to 20
        history.Undo(editor); // Back to 10
        history.Undo(editor); // Back to 0

        // Redo multiple times
        history.Redo(editor); // Forward to 10
        history.Redo(editor); // Forward to 20

        // Assert
        var memento = editor.Save();
        Assert.Equal(20, memento.Brightness);
    }

    [Fact]
    public void ComplexWorkflow_UndoRedoAndSave()
    {
        // Arrange
        var editor = new ImageEditor(1920, 1080);
        var history = new ImageHistory();

        // Act - Build initial history
        history.Save(editor); // State 0: brightness=0, filter=None
        editor.ApplyBrightness(10);
        history.Save(editor); // State 1: brightness=10, filter=None
        editor.ApplyFilter("Sepia");
        history.Save(editor); // State 2: brightness=10, filter=Sepia

        // Undo twice to get to State 1
        history.Undo(editor); // Restores State 2
        history.Undo(editor); // Restores State 1
        Assert.Equal("None", editor.Save().FilterApplied);

        // Make new change (creates new branch, invalidates old redo stack)
        editor.ApplyFilter("Grayscale");
        history.Save(editor); // New State 3 - clears redo stack

        // Try to redo (redo stack was cleared by Save, so nothing happens)
        history.Redo(editor);

        // Assert - Should still be at State 3
        var memento = editor.Save();
        Assert.Equal("Grayscale", memento.FilterApplied);
        Assert.Equal(10, memento.Brightness);
    }

    [Fact]
    public void Undo_ShouldPreserveAllStateProperties()
    {
        // Arrange
        var editor = new ImageEditor(1920, 1080);
        var history = new ImageHistory();

        // Act
        history.Save(editor);
        editor.ApplyBrightness(25);
        editor.ApplyFilter("HDR");
        editor.Rotate(45);
        editor.Crop(1280, 720);

        history.Undo(editor);

        // Assert
        var memento = editor.Save();
        Assert.Equal(1920, memento.Width);
        Assert.Equal(1080, memento.Height);
        Assert.Equal(0, memento.Brightness);
        Assert.Equal("None", memento.FilterApplied);
        Assert.Equal(0, memento.Rotation);
    }

    [Fact]
    public void Redo_ShouldPreserveAllStateProperties()
    {
        // Arrange
        var editor = new ImageEditor(1600, 900);
        var history = new ImageHistory();

        // Create a complex state
        history.Save(editor);
        editor.ApplyBrightness(30);
        editor.ApplyFilter("Vintage");
        editor.Rotate(90);
        editor.Crop(800, 450);

        // Act - Undo then Redo
        history.Undo(editor);
        history.Redo(editor);

        // Assert
        var memento = editor.Save();
        Assert.Equal(800, memento.Width);
        Assert.Equal(450, memento.Height);
        Assert.Equal(30, memento.Brightness);
        Assert.Equal("Vintage", memento.FilterApplied);
        Assert.Equal(90, memento.Rotation);
    }

    [Fact]
    public void AlternatingUndoRedo_ShouldMaintainConsistency()
    {
        // Arrange
        var editor = new ImageEditor(1024, 768);
        var history = new ImageHistory();

        history.Save(editor); // State 0: brightness = 0
        editor.ApplyBrightness(20);
        history.Save(editor); // State 1: brightness = 20

        // Act & Assert
        editor.ApplyBrightness(10); // Current: brightness = 30
        history.Undo(editor);
        Assert.Equal(20, editor.Save().Brightness);

        history.Redo(editor);
        Assert.Equal(30, editor.Save().Brightness);

        history.Undo(editor);
        Assert.Equal(20, editor.Save().Brightness);

        history.Redo(editor);
        Assert.Equal(30, editor.Save().Brightness);
    }

    [Fact]
    public void EmptyHistory_UndoAndRedo_ShouldHandleGracefully()
    {
        // Arrange
        var editor = new ImageEditor(800, 600);
        var history = new ImageHistory();
        var initialMemento = editor.Save();

        // Act
        history.Undo(editor);
        history.Redo(editor);
        history.Undo(editor);

        // Assert - State should not change
        var finalMemento = editor.Save();
        Assert.Equal(initialMemento.Brightness, finalMemento.Brightness);
        Assert.Equal(initialMemento.Width, finalMemento.Width);
        Assert.Equal(initialMemento.Height, finalMemento.Height);
    }
}

