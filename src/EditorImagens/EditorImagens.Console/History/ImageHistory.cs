using EditorImagens.Console.Core;

namespace EditorImagens.Console.History;

public class ImageHistory
{
    private readonly Stack<ImageEditor.ImageMemento> _undoStack = new();
    private readonly Stack<ImageEditor.ImageMemento> _redoStack = new();

    public void Save(ImageEditor editor)
    {
        _undoStack.Push(editor.Save());
        _redoStack.Clear(); // nova operação invalida redo
        System.Console.WriteLine($"[Histórico] Snapshot salvo (Undo: {_undoStack.Count})");
    }

    public void Undo(ImageEditor editor)
    {
        if (_undoStack.Count == 0)
            return;

        var memento = _undoStack.Pop();
        _redoStack.Push(editor.Save());

        editor.Restore(memento);

        System.Console.WriteLine("[Histórico] Undo executado");
    }

    public void Redo(ImageEditor editor)
    {
        if (_redoStack.Count == 0)
            return;

        var memento = _redoStack.Pop();
        _undoStack.Push(editor.Save());

        editor.Restore(memento);

        System.Console.WriteLine("[Histórico] Redo executado");
    }
}