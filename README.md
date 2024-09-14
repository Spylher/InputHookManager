#  InputHookManager
![Static Badge](https://img.shields.io/badge/dotnet-v8.0-blue)
![Static Badge](https://img.shields.io/badge/license-MIT-green)


InputHookManager is a tool developed to intercept keyboard and mouse input on Windows systems. In addition to monitoring these inputs, InputHookManager can also cancel actions or add new events based on predefined specific inputs.


## Features

- `Input Interception:` Allows capturing keyboard and mouse events, providing the ability to log these actions in real time.

- `Input Cancellation:` Allows blocking specific inputs, preventing the system from processing them.

- `Triggering Custom Events:` When identifying a specific sequence of keys or mouse actions, you can define custom events that will be triggered automatically.

## Possible Uses

**Task Automation**

> InputHookManager can be used to automate repetitive actions by intercepting and reacting to specific inputs, triggering automatic events. For example, pressing a key combination can activate a script or application.

**Security and Input Control**

> In environments where strict control over keyboard and mouse input is necessary, the tool can prevent unauthorized actions by blocking specific commands.

**Creating Custom Shortcuts**

> InputHookManager allows you to create custom shortcuts or macros that perform a series of actions based on specific key combinations or mouse clicks.

**Games and Simulations**

> Gamers can use the tool to create advanced control configurations, modifying the response to clicks and keys in games or simulators that require advanced customization of input devices.

**Development of Accessibility Solutions**

> Implementation of hotkeys or combinations that facilitate system usage for people with disabilities, allowing remapping and customization of functions.

## Installation

**Clone this repository**

```bash
git clone https://github.com/Spylher/InputHookManager.git
```
- **`Open the project in Visual Studio:`** Navigate to the cloned project directory and open the .csproj file with Visual Studio.
  
- **`Build the project:`** Build the project in Visual Studio using either the `Release` or `Debug` build mode, depending on your need.

- **`Reference in your own project:`** After building, you can reference the generated DLL in other projects to use InputHookManager.

## Samples

In the [`samples`](./samples) folder, you will find examples demonstrating how to use InputHookManager.

### Input Watcher

> This example demonstrates how to intercept and monitor keyboard and mouse inputs in real time. The program logs all key presses and mouse movements, allowing the user to define events triggered based on those inputs. It is useful for activity logging or input monitoring for automation purposes.

### Window Shortcuts

> This example shows how to use global shortcuts to manipulate Windows windows. The program allows, for example, minimizing, moving, or closing windows using custom key combinations. This example is ideal for those who want to create productivity tools or simplify window management on the desktop.

## Usage

Basic Example

```bash
# initialize hook
var hookManager = new InputController();

# set the hotkey
var sendMessageKey = new HotKey(InputKey.A);

# Define the associated action
void SendMessage(object sender) => Console.WriteLine("Hello, World!");

# Register the action for the hotkey
hookManager.RegisterAction(sendMessageKey, SendMessage);

Console.ReadLine();
```

## Contribution
Contributions are welcome! If you find any issues, please open an issue or submit a pull request.

## Licença
This project is licensed under the [licença MIT](./LICENSE.txt).

