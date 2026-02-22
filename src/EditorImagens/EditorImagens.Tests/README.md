# EditorImagens.Tests

## Resumo

Este projeto contém testes unitários abrangentes para o projeto **EditorImagens.Console**, que implementa o padrão **Memento** para gerenciamento de histórico de edição de imagens.

## Estrutura dos Testes

### 1. **Core/ImageEditorTests.cs** (22 testes)
Testes para a classe `ImageEditor` que implementa as operações de edição e o padrão Memento.

#### Testes de Criação e Inicialização
- `Constructor_ShouldCreateImageWithCorrectDimensions`: Verifica a criação do editor
- `InitialState_ShouldHaveDefaultValues`: Valida valores padrão do estado inicial

#### Testes de Operações
- `ApplyBrightness_ShouldAdjustBrightness`: Testa ajuste de brilho
- `ApplyBrightness_MultipleTimes_ShouldAccumulateValues`: Verifica acúmulo de brilho
- `ApplyFilter_ShouldSetFilterName`: Testa aplicação de filtro
- `ApplyFilter_MultipleTimes_ShouldUpdateToLatestFilter`: Verifica atualização de filtros
- `Rotate_ShouldAdjustRotation`: Testa rotação
- `Rotate_MultipleTimes_ShouldAccumulateRotation`: Verifica acúmulo de rotação
- `Crop_ShouldUpdateDimensions`: Testa corte de imagem
- `Crop_ShouldResizePixelsArray`: Verifica redimensionamento do array de pixels

#### Testes do Padrão Memento
- `Save_ShouldCreateMemento`: Verifica criação de memento
- `Save_ShouldCreateIndependentCopy`: Valida cópia independente
- `Restore_ShouldRestorePreviousState`: Testa restauração de estado
- `Restore_ShouldRestoreDimensions`: Verifica restauração de dimensões
- `Restore_ShouldRestorePixelArray`: Valida restauração do array de pixels
- `Memento_ShouldBeImmutableAfterCreation`: Verifica imutabilidade do memento

#### Testes de Workflow Complexo
- `ComplexWorkflow_ShouldMaintainCorrectState`: Testa fluxo complexo de operações

### 2. **History/ImageHistoryTests.cs** (17 testes)
Testes para a classe `ImageHistory` que gerencia o histórico de undo/redo.

#### Testes de Save
- `Save_ShouldStoreEditorState`: Verifica armazenamento de estado
- `Save_MultipleTimes_ShouldStackStates`: Valida empilhamento de estados
- `Save_AfterUndo_ShouldClearRedoStack`: Testa limpeza da pilha de redo

#### Testes de Undo
- `Undo_WithNoHistory_ShouldNotThrowException`: Verifica comportamento sem histórico
- `Undo_ShouldRestorePreviousState`: Testa restauração de estado anterior
- `Undo_ShouldEnableRedo`: Valida habilitação do redo
- `Undo_ShouldPreserveAllStateProperties`: Verifica preservação de todas as propriedades
- `MultipleUndo_ShouldTraverseHistoryBackward`: Testa múltiplos undos

#### Testes de Redo
- `Redo_WithNoHistory_ShouldNotThrowException`: Verifica comportamento sem histórico
- `Redo_WithoutUndo_ShouldNotChangeState`: Valida que redo sem undo não altera estado
- `Redo_ShouldRestoreUndoneState`: Testa restauração de estado desfeito
- `Redo_ShouldPreserveAllStateProperties`: Verifica preservação de todas as propriedades
- `MultipleRedo_ShouldTraverseHistoryForward`: Testa múltiplos redos

#### Testes de Workflow Complexo
- `ComplexWorkflow_UndoRedoAndSave`: Testa fluxo complexo de undo/redo/save
- `AlternatingUndoRedo_ShouldMaintainConsistency`: Verifica consistência em undo/redo alternados
- `EmptyHistory_UndoAndRedo_ShouldHandleGracefully`: Testa comportamento com histórico vazio

## Configuração do Projeto

### InternalsVisibleTo
Para permitir que os testes acessem membros internos da classe `ImageMemento`, foi adicionado o atributo `InternalsVisibleTo` no projeto Console:

```csharp
// Properties/AssemblyInfo.cs
[assembly: InternalsVisibleTo("EditorImagens.Tests")]
```

### Dependências
- **xUnit** 2.9.2 - Framework de testes
- **xunit.runner.visualstudio** 2.8.2 - Test runner
- **Microsoft.NET.Test.Sdk** 17.12.0 - SDK de testes
- **coverlet.collector** 6.0.2 - Cobertura de código

## Executando os Testes

```powershell
# Executar todos os testes
dotnet test

# Executar com verbosidade normal
dotnet test --verbosity normal

# Executar com cobertura de código
dotnet test /p:CollectCoverage=true
```

## Resultados

✅ **33 testes executados**
✅ **33 testes aprovados**
✅ **0 testes falharam**
✅ **Cobertura completa das funcionalidades**

## Padrão de Design Testado

Os testes validam a implementação correta do **Padrão Memento**, garantindo que:
1. O estado do editor pode ser capturado em um memento
2. O estado pode ser restaurado a partir de um memento
3. Os mementos são imutáveis
4. O histórico de undo/redo funciona corretamente
5. A pilha de redo é limpa quando novas alterações são feitas após um undo

