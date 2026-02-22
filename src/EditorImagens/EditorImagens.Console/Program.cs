using EditorImagens.Console.Core;
using EditorImagens.Console.History;

Console.WriteLine("=== Editor de Imagens - Memento Pattern ===\n");

var editor = new ImageEditor(1920, 1080);
var history = new ImageHistory();

history.Save(editor); // estado inicial

editor.ApplyBrightness(20);
history.Save(editor);

editor.ApplyFilter("Sepia");
history.Save(editor);

editor.Rotate(90);
history.Save(editor);

editor.Crop(1280, 720);
editor.DisplayInfo();

Console.WriteLine("=== Undo ===");
history.Undo(editor);
editor.DisplayInfo();

Console.WriteLine("=== Undo ===");
history.Undo(editor);
editor.DisplayInfo();

Console.WriteLine("=== Redo ===");
history.Redo(editor);
editor.DisplayInfo();