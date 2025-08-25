# ToLazyToCurl
A simple WPF app for playing with REST APIs and viewing responses.

> *Why bother with anything actually good, when you can make your own version?*

<br>

![Dark and Light theme](/images/App.png)

## Features
* Supports `GET`, `POST`, `PATCH`, `PUT` and `DELETE` requests
* Optional custom HTTP headers
* Dynamic syntax highlighting (JSON, XML, HTML, etc.)
* Responsive design with animations for messages and controls
* Dark/light mode
* Session-based logging (resets on app restart)

## Screenshots

Split View
![Split View](/images/SplitView.png)

Error Examples
![Error Examples](/images/UiErrorMessages.png)

## Try It Out?

**Requirements:**  
* Windows  
* [.NET 9.0](https://dotnet.microsoft.com/en-us/download/dotnet)


``` bash
git clone "https://github.com/Peppson/to-lazy-to-curl.git" &&
cd "to-lazy-to-curl" &&
dotnet publish -c Release -r win-x64 --self-contained true -o "$HOME/Desktop"
```

<br>
