# ToLazyToCurl [![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download) [![WPF](https://img.shields.io/badge/WPF-512BD4?logo=windows&logoColor=white)](https://learn.microsoft.com/dotnet/desktop/wpf/)   

Simple WPF app for playing with REST APIs and viewing responses.

> *Why bother with anything actually good, when you can make your own version?*

<br>

![Dark and Light theme](/images/App.png)

## Features
* Supports `GET`, `POST`, `PATCH`, `PUT` and `DELETE` requests
* Optional custom HTTP headers
* Dynamic syntax highlighting (JSON, XML, HTML, etc.)
* Responsive
* Dark/light mode
* Logging
* Restores last session on startup

## Screenshots

Split View
![Split View](/images/SplitView.png)

Error Examples
![Error Examples](/images/UiErrorMessages.png)

## Try It Out?

<details>
<summary>Run from source</summary>  

#### Requirements: 
* Windows  
* [.NET 9.0](https://dotnet.microsoft.com/en-us/download/dotnet)

``` bash
git clone "https://github.com/Peppson/to-lazy-to-curl.git" &&
cd "to-lazy-to-curl" &&
dotnet publish -c Release -r win-x64 --self-contained true -o "$HOME/Desktop"
```

</details> 

<details> 
<summary>Download prebuilt</summary>

#### Requirements:

* A computer! (Windows)

Grab the latest release from the [Releases tab](https://github.com/Peppson/to-lazy-to-curl/releases)  

</details>

<br>
