# Compiler Flag Wrapper for Visual Studio 2022

Compiler Flag Wrapper is a Visual Studio 2022 extension that surrounds selected lines with a configurable C or C++ preprocessor directive.

This repository contains a **single classic VSIX project**. Do not add a second packaging project.

## Features

- Wrap selected lines with a keyboard shortcut
- Configurable preprocessor flag
- Three supported directive styles:
  - `#ifdef FLAG`
  - `#if defined(FLAG)`
  - `#if FLAG`
- Optional flag comment after `#endif`
- Command available from the Visual Studio **Tools** menu
- Configurable keyboard shortcut

## Requirements

Install the following through **Visual Studio Installer**:

- Visual Studio 2022
- **Visual Studio extension development** workload
- .NET Framework 4.7.2 targeting pack

## Open and restore the project

1. Open `CompilerFlagWrapper.csproj` in Visual Studio 2022.
2. Allow NuGet package restore to finish.
3. When necessary, right-click the solution and select **Restore NuGet Packages**.

The project uses Visual Studio 2022-compatible `17.x` packages:

- `Microsoft.VisualStudio.SDK`
- `Microsoft.VSSDK.BuildTools`

## Test the extension

1. Keep the build configuration set to **Debug**.
2. Press `F5`.
3. Visual Studio opens an **Experimental Instance**.
4. Open a `.c`, `.cpp`, `.h`, or another text-based source file.
5. Select one or more lines.
6. Press `Ctrl+Shift+W`.

The command is also available from:

`Tools > Wrap Selection with Compiler Flag`

When no text is selected, the extension displays a message asking you to select one or more lines.

## Configure the extension

Open:

`Tools > Options > Compiler Flag Wrapper > General`

### Flag

Sets the preprocessor symbol used around the selected lines.

Example:

`FEATURE_FLAG`

### Directive style

Choose one of the following values.

#### IfDef

```c
#ifdef FEATURE_FLAG
selected_code();
#endif // FEATURE_FLAG
```

#### IfDefined

```c
#if defined(FEATURE_FLAG)
selected_code();
#endif // FEATURE_FLAG
```

#### IfExpression

```c
#if FEATURE_FLAG
selected_code();
#endif // FEATURE_FLAG
```

### Add flag to `#endif` comment

When enabled:

```c
#endif // FEATURE_FLAG
```

When disabled:

```c
#endif
```

## Change the keyboard shortcut

The default shortcut is:

`Ctrl+Shift+W`

To change it:

1. Open `Tools > Options > Environment > Keyboard`.
2. Search for:

   `CompilerFlagWrapper.WrapSelection`

3. Select the command.
4. Enter the desired shortcut.
5. Set the scope to **Global**.
6. Select **Assign**.

## Build the VSIX installer

1. Close the Experimental Instance.
2. Change the build configuration to **Release**.
3. Select:

   `Build > Rebuild Solution`

The installer is generated at:

`bin\Release\CompilerFlagWrapper.vsix`

## Install the extension on another computer

1. Copy `CompilerFlagWrapper.vsix` to the other computer.
2. Close all Visual Studio instances.
3. Double-click the `.vsix` file.
4. Select the Visual Studio 2022 installation.
5. Select **Install**.
6. Start Visual Studio again.

After installation, configure the extension under:

`Tools > Options > Compiler Flag Wrapper > General`

## Project structure

```text
CompilerFlagWrapper
├── Commands
│   └── WrapSelectionCommand.cs
├── Options
│   └── GeneralOptions.cs
├── Properties
│   └── AssemblyInfo.cs
├── CompilerFlagWrapper.csproj
├── CompilerFlagWrapper.vsct
├── CompilerFlagWrapperPackage.cs
└── source.extension.vsixmanifest
```

## Main files

### `Commands/WrapSelectionCommand.cs`

Contains the command implementation. It reads the active editor selection, expands it to complete lines, and inserts the selected preprocessor directive.

### `Options/GeneralOptions.cs`

Defines the options displayed under:

`Tools > Options > Compiler Flag Wrapper > General`

It contains:

- The compiler flag
- The directive style
- The optional `#endif` comment setting

### `CompilerFlagWrapper.vsct`

Registers:

- The command
- The entry under the **Tools** menu
- The default keyboard shortcut
- The command name `CompilerFlagWrapper.WrapSelection`

### `source.extension.vsixmanifest`

Contains the VSIX metadata, Visual Studio 2022 installation targets, prerequisites, and package asset registration.

## Publishing a release

Do not normally commit generated `.vsix` files to the repository.

Instead:

1. Build the project in **Release** mode.
2. Create a GitHub release.
3. Use a version tag such as `v1.0.0`.
4. Attach:

   `bin\Release\CompilerFlagWrapper.vsix`

5. Add release notes describing the available directive styles and configuration options.

## Suggested `.gitignore`

```gitignore
.vs/
bin/
obj/

*.user
*.suo
*.userosscache
*.sln.docstates

TestResults/
*.pdb
*.cache

*.vsix
```
