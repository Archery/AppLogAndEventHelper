# AppLogAndEventHelper 
This library was firstly developed for Desktop applications on .Net Framework 4.0, and now I am trying to upgrade it.

## App Helper. What does it do?
First purpose was logging, debuging by the help of log.

It has **EventCatcher**, that catches events like:

```csharp
//some result of work
AppLogAndEventHelper.Instance.RaiseEvent(EventType.Result, "P = ", 2 * (a + b));
//or
AppLogAndEventHelper.Instance.RaiseDebugInfo(some_string, some_array);
```
**Event** contains of EventType, Thread, Timestamp, Method (where it was raised, may be buggy) and array of Comments (user passed info)

To add a receiver:

```csharp
//creating app log with inbuild receivers
AppLogAndEventHelper.Instance.AddLog("MyAppName.log");
//or
AppLogAndEventHelper.Instance.AddReceiver(WriteEventToConsole);

...

public static void WriteEventToConsole(Event e) {
    if (e.Type < EventType.Info) // filter debug info
        return;

    Console.WriteLine($"{e.Type.ToString().PadRight(5)} {e.Time:hh:mm:ss}: {e.CommentsToString()}");
}
```
So you may 
* create you own log (please use class **Log**), put your own formatting and colors. HTML? You are welcome
* use pop-up receivers for error alerts
* ...

By the way, by default you will receive error messages in Debug Console in VS.

## Do not forget to dispose

if you want to use app log  better do it this way:

```csharp
internal class Program {
    private static void Main(string[] args) {
        AppLogAndEventHelper.Instance.AddLog("MyAppName.log");

        try {

            ...

        }
        catch (Exception ex) {
            AppLogAndEventHelper.Instance.RaiseError(ex);
        }
        finally {
            AppLogAndEventHelper.Instance.Dispose();
        }
    }
}
```

## What else?
Other usefull tools here
* **Formatter** - creates own format for types/classes/values. For example: arrays, lists. 
* **RotatingLog** - type of app log for applications working 24/7. Each new day it creates new log file. You may use app log and RotatingLog together
* **EmailManager** - under constraction. Supose to send current rotating log file on receiving errors. Also I dream to use telegram bot for that)
* **PackagingExtensions** - Base64 incode/decode, MD5 calculation
* **FileReadWriteExtensions** - reading and writing to files as extension to String and FileInfo 

## NuGet package

You may see here [Mew.LogAndEventHelper on NuGet](https://www.nuget.org/packages/Mew.LogAndEventHelper)

for Package Manager

```
Install-Package Mew.LogAndEventHelper -Version 1.0.0.7-beta
```

mew 
