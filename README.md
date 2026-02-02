# CheckpointingSystem

A lightweight C# checkpointing and serialization framework designed for **Unity** and general **C# applications**.
It enables saving and restoring program state by selectively serializing object graphs into **human-readable XML**, with an emphasis on performance, extensibility, and minimal integration overhead.

This project accompanies the research paper *Efficient Checkpointing via Object Serialization in C# Applications*.

---

## Features

* Attribute-based, **opt-in serialization**
* Human-readable and editable **XML checkpoints**
* Supports **complex object graphs**
* Optimized reflection via **cached expression trees**
* Extensible save/load system for custom types
* Unity-friendly (`MonoBehaviour`, `Vector3`, `Quaternion`)
* Optional **delta saving** using a blank reference object

---

## Installation

### Unity (recommended)

1. Clone the repository:

   ```bash
   git clone https://github.com/LukaBorozan/CheckpointingSystem.git
   ```

2. Copy the `SaveSystem` folder into your Unity project:

   ```
   Assets/
     └── SaveSystem/
   ```

3. The system works out of the box — no additional configuration required.

---

### Plain C# Project

1. Clone the repository:

   ```bash
   git clone https://github.com/LukaBorozan/CheckpointingSystem.git
   ```

2. Add the source files to your project:

   * `SaveManager.cs`
   * `SaveableTypes.cs`
   * Template classes (e.g. `ObjectTemplate`, `ListTemplate`, etc.)

3. Add references to:

   * `System.Xml`
   * `System.Reflection`
   * `System.Linq.Expressions`

> **Note**
> Unity-specific types (`Vector3`, `Quaternion`, `MonoBehaviour`) require Unity assemblies.
> Remove or replace those handlers when using pure .NET.

---

## Basic Usage

### Implement `ISaveable`

```csharp
using SaveSystem;

public class Player : ISaveable
{
    [Save]
    public int Health;

    [Save("player_name")]
    private string name;

    public void OnSave() { }
    public void OnLoad() { }
}
```

Only members explicitly marked with `[Save]` are serialized.

---

### Writing a Checkpoint

```csharp
SaveManager.BeginWrite("save.xml");
SaveManager.WriteSaveable(player, blankPlayer); // blankPlayer is optional
SaveManager.EndWrite();
```

When a blank reference object is provided, members whose values match the blank object are omitted.

---

### Reading a Checkpoint

```csharp
SaveManager.BeginRead("save.xml");

while (SaveManager.ReadNextSaveableType() != null)
{
    SaveManager.ReadSaveable(player);
}

SaveManager.EndRead();
```

---

## Supported Types

Built-in support includes:

* Primitive types (`int`, `float`, `bool`, `string`, etc.)
* `Array`
* `List<T>`
* `HashSet<T>`
* `Dictionary<TKey, TValue>`
* `ValueTuple`
* `Enum`
* `DateTime`
* `Guid`
* Unity types:

  * `Vector3`
  * `Quaternion`

---

## Extending the System

Custom serialization logic can be added by registering save and load handlers:

```csharp
SaveableTypes.saveHandlers.Add(typeof(MyType), MyTemplate.WriteValue);
SaveableTypes.loadHandlers.Add(typeof(MyType), MyTemplate.ReadValue);
```

Handlers operate on boxed objects and receive the runtime type information.

---

## XML Format

Checkpoints are stored as structured XML:

```xml
<SaveGame>
  <Saveable Type="MyNamespace.Player">
    <Health>100</Health>
    <player_name>Luka</player_name>
  </Saveable>
</SaveGame>
```

Advantages:

* Human-readable
* Easy to debug
* Editable using standard text editors

---

## Performance Notes

* Reflection is used only during initialization
* Member accessors and method calls are compiled into cached delegates
* Subsequent saves and loads avoid reflection overhead

This design enables efficient checkpointing even for large object graphs.

---

## Limitations

* Serialization and deserialization are currently single-threaded
* XML verbosity can increase file size for very large checkpoints
* No built-in checkpoint versioning

---

## Research Background

This framework was developed as part of academic research on efficient checkpointing in object-oriented systems, focusing on:

* Reducing redundant runtime state
* Improving serialization performance
* Preserving human readability

---

## Author

**Luka Borozan**
Faculty of Applied Mathematics and Informatics
J. J. Strossmayer University of Osijek
📧 [lborozan@mathos.hr](mailto:lborozan@mathos.hr)
